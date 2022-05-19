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
        private const string IdColumnName = "_id";
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

        public bool TryReadDoubleCell(Guid tableId, string groupName, string columnName, int rowIndex, out double value)
        {
            var doc = TryGetBsonRaw(tableId, rowIndex);
            if (doc == null)
            {
                value = double.NaN;
                return false;
            }
            return TryGetDouble(doc,groupName, columnName, out value);
        }

        public bool TryReadEnumCell<T>(Guid tableId, string groupName, string columnName, int rowIndex, out T value) where T : Enum
        {
            var doc = TryGetBsonRaw(tableId, rowIndex);
            if (doc == null)
            {
                value = default;
                return false;
            }
            return TryGetEnum(doc, groupName, columnName, out value);
        }

        private BsonDocument TryGetBsonRaw(Guid tableId, int rowIndex)
        {
            var tableColl = GetCollection(tableId);
            return tableColl.FindById(rowIndex);
        }

        private string GetSubCollectionName(int id) => $"{_subCollPrefix}{id:000}";

        public void UpsetDoubleCell(Guid tableId, string groupName, string columnName, int rowIndex, double value)
        {
            var tableColl = GetCollection(tableId);
            var doc = tableColl.FindById(rowIndex);
            
            if (doc == null)
            {
                doc = new BsonDocument { {IdColumnName, rowIndex }};
                WriteDoubleCell(doc,groupName,columnName,value);
                tableColl.Insert(doc);
            }
            else
            {
                WriteDoubleCell(doc, groupName, columnName, value);
                tableColl.Update(doc);
            }
        }

        public void UpsetEnumCell<T>(Guid tableId, string groupName, string columnName, int rowIndex, T value) where T : Enum
        {
            var tableColl = GetCollection(tableId);
            var doc = tableColl.FindById(rowIndex);

            if (doc == null)
            {
                doc = new BsonDocument { {IdColumnName, rowIndex } };
                WriteEnumCell(doc, groupName, columnName, value);
                tableColl.Insert(doc);
            }
            else
            {
                WriteEnumCell(doc, groupName, columnName, value);
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

        public int ObserveAll(Guid tableId, Action<IDynamicTablesRawObserver> callback)
        {
            var coll = _indexColl.FindOne(_ => _.TableId == tableId);
            if (coll == null) return 0;
            var subCollection = _db.GetCollection(GetSubCollectionName(coll.Id), BsonAutoId.Int32);
            var count = 0;
            foreach (var doc in subCollection.FindAll())
            {
                count++;
                var editor = new DynamicTablesRawObserver(doc, tableId);
                callback(editor);
                if (editor.IdEdited)
                {
                    subCollection.Update(doc);
                }
            }
            return count;
        }

        public void AddRaw(Guid tableId, Action<IDynamicTablesRawObserver> callback)
        {
            var tableColl = GetCollection(tableId);
            var id = tableColl.Insert(new BsonDocument());
            var doc = tableColl.FindById(id);
            var editor = new DynamicTablesRawObserver(doc, tableId);
            callback(editor);
            if (editor.IdEdited)
            {
                tableColl.Update(doc);
            }
        }

        public bool RemoveRaw(Guid tableId, int rawIndex)
        {
            var tableColl = GetCollection(tableId);
            return tableColl.Delete(rawIndex);
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

        internal static bool TryGetDouble(BsonDocument doc, string groupName, string columnName, out double value)
        {
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

        private static string CreateColumnName(string groupName, string columnName) => $"{groupName}.{columnName}";

        public static bool TryGetEnum<T>(BsonDocument doc, string groupName, string columnName, out T value)
            where T : Enum
        {
            var fieldKey = CreateColumnName(groupName, columnName);
            var val = doc[fieldKey];
            if (val.IsNull)
            {
                value = default;
                return false;
            }

            value = (T)Enum.Parse(typeof(T), val.AsString, true);
            return true;
        }

        public static void WriteDoubleCell(BsonDocument doc, string groupName, string columnName, double value)
        {
            doc[CreateColumnName(groupName, columnName)] = value;
        }

        public static void WriteEnumCell<T>(BsonDocument doc, string groupName, string columnName, T value) where T : Enum
        {
            doc[CreateColumnName(groupName, columnName)] = value.ToString();
        }
    }

    public class DynamicTablesRawObserver : IDynamicTablesRawObserver
    {
        private readonly BsonDocument _doc;

        public DynamicTablesRawObserver(BsonDocument doc, Guid tableId)
        {
            TableId = tableId;
            _doc = doc;
        }

        public Guid TableId { get; }
        public int RawIndex => _doc["_id"];

        public IEnumerable<string> Columns => _doc.Keys;

        internal bool IdEdited { get; private set; }

        public bool TryReadDoubleCell(string groupName, string columnName, out double value)
        {
            return LiteDbDynamicTablesStore.TryGetDouble(_doc, groupName, columnName, out value);
        }

        public bool TryReadEnumCell<T>(string groupName, string columnName, out T value) where T : Enum
        {
            return LiteDbDynamicTablesStore.TryGetEnum(_doc, groupName, columnName, out value);
        }

        public void WriteDoubleCell(string groupName, string columnName, double value)
        {
            LiteDbDynamicTablesStore.WriteDoubleCell(_doc, groupName, columnName, value);
            IdEdited = true;
        }

        public void WriteEnumCell<T>(string groupName, string columnName, T value)
            where T : Enum
        {
            LiteDbDynamicTablesStore.WriteEnumCell(_doc, groupName, columnName, value);
            IdEdited = true;
        }
    }
}
