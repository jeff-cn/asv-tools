using System;
using System.Collections.Generic;

namespace Asv.Tools.Store
{
    public interface ISimpleSeries<TRecord>:IDisposable
    {
        int GetRecordsCount();
        IEnumerable<TRecord> ReadAll();
        void Push(TRecord record);
        void ClearAll();
    }
}
