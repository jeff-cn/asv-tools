using System.Threading;
using Asv.Tools;


public abstract class DisposableOnceWithCancel : DisposableOnce
{
    private readonly CancellationTokenSource _cancel = new();

    protected CancellationToken DisposeCancel => _cancel.Token;

    protected override void InternalDisposeOnce()
    {
        _cancel.Cancel(false);
        _cancel.Dispose();
    }
}
