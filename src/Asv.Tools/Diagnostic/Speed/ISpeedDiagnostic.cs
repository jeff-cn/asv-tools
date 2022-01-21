using System;

namespace Asv.Tools
{
    public interface ISpeedDiagnostic:IDisposable
    {
        SpeedIndicator this[string name] { get; }
        SpeedIndicator this[string name, string format, TimeSpan? lifeTime = null, TimeSpan? updateTime = null] { get; }
    }
}
