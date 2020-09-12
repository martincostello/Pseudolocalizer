namespace PseudoLocalizer.Core.Tests
{
    using System;
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
            Assert.That(actual, Contains.Substring(expected));
        }

        [Test]
        [TestCase("Hi <em>{0}</em>, click <a href=\"{1}\">here</a>.", new[] { "<em>{0}</em>", "<a href=\"{1}\">", "</a>" })]
        [TestCase("Hi <em>{0}</em>, click <a href=\"{1}\">here</a>.", new[] { "<em>{0}</em>", "<a href=\"{1}\">", "</a>" })]
        [TestCase("2 > 0", new[] { "② ≥ ⓪" })]
        [TestCase("0 < 2", new[] { "⓪ ≤ ②" })]
        [TestCase("0 <> 2", new[] { "⓪ ≤≥ ②" })]
        [TestCase("<self/> <br> <a></a> <tag /> <br>", new[] { "<self/>", "<br>", "<a></a>", "<tag />" })]
        public void TestAccentsDoNotBreakHtml(string input, string[] expected)
        {
            string actual = Accents.Instance.Transform(input);

            foreach (string value in expected)
            {
                Assert.That(actual, Contains.Substring(value));
            }
        }

        [Test]
        [TestCase("Hi <em>{0}</em>, click <a href=\"{1}\">here</a>.", new[] { "<em>{0}</em>", "<a href=\"{1}\">", "</a>" })]
        [TestCase("Hi <em>{0}</em>, click <a href=\"https://www.google.com\">here</a>.", new[] { "<em>{0}</em>", "<a href=\"https://www.google.com\">", "</a>" })]
        [TestCase("<em>{0}</em>", new[] { "<em>{0}</em>" })]
        [TestCase("<em> {0} </em>", new[] { "<em> {0} </em>" })]
        [TestCase("<a href=\"{0}\">here</a>", new[] { "<a href=\"{0}\">" })]
        public void TestExtraLengthDoesNotBreakHtml(string input, string[] expected)
        {
            string actual = ExtraLength.Instance.Transform(input);

            foreach (string value in expected)
            {
                Assert.That(actual, Contains.Substring(value));
            }
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
        public void TestMirrorDoesNotBreakFormatStrings(string input, string expected)
        {
            string actual = Mirror.Instance.Transform(input);
            Assert.That(actual, Contains.Substring(expected));
        }

        [Test]
        public void TestPipeline()
        {
            var pipeline = new Pipeline(ExtraLength.Instance, Accents.Instance, Brackets.Instance);

            Assert.That(pipeline.Transform(string.Empty), Is.EqualTo("[]"));
            Assert.That(pipeline.Transform("hello, world!"), Is.EqualTo("[ĥéļļö،ẋẋ ŵöŕļð¡ẋẋ]"));
        }

        [Test]
        public void TestPipelineCustomLengthen()
        {
            var pipeline = new Pipeline(new ExtraLength() { LengthenCharacter = '.' }, Accents.Instance, Brackets.Instance);

            Assert.That(pipeline.Transform(string.Empty), Is.EqualTo("[]"));
            Assert.That(pipeline.Transform("hello, world!"), Is.EqualTo("[ĥéļļö،·· ŵöŕļð¡··]"));
            Assert.That(pipeline.Transform("Please confirm this e-mail address by clicking the following link:"), Is.EqualTo("[Þļéåšé·· çöñƒîŕɱ··· ţĥîš·· é‐ɱåîļ·· åððŕéšš··· ƀý· çļîçķîñĝ··· ţĥé· ƒöļļöŵîñĝ··· ļîñķ∶··]"));
        }

        [Test]
        public void TestPipelineNoTransforms()
        {
            var pipeline = new Pipeline(Array.Empty<ITransformer>());
            Assert.That(pipeline.Transform(string.Empty), Is.EqualTo(string.Empty));
        }

        [Test]
        public void ShouldIgnorePlaceholdersWhenApplyingUnderscores()
        {
            Assert.That(Underscores.Instance.Transform("{0}hello, world"), Is.EqualTo("{0}____________"));
            Assert.That(Underscores.Instance.Transform("hello, {1} world"), Is.EqualTo("_______{1}______"));
            Assert.That(Underscores.Instance.Transform("hello, world{99}"), Is.EqualTo("____________{99}"));
            Assert.That(Underscores.Instance.Transform("hello, world{0"), Is.EqualTo("______________"));
        }

        [Test]
        public void ShouldIgnoreHtmlWhenApplyingUnderscores()
        {
            Assert.That(Underscores.Instance.Transform("This <em>is</em> Sparta"), Is.EqualTo("_____<em>__</em>_______"));
            Assert.That(Underscores.Instance.Transform("Some text <div/> more text"), Is.EqualTo("__________<div/>__________"));
            Assert.That(Underscores.Instance.Transform("Some text <> more text"), Is.EqualTo("______________________"));
        }
    }
}
