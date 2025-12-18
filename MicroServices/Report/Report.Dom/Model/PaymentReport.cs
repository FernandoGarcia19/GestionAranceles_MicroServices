namespace Report.Dom.Model;

public class PaymentReport
{
    public int PaymentId { get; set; }
    public string EstablishmentName { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; }
    public int ReceiptNumber { get; set; }
    public decimal AmountPaid { get; set; }
    public List<PaymentReportItem> Items { get; set; } = new();
    public string? GeneratedBy { get; set; }
    public DateTime GeneratedDate { get; set; } = DateTime.Now;
}

public class PaymentReportItem
{
    public string CategoryName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal => Quantity * UnitPrice;
}
