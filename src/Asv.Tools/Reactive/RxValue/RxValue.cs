using System;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace Asv.Tools
{
    public class RxValue<TValue> : DisposableOnce, IRxEditableValue<TValue>
    {
        private readonly Subject<TValue> _subject = new();
        private TValue _value;

        public TValue Value 
        {
            get => _value;
            set
            {
                _value = value;
                OnNext(value);
            }
        }

        protected override void InternalDisposeOnce()
        {
            _subject.OnCompleted();
            _subject.Dispose();
        }

        public void OnNext(TValue value)
        {
            _value = value;
            if (_subject.HasObservers && !_subject.IsDisposed)
            {
                _subject.OnNext(value);
            }
        }
        
        public void OnError(Exception error)
        {
            if (_subject.HasObservers && !_subject.IsDisposed)
            {
                _subject.OnError(error);
            }
        }

        public void OnCompleted()
        {
            if (_subject.HasObservers && !_subject.IsDisposed)
            {
                _subject.OnCompleted();
            }
        }

        public IDisposable Subscribe(IObserver<TValue> observer)
        {
            if (_subject.IsDisposed) return Disposable.Empty;
            var result = _subject.Subscribe(observer);
            if (_value != null) observer.OnNext(_value);
            return result;
        }

        public override string ToString()
        {
            return Value?.ToString() ?? string.Empty;
        }

    }
}
