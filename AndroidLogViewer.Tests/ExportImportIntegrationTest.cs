using System;
using System.IO;
using AndroidLogViewer.Dialogs.Export;
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
        private void RunExportImportTest(string log)
        {
            var entries = LogcatParser.ReadLogEntries(new StringReader(log));
            var originalLines = log.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
            var sut = new SimpleLogExportVisitor();
            sut.Visit(entries);
            sut.LogLines.Should().Equal(originalLines);
        }
    }
}
