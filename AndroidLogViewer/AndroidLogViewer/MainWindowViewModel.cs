using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using AndroidLogViewer.Annotations;
using Microsoft.Win32;
using Prism.Commands;

namespace AndroidLogViewer
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private IEnumerable<LogEntry> _logEntries;
        
        private ICollectionView _defaultView;

        public MainWindowViewModel()
        {
            SearchForwardCommand = new DelegateCommand<string>(searchText =>
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
            });

            SearchBackwardCommand = new DelegateCommand<string>(searchText =>
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
            });

            RemoveWhitelistedTagCommand = new DelegateCommand<string>(tag => WhitelistedTags.Remove(tag));
            RemoveProcessThreadFilterCommand = new DelegateCommand<ProcessThreadFilter>(f => ActiveProcessThreadFilters.Remove(f));
            AddWhitelistedTagCommand = new DelegateCommand<string>(tag => WhitelistedTags.Add(tag));
            AddProcessThreadFilterCommand = new DelegateCommand<ProcessThreadFilter>(filter =>
            {
                ActiveProcessThreadFilters.Add(filter);
            });
            OpenFileCommand = new DelegateCommand(OpenFile);
            ActiveProcessThreadFilters.CollectionChanged += (sender, args) => _defaultView?.Refresh();
            WhitelistedTags.CollectionChanged += (sender, args) => _defaultView?.Refresh();
        }

        public IEnumerable<LogEntry> LogEntries
        {
            get => _logEntries;
            private set
            {
                if (Equals(value, _logEntries)) return;
                _logEntries = value;

                _defaultView = CollectionViewSource.GetDefaultView(_logEntries);
                _defaultView.Filter = FilterLogEntries;

                OnPropertyChanged();
            }
        }

        private bool FilterLogEntries(object x)
        {
            if (!(x is LogEntry entry)) return false;

            if (ActiveProcessThreadFilters.Any() && !ActiveProcessThreadFilters.Any(f => f.Matches(entry))) return false;
            if (WhitelistedTags.Any() && !WhitelistedTags.Any(t => entry.Tag.Equals(t))) return false;
            if (!ShowDebug && entry.Level.Equals("D")) return false;
            if (!ShowError && entry.Level.Equals("E")) return false;
            if (!ShowInfo && entry.Level.Equals("I")) return false;
            if (!ShowVerbose && entry.Level.Equals("V")) return false;
            if (!ShowWarn && entry.Level.Equals("W")) return false;

            return true;
        }

        public ICommand OpenFileCommand { get; }

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

        public ObservableCollection<ProcessThreadFilter> ActiveProcessThreadFilters { get; } = new ObservableCollection<ProcessThreadFilter>();

        public ICommand AddProcessThreadFilterCommand { get; }

        public ICommand RemoveProcessThreadFilterCommand { get; }
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

        public ICommand AddWhitelistedTagCommand { get; }

        public ICommand RemoveWhitelistedTagCommand { get; }
        #endregion

        #region Loglevel filtering

        private bool _showVerbose = true;
        private bool _showDebug = true;
        private bool _showInfo = true;
        private bool _showWarn = true;
        private bool _showError = true;
        private int _selectedLogEntryIndex;

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

            LogEntries = null;
            AvailableProcessThreadFilters = null;


            LogEntries = await Task<ObservableCollection<LogEntry>>.Factory.StartNew(() => ReadLogEntries(dlg.FileName));
            AvailableProcessThreadFilters = await Task<ObservableCollection<ProcessThreadFilter>>.Factory.StartNew(GenerateProcessThreadFilters);
            AvailableTags = new ObservableCollection<string>(await Task<string[]>.Factory.StartNew(() => LogEntries.Select(x => x.Tag).Distinct().OrderBy(x => x).ToArray()));
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

        private ObservableCollection<LogEntry> ReadLogEntries(string fileName)
        {
            using (var reader = File.OpenText(fileName))
            {
                var result = new ObservableCollection<LogEntry>();

                var line = reader.ReadLine();
                var regex = new Regex("^(?<datetime>\\d\\d-\\d\\d \\d\\d:\\d\\d:\\d\\d.\\d\\d\\d)\\s+(?<pid>\\d+)\\s+(?<tid>\\d+)\\s+(?<level>\\w)\\s+(?<tag>[^:]+):\\s(?<message>.*)$", RegexOptions.Compiled);

                while (line != null)
                {
                    var matches = regex.Matches(line);

                    foreach (Match match in matches)
                    {
                        result.Add(new LogEntry
                        {
                            Message = match.Groups["message"].Value,
                            Level = match.Groups["level"].Value,
                            Process = int.Parse(match.Groups["pid"].Value),
                            Thread = int.Parse(match.Groups["tid"].Value),
                            Tag = match.Groups["tag"].Value
                        });
                    }

                    line = reader.ReadLine();
                }

                return result;
            }
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

        private bool MatchesSearch(LogEntry entry, string searchText)
        {
            return entry.Message.Contains(searchText);
        }

        #endregion


    }
}