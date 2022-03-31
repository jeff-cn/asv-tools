using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Material.Icons;

namespace Asv.Tools.Avalonia
{
    public class StatusButton : TemplatedControl
    {
        public StatusButton()
        {
            Status = StatusEnum.Error;
            SubStatus = StatusEnum.Error;
        }

        public static readonly StyledProperty<StatusEnum> SubStatusProperty = AvaloniaProperty.Register<StatusButton, StatusEnum>(nameof(SubStatus), defaultValue: StatusEnum.Warning, notifying: WhenSubStatusChanged);

        private static void WhenSubStatusChanged(IAvaloniaObject source, bool beforeChanged)
        {
            if (source is not StatusButton btn) return;
            var value = source.GetValue(SubStatusProperty);
            if (beforeChanged) return;
            switch (value)
            {
                case StatusEnum.Unknown:
                    btn.PseudoClasses.Add(":Unknown");
                    btn.PseudoClasses.Remove(":Warning");
                    btn.PseudoClasses.Remove(":Error");
                    btn.PseudoClasses.Remove(":Success");
                    break;
                case StatusEnum.Warning:
                    btn.PseudoClasses.Remove(":Unknown");
                    btn.PseudoClasses.Add(":Warning");
                    btn.PseudoClasses.Remove(":Error");
                    btn.PseudoClasses.Remove(":Success");
                    break;
                case StatusEnum.Error:
                    btn.PseudoClasses.Remove(":Unknown");
                    btn.PseudoClasses.Remove(":Warning");
                    btn.PseudoClasses.Add(":Error");
                    btn.PseudoClasses.Remove(":Success");
                    break;
                case StatusEnum.Success:
                    btn.PseudoClasses.Remove(":Unknown");
                    btn.PseudoClasses.Remove(":Warning");
                    btn.PseudoClasses.Remove(":Error");
                    btn.PseudoClasses.Add(":Success");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public StatusEnum SubStatus
        {
            get => GetValue(SubStatusProperty);
            set => SetValue(SubStatusProperty, value);
        }

        public static readonly StyledProperty<StatusEnum> StatusProperty = AvaloniaProperty.Register<StatusButton, StatusEnum>(nameof(Status),defaultValue:StatusEnum.Warning, notifying:WhenStatusChanged);

        private static void WhenStatusChanged(IAvaloniaObject source, bool beforeChanged)
        {
            if (source is not StatusButton btn) return;
            var value = source.GetValue(StatusProperty);
            if (beforeChanged) return;
            switch (value)
            {
                case StatusEnum.Unknown:
                    btn.Classes.Add("Unknown");
                    btn.Classes.Remove("Warning");
                    btn.Classes.Remove("Error");
                    btn.Classes.Remove("Success");
                    break;
                case StatusEnum.Warning:
                    btn.Classes.Remove("Unknown");
                    btn.Classes.Add("Warning");
                    btn.Classes.Remove("Error");
                    btn.Classes.Remove("Success");
                    break;
                case StatusEnum.Error:
                    btn.Classes.Remove("Unknown");
                    btn.Classes.Remove("Warning");
                    btn.Classes.Add("Error");
                    btn.Classes.Remove("Success");
                    break;
                case StatusEnum.Success:
                    btn.Classes.Remove("Unknown");
                    btn.Classes.Remove("Warning");
                    btn.Classes.Remove("Error");
                    btn.Classes.Add("Success");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public StatusEnum Status
        {
            get => GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        public static readonly StyledProperty<ICommand> CommandProperty = AvaloniaProperty.Register<StatusButton, ICommand>(nameof(Command));
        public ICommand Command
        {
            get => GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly StyledProperty<object> CommandParameterProperty = AvaloniaProperty.Register<StatusButton, object>(nameof(CommandParameter));
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
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

        public static readonly StyledProperty<MaterialIconKind> IconProperty = AvaloniaProperty.Register<StatusButton, MaterialIconKind>(nameof(Icon));
        public MaterialIconKind Icon
        {
            get => (MaterialIconKind)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
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
