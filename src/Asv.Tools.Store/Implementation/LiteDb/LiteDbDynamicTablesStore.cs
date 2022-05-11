using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DynamicData;
using DynamicData.Kernel;
using LiteDB;

namespace Asv.Tools.Store
{

    public class LiteDbDynamicTableDescription: IDynamicTableInfo
    {
        [BsonId]
        public int Id { get; set; }
        public Guid TableId { get; set; }
    }

    public class ColumnStatistic : IColumnStatistic
    {
        public ColumnStatistic(int count)
        {
            Count = count;
        }

        public int Count { get; }
    }

    public class DynamicTableStatistic : IDynamicTableStatistic
    {
        public DynamicTableStatistic(int rowCount, int columnCount)
        {
            RowCount = rowCount;
            ColumnCount = columnCount;
        }

        public int RowCount { get; }
        public int ColumnCount { get; }
    }

    public class LiteDbDynamicTablesStore:IDynamicTablesStore
    {
        private readonly LiteDatabase _db;
        private readonly string _subCollPrefix;
        private readonly ILiteCollection<LiteDbDynamicTableDescription> _indexColl;
        private readonly ConcurrentDictionary<Guid,ILiteCollection<BsonDocument>> _indexCache = new ConcurrentDictionary<Guid, ILiteCollection<BsonDocument>>();

        public LiteDbDynamicTablesStore(string name, LiteDatabase db, string indexCollectionName, string subCollPrefix)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or empty.", nameof(name));
            if (string.IsNullOrWhiteSpace(indexCollectionName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(indexCollectionName));
            if (string.IsNullOrWhiteSpace(subCollPrefix))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(subCollPrefix));
            Name = name;
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _subCollPrefix = subCollPrefix;
            _indexColl = _db.GetCollection<LiteDbDynamicTableDescription>(indexCollectionName, BsonAutoId.Int32);
            _indexColl.EnsureIndex(_ => _.TableId, true);
        }

        public string Name { get; }
        public IEnumerable<IDynamicTableInfo> Tables => _indexColl.FindAll();

        public IColumnStatistic GetColumnStatistic(Guid tableId, string groupName, string columnName)
        {
            var tableColl = GetCollection(tableId);
            return new ColumnStatistic(tableColl.Count());
        }

        public bool TryGetCell(Guid tableId, string groupName, string columnName, int rowIndex, out double value)
        {
            var tableColl = GetCollection(tableId);
            var doc = tableColl.FindById(rowIndex);
            if (doc == null)
            {
                value = double.NaN;
                return false;
            }

            var fieldKey = CreateColumnName(groupName, columnName);
            var val = doc[fieldKey];
            if (val.IsNull)
            {
                value = double.NaN;
                return false;
            }
            value = val;
            return true;
        }

        private string CreateColumnName(string groupName, string columnName) => $"{groupName}.{columnName}";
        private string GetSubCollectionName(int id) => $"{_subCollPrefix}{id:000}";

        public void Push(Guid tableId, string groupName, string columnName, int rowIndex, double value)
        {
            var tableColl = GetCollection(tableId);
            var doc = tableColl.FindById(rowIndex);
            var column = CreateColumnName(groupName, columnName);
            if (doc == null)
            {
                tableColl.Insert(new BsonDocument { {"_id",rowIndex}, { column, value } });
            }
            else
            {
                doc[column] = value;
                tableColl.Update(doc);
            }
        }

        public IDynamicTableStatistic GetTableStatistic(Guid tableId)
        {
            var coll = _indexColl.FindOne(_ => _.TableId == tableId);
            if (coll == null) return null;
            var subCollection = _db.GetCollection(GetSubCollectionName(coll.Id), BsonAutoId.Int32);
            var first = subCollection.FindAll().FirstOrDefault();
            var count = (first == null) ? 0 : (first.Count - 1);
            return new DynamicTableStatistic(subCollection.Count(), count);
        }

        private ILiteCollection<BsonDocument> GetCollection(Guid sessionId)
        {
            if (_indexCache.TryGetValue(sessionId, out var value))
            {
                return value;
            }

            var coll = _indexColl.FindOne(_=>_.TableId == sessionId);
            ILiteCollection<BsonDocument> subCollection;
            if (coll == null)
            {
                var id = (int)_indexColl.Insert(new LiteDbDynamicTableDescription{ TableId = sessionId});
                subCollection = _db.GetCollection(GetSubCollectionName(id), BsonAutoId.Int32);
            }
            else
            {
                subCollection = _db.GetCollection(GetSubCollectionName(coll.Id), BsonAutoId.Int32);
            }
            return _indexCache.AddOrUpdate(sessionId, _ => subCollection, (guid, collection) => subCollection);
        }
        
    }
}
