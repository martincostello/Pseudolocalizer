using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace PseudoLocalizer
{
    [TestFixture]
    public class EndToEndTest
    {
        [Test]
        public async Task ShouldLeaveTheXmlUntouchedWhenAlreadyTransformed()
        {
            // Arrange
            string fileName = Path.GetFullPath("localized.xlf");
            byte[] original = await File.ReadAllBytesAsync(fileName);

            // Act
            Program.Main(new[] { fileName, "--overwrite", "--force" });

            // Assert
            Assert.That(File.ReadAllBytes(fileName), Is.EqualTo(original), "The input file has changed.");
        }

        [Test]
        public async Task ShouldChangeTheLengthenCharacter()
        {
            // Arrange
            string inputFileName = Path.GetFullPath("not-localized.xlf");
            string outputFileName = Path.GetFullPath("not-localized.qps-Ploc.xlf");
            byte[] original = await File.ReadAllBytesAsync(inputFileName);

            // Act
            Program.Main(new[] { inputFileName, "--lengthen-char", "." });

            // Assert
            Assert.That(File.ReadAllBytes(inputFileName), Is.EqualTo(original), "The input file has changed.");
            Assert.That(File.ReadAllBytes(outputFileName), Is.Not.EqualTo(outputFileName), "The output file has not changed.");
            Assert.That(File.ReadAllText(outputFileName), Contains.Substring(">[Åñýţĥîñĝ···]<"), "The specified lengthen character was not used.");
        }
    }
}
