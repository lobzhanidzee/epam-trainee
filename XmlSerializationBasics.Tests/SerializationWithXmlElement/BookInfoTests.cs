using NUnit.Framework;
using Org.XmlUnit.Diff;
using XmlSerializationBasics.SerializationWithXmlElement;

namespace XmlSerializationBasics.Tests.SerializationWithXmlElement;

[TestFixture]
public class BookInfoTests : SerializationTestFixtureBase
{
    private const string SampleFileName = "SerializationWithXmlElement.serialization-with-xml-element.xml";

    [Test]
    public void SerializeAndCompareWithSample()
    {
        // Arrange
        var book = new BookInfo();

        Type bookInfoType = typeof(BookInfo);
        bookInfoType.GetProperty("Title")?.SetValue(book, "Pride And Prejudice");
        bookInfoType.GetProperty("Price")?.SetValue(book, 24.95m);
        bookInfoType.GetProperty("Genre")?.SetValue(book, "novel");
        bookInfoType.GetProperty("Isbn")?.SetValue(book, "1-861001-57-8");
        bookInfoType.GetProperty("PublicationDate")?.SetValue(book, "1823-01-28");

        // Act
        Diff diff = this.SerializeAndCompareWithSample(book, SampleFileName);

        // Assert
        Assert.That(diff.HasDifferences(), Is.False, diff.ToString());
    }
}
