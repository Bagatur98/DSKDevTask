using Dapper;
using DSKDevTask.Models;
using System.Data;

namespace DSKDevTask
{
    public class CreditRepository
    {
        public readonly IDbConnection _dbConnection;

        public CreditRepository(DBContext context)
        {
            _dbConnection = context._dbConnection;
        }
        public CreditRepository(IDbConnection connection)
        {
            _dbConnection = connection;
        }

        public async Task<IEnumerable<Credit>> GetAllCredits()
        {

            return await _dbConnection.QueryAsync<Credit>(@"
                Select *
                from Credits");
        }

        public async Task<IEnumerable<Invoice>> GetAllInvoicese()
        {
            return await _dbConnection.QueryAsync<Invoice>(@"
                Select *
                from Invoices");
        }

        public async Task<IEnumerable<Credit>> GetCreditsWithInvoices()
        {
            Dictionary<int, Credit> uniqueCredits = new Dictionary<int, Credit>();

            string query = @"
            Select c.Id as CreditId, c.ClientName, c.CreditAmount, c.CreditDate, c.Status,
                   i.Id as InvoiceId, i.Amount, i.CreditId
            from Credits as c 
            left join Invoices as i on i.CreditId = c.Id ";

            await _dbConnection.QueryAsync<Credit, Invoice, Credit>(
                query,
                (credit, invoice) =>
                {
                    if(uniqueCredits.TryGetValue(credit.CreditId, out Credit existingCredit))
                    {
                        credit = existingCredit;
                    }
                    else
                    {
                        uniqueCredits.Add(credit.CreditId, credit);
                        credit.Invoices = new List<Invoice>();

                    }
                    if(invoice != null)
                        credit.Invoices.Add(invoice);
                    return credit;
                },
                splitOn: "InvoiceId");

            return uniqueCredits.Values.ToList();

        }


        public async Task<PaidPercentViewModel> GetPaidAndAwaitingTotals()
        {
            double totalPaid = await _dbConnection.ExecuteScalarAsync<double>("Select coalesce(sum(CreditAmount),0) from Credits where Status = " + (int)CreditStatus.Paid);
            double totalAwaiting = await _dbConnection.ExecuteScalarAsync<double>("Select sum(CreditAmount) from Credits where Status = " + (int)CreditStatus.AwaitingPayment);

            PaidPercentViewModel result = new PaidPercentViewModel()
            {
                TotalPaid = totalPaid,
                TotalAwaiting = totalAwaiting,
                PaidPercentage = 100 * totalPaid / (totalAwaiting + totalPaid),
                AwaitingPercentage = 100 * totalAwaiting / (totalAwaiting + totalPaid),
            };
            return result;
        }
        public async Task AddCredit(Credit credit)
        {
            await _dbConnection.ExecuteAsync(@"insert into Credits 
                            (ClientName, CreditDate, CreditAmount, Status) 
                        values 
                            (@ClientName, @CreditDate, @CreditAmount, @Status)", credit);
        }

        public async Task AddInvoice(Invoice invoice)
        {
            await _dbConnection.ExecuteAsync(@"insert into Invoices 
                            (CreditId, Amount) 
                        values 
                            (@CreditId, @Amount)", invoice);
        }
    }
}
