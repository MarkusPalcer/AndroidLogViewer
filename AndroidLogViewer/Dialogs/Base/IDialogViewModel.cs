using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AndroidLogViewer.Dialogs
{

    /// <summary>
    /// The interface for all dialogs
    /// </summary>
    public interface IDialogViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The dialog title
        /// </summary>
        string Title { get; }

        /// <summary>
        /// The command to execute when the <c>X</c> button is pressed. <br />
        /// If this is set to <c>null</c>, the button will not be displayed.
        /// </summary>
        ICommand CancelCommand { get; }
    }

    /// <summary>
    /// The interface for a specific dialog which returns a result.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IDialogViewModel<TResult> : IDialogViewModel
    {
        Task<TResult> Result { get; }
    }
}