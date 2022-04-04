using System;
using System.Windows.Input;
using Material.Icons;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Asv.Tools.Avalonia
{
    public class StatusButtonViewModel:ViewModelBase,IActivatableViewModel
    {
        public StatusButtonViewModel()
        {
            
        }

        public StatusButtonViewModel(string id)
        {
            Id = id;
        }

        public ViewModelActivator Activator { get; } = new();

        public string Id { get; }

        public int Order { get; set; }
        [Reactive]
        public MaterialIconKind Icon { get; set; }
        [Reactive]
        public string StatusText { get; set; }
        [Reactive]
        public string TopRightStatus { get; set; }
        [Reactive]
        public string BottomRightStatus { get; set; }
        [Reactive]
        public StatusEnum SubStatus { get; set; }
        [Reactive]
        public StatusEnum Status { get; set; }
        [Reactive]
        public ICommand Command { get; set; }
        [Reactive]
        public object CommandParameter { get; set; }
        [Reactive]
        public string Title { get; set; }

        
    }
}
