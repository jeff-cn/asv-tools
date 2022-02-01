using System;

namespace Asv.Tools
{
    public interface IDiagnosticSource:IDisposable
    {
        string GroupName { get; }
        IDigitDiagnostic<double> Real { get; }
        IDigitDiagnostic<int> Int { get; }
        IStringDiagnostic Str { get; }
        IRateDiagnostic Rate { get; }
    }
}
