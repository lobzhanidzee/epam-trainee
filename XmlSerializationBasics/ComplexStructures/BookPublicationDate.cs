using System.Xml.Serialization;

namespace XmlSerializationBasics.ComplexStructures;

public class BookPublicationDate
{
    [XmlElement("publication-day")]
    public int Day { get; set; }

    [XmlElement("publication-month")]
    public int Month { get; set; }

    [XmlElement("publication-year")]
    public int Year { get; set; }

    [XmlAttribute("first-publication")]
    public bool FirstPublication { get; set; }
}
