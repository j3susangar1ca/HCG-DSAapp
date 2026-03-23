namespace DSAapp.Core.Models;

// Model for the SampleDataService. Replace with your own model.
public class SampleOrder
{
    public long OrderID
    {
        get; set;
    }

    public DateTime OrderDate
    {
        get; set;
    }

    public DateTime RequiredDate
    {
        get; set;
    }

    public DateTime ShippedDate
    {
        get; set;
    }

    public required string ShipperName { get; set; }
    public required string ShipperPhone { get; set; }
    public double Freight { get; set; }
    public required string Company { get; set; }
    public required string ShipTo { get; set; }
    public double OrderTotal { get; set; }
    public required string Status { get; set; }

    public int SymbolCode
    {
        get; set;
    }

    public required string SymbolName { get; set; }

    public char Symbol => (char)SymbolCode;

    public ICollection<SampleOrderDetail> Details { get; set; } = new List<SampleOrderDetail>();

    public string ShortDescription => $"Order ID: {OrderID}";

    public override string ToString() => $"{Company} {Status}";
}
