using System;
using System.Collections.Generic;
using System.Linq;
using Asv.Mavlink;
using LiteDB;

namespace Asv.Tools.Store
{
    public class RxStoredBool : RxStoredValue<bool>
    {
        public RxStoredBool(IKeyValueStore store, string id, bool defaultValue, TimeSpan? saveDelay = null) : base(store, id, defaultValue, saveDelay)
        {
        }

        protected override bool ConvertFromBson(BsonValue bson)
        {
            return bson;
        }

        protected override BsonValue ConvertToBson(bool value)
        {
            return value;
        }
    }

    public class RxStoredString : RxStoredValue<string>
    {
        public RxStoredString(IKeyValueStore store, string id, string defaultValue, TimeSpan? saveDelay = null) : base(store, id, defaultValue, saveDelay)
        {
        }

        protected override string ConvertFromBson(BsonValue bson)
        {
            return bson;
        }

        protected override BsonValue ConvertToBson(string value)
        {
            return value;
        }
    }

    public class RxStoredDateTime : RxStoredValue<DateTime>
    {
        public RxStoredDateTime(IKeyValueStore store, string id, DateTime defaultValue, TimeSpan? saveDelay = null) : base(store, id, defaultValue, saveDelay)
        {
        }


        protected override DateTime ConvertFromBson(BsonValue bson)
        {
            return bson.AsDateTime;
        }

        protected override BsonValue ConvertToBson(DateTime value)
        {
            return value;
        }
    }

    public class RxStoredGeoPoint : RxStoredValue<GeoPoint>
    {
        public RxStoredGeoPoint(IKeyValueStore store, string id, GeoPoint defaultValue, TimeSpan? saveDelay = null) : base(store, id, defaultValue, saveDelay)
        {
        }

        protected override GeoPoint ConvertFromBson(BsonValue bson)
        {
            if (bson.IsNull) return GeoPoint.ZeroWithAlt;
            var doc = bson.AsDocument;
            return new GeoPoint(doc["lat"].AsDouble, doc["lon"].AsDouble, doc["alt"].AsDouble);
        }

        protected override BsonValue ConvertToBson(GeoPoint value)
        {
            return new BsonDocument(new Dictionary<string, BsonValue>
            {
                { "lat", value.Latitude},
                { "lon", value.Longitude},
                { "alt", value.Altitude},
            });
        }
    }

    public class RxStoredEnum<TValue> : RxStoredValue<TValue>
    {
        public RxStoredEnum(IKeyValueStore store, string id, TValue defaultValue, TimeSpan? saveDelay = null) : base(store, id, defaultValue, saveDelay)
        {
        }

        protected override TValue ConvertFromBson(BsonValue bson)
        {
            try
            {
                return (TValue)Enum.Parse(typeof(TValue), bson, true);
            }
            catch
            {
                return default(TValue);
            }
        }

        protected override BsonValue ConvertToBson(TValue value)
        {
            if (!typeof(TValue).IsEnum) return default(BsonValue);
            try
            {
                return Enum.GetName(typeof(TValue), value);
            }
            catch
            {
                return Enum.GetNames(typeof(TValue)).FirstOrDefault();
            }
        }
    }
}
