using ReactiveUI;

namespace Asv.Tools.Avalonia
{


    public class StatusButtonViewModel:ViewModelBase
    {
        private string _statusText;
        private string _topRightStatus;
        private string _bottomRightStatus;
        private StatusEnum _status;

        public StatusButtonViewModel()
        {
           
        }

        public string StatusText
        {
            get => _statusText;
            set => this.RaiseAndSetIfChanged(ref _statusText, value);
        }
        public string TopRightStatus
        {
            get => _topRightStatus;
            set => this.RaiseAndSetIfChanged(ref _topRightStatus, value);
        }

        public string BottomRightStatus
        {
            get => _bottomRightStatus;
            set => this.RaiseAndSetIfChanged(ref _bottomRightStatus, value);
        }

        public StatusEnum Status
        {
            get => _status;
            set => this.RaiseAndSetIfChanged(ref _status, value);
        }
    }
}
