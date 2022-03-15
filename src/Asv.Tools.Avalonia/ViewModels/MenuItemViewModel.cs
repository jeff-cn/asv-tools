using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Input;

namespace Asv.Tools.Avalonia
{
    public class MenuItemViewModel
    {
        
        public string Header { get; set; }
        public ICommand Command { get; set; }
        public KeyGesture Gesture { get; set; }
        public object CommandParameter { get; set; }
        public ReadOnlyObservableCollection<MenuItemViewModel> Items { get; set; }
    }
}
