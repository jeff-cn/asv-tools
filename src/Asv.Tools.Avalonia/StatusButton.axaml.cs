using Avalonia;
using Avalonia.Controls.Primitives;
using Material.Icons;

namespace Asv.Tools.Avalonia
{
    public class StatusButton : TemplatedControl
    {
        public StatusButton()
        {
            
        }

        public static readonly StyledProperty<StatusEnum> StatusProperty = AvaloniaProperty.Register<StatusButton, StatusEnum>(nameof(Status),defaultValue:StatusEnum.Warning, notifying:WhenStatusChanged);

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


        public static readonly StyledProperty<string> StatusTextProperty = AvaloniaProperty.Register<StatusButton, string>(nameof(StatusText));
        public string StatusText
        {
            get => GetValue(StatusTextProperty);
            set => SetValue(StatusTextProperty, value);
        }

        public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<StatusButton, string>(nameof(Title));
        public string Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }


        public static readonly StyledProperty<string> TopRightStatusProperty = AvaloniaProperty.Register<StatusButton, string>(nameof(TopRightStatus));
        public string TopRightStatus
        {
            get => GetValue(TopRightStatusProperty);
            set => SetValue(TopRightStatusProperty, value);
        }

        public static readonly StyledProperty<string> BottomRightStatusProperty = AvaloniaProperty.Register<StatusButton, string>(nameof(BottomRightStatus));
        public string BottomRightStatus
        {
            get => GetValue(BottomRightStatusProperty);
            set => SetValue(BottomRightStatusProperty, value);
        }

        public static readonly StyledProperty<MaterialIconKind> IconNameProperty = AvaloniaProperty.Register<StatusButton, MaterialIconKind>(nameof(IconName));
        public MaterialIconKind IconName
        {
            get => (MaterialIconKind)GetValue(IconNameProperty);
            set => SetValue(IconNameProperty, value);
        }
    }

    public enum StatusEnum
    {
        Unknown,
        Warning,
        Error,
        Success,

    }
}
