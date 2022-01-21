using System;
using System.Reactive.Linq;
using LiteDB;

namespace Asv.Tools.Store
{
    public abstract class RxStoredValue<T> : RxValue<T>
    {
        private readonly IKeyValueStore _store;
        private readonly string _id;
        private readonly IDisposable _subscribe;

        protected RxStoredValue(IKeyValueStore store, string id, T defaultValue, TimeSpan? saveDelay = null)
        {
            _store = store;
            _id = id;
            if (saveDelay == null)
            {
                _subscribe = this.Subscribe(WriteValue);
            }
            else
            {
                _subscribe = this.Throttle(saveDelay.Value).Subscribe(WriteValue);
            }
            OnNext(ReadValue(defaultValue));
        }

        private T ReadValue(T defaultValue)
        {
            var value = _store.Read(_id);
            if (value == null || value.IsNull)
            {
                WriteValue(defaultValue);
                return defaultValue;
            }
            return ConvertFromBson(value);
        }

        protected abstract T ConvertFromBson(BsonValue bson);
        protected abstract BsonValue ConvertToBson(T value);

        private void WriteValue(T value)
        {
            var bson = ConvertToBson(value);
            _store.Write(_id,bson);
        }

        public override void Dispose()
        {
            _subscribe.Dispose();
            base.Dispose();
        }
    }
}
