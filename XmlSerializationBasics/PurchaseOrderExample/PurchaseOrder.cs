using System.Xml.Serialization;

namespace XmlSerializationBasics.PurchaseOrderExample;

[XmlRoot("purchase-order", Namespace = "http://www.cpandl.com/purchase-order")]
public class PurchaseOrder
{
    [XmlElement("destination-address", Namespace = "http://www.cpandl.com/address", Order = 4)]
    public Address? ShipTo { get; set; }

    [XmlElement("order-date", Order = 1)]
    public string? OrderDate { get; set; }

    [XmlElement("delivery-date", Namespace = "http://www.cpandl.com/delivery-date", Order = 2)]
    public DeliveryDate? DeliveryDate { get; set; }

    [XmlArray("items", Namespace = "http://www.cpandl.com/purchase-order-item", Order = 3)]
    [XmlArrayItem("order-item")]
    public OrderedItem[]? OrderedItems { get; set; }

    [XmlIgnore]
    public decimal SubTotal { get; set; }

    [XmlAttribute("ship-cost")]
    public decimal ShipCost { get; set; }

    [XmlAttribute("total-cost")]
    public decimal TotalCost { get; set; }

    public void CalculateSubTotal()
    {
        if (this.OrderedItems is not null)
        {
            decimal subTotal = 0;
            foreach (var item in this.OrderedItems)
            {
                subTotal += item.LineTotal;
            }

            this.SubTotal = subTotal;
        }
    }

    public void CalculateTotalCost()
    {
        this.TotalCost = this.SubTotal + this.ShipCost;
    }
}
