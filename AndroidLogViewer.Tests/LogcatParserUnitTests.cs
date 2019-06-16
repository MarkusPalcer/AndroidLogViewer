using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AndroidLogViewer.Tests
{
    [TestClass]
    public class LogcatParserUnitTests
    {
        [TestMethod]
        public void ParseBeginOfBufferEntries()
        {
            var entries = LogcatParser.ReadLogEntries(new StringReader(Properties.Resources.StartOfBufferLog));
            entries.Should().AllBeOfType<StartOfBufferEntry>();
        }

        [TestMethod]
        public void ParseRegularEntries()
        {
            var entries = LogcatParser.ReadLogEntries(new StringReader(Properties.Resources.RegularLogLines));
            entries.Should().AllBeOfType<LogEntry>();
            entries.OfType<StartOfBufferEntry>().Should().BeEmpty();
        }
    }
}