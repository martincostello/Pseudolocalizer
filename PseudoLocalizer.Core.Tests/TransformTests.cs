namespace PseudoLocalizer.Core.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class TransformTests
    {
        [Test]
        public void TestExtraLength()
        {
            Assert.That(ExtraLength.Instance.Transform(string.Empty), Is.EqualTo(string.Empty), "No extra length added to the empty string.");

            var singleWord = "hello";
            var transformed = ExtraLength.Instance.Transform(singleWord);
            Assert.That(transformed.Length, Is.GreaterThan(singleWord.Length));
            Assert.That(transformed.Split(' ').Length, Is.EqualTo(singleWord.Split(' ').Length), "The number of words stays the same.");

            var sentence = "The quick brown fox bla bla bla.";
            transformed = ExtraLength.Instance.Transform(sentence);
            Assert.That(transformed.Length, Is.GreaterThan(sentence.Length));
            Assert.That(transformed.Split(' ').Length, Is.EqualTo(sentence.Split(' ').Length), "The number of words stays the same.");
        }

        [Test]
        public void TestBrackets()
        {
            Assert.That(Brackets.Instance.Transform(string.Empty), Is.EqualTo("[]"));
            Assert.That(Brackets.Instance.Transform("hello"), Is.EqualTo("[hello]"));
            Assert.That(Brackets.Instance.Transform("The quick brown fox bla bla bla."), Is.EqualTo("[The quick brown fox bla bla bla.]"));
        }

        [Test]
        public void TestMirror()
        {
            Assert.That(Mirror.Instance.Transform(string.Empty), Is.EqualTo(string.Empty));
            Assert.That(Mirror.Instance.Transform("hello, world!"), Is.EqualTo("!dlrow ,olleh"));
        }

        [Test]
        public void TestUnderscores()
        {
            Assert.That(Underscores.Instance.Transform(string.Empty), Is.EqualTo(string.Empty));
            var message = "hello, world!";
            Assert.That(Underscores.Instance.Transform(message), Is.EqualTo(new string('_', message.Length)));
        }

        [Test]
        public void TestAccents()
        {
            Assert.That(Accents.Instance.Transform(string.Empty), Is.EqualTo(string.Empty));
            var message = "hello, world!";
            Assert.That(Accents.Instance.Transform(message), Is.EqualTo("ĥéļļö، ŵöŕļð¡"));
        }

        [Test]
        [TestCase("{0}", "{0}")]
        [TestCase("{10}", "{10}")]
        [TestCase("Hello, {0}", "{0}")]
        [TestCase("Something {100:0abc} something", "{100:0abc}")]
        [TestCase("{0} is a person and so is {1}.", "{0}")]
        [TestCase("{0} is a person and so is {1}.", "{1}")]
        [TestCase("Welcome to the world of tomorrow, {0}; it's the year {1:yyyy}!", "{0}")]
        [TestCase("Welcome to the world of tomorrow, {0}; it's the year {1:yyyy}!", "{1:yyyy}")]
        public void TestAccentsDoNotBreakFormatStrings(string input, string expected)
        {
            string actual = Accents.Instance.Transform(input);
            Assert.That(actual.Contains(expected));
        }
    }
}
