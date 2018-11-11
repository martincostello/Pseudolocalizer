namespace PseudoLocalizer.Core.Tests
{
    using System.IO;
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class ResxProcessorTests
    {
        private const string Test1FileName = "Test1.resx";
        private const string OutputFileName = "out.resx";

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
                var processor = new ResxProcessor();
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
                var processor = new ResxProcessor();
                processor.TransformString += (s, e) => { e.Value = Mirror.Instance.Transform(e.Value); };
                processor.Transform(inputStream, outputStream);
            }

            var original = File.ReadAllText(Test1FileName);
            var transformed = File.ReadAllText(OutputFileName);
            Assert.That(original.Contains("<value>Dude</value>"));
            Assert.That(!original.Contains("<value>eduD</value>"));
            Assert.That(transformed.Contains("<value>eduD</value>"));
            Assert.That(!transformed.Contains("<value>Dude</value>"));
            Assert.That(original.Contains("<comment>Foo</comment>"));
            Assert.That(transformed.Contains("<comment>Foo</comment>"));
        }

        [Test]
        public void ShouldAddFunnyAccentsWhenTransformingWithTheAccenterTransformation()
        {
            using (var inputStream = new FileStream(Test1FileName, FileMode.Open, FileAccess.Read))
            using (var outputStream = new FileStream(OutputFileName, FileMode.Create, FileAccess.Write))
            {
                var processor = new ResxProcessor();
                processor.TransformString += (s, e) => { e.Value = Accents.Instance.Transform(e.Value); };
                processor.Transform(inputStream, outputStream);
            }

            var transformed = File.ReadAllText(OutputFileName);
            Assert.That(transformed.Contains("<value>\u00d0\u00fb\u00f0\u00e9</value>"));
            Assert.That(!transformed.Contains("<value>Dude</value>"));
            Assert.That(transformed.Contains("<comment>Foo</comment>"));
        }

        [Test]
        public void ShouldApplyMultipleTransformations()
        {
            using (var inputStream = new FileStream(Test1FileName, FileMode.Open, FileAccess.Read))
            using (var outputStream = new FileStream(OutputFileName, FileMode.Create, FileAccess.Write))
            {
                var processor = new ResxProcessor();
                processor.TransformString += (s, e) => { e.Value = e.Value + "1"; };
                processor.TransformString += (s, e) => { e.Value = Brackets.Instance.Transform(e.Value); };
                processor.TransformString += (s, e) => { e.Value = e.Value + "2"; };
                processor.Transform(inputStream, outputStream);
            }

            var original = File.ReadAllText(Test1FileName);
            var transformed = File.ReadAllText(OutputFileName);
            Assert.That(original.Contains("<value>Dude</value>"));
            Assert.That(transformed.Contains("<value>[Dude1]2</value>"));
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
