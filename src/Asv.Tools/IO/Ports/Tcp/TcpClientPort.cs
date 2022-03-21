using System;
using System.Buffers;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Asv.Tools.Tcp
{
    public class TcpClientPort:PortBase
    {
        private TcpPortConfig _cfg;
        private TcpClient _tcp;
        private CancellationTokenSource _stop;
        private DateTime _lastData;
        private static int _counter;

        public TcpClientPort(TcpPortConfig cfg)
        {
            _cfg = cfg;
        }

        public override PortType PortType { get; } = PortType.Tcp;

        public override string PortLogName => _cfg.ToString();

        protected override Task InternalSend(byte[] data, int count, CancellationToken cancel)
        {
            if (_tcp == null || _tcp.Connected == false) return Task.CompletedTask;
            return _tcp.GetStream().WriteAsync(data, 0, count, cancel);
        }

        protected override void InternalStop()
        {
            _tcp?.Close();
            _tcp?.Dispose();
            _stop?.Cancel(false);
            _stop?.Dispose();
            _stop = null;

        }

        protected override void InternalStart()
        {
            _counter++;
            _tcp?.Close();
            _tcp?.Dispose();
            _tcp = new TcpClient();
            _tcp.Connect(_cfg.Host,_cfg.Port);
            _stop = new CancellationTokenSource();
            var recvThread = new Thread(ListenAsync) { Name = "TCP_C"+_counter,IsBackground = true, Priority = ThreadPriority.Normal };
            _stop.Token.Register(() =>
            {
                try
                {
                    _tcp?.Close();
                    _tcp?.Dispose();
                    recvThread.Interrupt();
                }
                catch (Exception e)
                {
                    Debug.Assert(false);
                    // ignore
                }
            });
            recvThread.Start(_stop);

        }

        private void ListenAsync(object obj)
        {
            var cancellationTokenSource = (CancellationTokenSource)obj;
            try
            {
                while (cancellationTokenSource.IsCancellationRequested == false)
                {
                    if (_cfg.ReconnectTimeout != 0)
                    {
                        if ((DateTime.Now - _lastData).TotalMilliseconds > _cfg.ReconnectTimeout)
                        {
                            _tcp.GetStream().Write(Array.Empty<byte>(), 0, 0);
                        }
                    }
                    if (_tcp.Available != 0)
                    {
                        _lastData = DateTime.Now;
                        var buff = new byte[_tcp.Available];
                        var readed = _tcp.GetStream().Read(buff, 0, buff.Length);
                        Debug.Assert(readed == buff.Length);
                        if (readed != 0) InternalOnData(buff);
                    }
                    else
                    {
                        Thread.Sleep(30);
                    }
                }
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.Interrupted) return;
                InternalOnError(ex);
            }
            catch (ThreadAbortException e)
            {
                Debug.Assert(false);
                //ignore
            }
            catch (Exception e)
            {
                InternalOnError(e);
            }
        }

        protected override void InternalDisposeOnce()
        {
            base.InternalDisposeOnce();
            _tcp?.Dispose();
        }

        public override string ToString()
        {
            try
            {
                return $"TCP\\IP Client      {_tcp?.Client?.LocalEndPoint}:\n" +
                       $"Reconnect timeout   {_cfg.ReconnectTimeout} ms\n" +
                       $"Remote server       {_cfg.Host}:{_cfg.Port}";
            }
            catch (Exception e)
            {
                return $"TCP\\IP Client      \n" +
                       $"Reconnect timeout   {_cfg.ReconnectTimeout} ms\n" +
                       $"Remote server       {_cfg.Host}:{_cfg.Port}";
                Debug.Assert(false);
            }
            
        }

    }
}
