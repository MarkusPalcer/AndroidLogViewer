using System.IO;
using AndroidLogViewer.Dialogs.Export;
using AndroidLogViewer.Extensions;
using AndroidLogViewer.Tests.Annotations;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AndroidLogViewer.Tests
{
    [TestClass]
    public class ExportImportIntegrationTest
    {
        [TestMethod]
        [Issue(26)]
        public void ExportStartOfBuffer()
        {
            RunExportImportTest(Properties.Resources.StartOfBufferLog);
        }

        // Runs an import -> export -> import and checks that the data is unchanged.
        private static void RunExportImportTest(string log)
        {
            var entries = LogcatParser.ReadLogEntries(new StringReader(log));
            var originalLines = new StringReader(log).GetLines();
            var sut = new SimpleLogExportVisitor();
            sut.Visit(entries);
            sut.LogLines.Should().Equal(originalLines);
        }
    }
}
