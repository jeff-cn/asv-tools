using System;
using System.Collections.Concurrent;

namespace Asv.Tools
{
    public class RateDiagnostic : IRateDiagnostic
    {
        private readonly IDigitDiagnostic<double> _src;
        private readonly ConcurrentDictionary<string, RateIndicator> _indicators = new ConcurrentDictionary<string, RateIndicator>();

        public RateDiagnostic(IDigitDiagnostic<double> src)
        {
            _src = src;
        }

        public void Dispose()
        {
            foreach (var keyValuePair in _indicators)
            {
                keyValuePair.Value.Dispose();
            }
            _indicators.Clear();
        }

        public RateIndicator this[string name] => _indicators.GetOrAdd(name, _=> _src.CreateSpeedIndicator(_));

        public RateIndicator
            this[string name, string format, TimeSpan? lifeTime = null, TimeSpan? updateTime = null] =>
            _indicators.GetOrAdd(name, _src.CreateSpeedIndicator(name, format, lifeTime, updateTime));

    }
}
