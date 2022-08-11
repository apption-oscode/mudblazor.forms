using MudForms.Documentation;
using System.Linq;
using Xunit;

namespace Elemental.Tests
{
    public class SampleParsingTests
    {
        private const string _testFilePath = "./Samples/Examples/StarshipForm.razor";
        private const string _testTitle = "Test Component";
        private const string _testDescription = "Lorem ipsum dolor sit amet.";


        [Fact]
        public void CanReadFile()
        {
            var lines = ParsedFile.ReadLines(_testFilePath);
            
            Assert.NotNull(lines);
            Assert.Equal(71, lines.Count());
        }

        [Fact]
        public void CanFindTitle()
        {
            var lines = ParsedFile.ReadLines(_testFilePath);
            var title = ParsedFile.ParseTitle(lines);
            
            Assert.Equal("Model Form", title);
        }

        [Fact]
        public void CanFindDescription()
        {
            var lines = ParsedFile.ReadLines(_testFilePath);
            var description = ParsedFile.ParseDescription(lines);

            Assert.StartsWith("Form is generated from .NET", description);            
        }

        [Fact]
        public void CanFindHtml()
        {
            var lines = ParsedFile.ReadLines(_testFilePath);
            var html = ParsedFile.ParseHtml(lines);

            Assert.Equal(11, html.Count);
            Assert.Contains(@"<MudModelForm", html.First());
            Assert.Contains(@"/>", html.Last());
        }

        [Fact]
        public void CanFindCode()
        {
            var lines = ParsedFile.ReadLines(_testFilePath);
            var code = ParsedFile.ParseCode(lines);

            Assert.Equal(40, code.Count);
            Assert.Equal("@code {", code.First());
            Assert.Equal("}", code.Last());
        }


    }
}
