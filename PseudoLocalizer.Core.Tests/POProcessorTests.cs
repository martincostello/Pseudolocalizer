using System.IO;
using NUnit.Framework;

namespace PseudoLocalizer.Core.Tests
{
    [TestFixture]
    public class POProcessorTests
    {
        private const string Test1FileName = "Test1.po";
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

            FileAssert.AreEqual(Test1FileName, OutputFileName);
        }

        [Test]
        public void ShouldReverseStringsButLeaveTheCommentsUntouchedWhenTransformingWithAStringReverseTransformation()
        {
            using (var inputStream = new FileStream(Test1FileName, FileMode.Open, FileAccess.Read))
            using (var outputStream = new FileStream(OutputFileName, FileMode.Create, FileAccess.Write))
            {
                var processor = new POProcessor("qps-Ploc");
                processor.TransformString += (s, e) => { e.Value = Mirror.Instance.Transform(e.Value); };
                processor.Transform(inputStream, outputStream);
            }

            FileAssert.AreEqual("Mirror.po", OutputFileName);
        }

        [Test]
        public void ShouldAddFunnyAccentsWhenTransformingWithTheAccenterTransformation()
        {
            using (var inputStream = new FileStream(Test1FileName, FileMode.Open, FileAccess.Read))
            using (var outputStream = new FileStream(OutputFileName, FileMode.Create, FileAccess.Write))
            {
                var processor = new POProcessor("qps-Ploc");
                processor.TransformString += (s, e) => { e.Value = Accents.Instance.Transform(e.Value); };
                processor.Transform(inputStream, outputStream);
            }

            FileAssert.AreEqual("Accents.po", OutputFileName);
        }

        [Test]
        public void ShouldApplyMultipleTransformations()
        {
            using (var inputStream = new FileStream(Test1FileName, FileMode.Open, FileAccess.Read))
            using (var outputStream = new FileStream(OutputFileName, FileMode.Create, FileAccess.Write))
            {
                var processor = new POProcessor("qps-Ploc");
                processor.TransformString += (s, e) => { e.Value = e.Value + "1"; };
                processor.TransformString += (s, e) => { e.Value = Brackets.Instance.Transform(e.Value); };
                processor.TransformString += (s, e) => { e.Value = e.Value + "2"; };
                processor.Transform(inputStream, outputStream);
            }

            FileAssert.AreEqual("Multiple.po", OutputFileName);
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
