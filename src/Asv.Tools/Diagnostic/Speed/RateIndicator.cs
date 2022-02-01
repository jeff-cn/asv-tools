using System;
using System.Reactive.Linq;
using System.Threading;

namespace Asv.Tools
{
    public class RateIndicator : IDisposable
    {
        private readonly IDisposable _subscription;
        private int _value;
        private DateTime _lastUpdate;

        public RateIndicator(IDigitDiagnostic<double> source, string name, string format = null, TimeSpan? lifeTime = null, TimeSpan? updateTime = null)
        {
            var time = updateTime ?? TimeSpan.FromSeconds(5);
            if (lifeTime.HasValue)
            {
                source[name, format ?? "0.00 Hz", lifeTime.Value] = 0;
            }
            else
            {
                source[name, format ?? "0.00 Hz"] = 0;
            }
            
            _subscription = Observable.Timer(TimeSpan.FromSeconds(1), time).Subscribe(_ =>
            {
                var delay = DateTime.Now - _lastUpdate;
                _lastUpdate = DateTime.Now;
                var value = Interlocked.Exchange(ref _value, 0);
                source[name] = (double)value / delay.TotalSeconds;
            });
        }

        public void Increment(int value)
        {
            Interlocked.Add(ref _value, value);
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }
    }
}
