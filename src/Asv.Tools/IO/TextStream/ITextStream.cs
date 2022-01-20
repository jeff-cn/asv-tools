using System;
using System.Threading;
using System.Threading.Tasks;

namespace Asv.Tools
{
    public interface ITextStream : IDisposable, IObservable<string>
    {
        IObservable<Exception> OnError { get; }

        Task Send(string value, CancellationToken cancel);
    }
}
