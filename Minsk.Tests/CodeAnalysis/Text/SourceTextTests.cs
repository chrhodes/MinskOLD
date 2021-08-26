
using Minsk.CodeAnalysis.Text;

using Xunit;

namespace Minsk.Tests.CodeAnalysis.Syntax.Text
{
    public class SourceTextTests
    {
        [Theory]
        [InlineData(".", 1)]
        [InlineData(".\r\n", 2)]
        [InlineData(".\r\n\r\n", 3)]
        public void SourceText_IncludesLastLine(string text, int expectedLineCount)
        {
            var soruceText = SourceText.From(text);
            Assert.Equal(expectedLineCount, soruceText.Lines.Length);
        }
    }
}