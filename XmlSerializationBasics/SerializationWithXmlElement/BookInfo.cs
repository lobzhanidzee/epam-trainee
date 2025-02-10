using System.Xml.Serialization;

namespace XmlSerializationBasics.SerializationWithXmlElement;

[XmlRoot(ElementName = "book")]
public class BookInfo
{
    [XmlElement(ElementName = "book-title")]
    public string? Title { get; set; }

    [XmlElement(ElementName = "book-price")]
    public decimal Price { get; set; }

    [XmlElement(ElementName = "book-genre")]
    public string? Genre { get; set; }

    [XmlElement(ElementName = "book-isbn")]
    public string? Isbn { get; set; }

    [XmlElement(ElementName = "book-publication-date")]
    public string? PublicationDate { get; set; }
}
