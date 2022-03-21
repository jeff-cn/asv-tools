using System.Reactive.Disposables;
using System.Threading;
using Asv.Tools;


public abstract class DisposableOnceWithCancel : DisposableOnce
{
    private readonly CancellationTokenSource _cancel = new();
    private CompositeDisposable _dispose;

    protected CancellationToken DisposeCancel => _cancel.Token;
    protected CompositeDisposable Disposable
    {
        get
        {
            if (_dispose != null) return _dispose;
            lock (_cancel)
            {
                return _dispose ??= new CompositeDisposable();
            }
        }
    }

    protected override void InternalDisposeOnce()
    {
        if (_cancel.Token.CanBeCanceled)
            _cancel.Cancel(false);
        _cancel.Dispose();
        _dispose?.Dispose();
    }
}
