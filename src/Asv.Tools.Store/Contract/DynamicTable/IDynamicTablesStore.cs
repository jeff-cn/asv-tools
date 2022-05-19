using System;
using System.Collections.Generic;
using DynamicData;
using LiteDB;

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
        bool TryReadDoubleCell(Guid tableId, string groupName, string columnName, int rowIndex, out double value);
        bool TryReadEnumCell<T>(Guid tableId, string groupName, string columnName, int rowIndex, out T value)
            where T:Enum;
        void UpsetDoubleCell(Guid tableId, string groupName, string columnName, int rowIndex, double value);
        void UpsetEnumCell<T>(Guid tableId, string groupName, string columnName, int rowIndex, T value)
            where T : Enum;
        IDynamicTableStatistic GetTableStatistic(Guid tableId);
        int ObserveAll(Guid tableId, Action<IDynamicTablesRawObserver> callback);
        void AddRaw(Guid tableId, Action<IDynamicTablesRawObserver> callback);
        bool RemoveRaw(Guid tableId, int rawIndex);
    }

    public interface IDynamicTablesRawObserver
    {
        Guid TableId { get; }
        int RawIndex { get; }
        IEnumerable<string> Columns { get; }
        bool TryReadDoubleCell(string groupName, string columnName, out double value);
        bool TryReadEnumCell<T>(string groupName, string columnName, out T value)
            where T:Enum;
        void WriteDoubleCell(string groupName, string columnName, double value);
        void WriteEnumCell<T>(string groupName, string columnName, T value)
            where T : Enum;
    }

    
}
