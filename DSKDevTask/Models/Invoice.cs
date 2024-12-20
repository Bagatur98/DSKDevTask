namespace DSKDevTask.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public double Amount { get; set; }
        public int CreditId { get; set; }
    }
}
