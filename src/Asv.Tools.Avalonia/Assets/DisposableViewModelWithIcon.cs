using System;
using System.ComponentModel;
using DynamicData;
using JetBrains.Annotations;
using Material.Icons;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Asv.Tools.Avalonia
{
    public interface IWidgetProvider<TWidget>
        where TWidget:IWidget
    {
        IObservable<IChangeSet<TWidget, string>> Items { get; }
    }

    public class WidgetProviderBase<TWidget> : IWidgetProvider<TWidget>
        where TWidget : IWidget
    {
        private readonly SourceCache<TWidget, string> _sourceCache = new(_ => _.Id);

        protected ISourceCache<TWidget, string> Source => _sourceCache;

        public IObservable<IChangeSet<TWidget, string>> Items => _sourceCache.Connect();
    }


    public interface IWidget : INotifyPropertyChanged,IDisposable, IActivatableViewModel
    {
        string Id { get; }
        int Order { get; }
        MaterialIconKind Icon { get; }
        string Title { get; }
    }


    public class WidgetBase : DisposableViewModelBase, IWidget
    {
        protected WidgetBase([NotNull] string id,int order)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(id));
            Id = id;
            Order = order;
        }

        public string Id { get; }
        [Reactive]
        public int Order { get; set; }
        [Reactive]
        public MaterialIconKind Icon { get; set; }
        [Reactive]
        public string Title { get; set; }

        public ViewModelActivator Activator { get; } = new();
    }
}
