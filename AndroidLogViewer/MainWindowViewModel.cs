using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using AndroidLogViewer.Command;
using AndroidLogViewer.Dialogs;
using AndroidLogViewer.Properties;
using Microsoft.Win32;

namespace AndroidLogViewer
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private IEnumerable<LogEntry> _logEntries;
        
        private ICollectionView _defaultView;

        public MainWindowViewModel()
        {
            SearchForwardCommand = new DelegateCommand<string>(SearchForward);
            SearchBackwardCommand = new DelegateCommand<string>(SearchBackward);

            RemoveWhitelistedTagCommand = new DelegateCommand<string>(RemoveWhitelistedTag);
            AddWhitelistedTagCommand = new DelegateCommand<string>(AddWhitelistedTag);
            ShowOnlyTagForSelectedItemCommand = new DelegateCommand(ShowOnlyTagForSelectedItem);
            RemoveBlacklistedTagCommand = new DelegateCommand<string>(RemoveBlacklistedTag);
            AddBlacklistedTagCommand = new DelegateCommand<string>(AddBlacklistedTag);
            HideTagOfSelectedItemCommand = new DelegateCommand(HideTagOfSelectedItem);

            RemoveWhitelistedProcessThreadFilterCommand = new DelegateCommand<ProcessThreadFilter>(RemoveWhitelistedProcesThreadFilter);
            AddWhitelistedProcessThreadFilterCommand = new DelegateCommand<ProcessThreadFilter>(AddWhitelistedProcessThreadFilter);
            ShowOnlyProcessOfSelectedItemCommand = new DelegateCommand(ShowOnlyProcessOfSelectedItem);
            ShowOnlyThreadOfSelectedItemCommand = new DelegateCommand(ShowOnlyThreadOfSelectedItem);

            RemoveBlacklistedProcessThreadFilterCommand = new DelegateCommand<ProcessThreadFilter>(RemoveBlacklistedProcessThreadFilter);
            AddBlacklistedProcessThreadFilterCommand = new DelegateCommand<ProcessThreadFilter>(AddBlacklistedProcessThreadFilter);
            HideProcessOfSelectedItemCommand = new DelegateCommand(HideProcessOfSelectedItem);
            HideThreadOfSelectedItemCommand = new DelegateCommand(HideThreadOfSelectedItem);

            ExportCommand = new DelegateCommand(
                () => ShowDialog<ExportDialogViewModel, object>(new ExportDialogViewModel(_defaultView.OfType<LogEntry>().ToArray())).FireAndForget(), 
                () => !string.IsNullOrEmpty(FileName)).ObservesProperty(() => FileName);

            OpenFileCommand = new DelegateCommand(OpenFile);
            OpenUrlCommand = new DelegateCommand(OpenUrl);
            ImportClipboardCommand = new DelegateCommand(ImportFromClipboard);

            WhitelistedProcessThreadFilters.CollectionChanged += RefreshView;
            BlacklistedProcessThreadFilters.CollectionChanged += RefreshView;
            WhitelistedTags.CollectionChanged += RefreshView;
            BlacklistedTags.CollectionChanged += RefreshView;

            RemoveLeadingLinesCommand = new DelegateCommand(RemoveLinesBeforeSelected);
            RemoveTrailingLinesCommand = new DelegateCommand(() => LogEntries = RemoveLinesAfterSelected().ToArray());
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

        private void RefreshView(object sender, NotifyCollectionChangedEventArgs args)
        {
            // Try to find the entry in the filtered list or select the first item that comes after it.
            var selectedLogEntry = SelectedLogEntry;

            if (_defaultView == null) return;
            if (_logEntries == null) return;

            _defaultView.Refresh();

            using(var logEntryEnumerator = _logEntries.GetEnumerator())
            using(var filteredEnumerator = _defaultView.OfType<LogEntry>().GetEnumerator())
            {
                if (!logEntryEnumerator.MoveNext()) return;
                var index = -1;

                while (filteredEnumerator.MoveNext())
                {
                    index++;

                    if (filteredEnumerator.Current == selectedLogEntry)
                    {
                        SelectedLogEntryIndex = index;
                        return;
                    }

                    while (logEntryEnumerator.Current != filteredEnumerator.Current)
                    {
                        logEntryEnumerator.MoveNext();
                        if (logEntryEnumerator.Current != selectedLogEntry) continue;

                        SelectedLogEntryIndex = index;
                        return;
                    }
                }
            }
        }

        public IEnumerable<LogEntry> LogEntries
        {
            get => _logEntries;
            private set
            {
                if (Equals(value, _logEntries)) return;
                _logEntries = value;


                _defaultView = CollectionViewSource.GetDefaultView(_logEntries);
                if (_defaultView != null) {
                    _defaultView.Filter = FilterLogEntries;
                }

                OnPropertyChanged();
            }
        }

        private bool FilterLogEntries(object x)
        {
            if (!(x is LogEntry entry)) return false;

            if (WhitelistedProcessThreadFilters.Any() && !WhitelistedProcessThreadFilters.Any(f => f.Matches(entry))) return false;
            if (BlacklistedProcessThreadFilters.Any(f => f.Matches(entry))) return false;
            if (WhitelistedTags.Any() && !WhitelistedTags.Any(t => entry.Tag.Equals(t))) return false;
            if (BlacklistedTags.Any(t => entry.Tag.Equals(t))) return false;
            if (!ShowDebug && entry.Level.Equals("D")) return false;
            if (!ShowError && entry.Level.Equals("E")) return false;
            if (!ShowInfo && entry.Level.Equals("I")) return false;
            if (!ShowVerbose && entry.Level.Equals("V")) return false;
            if (!ShowWarn && entry.Level.Equals("W")) return false;

            return true;
        }

        public ICommand ExportCommand { get; }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Process/Thread filtering
        private ObservableCollection<ProcessThreadFilter> _availableProcessThreadFilters;

        public ObservableCollection<ProcessThreadFilter> AvailableProcessThreadFilters
        {
            get => _availableProcessThreadFilters;
            set
            {
                if (Equals(value, _availableProcessThreadFilters)) return;
                _availableProcessThreadFilters = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ProcessThreadFilter> WhitelistedProcessThreadFilters { get; } = new ObservableCollection<ProcessThreadFilter>();
        public ObservableCollection<ProcessThreadFilter> BlacklistedProcessThreadFilters { get; } = new ObservableCollection<ProcessThreadFilter>();

        public ICommand AddWhitelistedProcessThreadFilterCommand { get; }

        public ICommand RemoveWhitelistedProcessThreadFilterCommand { get; }

        public ICommand AddBlacklistedProcessThreadFilterCommand { get; }

        public ICommand RemoveBlacklistedProcessThreadFilterCommand { get; }

        public ICommand ShowOnlyThreadOfSelectedItemCommand { get; private set; }

        public ICommand ShowOnlyProcessOfSelectedItemCommand { get; private set; }

        public ICommand HideThreadOfSelectedItemCommand { get; private set; }

        public ICommand HideProcessOfSelectedItemCommand { get; private set; }

        private void HideThreadOfSelectedItem()
        {
            var entry = SelectedLogEntry;
            BlacklistedProcessThreadFilters.Add(new ProcessThreadFilter {Process = entry.Process, Thread = entry.Thread});
        }

        private void HideProcessOfSelectedItem()
        {
            var entry = SelectedLogEntry;
            BlacklistedProcessThreadFilters.Add(new ProcessThreadFilter {Process = entry.Process, Thread = null});
        }

        private void AddBlacklistedProcessThreadFilter(ProcessThreadFilter filter)
        {
            BlacklistedProcessThreadFilters.Add(filter);
        }

        private void RemoveBlacklistedProcessThreadFilter(ProcessThreadFilter f)
        {
            BlacklistedProcessThreadFilters.Remove(f);
        }

        private void ShowOnlyThreadOfSelectedItem()
        {
            var entry = SelectedLogEntry;
            WhitelistedProcessThreadFilters.Clear();
            WhitelistedProcessThreadFilters.Add(new ProcessThreadFilter {Process = entry.Process, Thread = entry.Thread});
        }

        private void ShowOnlyProcessOfSelectedItem()
        {
            var entry = SelectedLogEntry;
            WhitelistedProcessThreadFilters.Clear();
            WhitelistedProcessThreadFilters.Add(new ProcessThreadFilter {Process = entry.Process, Thread = null});
        }

        private void AddWhitelistedProcessThreadFilter(ProcessThreadFilter filter)
        {
            WhitelistedProcessThreadFilters.Add(filter);
        }

        private void RemoveWhitelistedProcesThreadFilter(ProcessThreadFilter f)
        {
            WhitelistedProcessThreadFilters.Remove(f);
        }

        #endregion

        #region Tag filtering

        private ObservableCollection<string> _availableTags;

        public ObservableCollection<string> AvailableTags
        {
            get => _availableTags;
            set
            {
                if (Equals(value, _availableTags)) return;
                _availableTags = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> WhitelistedTags { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> BlacklistedTags { get; } = new ObservableCollection<string>();

        public ICommand AddWhitelistedTagCommand { get; }

        public ICommand RemoveWhitelistedTagCommand { get; }

        public ICommand ShowOnlyTagForSelectedItemCommand { get; private set; }

        public ICommand AddBlacklistedTagCommand { get; }

        public ICommand RemoveBlacklistedTagCommand { get; }

        public ICommand HideTagOfSelectedItemCommand { get; private set; }

        private void HideTagOfSelectedItem()
        {
            var entry = SelectedLogEntry;
            BlacklistedTags.Add(entry.Tag);
        }

        private void AddBlacklistedTag(string tag)
        {
            BlacklistedTags.Add(tag);
        }

        private void RemoveBlacklistedTag(string tag)
        {
            BlacklistedTags.Remove(tag);
        }

        private void ShowOnlyTagForSelectedItem()
        {
            var entry = SelectedLogEntry;
            WhitelistedTags.Clear();
            WhitelistedTags.Add(entry.Tag);
        }

        private void AddWhitelistedTag(string tag)
        {
            WhitelistedTags.Add(tag);
        }

        private void RemoveWhitelistedTag(string tag)
        {
            WhitelistedTags.Remove(tag);
        }
        #endregion

        #region Loglevel filtering

        private bool _showVerbose = true;
        private bool _showDebug = true;
        private bool _showInfo = true;
        private bool _showWarn = true;
        private bool _showError = true;
        private int _selectedLogEntryIndex;
        private string _fileName;
        private object _activeDialog;

        public bool ShowVerbose
        {
            get => _showVerbose;
            set
            {
                if (value == _showVerbose) return;
                _showVerbose = value;
                _defaultView?.Refresh();
                OnPropertyChanged();
            }
        }

        public bool ShowDebug
        {
            get => _showDebug;
            set
            {
                if (value == _showDebug) return;
                _showDebug = value;
                _defaultView?.Refresh();
                OnPropertyChanged();
            }
        }

        public bool ShowInfo
        {
            get => _showInfo;
            set
            {
                if (value == _showInfo) return;
                _showInfo = value;
                _defaultView?.Refresh();
                OnPropertyChanged();
            }
        }

        public bool ShowWarn
        {
            get => _showWarn;
            set
            {
                if (value == _showWarn) return;
                _showWarn = value;
                _defaultView?.Refresh();
                OnPropertyChanged();
            }
        }

        public bool ShowError
        {
            get => _showError;
            set
            {
                if (value == _showError) return;
                _showError = value;
                _defaultView?.Refresh();
                OnPropertyChanged();
            }
        }

        #endregion

        #region Loading
        public ICommand OpenUrlCommand { get; }

        public ICommand OpenFileCommand { get; }

        public ICommand ImportClipboardCommand { get; }

        private async void OpenFile()
        {
            var dlg = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Title = "Select logcat file",
                RestoreDirectory = true,
            };

            var dialogResult = dlg.ShowDialog();
            if (!dialogResult.HasValue || !dialogResult.Value) return;
            

            using (var reader = File.OpenText(dlg.FileName))
            using (var progressDialog = new ProgressDialogViewModel())
            {
                ShowDialog<ProgressDialogViewModel, object>(progressDialog).FireAndForget();
                FileName = dlg.FileName;
                await ProcessLogData(reader);
                await Task.Delay(1000);
            }
        }

        private async void OpenUrl()
        {
            var dialog = ShowDialog<StringInputDialogViewModel,string>(new StringInputDialogViewModel("Open URL")
            {
                Prompt = "Please enter the URL to load"
            });

            try
            {
                var address = await dialog;

                if (string.IsNullOrEmpty(address)) return;

                using (var webClient = new WebClient())
                {
                    Stream stream;
                    try
                    {
                        stream = webClient.OpenRead(address);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }

                    if (stream == null) return;

                    using (var reader = new StreamReader(stream))
                    using (var progressDialog = new ProgressDialogViewModel())
                    {
                        ShowDialog<ProgressDialogViewModel, object>(progressDialog).FireAndForget();
                        FileName = address;
                        await ProcessLogData(reader);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // NOP
            }
        }

        private async void ImportFromClipboard()
        {
            if (!Clipboard.ContainsText(TextDataFormat.Text)) return;
            var data = Clipboard.GetText(TextDataFormat.Text);
            using (var reader = new StringReader(data))
            using (var progressDialog = new ProgressDialogViewModel())
            {
                ShowDialog<ProgressDialogViewModel, object>(progressDialog).FireAndForget();
                FileName = "[From Clipboard]";
                await ProcessLogData(reader);
            }
        }

        private async Task ProcessLogData(TextReader reader)
        {
            LogEntries = null;
            AvailableProcessThreadFilters = null;

            BlacklistedProcessThreadFilters.Clear();
            WhitelistedProcessThreadFilters.Clear();

            LogEntries = await Task<ObservableCollection<LogEntry>>.Factory.StartNew(() => LogcatParser.ReadLogEntries(reader));
            AvailableProcessThreadFilters =
                await Task<ObservableCollection<ProcessThreadFilter>>.Factory.StartNew(GenerateProcessThreadFilters);
            AvailableTags = new ObservableCollection<string>(
                await Task<string[]>.Factory.StartNew(() =>
                    LogEntries.Select(x => x.Tag).Distinct().OrderBy(x => x).ToArray()));
        }

        private ObservableCollection<ProcessThreadFilter> GenerateProcessThreadFilters()
        {
            ObservableCollection<ProcessThreadFilter> result = new ObservableCollection<ProcessThreadFilter>();

            var processThreadCombinations = new Dictionary<int, HashSet<int>>();
            foreach (var entry in LogEntries)
            {
                if (!processThreadCombinations.ContainsKey(entry.Process))
                {
                    processThreadCombinations.Add(entry.Process, new HashSet<int>());
                }

                processThreadCombinations[entry.Process].Add(entry.Thread);
            }

            foreach (var process in processThreadCombinations)
            {
                if (process.Value.Count == 1)
                {
                    process.Value.Clear();
                }

                result.Add(new ProcessThreadFilter {Process = process.Key, Thread = null});

                foreach (var thread in process.Value)
                {
                    result.Add(new ProcessThreadFilter {Process = process.Key, Thread = thread});
                }
            }

            return result;
        }

        #endregion

        #region Search

        public ICommand SearchForwardCommand { get; set; }

        public ICommand SearchBackwardCommand { get; set; }

        public int SelectedLogEntryIndex
        {
            get => _selectedLogEntryIndex;
            set
            {
                if (value == _selectedLogEntryIndex) return;
                _selectedLogEntryIndex = value;
                OnPropertyChanged();
            }
        }

        public LogEntry SelectedLogEntry { get; set; }

        public bool SearchCaseSensitive { get; set; }

        private void SearchBackward(string searchText)
        {
            if (_defaultView == null) return;

            var items = _defaultView.OfType<LogEntry>().ToArray();

            var newIndex = SelectedLogEntryIndex;
            while (newIndex > 0)
            {
                newIndex--;
                if (MatchesSearch(items[newIndex], searchText))
                {
                    SelectedLogEntryIndex = newIndex;
                    break;
                }
            }
        }

        private void SearchForward(string searchText)
        {
            if (_defaultView == null) return;

            var newIndex = SelectedLogEntryIndex;
            foreach (var entry in _defaultView.OfType<LogEntry>().Skip(SelectedLogEntryIndex + 1))
            {
                newIndex++;
                if (MatchesSearch(entry, searchText))
                {
                    SelectedLogEntryIndex = newIndex;
                    break;
                }
            }
        }

        private bool MatchesSearch(LogEntry entry, string searchText)
        {
            if (SearchCaseSensitive)
            {
                return entry.Message.Contains(searchText) || entry.Tag.Contains(searchText);
            }
            else
            {
                searchText = searchText.ToLowerInvariant();
                return entry.Message.ToLowerInvariant().Contains(searchText) ||
                       entry.Tag.ToLowerInvariant().Contains(searchText);
            }
        }

        #endregion

        #region Dialogs

        public object ActiveDialog
        {
            get => _activeDialog;
            private set
            {
                if (Equals(value, _activeDialog)) return;
                _activeDialog = value;
                OnPropertyChanged();
            }
        }

        private readonly object _activeDialogLock = new object();

        public async Task<TResult> ShowDialog<TViewModel, TResult>(TViewModel viewModel) where TViewModel : IDialogViewModel<TResult>
        {
            object oldActiveDialog;
            lock (_activeDialogLock)
            {
                oldActiveDialog = ActiveDialog;
                ActiveDialog = viewModel;
            }

            try
            {
                return await viewModel.Result;
            }
            finally
            {
                lock (_activeDialogLock)
                {
                    ActiveDialog = oldActiveDialog;
                }
            }
        }

        #endregion

        #region Line removal
        public ICommand RemoveLeadingLinesCommand { get; }

        public ICommand RemoveTrailingLinesCommand { get; }

        private void RemoveLinesBeforeSelected()
        {
            LogEntries = LogEntries.SkipWhile(x => x != SelectedLogEntry).ToArray();
        }

        private IEnumerable<LogEntry> RemoveLinesAfterSelected()
        {
            foreach (var logEntry in LogEntries)
            {
                yield return logEntry;
                if (logEntry == SelectedLogEntry) yield break;
            }
        }

        #endregion
    }
}