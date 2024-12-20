namespace DSKDevTask.Models
{
    public class Credit
    {
        public int CreditId { get; set; }
        public string ClientName { get; set; }
        public double CreditAmount { get; set; }
        public DateTime CreditDate { get; set; }
        public CreditStatus Status { get; set; }
        public List<Invoice> Invoices { get; set; }
    }
}
