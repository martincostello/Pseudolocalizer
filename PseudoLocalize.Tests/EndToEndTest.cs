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
    }
}
