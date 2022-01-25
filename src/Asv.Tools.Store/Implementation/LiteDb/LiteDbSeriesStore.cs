using System.Collections.Generic;
using System.Linq;
using LiteDB;

namespace Asv.Tools.Store
{
    public abstract class LiteDbSeriesStore<TXValue, TYValue> : ISeriesValueStore<TXValue, TYValue> where TXValue : struct
    {
        protected const string XName = "_id";

        private readonly LiteCollection<SeriesPoint<TXValue, TYValue>> _coll;

        public LiteDbSeriesStore(string name, LiteCollection<SeriesPoint<TXValue, TYValue>> collection)
        {
            Name = name;
            _coll = collection;
            _coll.EnsureIndex(_=>_.X, true);
        }

        public void Push(SeriesPoint<TXValue, TYValue> point)
        {
            if (!_coll.Update(point))
            {
                _coll.Insert(point);
            }
            
        }

        public IEnumerable<SeriesPoint<TXValue, TYValue>> Read(SeriesQuery<TXValue> query)
        {
            var list = new List<Query>();
            if (query.From.HasValue)
            {
                list.Add(Query.GTE(XName, new BsonValue(query.From.Value)));
            }
            if (query.To.HasValue)
            {
                list.Add(Query.LTE(XName, new BsonValue(query.To.Value)));
            }
          
            list.Add(Query.All(XName, Query.Ascending));

            var q = list.Count == 1 ? list.First() : Query.And(list.ToArray());

            return _coll.Find(q, query.Skip, query.Take);
        }

        public abstract TXValue GetXMinValue();

        public abstract TXValue GetXMaxValue();
        

        public void ClearAll()
        {
            _coll.Delete(_ => true);
        }

        public string Name { get; }
    }

    public class LiteDbIntSeriesStore<TYValue> : LiteDbSeriesStore<int, TYValue>
    {
        private readonly LiteCollection<SeriesPoint<int, TYValue>> _collection;

        public LiteDbIntSeriesStore(string name, LiteCollection<SeriesPoint<int, TYValue>> collection) : base(name, collection)
        {
            _collection = collection;
        }

        public override int GetXMinValue()
        {
            return _collection.Min(XName);
        }

        public override int GetXMaxValue()
        {
            return _collection.Max(XName);
        }
    }

    public class LiteDbDoubleSeriesStore<TYValue> : LiteDbSeriesStore<double, TYValue>
    {
        private readonly LiteCollection<SeriesPoint<double, TYValue>> _collection;

        public LiteDbDoubleSeriesStore(string name, LiteCollection<SeriesPoint<double, TYValue>> collection) : base(name,collection)
        {
            _collection = collection;
        }

        public override double GetXMinValue()
        {
            
            return _collection.Min(XName);
        }

        public override double GetXMaxValue()
        {
            return _collection.Max(XName);
        }
    }
}
