using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using AndroidLogViewer.Command;
using AndroidLogViewer.Properties;

namespace AndroidLogViewer.Dialogs
{
    /// <summary>
    /// A dialog to enter a generic string
    /// </summary>
    public class StringInputDialogViewModel : DialogViewModelBase<string>
    {
        private string _enteredValue;
        private string _prompt;

        public StringInputDialogViewModel(string title) : base(title)
        {
            ConfirmCommand = new DelegateCommand(() => Done(EnteredValue));
            CanBeCancelled = true;
        }

        /// <summary>
        /// The value to display
        /// </summary>
        public string EnteredValue
        {
            get => _enteredValue;
            set
            {
                if (value == _enteredValue) return;
                _enteredValue = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The prompt to show to the user.
        /// </summary>
        public string Prompt
        {
            get => _prompt;
            set
            {
                if (value == _prompt) return;
                _prompt = value;
                OnPropertyChanged();
            }
        }

        public ICommand ConfirmCommand { get; }
    }
}