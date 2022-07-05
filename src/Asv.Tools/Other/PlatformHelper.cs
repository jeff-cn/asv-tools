using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace Asv.Tools
{
    public static class PlatformHelper
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public enum OperatingSystem
        {
            Undefined,
            Windows,
            Linux,
            MacOsX
        }

        public static OperatingSystem DetectPlatform()
        {
            var windir = Environment.GetEnvironmentVariable("windir");
            if (!string.IsNullOrEmpty(windir) && windir.Contains(@"\") && Directory.Exists(windir)) return OperatingSystem.Windows;

            if (File.Exists(@"/proc/sys/kernel/ostype"))
            {
                var osType = File.ReadAllText(@"/proc/sys/kernel/ostype");
                return osType.StartsWith("Linux", StringComparison.OrdinalIgnoreCase)
                    ? OperatingSystem.Linux
                    : OperatingSystem.Undefined;
            }

            return File.Exists(@"/System/Library/CoreServices/SystemVersion.plist")
                ? OperatingSystem.MacOsX
                : OperatingSystem.Undefined;
        }

        #region shutdown \reboot

        public static Task Shutdown(CancellationToken cancel)
        {
            switch (DetectPlatform())
            {
                
                case OperatingSystem.Windows:
                    Process.Start("shutdown", "/s /t 0");
                    return Task.CompletedTask;
                case OperatingSystem.Undefined:
                case OperatingSystem.MacOsX:
                case OperatingSystem.Linux:
                    Process.Start("/usr/bin/sudo", "/bin/systemctl poweroff");
                    return Task.CompletedTask; ;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static Task Reboot(CancellationToken cancel)
        {
            return DetectPlatform() switch
            {
                OperatingSystem.Windows => Task.Factory.StartNew(
                    () => Process.Start("shutdown", "/r /t 0"), cancel),
                OperatingSystem.MacOsX => Task.Factory.StartNew(
                    () => Process.Start("/usr/bin/sudo", "/bin/systemctl reboot"), cancel),
                OperatingSystem.Undefined => Task.Factory.StartNew(
                    () => Process.Start("/usr/bin/sudo", "/bin/systemctl reboot"), cancel),
                OperatingSystem.Linux => Task.Factory.StartNew(
                    () => Process.Start("/usr/bin/sudo", "/bin/systemctl reboot"), cancel),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        #endregion

        #region Ip \ Wlan 

        public static Task<string> GetWlanSsid(CancellationToken cancel = default)
        {
            return DetectPlatform() switch
            {
                OperatingSystem.Linux => Task.Factory.StartNew(GetWlanSsidLinux, cancel),
                OperatingSystem.Undefined => Task.FromResult(default(string)),
                OperatingSystem.Windows => Task.FromResult(default(string)),
                OperatingSystem.MacOsX => Task.FromResult(default(string)),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static Task<List<IPAddress>> GetIpAddress(CancellationToken cancel = default)
        {
            return DetectPlatform() switch
            {
                OperatingSystem.Linux => Task.Factory.StartNew(GetIpLinux, cancel),
                OperatingSystem.Undefined => Task.FromResult(default(List<IPAddress>)),
                OperatingSystem.Windows => Task.FromResult(default(List<IPAddress>)),
                OperatingSystem.MacOsX => Task.FromResult(default(List<IPAddress>)),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static string GetWlanSsidLinux()
        {
            var rx = new ProcessRx();
            string result = null;
            using var subscribe = rx.OnOutput.Where(_ => _.IsNullOrWhiteSpace() == false).Subscribe(_ => result = _);
            rx.Start("/sbin/iwgetid", " -r", true);
            rx.WaitForExit();
            return result;
        }

        private static List<IPAddress> GetIpLinux()
        {
            using var rx = new ProcessRx();
            var result = new List<IPAddress>(4);
            using var subscribe = rx.OnOutput.Subscribe(value =>
            {
                if (value.IsNullOrWhiteSpace()) return;
                if (!Regex.IsMatch(value, "inet [0-9]*.[0-9]*.[0-9]*.[0-9]*", RegexOptions.Compiled | RegexOptions.IgnoreCase)) return;
                var ip = Regex.Replace(value, @"(.*inet )([0-9]*\.[0-9]*.[0-9]*.[0-9]*)(.*)", "$2", RegexOptions.Compiled);
                if (ip == "127.0.0.1") return;
                if (IPAddress.TryParse(ip, out var address))
                {
                    result.Add(address);
                }
            });
            rx.Start("/sbin/ifconfig", "", true);
            rx.WaitForExit();
            return result;
        }

        #endregion

    }
}
