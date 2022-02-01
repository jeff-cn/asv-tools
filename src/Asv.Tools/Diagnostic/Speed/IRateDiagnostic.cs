using System;

namespace Asv.Tools
{
    public interface IRateDiagnostic:IDisposable
    {
        RateIndicator this[string name] { get; }
        RateIndicator this[string name, string format, TimeSpan? lifeTime = null, TimeSpan? updateTime = null] { get; }
    }
}
