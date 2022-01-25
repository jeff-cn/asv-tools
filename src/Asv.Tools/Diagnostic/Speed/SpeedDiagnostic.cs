using System;
using System.Collections.Concurrent;

namespace Asv.Tools
{
    public class SpeedDiagnostic : ISpeedDiagnostic
    {
        private readonly IDigitDiagnostic<double> _src;
        private readonly ConcurrentDictionary<string, SpeedIndicator> _indicators = new ConcurrentDictionary<string, SpeedIndicator>();

        public SpeedDiagnostic(IDigitDiagnostic<double> src)
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

        public SpeedIndicator this[string name] => _indicators.GetOrAdd(name, _=> _src.CreateSpeedIndicator(_));

        public SpeedIndicator
            this[string name, string format, TimeSpan? lifeTime = null, TimeSpan? updateTime = null] =>
            _indicators.GetOrAdd(name, _src.CreateSpeedIndicator(name, format, lifeTime, updateTime));

    }
}
