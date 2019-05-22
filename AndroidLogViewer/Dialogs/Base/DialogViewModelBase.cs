using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using AndroidLogViewer.Annotations;
using AndroidLogViewer.Command;

namespace AndroidLogViewer.Dialogs
{
    public abstract class DialogViewModelBase<TResult> : IDialogViewModel<TResult>
    {
        private readonly TaskCompletionSource<TResult> _taskCompletionSource = new TaskCompletionSource<TResult>();

        protected DialogViewModelBase(string defaultTitle)
        {
            _title = defaultTitle;
            _cancelCommand = new DelegateCommand(Cancel, ()  => CanBeCancelled).ObservesCanExecute(() => CanBeCancelled);
        }

        private string _title;
        private readonly DelegateCommand _cancelCommand;
        private bool _canBeCancelled;

        public string Title
        {
            get => _title;
            set
            {
                if (value == _title) return;
                _title = value;
                OnPropertyChanged();
            }
        }

        public ICommand CancelCommand => _cancelCommand;

        Task<TResult> IDialogViewModel<TResult>.Result => _taskCompletionSource.Task;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void Done(TResult result)
        {
            _taskCompletionSource.SetResult(result);
        }

        protected void Error(Exception exception)
        {
            _taskCompletionSource.SetException(exception);
        }

        protected virtual void Cancel()
        {
            _taskCompletionSource.SetCanceled();
        }

        protected bool CanBeCancelled
        {
            get => _canBeCancelled;
            set
            {
                if (value == _canBeCancelled) return;
                _canBeCancelled = value;
                OnPropertyChanged();
            }
        }
    }
}