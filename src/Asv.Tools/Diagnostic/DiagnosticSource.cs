using System.Collections.Concurrent;

namespace Asv.Tools
{
    public class DiagnosticSource : IDiagnosticSource
    {
        public DiagnosticSource(string group, ConcurrentDictionary<DiagnosticKey, DiagnosticItem> values)
        {
            GroupName = group;
            Real = new DoubleDiagnostic(group,values);
            Int = new IntegerDiagnostic(group, values);
            Str = new StringDiagnostic(group,values);
            Rate = new RateDiagnostic(Real);
        }

        public string GroupName { get; }
        public IDigitDiagnostic<double> Real { get; }
        public IDigitDiagnostic<int> Int { get; }
        public IStringDiagnostic Str { get; }
        public IRateDiagnostic Rate { get; }

        public void Dispose()
        {
            Real.Dispose();
            Int.Dispose();
            Str.Dispose();
            Rate.Dispose();
        }
    }
}
