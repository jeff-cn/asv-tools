using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;

namespace Asv.Tools
{
    public class ConcurrentRxCollection<TModel> : IRxCollection<TModel>, IDisposable
    {
        private readonly SynchronizedList<TModel> _items = new(new List<TModel>());
        private readonly Subject<TModel> _addSubject = new();
        private readonly Subject<TModel> _remSubject = new();

        public IEnumerator<TModel> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public int Count => _items.Count;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Remove(TModel model)
        {
            if (_items.Remove(model))
            {
                _remSubject.OnNext(model);
            }
        }

        public TModel this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }

        public void Clear()
        {
            var itemsToDelete = _items.ToArray();
            foreach (var item in itemsToDelete)
            {
                Remove(item);
            }
        }

        public void Clear(Action<TModel> callbackForEach)
        {
            var itemsToDelete = _items.ToArray();
            foreach (var item in itemsToDelete)
            {
                callbackForEach(item);
                Remove(item);
            }
        }

        public void Add(TModel model)
        {
            _items.Add(model);
            _addSubject.OnNext(model);
        }

        public IObservable<TModel> OnAdd => _addSubject;
        public IObservable<TModel> OnRemove => _remSubject;

        public void Dispose()
        {
            _addSubject?.OnCompleted();
            _addSubject?.Dispose();

            _remSubject?.OnCompleted();
            _remSubject?.Dispose();
            
        }
    }
}
