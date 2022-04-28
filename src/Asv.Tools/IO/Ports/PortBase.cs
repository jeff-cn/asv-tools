using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace Asv.Tools
{
    public abstract class PortBase : IPort
    {
        private readonly CancellationTokenSource _disposedCancel = new();
        private int _isEvaluating;
        private readonly RxValue<Exception> _portErrorStream = new();
        private readonly RxValue<PortState> _portStateStream = new();
        private readonly RxValue<bool> _enableStream = new();
        private readonly Subject<byte[]> _outputData = new();
        private long _rxBytes;
        private long _txBytes;
        public IRxValue<bool> IsEnabled => _enableStream;
        public long RxBytes => Interlocked.Read(ref _rxBytes);
        public long TxBytes => Interlocked.Read(ref _txBytes);
        public abstract PortType PortType { get; }
        public TimeSpan ReconnectTimeout { get; set; } = TimeSpan.FromSeconds(5);
        public IRxValue<PortState> State => _portStateStream;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private int _errCnt;
        private DateTime _lastSuccess;
        private int _isDisposed;

        #region Metrics

        protected static readonly Meter Metric = new("Asv.Tools.IO");
        protected KeyValuePair<string, object> MetricProbeTag;
        private static readonly Counter<long> _probeTx = Metric.CreateCounter<long>("tx", "bytes", "Transmit rate");
        private static readonly Counter<long> _probeRx = Metric.CreateCounter<long>("rx", "bytes", "Received rate");
        private ObservableCounter<int> _probeError;
        

        #endregion

        protected PortBase()
        {
            _probeError = Metric.CreateObservableCounter("err", () => new Measurement<int>(_errCnt,MetricProbeTag) , "items", "Connection errors");

            State.Where(_ => _ == PortState.Connected).Subscribe(_ =>
            {
                _lastSuccess = DateTime.Now;
                _logger.Info($"Port {this}: {_:G}");
            });
            State.Where(_ => _ == PortState.Error).Subscribe(_ =>
            {
                _errCnt++;
            });
            _enableStream.Where(_ => _).Subscribe(_ => Task.Factory.StartNew(TryConnect), _disposedCancel.Token);

            
            Observable.Timer(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(60)).Subscribe(_ =>
            {
                if (State.Value == PortState.Connected)
                {
                    _logger.Info($"Port {PortLogName}: work {DateTime.Now - _lastSuccess:g}; rx:{_rxBytes / 1024:F0} KiB; tx:{_rxBytes / 1024:F0} KiB; conn err:{_errCnt};");
                }
                else
                {
                    _logger.Info($"Port error {PortLogName}: {Error.Value?.Message}");
                }
            }, _disposedCancel.Token);
        }

        public abstract string PortLogName { get; }

        public async Task<bool> Send(byte[] data, int count, CancellationToken cancel)
        {
            if (!IsEnabled.Value) return false;
            if (_portStateStream.Value != PortState.Connected) return false;
            try
            {
                await InternalSend(data, count, cancel).ConfigureAwait(false);
                _probeTx.Add(count,MetricProbeTag);
                Interlocked.Add(ref _txBytes, count);
                return true;
            }
            catch (Exception exception)
            {
                _logger.Error($"Error to send data to port:{exception.Message}");
                InternalOnError(exception);
                return false;
            }
        }

        public IRxValue<Exception> Error => _portErrorStream;

        public void Enable()
        {
            _enableStream.OnNext(true);
        }

        public void Disable()
        {
            _enableStream.OnNext(false);
            _portStateStream.OnNext(PortState.Disabled);
            Task.Factory.StartNew(Stop);
        }

        private void Stop()
        {
            try
            {
                InternalStop();
            }
            catch (Exception ex)
            {
                Debug.Assert(true,ex.Message);
            }
            finally
            {
                // ignore
            }
        }

        protected CancellationToken DisposeCancel => _disposedCancel.Token;


        private void TryConnect()
        {
            if (Interlocked.CompareExchange(ref _isEvaluating, 1, 0) != 0) return;
            try
            {
                if (!_enableStream.Value) return;

                if (_disposedCancel.IsCancellationRequested) return;
                _portStateStream.OnNext(PortState.Connecting);
                MetricProbeTag = new KeyValuePair<string, object>("cs", ToString());
                InternalStart();
                _portStateStream.OnNext(PortState.Connected);
            }
            catch (Exception e)
            {
                InternalOnError(e);
            }
            finally
            {
                Interlocked.Exchange(ref _isEvaluating, 0);
            }
        }

        protected abstract Task InternalSend(byte[] data, int count, CancellationToken cancel);

        protected abstract void InternalStop();

        protected abstract void InternalStart();

        protected void InternalOnData(byte[] data)
        {
            try
            {
                Interlocked.Add(ref _rxBytes, data.Length);
                _outputData.OnNext(data);
                _probeRx.Add(data.Length, MetricProbeTag);
            }
            catch (Exception ex)
            {
                // ignored
            }
            finally
            {
                // ignore
            }
        }

        protected void InternalOnError(Exception exception)
        {
            _portStateStream.OnNext(PortState.Error);
            _portErrorStream.OnNext(exception);
            Observable.Timer(ReconnectTimeout).Subscribe(_ => TryConnect(), _disposedCancel.Token);
            Stop();
        }


        public IDisposable Subscribe(IObserver<byte[]> observer)
        {
            return _outputData.Subscribe(observer);
        }

        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref _isDisposed,1,0) != 0) return;
            Disable();
            _portErrorStream.Dispose();
            _portStateStream.Dispose();
            _enableStream.Dispose();
            _outputData.OnCompleted();
            _outputData.Dispose();
            InternalDisposeOnce();
        }

        protected virtual void InternalDisposeOnce()
        {

        }
    }
}
