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

    public interface IDynamicTableStatistic
    {
        int RowCount { get; }
        int ColumnCount { get; }
    }

    public interface IDynamicTablesStore
    {
        string Name { get; }
        IEnumerable<IDynamicTableInfo> Tables { get; }

        IColumnStatistic GetColumnStatistic(Guid tableId, string groupName, string columnName);
        bool TryGetCell(Guid tableId, string groupName, string columnName, int rowIndex, out double value);
        void Push(Guid tableId, string groupName, string columnName, int rowIndex, double value);
        IDynamicTableStatistic GetTableStatistic(Guid tableId);
    }

    
}
