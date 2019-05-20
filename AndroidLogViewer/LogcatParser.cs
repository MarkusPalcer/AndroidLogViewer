﻿using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AndroidLogViewer
{
    public class LogcatParser
    {
        private const string DateTimeRegularExpression = "(?<datetime>\\d\\d-\\d\\d \\d\\d:\\d\\d:\\d\\d.\\d\\d\\d)";
        private const string PidRegularExpression = "(?<pid>\\d+)";
        private const string TidRegularExpression = "(?<tid>\\d+)";
        private const string LogLevelRegularExpression = "(?<level>\\w)";
        private const string TagRegularExpression = "(?<tag>[^:]+):";
        private const string MessageRegularExpression = "(?<message>.*)";
        private static readonly string TrailingLineRegularExpression =$"(?<trailingline>\\s+?){MessageRegularExpression}";
        private static readonly string DefaultLogcatRegularExpression = $"(?<premessage>{DateTimeRegularExpression}\\s+{PidRegularExpression}\\s+{TidRegularExpression}\\s+{LogLevelRegularExpression}\\s+{TagRegularExpression}\\s+?){MessageRegularExpression}";
        private static readonly string AndroidStudioRegularExpression = $"(?<premessage>{DateTimeRegularExpression}\\s+{PidRegularExpression}-{TidRegularExpression}[^\\s]+\\s+{LogLevelRegularExpression}/{TagRegularExpression}\\s+?){MessageRegularExpression}";
        
        
        private static readonly string[] RecognizedRegularExpressions = {DefaultLogcatRegularExpression, AndroidStudioRegularExpression, TrailingLineRegularExpression};
        private static readonly string Pattern = $"^{string.Join("|", RecognizedRegularExpressions.Select(x => $"({x})"))}$";
        private static readonly Regex RegularExpression = new Regex(Pattern, RegexOptions.Compiled);

        static LogcatParser()
        {
        }

        public static ObservableCollection<LogEntry> ReadLogEntries(TextReader reader)
        {
            var result = new ObservableCollection<LogEntry>();
            LogEntry pivotEntry = null;

            var line = reader.ReadLine();

            while (line != null)
            {
                var match = RegularExpression.Match(line);

                if (match.Success)
                {
                    LogEntry newEntry = null;
                    if (match.Groups["datetime"].Success)
                    {
                        newEntry = new LogEntry
                        {
                            Message = match.Groups["message"].Value.Trim(),
                            Level = match.Groups["level"].Value.Trim(),
                            Process = int.Parse(match.Groups["pid"].Value.Trim()),
                            Thread = int.Parse(match.Groups["tid"].Value.Trim()),
                            Tag = match.Groups["tag"].Value.Trim(),
                            Time = match.Groups["datetime"].Value.Trim(),
                        };
                        if (match.Groups["premessage"].Success) newEntry.PivotSize = match.Groups["premessage"].Length;

                        pivotEntry = newEntry;
                    }
                    else if (match.Groups["trailingline"].Success && pivotEntry != null)
                    {
                        var trimmedMessage = match.Groups["message"].Value.Trim();
                        var originalSpaceCount = match.Groups["message"].Length - trimmedMessage.Length + 1;
                        var messageSpaceCount = originalSpaceCount - pivotEntry.PivotSize;

                        newEntry = new LogEntry
                        {
                            Message = new string(' ', messageSpaceCount) + trimmedMessage,
                            Level = pivotEntry.Level,
                            Process = pivotEntry.Process,
                            Thread = pivotEntry.Thread,
                            Tag = pivotEntry.Tag,
                            Time = pivotEntry.Time,
                        };
                    }
                    if (newEntry != null) result.Add(newEntry);
                }

                line = reader.ReadLine();
            }

            return result;
        }
    }
}