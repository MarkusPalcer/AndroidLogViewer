using System.ComponentModel;
using System.Runtime.CompilerServices;
using AndroidLogViewer.Annotations;

namespace AndroidLogViewer
{
    public class ProcessThreadFilter : INotifyPropertyChanged
    {
        public int Process { get; set; }

        public int? Thread { get; set; }

        public bool Matches(LogEntry entry)
        {
            if (entry.Process != Process) return false;
            if (Thread.HasValue && Thread.Value != entry.Thread) return false;
            return true;
        }

        public override string ToString() => $"{Process}/{(Thread.HasValue ? Thread.Value.ToString() : "*")}";

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Equals(ProcessThreadFilter other)
        {
            return Process == other.Process && Thread == other.Thread;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ProcessThreadFilter other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Process * 397) ^ Thread.GetHashCode();
            }
        }
    }
}