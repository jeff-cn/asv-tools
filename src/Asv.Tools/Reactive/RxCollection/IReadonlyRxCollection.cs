using System;
using System.Collections.Generic;

namespace Asv.Tools
{
    public interface IReadonlyRxCollection<out TModel>:IEnumerable<TModel>,IDisposable
    {
        IObservable<TModel> OnAdd { get; }
        IObservable<TModel> OnRemove { get; }
    }

    
}
