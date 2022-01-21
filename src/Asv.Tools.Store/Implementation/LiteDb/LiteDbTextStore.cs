using System.Collections.Generic;
using System.Linq;
using LiteDB;

namespace Asv.Tools.Store
{
    public class LiteDbTextStore : ITextStore
    {
        private readonly LiteCollection<TextMessage> _coll;

        public LiteDbTextStore(LiteCollection<TextMessage> coll)
        {
            _coll = coll;
            _coll.EnsureIndex(_ => _.IntTag);
            _coll.EnsureIndex(_ => _.StrTag);
            _coll.EnsureIndex(_ => _.Date);
            _coll.EnsureIndex(_ => _.Text);
        }

        public IEnumerable<TextMessage> Find(TextMessageQuery query)
        {
            return _coll.Find(Convert(query), query.Skip, query.Take);
        }

        private static Query Convert(TextMessageQuery query)
        {
            var list = new List<Query>();
            if (query.Begin.HasValue)
            {
                list.Add(Query.GTE(nameof(TextMessageQuery.Begin), query.Begin.Value));
            }
            if (query.End.HasValue)
            {
                list.Add(Query.LTE(nameof(TextMessageQuery.Begin), query.End.Value));
            }
            if (query.IntTags != null && query.IntTags.Length > 0)
            {
                var qq = query.IntTags.Select(_ => Query.EQ(nameof(TextMessage.IntTag), _)).ToArray();
                list.Add(qq.Length == 1 ? qq.First() : Query.Or(qq));
            }

            if (query.StrTags != null && query.StrTags.Length > 0)
            {
                list.Add(Query.Or(query.StrTags.Select(_ => Query.EQ(nameof(TextMessage.StrTag), _)).ToArray()));
            }

            if (!query.Search.IsNullOrWhiteSpace())
            {
                list.Add(Query.Contains(nameof(TextMessage.Text), query.Search));
            }

            list.Add(Query.All(nameof(TextMessage.Date), query.OrderAscending ? Query.Ascending : Query.Descending));

            var q = list.Count == 1 ? list.First() : Query.And(list.ToArray());
            return q;
        }

        public int Count(TextMessageQuery query)
        {
            return _coll.Count(Convert(query));
        }

        public void Insert(TextMessage textMessage)
        {
            _coll.Insert(textMessage);
        }

        public void ClearAll()
        {
            _coll.Delete(_ => true);
        }

        public int Count()
        {
            return _coll.Count();
        }
    }
}
