using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AndroidLogViewer.Annotations;

namespace AndroidLogViewer
{
    public class LogEntry : INotifyPropertyChanged
    {
        public String Time { get; set; }

        public int Process { get; set; }

        public int Thread { get; set; }

        public string Level { get; set; }

        public string Message { get; set; }
        public string Tag { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}