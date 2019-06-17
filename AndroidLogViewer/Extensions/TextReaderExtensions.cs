using System.Collections.Generic;
using System.IO;

namespace AndroidLogViewer.Extensions
{
    public static class TextReaderExtensions
    {
        public static IEnumerable<string> GetLines(this TextReader self)
        {
            var line = self.ReadLine();
            while (line != null)
            {
                yield return line;
                line = self.ReadLine();
            }
        }
    }
}