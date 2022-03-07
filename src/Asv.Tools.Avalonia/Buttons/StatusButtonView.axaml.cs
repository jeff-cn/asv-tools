using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Asv.Tools.Avalonia
{
    public partial class StatusButtonView : UserControl
    {
        public StatusButtonView()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<bool> IsErrorProperty =
            AvaloniaProperty.Register<StatusButtonView, bool>(nameof(IsError), false);

        public bool IsError
        {
            get => GetValue(IsErrorProperty);
            set => SetValue(IsErrorProperty, value);
        }

        public static readonly StyledProperty<string> TopStatusProperty =
            AvaloniaProperty.Register<StatusButtonView, string>(nameof(TopStatus), "100%");

        public string TopStatus
        {
            get => GetValue(TopStatusProperty);
            set => SetValue(TopStatusProperty,value);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
