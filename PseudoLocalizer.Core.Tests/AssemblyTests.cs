using NUnit.Framework;

namespace PseudoLocalizer.Core.Tests;

public static class AssemblyTests
{
    [Test]
    public static void Library_Is_Strong_Named()
    {
        // Arrange
        var assembly = typeof(IProcessor).Assembly;

        // Act
        var name = assembly.GetName();
        var actual = name.GetPublicKeyToken();

        // Assert
        Assert.That(actual, Is.Not.Null);
        Assert.That(actual, Is.Not.Empty);
        Assert.That(Convert.ToHexString(actual).ToLowerInvariant(), Is.EqualTo("0846a21b34b42a7c"));
    }
}
