using System;
using Asv.Tools.Serial;
using Asv.Tools.Tcp;

namespace Asv.Tools
{
    public enum PortState
    {
        Disabled,
        Connecting,
        Error,
        Connected
    }

    public enum PortType
    {
        Serial,
        Udp,
        Tcp
    }


    public interface IPort: IDataStream, IDisposable
    {
        long RxBytes { get; }
        long TxBytes { get; }
        PortType PortType { get; }
        TimeSpan ReconnectTimeout { get; set; }
        IRxValue<bool> IsEnabled { get; }
        IRxValue<PortState> State { get; }
        IRxValue<Exception> Error { get; }
        void Enable();
        void Disable();
    }

    public static class PortFactory
    {
        public static IPort Create(string connectionString, bool enabled = false)
        {
            var uri = new Uri(connectionString);
            IPort result = null;
            if (TcpPortConfig.TryParseFromUri(uri, out var tcp))
            {
                if (tcp.IsServer)
                {
                    result = new TcpServerPort(tcp);
                }
                else
                {
                    result = new TcpClientPort(tcp);
                }
            }
            else if (UdpPortConfig.TryParseFromUri(uri, out var udp))
            {
                result = new UdpPort(udp);
            }
            else if (SerialPortConfig.TryParseFromUri(uri, out var ser))
            {
                result = new CustomSerialPort(ser);
            }
            else
            {
                throw new Exception($"Connection string '{connectionString}' is invalid");
            }
            if (enabled)
            {
                result.Enable();
            }
            return result;
        }
    }
}
