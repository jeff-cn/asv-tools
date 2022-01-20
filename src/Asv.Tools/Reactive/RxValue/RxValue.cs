using System;
using System.Reactive.Subjects;

namespace Asv.Tools
{
    public class RxValue<TValue> :IObserver<TValue>, IRxEditableValue<TValue>, IRxValue<TValue>,IDisposable
    {
        private readonly Subject<TValue> _subject = new Subject<TValue>();
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

        public virtual void Dispose()
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
            return _subject.Subscribe(observer);
        }

        public override string ToString()
        {
            return Value?.ToString() ?? string.Empty;
        }

    }
}
