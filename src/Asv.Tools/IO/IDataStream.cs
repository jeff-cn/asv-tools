using System;
using System.Threading;
using System.Threading.Tasks;

namespace Asv.Tools
{
    public interface IDataStream:IObservable<byte[]>
    {
        Task<bool> Send(byte[] data, int count, CancellationToken cancel);
        long RxBytes { get; }
        long TxBytes { get; }
    }

}
