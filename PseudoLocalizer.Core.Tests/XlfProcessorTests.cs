namespace PseudoLocalizer.Core.Tests
{
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    public class XlfProcessorTests
    {
        private const string Test1FileName = "Test1.xlf";
        private const string OutputFileName = "out.xlf";

        [SetUp]
        public void SetUp()
        {
            DeleteOutputFile();
        }

        [Test]
        public void ShouldLeaveTheXmlUntouchedWhenUsingAnIdentityTransformation()
        {
            using (var inputStream = new FileStream(Test1FileName, FileMode.Open, FileAccess.Read))
            using (var outputStream = new FileStream(OutputFileName, FileMode.Create, FileAccess.Write))
            {
                var processor = new XlfProcessor();
                processor.Transform(inputStream, outputStream);
            }

            Assert.That(File.ReadAllBytes(Test1FileName), Is.EqualTo(File.ReadAllBytes(OutputFileName)), "the output file is identical to the input file.");
        }

        [Test]
        public void ShouldReverseStringsButLeaveTheCommentsUntouchedWhenTransformingWithAStringReverseTransformation()
        {
            using (var inputStream = new FileStream(Test1FileName, FileMode.Open, FileAccess.Read))
            using (var outputStream = new FileStream(OutputFileName, FileMode.Create, FileAccess.Write))
            {
                var processor = new XlfProcessor();
                processor.TransformString += (s, e) => { e.Value = Mirror.Transform(e.Value); };
                processor.Transform(inputStream, outputStream);
            }

            var original = File.ReadAllText(Test1FileName);
            var transformed = File.ReadAllText(OutputFileName);
            Assert.That(original.Contains("<source>Something</source>"));
            Assert.That(!original.Contains("<target state=\"translated\">gnihtemoS</target>"));
            Assert.That(transformed.Contains("<target state=\"translated\">gnihtemoS</target>"));
            Assert.That(!transformed.Contains("<target state=\"translated\">Something</target>"));
            Assert.That(original.Contains("<note>Blah</note>"));
            Assert.That(transformed.Contains("<note>Blah</note>"));
        }

        [Test]
        public void ShouldAddFunnyAccentsWhenTransformingWithTheAccenterTransformation()
        {
            using (var inputStream = new FileStream(Test1FileName, FileMode.Open, FileAccess.Read))
            using (var outputStream = new FileStream(OutputFileName, FileMode.Create, FileAccess.Write))
            {
                var processor = new XlfProcessor();
                processor.TransformString += (s, e) => { e.Value = Accents.Transform(e.Value); };
                processor.Transform(inputStream, outputStream);
            }

            var transformed = File.ReadAllText(OutputFileName);
            Assert.That(!transformed.Contains("<target state=\"translated\">Dude</target>"));
            Assert.That(transformed.Contains("<note>Foo</note>"));
            Assert.That(transformed.Contains("<target state=\"translated\">\u00d0\u00fb\u00f0\u00e9</target>"));
        }

        [Test]
        public void ShouldApplyMultipleTransformations()
        {
            using (var inputStream = new FileStream(Test1FileName, FileMode.Open, FileAccess.Read))
            using (var outputStream = new FileStream(OutputFileName, FileMode.Create, FileAccess.Write))
            {
                var processor = new XlfProcessor();
                processor.TransformString += (s, e) => { e.Value = e.Value + "1"; };
                processor.TransformString += (s, e) => { e.Value = Brackets.Transform(e.Value); };
                processor.TransformString += (s, e) => { e.Value = e.Value + "2"; };
                processor.Transform(inputStream, outputStream);
            }

            var original = File.ReadAllText(Test1FileName);
            var transformed = File.ReadAllText(OutputFileName);
            Assert.That(original.Contains("<source>Dude</source>"));
            Assert.That(original.Contains("<source>Whatever</source>"));
            Assert.That(original.Contains("<source>Something</source>"));
            Assert.That(original.Contains("<source>Anything</source>"));
            Assert.That(transformed.Contains("<target state=\"translated\">[Dude1]2</target>"));
            Assert.That(transformed.Contains("<target state=\"translated\">[Whatever1]2</target>"));
            Assert.That(transformed.Contains("<target state=\"translated\">[Something1]2</target>"));
            Assert.That(transformed.Contains("<target state=\"translated\">[Anything1]2</target>"));
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
