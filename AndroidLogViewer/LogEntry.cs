using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AndroidLogViewer.Properties;

namespace AndroidLogViewer
{
    public class LogEntry : INotifyPropertyChanged
    {
        public string Time { get; set; } = string.Empty;

        public int Process { get; set; }

        public int Thread { get; set; }

        public string Level { get; set; }  = string.Empty;

        public string Message { get; set; }  = string.Empty;

        public string Tag { get; set; }  = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}