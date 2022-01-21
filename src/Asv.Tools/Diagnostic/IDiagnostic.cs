using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Asv.Tools
{


    public interface IDiagnostic: IDisposable
    {
        IEnumerable<KeyValuePair<DiagnosticKey, DiagnosticItem>> GetItems();
        void ClearItems();
        IDiagnosticSource this[string group] { get; }
    }


    public enum DiagnosticItemType
    {
        String,
        Real,
        Integer
    }

    public class DiagnosticItem
    {
        public DiagnosticItemType ItemType { get; set; }
        public int IntValue { get; set; }
        public double RealValue { get; set; }
        public string StrValue { get; set; }

        public string FormatString { get; set; }
        public DateTime LastUpdate { get; set; }
        public TimeSpan? LifeTime { get; set; }

        public override string ToString()
        {
            switch (ItemType)
            {
                case DiagnosticItemType.String:
                    return StrValue;
                case DiagnosticItemType.Real:
                    return FormatString == null ? RealValue.ToString(CultureInfo.InvariantCulture) : RealValue.ToString(FormatString, CultureInfo.InvariantCulture);
                case DiagnosticItemType.Integer:
                    return FormatString == null ? IntValue.ToString(CultureInfo.InvariantCulture) : IntValue.ToString(FormatString, CultureInfo.InvariantCulture);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public interface IDigitDiagnostic<T>: IDisposable
    {
        string GroupName { get; }
        T this[string name] { get; set; }
        T this[string name, string format] { set; }
        T this[string name, string format, TimeSpan lifeTime] { set; }
        T this[string name, TimeSpan lifeTime] { set; }
    }

    public interface IStringDiagnostic: IDisposable
    {
        string GroupName { get; }
        string this[string name] { get; set; }
        string this[string name, TimeSpan lifeTime] { set; }
    }

    public static class SimpleDiagnosticHelper
    {
        public static void Print(this IDiagnostic src, Action<string> printCallback, TextTableBorder border = null)
        {
            foreach (var group in src.GetItems().GroupBy(_=>_.Key.Group))
            {
                TextTable.PrintKeyValue(printCallback, border ?? new DoubleTextTableBorder(), 20,40,group.Key, group.OrderBy(_ => _.Key.Param).Select(_=>new KeyValuePair<string, string>(_.Key.Param,_.Value.ToString())));
            }
        }

        public static SpeedIndicator CreateSpeedIndicator(this IDiagnosticSource src, string name, string format = null,
            TimeSpan? lifeTime = null, TimeSpan? updateTime = null)
        {
            return new SpeedIndicator(src.Real, name, format, lifeTime, updateTime);
        }

        public static SpeedIndicator CreateSpeedIndicator(this IDigitDiagnostic<double> src, string name, string format = null,
            TimeSpan? lifeTime = null, TimeSpan? updateTime = null)
        {
            return new SpeedIndicator(src, name, format, lifeTime, updateTime);
        }
    }


    
}
