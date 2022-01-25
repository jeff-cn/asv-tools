using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Asv.Tools.Tcp;

namespace Asv.Tools
{
    public class TelnetStream : ITextStream
    {
        private enum State
        {
            Start,
            Process
        }

        private readonly char[] _endBytes = {'\r', '\n'};
        
        private readonly CancellationTokenSource _cancel = new CancellationTokenSource();
        private readonly TelnetConfig _config;
        private State _sync = State.Start;
        private int _readIndex = 0;
        private readonly Subject<string> _output = new Subject<string>();
        private readonly Subject<Exception> _onErrorSubject = new Subject<Exception>();
        private readonly byte[] _buffer;
        private readonly IDataStream _input;

        public TelnetStream(TelnetConfig config)
        {
            _cancel.Token.Register((Action)(() => _output.Dispose()));
            _config = config ?? new TelnetConfig();
            _buffer = new byte[_config.MaxMessageSize];
            _input = ConnectionStringConvert(_config.ConnectionString);
            _input.SelectMany<byte[], byte>((Func<byte[], IEnumerable<byte>>)(_ => (IEnumerable<byte>)_)).Subscribe<byte>(new Action<byte>(this.OnData), this._cancel.Token);
        }

        private static IDataStream ConnectionStringConvert(string connString)
        {
            var p = (TcpClientPort)PortFactory.Create(connString);
            p.Enable();
            return p;
        }

        private void OnData(byte data)
        {
            switch (_sync)
            {
                case State.Start:
                    _sync = (data != (byte)_endBytes[0] && data != (byte)_endBytes[1]) ? State.Process : State.Start;
                    _readIndex = 0;
                    if (_sync == State.Process)
                    {
                        _buffer[_readIndex] = data;
                        ++_readIndex;
                    }
                    break;
                case State.Process:
                    if (data == (byte)_endBytes[0] || data == (byte)_endBytes[1])
                    {
                        _sync = State.Start;
                        try
                        {
                            _output.OnNext(_config.DefaultEncoding.GetString(_buffer, 0, _readIndex));
                        }
                        catch (Exception ex)
                        {
                            _onErrorSubject.OnNext(ex);
                        }
                    }
                    else
                    {
                        _buffer[_readIndex] = data;
                        ++_readIndex;
                        if (_readIndex >= _config.MaxMessageSize)
                        {
                            _onErrorSubject.OnNext(new Exception(string.Format("Receive buffer overflow. Max message size={0}", (object)this._config.MaxMessageSize)));
                            _sync = State.Start;
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Dispose()
        {
            _cancel.Cancel(false);
        }

        public IDisposable Subscribe(IObserver<string> observer)
        {
            return _output.Subscribe(observer);
        }

        public IObservable<Exception> OnError => (IObservable<Exception>)_onErrorSubject;

        public async Task Send(string value, CancellationToken cancel)
        {
            var linkedCancel = (CancellationTokenSource)null;
            try
            {
                linkedCancel = CancellationTokenSource.CreateLinkedTokenSource(cancel, _cancel.Token);
                var data = _config.DefaultEncoding.GetBytes(value + _endBytes[0] + _endBytes[1]);
                await _input.Send(data, data.Length, linkedCancel.Token);
            }
            catch (Exception ex)
            {
                _onErrorSubject.OnNext(new Exception(string.Format("Error to send text stream data '{0}':{1}", (object)value, (object)ex.Message), ex));
                throw;
            }
            finally
            {
                linkedCancel?.Dispose();
            }
        }
    }
}