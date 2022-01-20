using System;
using Asv.Tools.Serial;
using Asv.Tools.Tcp;
using Asv.Tools.Udp;

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
        public static IPort Create(string connectionString)
        {
            var uri = new Uri(connectionString);

            TcpPortConfig tcp;
            if (TcpPortConfig.TryParseFromUri(uri, out tcp))
            {
                if (tcp.IsServer)
                {
                    return new TcpServerPort(tcp);
                }
                else
                {
                    return new TcpClientPort(tcp);
                }
            }

            UdpPortConfig udp;
            if (UdpPortConfig.TryParseFromUri(uri, out udp)) return new UdpPort(udp);

            SerialPortConfig ser;
            if (SerialPortConfig.TryParseFromUri(uri, out ser)) return new CustomSerialPort(ser);



            throw new Exception(string.Format("Connection string '{0}' is invalid", connectionString));
        }
    }
}
