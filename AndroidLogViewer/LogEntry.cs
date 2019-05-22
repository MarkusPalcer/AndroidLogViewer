using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AndroidLogViewer.Properties;

namespace AndroidLogViewer
{
    public class LogEntry : INotifyPropertyChanged
    {
        public string Time { get; set; }

        public int Process { get; set; }

        public int Thread { get; set; }

        public string Level { get; set; }

        public string Message { get; set; }
        public string Tag { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}