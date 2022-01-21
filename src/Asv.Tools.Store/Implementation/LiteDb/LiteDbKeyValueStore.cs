using System.Collections.Generic;
using System.Linq;
using LiteDB;

namespace Asv.Tools.Store
{
    public class LiteDbKeyValueStore : IKeyValueStore
    {
        private readonly LiteCollection<BsonDocument> _coll;

        public LiteDbKeyValueStore(string name, LiteCollection<BsonDocument> coll)
        {
            Name = name;
            _coll = coll;
            _coll.EnsureIndex("_id", true);
        }

        public string Name { get; }

        public IEnumerable<string> Ids => _coll.FindAll().Select(_ => _["_id"].AsString);

        public BsonValue Read(string id)
        {
            return _coll.FindOne(Query.EQ("_id", id))?["v"] ?? BsonValue.Null;
        }

        public void Write(string id, BsonValue bsonValue)
        {
            var fullId = id;
            if (!_coll.Update(id, new BsonDocument {{"_id", fullId }, {"v", bsonValue}}))
            {
                _coll.Insert(new BsonDocument {{"_id", fullId }, {"v", bsonValue}});
            }
        }
    }
}
