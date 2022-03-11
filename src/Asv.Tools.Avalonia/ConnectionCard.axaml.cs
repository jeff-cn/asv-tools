using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Material.Icons;
using ReactiveUI;

namespace Asv.Tools.Avalonia
{
    public class ConnectionCard : TemplatedControl
    {

        public static readonly StyledProperty<StatusEnum> StatusProperty = AvaloniaProperty.Register<StatusButton, StatusEnum>(nameof(Status), defaultValue: StatusEnum.Warning, notifying: WhenStatusChanged);

        private static void WhenStatusChanged(IAvaloniaObject source, bool beforeChanged)
        {
            if (source is not StyledElement btn) return;
            var value = source.GetValue(StatusProperty);
            if (beforeChanged) return;
            btn.Classes.Clear();
            btn.Classes.Set(value.ToString(), true);
        }

        public StatusEnum Status
        {
            get => GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        public static readonly StyledProperty<MaterialIconKind> IconNameProperty = AvaloniaProperty.Register<StatusButton, MaterialIconKind>(nameof(IconName));
        public MaterialIconKind IconName
        {
            get => (MaterialIconKind)GetValue(IconNameProperty);
            set => SetValue(IconNameProperty, value);
        }

        public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<ConnectionCard, string>(nameof(Title));
        public string Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }


        public static readonly StyledProperty<string> SubTitleProperty = AvaloniaProperty.Register<ConnectionCard, string>(nameof(SubTitle));
        public string SubTitle
        {
            get => GetValue(SubTitleProperty);
            set => SetValue(SubTitleProperty, value);
        }

        public static readonly StyledProperty<string> StatusTextProperty = AvaloniaProperty.Register<ConnectionCard, string>(nameof(StatusText));
        public string StatusText
        {
            get => GetValue(StatusTextProperty);
            set => SetValue(StatusTextProperty, value);
        }

        public static readonly StyledProperty<string> ErrorTextProperty = AvaloniaProperty.Register<ConnectionCard, string>(nameof(ErrorText));
        public string ErrorText
        {
            get => GetValue(ErrorTextProperty);
            set => SetValue(ErrorTextProperty, value);
        }

        public static readonly StyledProperty<string> TxTextProperty = AvaloniaProperty.Register<ConnectionCard, string>(nameof(TxText));
        public string TxText
        {
            get => GetValue(TxTextProperty);
            set => SetValue(TxTextProperty, value);
        }

        public static readonly StyledProperty<string> RxTextProperty = AvaloniaProperty.Register<ConnectionCard, string>(nameof(RxText));
        public string RxText
        {
            get => GetValue(RxTextProperty);
            set => SetValue(RxTextProperty, value);
        }

        public static readonly StyledProperty<bool> IsConnectionEnabledProperty = AvaloniaProperty.Register<ConnectionCard, bool>(nameof(IsConnectionEnabled));
        public bool IsConnectionEnabled
        {
            get => GetValue(IsConnectionEnabledProperty);
            set => SetValue(IsConnectionEnabledProperty, value);
        }

        public static readonly StyledProperty<ICommand> RemoveProperty = AvaloniaProperty.Register<ConnectionCard, ICommand>(nameof(Remove));
        public ICommand Remove
        {
            get => GetValue(RemoveProperty);
            set => SetValue(RemoveProperty, value);
        }

        public static readonly StyledProperty<ICommand> SwitchEnableProperty = AvaloniaProperty.Register<ConnectionCard, ICommand>(nameof(SwitchEnable));
        public ICommand SwitchEnable
        {
            get => GetValue(SwitchEnableProperty);
            set => SetValue(SwitchEnableProperty, value);
        }


    }
}
