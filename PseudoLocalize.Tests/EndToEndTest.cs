using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
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
            await Program.Main([fileName, "--overwrite", "--force"]);

            Console.WriteLine(await File.ReadAllTextAsync(fileName));

            // Assert
            Assert.That(await File.ReadAllBytesAsync(fileName), Is.EqualTo(original), "The input file has changed.");
        }

        [Test]
        public async Task ShouldChangeTheLengthenCharacter()
        {
            // Arrange
            string inputFileName = Path.GetFullPath("not-localized.xlf");
            string outputFileName = Path.GetFullPath("not-localized.qps-Ploc.xlf");
            byte[] original = await File.ReadAllBytesAsync(inputFileName);

            // Act
            await Program.Main([inputFileName, "--lengthen-char", "."]);

            Console.WriteLine(await File.ReadAllTextAsync(outputFileName));

            // Assert
            Assert.That(await File.ReadAllBytesAsync(inputFileName), Is.EqualTo(original), "The input file has changed.");
            Assert.That(await File.ReadAllBytesAsync(outputFileName), Is.Not.EqualTo(outputFileName), "The output file has not changed.");
            Assert.That(await File.ReadAllTextAsync(outputFileName), Contains.Substring(">[Åñýţĥîñĝ···]<"), "The specified lengthen character was not used.");

            var document = new XmlDocument();
            document.Load(outputFileName);
        }

        [Test]
        public async Task ShouldChangeXlfFile()
        {
            // Arrange
            string inputFileName = Path.GetFullPath("issue-362.xlf");
            string outputFileName = Path.GetFullPath("issue-362.qps-Ploc.xlf");
            byte[] original = await File.ReadAllBytesAsync(inputFileName);

            // Act
            await Program.Main([inputFileName, "--lengthen", "--accents", "--brackets"]);

            Console.WriteLine(await File.ReadAllTextAsync(outputFileName));

            // Assert
            Assert.That(await File.ReadAllBytesAsync(inputFileName), Is.EqualTo(original), "The input file has changed.");
            Assert.That(await File.ReadAllBytesAsync(outputFileName), Is.Not.EqualTo(original), "The output file has not changed.");

            var document = new XmlDocument();
            document.Load(outputFileName);
        }

        [Test]
        public async Task ShouldChangeXlfFileWithGroups()
        {
            // Arrange
            string inputFileName = Path.GetFullPath("issue-362.grouped.xlf");
            string outputFileName = Path.GetFullPath("issue-362.grouped.qps-Ploc.xlf");
            byte[] original = await File.ReadAllBytesAsync(inputFileName);

            // Act
            await Program.Main([inputFileName, "--lengthen", "--accents", "--brackets"]);

            Console.WriteLine(await File.ReadAllTextAsync(outputFileName));

            // Assert
            Assert.That(await File.ReadAllBytesAsync(inputFileName), Is.EqualTo(original), "The input file has changed.");
            Assert.That(await File.ReadAllBytesAsync(outputFileName), Is.Not.EqualTo(original), "The output file has not changed.");

            var document = new XmlDocument();
            document.Load(outputFileName);
        }
    }
}
