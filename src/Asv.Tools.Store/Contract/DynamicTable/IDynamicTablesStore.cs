using System;
using System.Collections.Generic;
using DynamicData;

namespace Asv.Tools.Store
{

    public interface IDynamicTableInfo
    {
        Guid TableId { get; }
    }

    public interface IColumnStatistic
    {
        int Count { get; }
    }

    public interface IDynamicTablesStore
    {
        string Name { get; }
        IObservableCache<IDynamicTableInfo,Guid> Tables { get; }

        IColumnStatistic GetColumnStatistic(Guid tableId, string groupName, string columnName);
        bool TryGetCell(Guid tableId, string groupName, string columnName, int rowIndex, out double value);
        void Push(Guid tableId, string groupName, string columnName, int rowIndex, double value);
    }

    
}
