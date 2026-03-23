namespace DSAapp.Core.Models;

// Model for the SampleDataService. Replace with your own model.
public class SampleCompany
{
    public required string CompanyID { get; set; }
    public required string CompanyName { get; set; }
    public required string ContactName { get; set; }
    public required string ContactTitle { get; set; }
    public required string Address { get; set; }
    public required string City { get; set; }
    public required string PostalCode { get; set; }
    public required string Country { get; set; }
    public required string Phone { get; set; }
    public required string Fax { get; set; }

    public ICollection<SampleOrder> Orders { get; set; } = new List<SampleOrder>();
}
