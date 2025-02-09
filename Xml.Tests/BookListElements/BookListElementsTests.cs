using System.Xml.Serialization;
using NUnit.Framework;

namespace Xml.Tests.BookListElements;

[TestFixture]
public class BookListElementsTests : XmlTestFixtureBase
{
    private const string SourceFileName = "book-list-elements.xml";
    private const string SchemaFileName = "BookListElements.book-list-elements.xsd";
    private const string NamespacePrefix = "co";

    private string content = null!;
    private string schema = null!;

    [SetUp]
    public void SetUp()
    {
            this.content = ReadContent(SourceFileName);
            this.schema = ReadSchema(SchemaFileName);
        }

    [Test]
    public void DeserializeAndTestContent()
    {
            BookList bookList = Deserialize<BookList>(this.content);

            Assert.That(bookList, Is.Not.Null);
            Assert.That(bookList.Books, Is.Not.Null);
            Assert.That(bookList.Books.Count, Is.EqualTo(3));

            var book = bookList.Books[0];
            Assert.That(book.Title, Is.EqualTo("Pride And Prejudice"));
            Assert.That(book.Price, Is.EqualTo(24.95));
            Assert.That(book.Genre, Is.EqualTo("novel"));
            Assert.That(book.Isbn, Is.EqualTo("1-861001-57-8"));
            Assert.That(book.PublicationDate, Is.EqualTo("1823-01-28"));

            book = bookList.Books[1];
            Assert.That(book.Title, Is.EqualTo("The Handmaid's Tale"));
            Assert.That(book.Price, Is.EqualTo(29.95));
            Assert.That(book.Genre, Is.EqualTo("novel"));
            Assert.That(book.Isbn, Is.EqualTo("1-861002-30-1"));
            Assert.That(book.PublicationDate, Is.EqualTo("1985-01-01"));

            book = bookList.Books[2];
            Assert.That(book.Title, Is.EqualTo("Sense and Sensibility"));
            Assert.That(book.Price, Is.EqualTo(19.95));
            Assert.That(book.Genre, Is.EqualTo("novel"));
            Assert.That(book.Isbn, Is.EqualTo("1-861001-45-3"));
            Assert.That(book.PublicationDate, Is.EqualTo("1811-01-01"));
        }

    [Test]
    public void ValidateSchema()
    {
            ValidateSchema(this.content, this.schema, TargetNamespaces.BooksNamespace);
        }

    [Test]
    public void LoadXmlAndTestElementPrefixes()
    {
        LoadXmlAndTestElementPrefixes(this.content, NamespacePrefix);
        }

    [XmlRoot("books", Namespace = TargetNamespaces.BooksNamespace)]
    public class BookList
    {
        [XmlElement("book")]
        public List<Book> Books { get; } = new();
    }

    public class Book
    {
        [XmlElement("title")]
        public string Title { get; set; } = null!;

        [XmlElement("price")]
        public decimal Price { get; set; }

        [XmlElement("genre")]
        public string Genre { get; set; } = null!;

        [XmlElement("isbn")]
        public string Isbn { get; set; } = null!;

        [XmlElement("publicationDate")]
        public string PublicationDate { get; set; } = null!;
    }
}
