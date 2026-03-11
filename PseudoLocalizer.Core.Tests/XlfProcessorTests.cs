namespace PseudoLocalizer.Core.Tests
{
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    public class XlfProcessorTests
    {
        private const string Xlf12FileName = "1.2.xlf";
        private const string Xlf20FileName = "2.0.xlf";
        private const string Xlf12InlineFileName = "1.2-inline.xlf";
        private const string Xlf20InlineFileName = "2.0-inline.xlf";
        private const string OutputFileName = "out.xlf";

        [SetUp]
        public void SetUp()
        {
            DeleteOutputFile();
        }

        [Test]
        [TestCase(Xlf12FileName)]
        [TestCase(Xlf20FileName)]
        public void ShouldLeaveTheXmlUntouchedWhenUsingAnIdentityTransformation(string inputFileName)
        {
            using (var inputStream = new FileStream(inputFileName, FileMode.Open, FileAccess.Read))
            using (var outputStream = new FileStream(OutputFileName, FileMode.Create, FileAccess.Write))
            {
                var processor = new XlfProcessor();
                processor.Transform(inputStream, outputStream);
            }

            Assert.That(File.ReadAllBytes(inputFileName), Is.EqualTo(File.ReadAllBytes(OutputFileName)), "the output file is identical to the input file.");
        }

        [Test]
        [TestCase(Xlf12FileName)]
        [TestCase(Xlf20FileName)]
        public void ShouldReverseStringsButLeaveTheCommentsUntouchedWhenTransformingWithAStringReverseTransformation(string inputFileName)
        {
            using (var inputStream = new FileStream(inputFileName, FileMode.Open, FileAccess.Read))
            using (var outputStream = new FileStream(OutputFileName, FileMode.Create, FileAccess.Write))
            {
                var processor = new XlfProcessor();
                processor.TransformString += (_, e) => e.Value = Mirror.Instance.Transform(e.Value);
                processor.Transform(inputStream, outputStream);
            }

            var original = File.ReadAllText(Xlf12FileName);
            var transformed = File.ReadAllText(OutputFileName);

            Assert.That(original.Contains("<source>Something</source>"));
            Assert.That(!original.Contains("<target state=\"translated\">gnihtemoS</target>"));
            Assert.That(transformed.Contains("<target state=\"translated\">gnihtemoS</target>"));
            Assert.That(!transformed.Contains("<target state=\"translated\">Something</target>"));
            Assert.That(transformed.Contains("=\"qps-Ploc\""));
            Assert.That(!transformed.Contains("=\"ja-JP\""));
        }

        [Test]
        [TestCase(Xlf12FileName)]
        [TestCase(Xlf20FileName)]
        public void ShouldAddFunnyAccentsWhenTransformingWithTheAccenterTransformation(string inputFileName)
        {
            using (var inputStream = new FileStream(inputFileName, FileMode.Open, FileAccess.Read))
            using (var outputStream = new FileStream(OutputFileName, FileMode.Create, FileAccess.Write))
            {
                var processor = new XlfProcessor();
                processor.TransformString += (_, e) => e.Value = Accents.Instance.Transform(e.Value);
                processor.Transform(inputStream, outputStream);
            }

            var transformed = File.ReadAllText(OutputFileName);

            Assert.That(!transformed.Contains("<target>Dude</target>"));
            Assert.That(transformed.Contains("<target>\u00d0\u00fb\u00f0\u00e9</target>"));
        }

        [Test]
        [TestCase(Xlf12FileName)]
        [TestCase(Xlf20FileName)]
        public void ShouldApplyMultipleTransformationsXlf(string inputFileName)
        {
            using (var inputStream = new FileStream(inputFileName, FileMode.Open, FileAccess.Read))
            using (var outputStream = new FileStream(OutputFileName, FileMode.Create, FileAccess.Write))
            {
                var processor = new XlfProcessor();
                processor.TransformString += (_, e) => e.Value += "1";
                processor.TransformString += (_, e) => e.Value = Brackets.Instance.Transform(e.Value);
                processor.TransformString += (_, e) => e.Value += "2";
                processor.Transform(inputStream, outputStream);
            }

            var original = File.ReadAllText(Xlf12FileName);
            var transformed = File.ReadAllText(OutputFileName);

            Assert.That(original.Contains("<source>Dude</source>"));
            Assert.That(original.Contains("<source>Whatever</source>"));
            Assert.That(original.Contains("<source>Something</source>"));
            Assert.That(original.Contains("<source>Anything</source>"));
            Assert.That(transformed.Contains("<target>[Dude1]2</target>"));
            Assert.That(transformed.Contains("<target>[Whatever1]2</target>"));
            Assert.That(transformed.Contains("<target state=\"translated\">[Something1]2</target>"));
            Assert.That(transformed.Contains("<target state=\"translated\">[Anything1]2</target>"));
            Assert.That(transformed.Contains("=\"qps-Ploc\""));
            Assert.That(!transformed.Contains("=\"ja-JP\""));
        }

        [Test]
        [TestCase(Xlf12InlineFileName)]
        [TestCase(Xlf20InlineFileName)]
        public void ShouldLeaveTheXmlUntouchedWhenUsingAnIdentityTransformationWithInlineElements(string inputFileName)
        {
            using (var inputStream = new FileStream(inputFileName, FileMode.Open, FileAccess.Read))
            using (var outputStream = new FileStream(OutputFileName, FileMode.Create, FileAccess.Write))
            {
                var processor = new XlfProcessor();
                processor.Transform(inputStream, outputStream);
            }

            Assert.That(File.ReadAllBytes(inputFileName), Is.EqualTo(File.ReadAllBytes(OutputFileName)), "the output file is identical to the input file.");
        }

        [Test]
        [TestCase(Xlf12InlineFileName)]
        [TestCase(Xlf20InlineFileName)]
        public void ShouldPreserveInlineElementsWhenTransformingWithAccents(string inputFileName)
        {
            using (var inputStream = new FileStream(inputFileName, FileMode.Open, FileAccess.Read))
            using (var outputStream = new FileStream(OutputFileName, FileMode.Create, FileAccess.Write))
            {
                var processor = new XlfProcessor();
                processor.TransformString += (_, e) => e.Value = Accents.Instance.Transform(e.Value);
                processor.Transform(inputStream, outputStream);
            }

            var transformed = File.ReadAllText(OutputFileName);

            // Inline <x> elements should be preserved in target
            Assert.That(transformed, Does.Contain("equiv-text=\"{{ count }}\""), "The interpolation element should be preserved.");
            Assert.That(transformed, Does.Contain("equiv-text=\"{{ name }}\""), "The name interpolation element should be preserved.");
            Assert.That(transformed, Does.Contain("equiv-text=\"{{ place }}\""), "The place interpolation element should be preserved.");

            // Text around inline elements should be transformed
            Assert.That(transformed, Does.Not.Contain("<target>All ("), "The text should be transformed with accents.");
            Assert.That(transformed, Does.Not.Contain("<target>Hello "), "The text should be transformed with accents.");

            // Plain text entry should also be transformed
            Assert.That(transformed, Does.Contain("<target>"), "There should be target elements.");
        }

        private static void DeleteOutputFile()
        {
            if (File.Exists(OutputFileName))
            {
                File.Delete(OutputFileName);
                Assert.That(!File.Exists(OutputFileName));
            }
        }
    }
}
