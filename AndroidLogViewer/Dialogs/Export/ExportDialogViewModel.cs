using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using AndroidLogViewer.Command;
using Microsoft.Win32;

namespace AndroidLogViewer.Dialogs
{
    public class ExportDialogViewModel : DialogViewModelBase<object>
    {
        private readonly LogEntry[] _entries;
        private string _fileName;
        private bool _alignColumns;

        public ExportDialogViewModel(LogEntry[] entries) : base("Export")
        {
            _entries = entries;
            BrowseFileCommand = new DelegateCommand(ExecuteBrowseFileCommand);
            ConfirmCommand = new DelegateCommand(ExecuteConfirmCommand, CanExecuteConfirmCommand)
                .ObservesProperty(() => FileName);

            CanBeCancelled = true;
        }

        public string FileName
        {
            get => _fileName;
            set
            {
                if (value == _fileName) return;
                _fileName = value;
                OnPropertyChanged();
            }
        }

        public bool AlignColumns
        {
            get => _alignColumns;
            set
            {
                if (value == _alignColumns) return;
                _alignColumns = value;
                OnPropertyChanged();
            }
        }

        public ICommand BrowseFileCommand { get; } 

        private void ExecuteBrowseFileCommand()
        {
            var dlg = new SaveFileDialog
            {
                CheckPathExists = true,
                Title = "Select logcat file",
                RestoreDirectory = true,
                OverwritePrompt = true,
            };

            var dialogResult = dlg.ShowDialog();
            if (!dialogResult.HasValue || !dialogResult.Value) return;

            FileName = dlg.FileName;
        }

        public ICommand ConfirmCommand { get; }

        private bool CanExecuteConfirmCommand()
        {
            if (string.IsNullOrEmpty(FileName)) return false;

            try
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                Path.GetFullPath(FileName);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private void ExecuteConfirmCommand()
        {
            IEnumerable<string> lines;

            if (AlignColumns)
            {
                var dateTimeLength = _entries.Max(x => x.Time.Length);
                var pidLength = _entries.Max(x => x.Process.ToString().Length);
                var tidLength = _entries.Max(x => x.Thread.ToString().Length);
                var tagLength = _entries.Max(x => x.Tag.Length) + 1;

                lines = _entries.Select(x => $"{x.Time.PadRight(dateTimeLength)} {x.Process.ToString().PadRight(pidLength)} {x.Thread.ToString().PadRight(tidLength)} {x.Level} {$"{x.Tag}:".PadRight(tagLength)} {x.Message}");
            }
            else
            {
                lines = _entries.Select(FormatLogEntry);
            }
            

            File.WriteAllLines(FileName, lines);

            Done(null);
        }

        public static string FormatLogEntry(LogEntry x)
        {
            return $"{x.Time} {x.Process} {x.Thread} {x.Level} {x.Tag}: {x.Message}";
        }
    }
}