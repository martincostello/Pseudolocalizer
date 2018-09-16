namespace PseudoLocalizer.Core.Tests
{
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    public class XlfProcessorTests
    {
        private const string Xlf12FileName = "1.2.xlf";
        private const string Xlf20FileName = "2.0.xlf";
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
                processor.TransformString += (s, e) => { e.Value = Mirror.Transform(e.Value); };
                processor.Transform(inputStream, outputStream);
            }

            var original = File.ReadAllText(Xlf12FileName);
            var transformed = File.ReadAllText(OutputFileName);
            Assert.That(original.Contains("<source>Something</source>"));
            Assert.That(!original.Contains("<target state=\"translated\">gnihtemoS</target>"));
            Assert.That(transformed.Contains("<target state=\"translated\">gnihtemoS</target>"));
            Assert.That(!transformed.Contains("<target state=\"translated\">Something</target>"));
            Assert.That(transformed.Contains("=\"qps-ploc\""));
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
                processor.TransformString += (s, e) => { e.Value = Accents.Transform(e.Value); };
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
                processor.TransformString += (s, e) => { e.Value = e.Value + "1"; };
                processor.TransformString += (s, e) => { e.Value = Brackets.Transform(e.Value); };
                processor.TransformString += (s, e) => { e.Value = e.Value + "2"; };
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
            Assert.That(transformed.Contains("=\"qps-ploc\""));
            Assert.That(!transformed.Contains("=\"ja-JP\""));
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
