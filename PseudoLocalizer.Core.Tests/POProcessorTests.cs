using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace PseudoLocalizer.Core.Tests
{
    [TestFixture]
    public class POProcessorTests
    {
        private const string Test1FileName = "Test1.po";
        private const string Test2FileName = "Test2.po";
        private const string OutputFileName = "out.po";

        [SetUp]
        public void SetUp()
        {
            DeleteOutputFile();
        }

        [Test]
        public void ShouldLeaveTheFileUntouchedWhenUsingAnIdentityTransformation()
        {
            using (var inputStream = new FileStream(Test1FileName, FileMode.Open, FileAccess.Read))
            using (var outputStream = new FileStream(OutputFileName, FileMode.Create, FileAccess.Write))
            {
                var processor = new POProcessor("en");
                processor.Transform(inputStream, outputStream);
            }

            Assert.That(OutputFileName, Does.Exist);
            FileAssert.AreEqual(Test1FileName, OutputFileName);
        }

        [Test]
        public void ShouldReverseStringsButLeaveTheCommentsUntouchedWhenTransformingWithAStringReverseTransformation()
        {
            using (var inputStream = new FileStream(Test1FileName, FileMode.Open, FileAccess.Read))
            using (var outputStream = new FileStream(OutputFileName, FileMode.Create, FileAccess.Write))
            {
                var processor = new POProcessor("qps-Ploc");
                processor.TransformString += (_, e) => e.Value = Mirror.Instance.Transform(e.Value);
                processor.Transform(inputStream, outputStream);
            }

            Assert.That(OutputFileName, Does.Exist);
            FileAssert.AreEqual("Mirror.po", OutputFileName);
        }

        [Test]
        public void ShouldAddFunnyAccentsWhenTransformingWithTheAccenterTransformation()
        {
            using (var inputStream = new FileStream(Test1FileName, FileMode.Open, FileAccess.Read))
            using (var outputStream = new FileStream(OutputFileName, FileMode.Create, FileAccess.Write))
            {
                var processor = new POProcessor("qps-Ploc");
                processor.TransformString += (_, e) => e.Value = Accents.Instance.Transform(e.Value);
                processor.Transform(inputStream, outputStream);
            }

            Assert.That(OutputFileName, Does.Exist);
            FileAssert.AreEqual("Accents.po", OutputFileName);
        }

        [Test]
        public void ShouldApplyMultipleTransformations()
        {
            using (var inputStream = new FileStream(Test1FileName, FileMode.Open, FileAccess.Read))
            using (var outputStream = new FileStream(OutputFileName, FileMode.Create, FileAccess.Write))
            {
                var processor = new POProcessor("qps-Ploc");
                processor.TransformString += (_, e) => e.Value += "1";
                processor.TransformString += (_, e) => e.Value = Brackets.Instance.Transform(e.Value);
                processor.TransformString += (_, e) => e.Value += "2";
                processor.Transform(inputStream, outputStream);
            }

            Assert.That(OutputFileName, Does.Exist);
            FileAssert.AreEqual("Multiple.po", OutputFileName);
        }


        [Test]
        public void ShouldThrowForBadInputFile()
        {
            POFileFormatException ex;

            using (var inputStream = new FileStream(Test2FileName, FileMode.Open, FileAccess.Read))
            using (var outputStream = new FileStream(OutputFileName, FileMode.Create, FileAccess.Write))
            {
                var processor = new POProcessor("qps-Ploc");
                ex = Assert.Throws<POFileFormatException>(() => processor.Transform(inputStream, outputStream));
            }

            Assert.That(ex.Message, Is.EqualTo("Entry beginning at 18,1 must not have an empty id." + Environment.NewLine));
            Assert.That(ex.DiagnosticMessages.First(), Is.EqualTo("Entry beginning at 18,1 must not have an empty id."));
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
