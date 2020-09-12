namespace PseudoLocalizer.Core.Tests
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class TransformTests
    {
        [Test]
        public void TestExtraLengthEmpty()
        {
            Assert.That(ExtraLength.Instance.Transform(string.Empty), Is.EqualTo(string.Empty), "No extra length added to the empty string.");
        }

        [Test]
        [TestCase("hello")]
        [TestCase("The quick brown fox bla bla bla.")]
        public void TestExtraLength(string input)
        {
            var actual = ExtraLength.Instance.Transform(input);
            Assert.That(actual.Length, Is.GreaterThan(input.Length));
            Assert.That(actual.Split(' ').Length, Is.EqualTo(input.Split(' ').Length), "The number of words stays the same.");
        }

        [Test]
        [TestCase("", "[]")]
        [TestCase("hello", "[hello]")]
        [TestCase("The quick brown fox bla bla bla.", "[The quick brown fox bla bla bla.]")]
        public void TestBrackets(string value, string expected)
        {
            Assert.That(Brackets.Instance.Transform(value), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("", "")]
        [TestCase("hello, world!", "!dlrow ,olleh")]
        [TestCase("{hello, world!", "!dlrow ,olleh}")]
        [TestCase("{}hello, world!", "!dlrow ,olleh{}")]
        [TestCase("(hello, world!", "!dlrow ,olleh)")]
        [TestCase("(hello, world!)", "(!dlrow ,olleh)")]
        [TestCase("[hello, world!", "!dlrow ,olleh]")]
        [TestCase("[hello, world!]", "[!dlrow ,olleh]")]
        public void TestMirror(string value, string expected)
        {
            Assert.That(Mirror.Instance.Transform(value), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("", "")]
        [TestCase("hello, world!", "_____________")]
        public void TestUnderscores(string value, string expected)
        {
            Assert.That(Underscores.Instance.Transform(value), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("", "")]
        [TestCase("hello, world!", "ĥéļļö، ŵöŕļð¡")]
        public void TestAccents(string value, string expected)
        {
            Assert.That(Accents.Instance.Transform(value), Is.EqualTo(expected));
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
        [TestCase("My name is {0}.", ".{0} si eman yM")]
        public void TestMirrorDoesNotBreakFormatStrings(string input, string expected)
        {
            string actual = Mirror.Instance.Transform(input);
            Assert.That(actual, Contains.Substring(expected));
        }

        [Test]
        [TestCase("Hi <em>{0}</em>, click <a href=\"{1}\">here</a>.", new[] { "<em>{0}</em>", "<a href=\"{1}\">", "</a>" })]
        [TestCase("Hi <em>{0}</em>, click <a href=\"https://www.google.com\">here</a>.", new[] { "<em>{0}</em>", "<a href=\"https://www.google.com\">", "</a>" })]
        [TestCase("<em></em>", new[] { "<em></em>" })]
        [TestCase("<em>X</em>", new[] { "<em>X</em>" })]
        [TestCase("<em>ABC</em>", new[] { "<em>CBA</em>" })]
        [TestCase("<em>{0}</em>", new[] { "<em>{0}</em>" })]
        [TestCase("<em> {0} </em>", new[] { "<em> {0} </em>" })]
        [TestCase("<a href=\"{0}\">here</a>", new[] { "<a href=\"{0}\">" })]
        [TestCase("Here is a <br/> sentence.", new[] { ".ecnetnes <br/> a si ereH" })]
        public void TestMirrorDoesNotBreakHtml(string input, string[] expected)
        {
            string actual = Mirror.Instance.Transform(input);

            foreach (string value in expected)
            {
                Assert.That(actual, Contains.Substring(value));
            }
        }

        [Test]
        [TestCase("This should be flipped (except for these parentheses).", ".(sesehtnerap eseht rof tpecxe) deppilf eb dluohs sihT")]
        [TestCase("Section tags [like this] should not be flipped.", ".deppilf eb ton dluohs [siht ekil] sgat noitceS")]
        [TestCase("This should also be preserved {even though it's not a format string}.", ".{gnirts tamrof a ton s'ti hguoht neve} devreserp eb osla dluohs sihT")]
        public void TestMirrorDoesNotBreakBrackets(string input, string expected)
        {
            string actual = Mirror.Instance.Transform(input);
            Assert.That(actual, Contains.Substring(expected));
        }

        [Test]
        [TestCase("", "[]")]
        [TestCase("hello, world!", "[ĥéļļö،ẋẋ ŵöŕļð¡ẋẋ]")]
        public void TestPipeline(string input, string expected)
        {
            var pipeline = new Pipeline(ExtraLength.Instance, Accents.Instance, Brackets.Instance);
            Assert.That(pipeline.Transform(input), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("", "[]")]
        [TestCase("hello, world!", "[ĥéļļö،·· ŵöŕļð¡··]")]
        [TestCase("Please confirm this e-mail address by clicking the following link:", "[Þļéåšé·· çöñƒîŕɱ··· ţĥîš·· é‐ɱåîļ·· åððŕéšš··· ƀý· çļîçķîñĝ··· ţĥé· ƒöļļöŵîñĝ··· ļîñķ∶··]")]
        public void TestPipelineCustomLengthen(string input, string expected)
        {
            var pipeline = new Pipeline(new ExtraLength() { LengthenCharacter = '.' }, Accents.Instance, Brackets.Instance);
            Assert.That(pipeline.Transform(input), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("", "")]
        [TestCase("hello, world!", "hello, world!")]
        public void TestPipelineNoTransforms(string input, string expected)
        {
            var pipeline = new Pipeline(Array.Empty<ITransformer>());
            Assert.That(pipeline.Transform(input), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("{0}hello, world", "{0}____________")]
        [TestCase("hello, {1} world", "_______{1}______")]
        [TestCase("hello, world{99}", "____________{99}")]
        [TestCase("hello, world{0", "______________")]
        [TestCase("The year is {0:yyyy}.", "____________{0:yyyy}_")]
        public void ShouldIgnorePlaceholdersWhenApplyingUnderscores(string input, string expected)
        {
            Assert.That(Underscores.Instance.Transform(input), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("This <em>is</em> Sparta", "_____<em>__</em>_______")]
        [TestCase("Some text <div/> more text", "__________<div/>__________")]
        [TestCase("Some text <> more text", "______________________")]
        [TestCase("Click <a href=\"https://www.google.com\">here</a>", "______<a href=\"https://www.google.com\">____</a>")]
        public void ShouldIgnoreHtmlWhenApplyingUnderscores(string input, string expected)
        {
            Assert.That(Underscores.Instance.Transform(input), Is.EqualTo(expected));
        }
    }
}
