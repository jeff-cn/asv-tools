using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LiteDB;

namespace Asv.Tools.Store
{
    public class LiteDbSimpleSeries<TRecord, TKey>:ISimpleSeries<TRecord>
    {
        private readonly LiteCollection<TRecord> _collection;

        public LiteDbSimpleSeries(string name, LiteCollection<TRecord> collection, Expression<Func<TRecord, TKey>> property)
        {
            _collection = collection;
            BsonMapper.Global.Entity<TRecord>().Id(property);
        }

        public int GetRecordsCount()
        {
            return _collection.Count();
        }

        public IEnumerable<TRecord> ReadAll()
        {
            return _collection.FindAll();
        }

        public void Push(TRecord record)
        {
            _collection.Upsert(record);
        }

        public void ClearAll()
        {
            _collection.Delete(_ => true);
        }

        public void Dispose()
        {

        }
    }
}
