using DSKDevTask;
using DSKDevTask.Models;
using Microsoft.Data.Sqlite;
using System.Data.Common;
using System.Security.Cryptography.X509Certificates;

namespace DSKTest
{
    public class RepoTests
    {

        [Fact]
        public async Task AddCreditTest()
        {
            using (var connection = ConnectionHelper.GetConnection())
            {
                var repo = new CreditRepository(connection);

                Credit credit = new Credit()
                {
                    ClientName = "A",
                    CreditDate = DateTime.UtcNow.AddDays(-7),
                    CreditAmount = 1234.56,
                    Status = CreditStatus.AwaitingPayment
                };

                await repo.AddCredit(credit);

                var credits = await repo.GetAllCredits();
                Assert.NotNull(credits);
                Assert.Single(credits);
                Assert.Equal("A", credits.First().ClientName);
            }
        }

        [Fact]
        public async Task AddInvoiceTest()
        {
            using (var connection = ConnectionHelper.GetConnection())
            {
                var repo = new CreditRepository(connection);

                Credit credit = new Credit()
                {
                    ClientName = "A",
                    CreditDate = DateTime.UtcNow.AddDays(-7),
                    CreditAmount = 1234.56,
                    Status = CreditStatus.AwaitingPayment
                };

                await repo.AddCredit(credit);

                Invoice invoice = new Invoice()
                {
                    CreditId = 1,
                    Amount = 1000,
                };

                await repo.AddInvoice(invoice);

                var invoices = await repo.GetAllInvoicese();
                Assert.Single(invoices);
                Assert.Equal(1000, invoices.First().Amount);
            }
        }

        [Fact]
        public async Task GetAllCredditsTest()
        {
            using (var connection = ConnectionHelper.GetConnection())
            {
                var repo = new CreditRepository(connection);

                List<Credit> creditList = new();
                List<Invoice> invoiceList = new();
                creditList.Add(new Credit()
                {
                    ClientName = "A",
                    CreditDate = DateTime.UtcNow.AddDays(-7),
                    CreditAmount = 1234.56,
                    Status = CreditStatus.AwaitingPayment
                });
                creditList.Add(new Credit()
                {
                    ClientName = "B",
                    CreditDate = DateTime.UtcNow.AddDays(-6),
                    CreditAmount = 10000000.00,
                    Status = CreditStatus.Paid,
                });
                creditList.Add(new Credit()
                {
                    ClientName = "C",
                    CreditDate = DateTime.UtcNow.AddDays(-5),
                    CreditAmount = 30500,
                    Status = CreditStatus.Created,
                });
                


                invoiceList.Add(new Invoice()
                {
                    CreditId = 1,
                    Amount = 1000,
                });
                invoiceList.Add(new Invoice()
                {
                    CreditId = 2,
                    Amount = 2000,
                });
                invoiceList.Add(new Invoice()
                {
                    CreditId = 1,
                    Amount = 3000,
                });


                foreach (var credit in creditList)
                {
                    await repo.AddCredit(credit);
                }

                foreach (var invoice in invoiceList)
                {
                    await repo.AddInvoice(invoice);
                }

                var response = await repo.GetCreditsWithInvoices();
                Assert.Equal(3, response.Count());
                Assert.Equal(2, response.Single(s => s.ClientName.Equals("A")).Invoices.Count);
                Assert.Empty(response.Single(s => s.ClientName.Equals("C")).Invoices);
            }
        }

        [Fact]
        public async Task CalcualtionsTest()
        {
            using (var connection = ConnectionHelper.GetConnection())
            {
                var repo = new CreditRepository(connection);

                List<Credit> creditList = new();
                creditList.Add(new Credit()
                {
                    ClientName = "A",
                    CreditDate = DateTime.UtcNow.AddDays(-7),
                    CreditAmount = 400,
                    Status = CreditStatus.AwaitingPayment
                });
                creditList.Add(new Credit()
                {
                    ClientName = "B",
                    CreditDate = DateTime.UtcNow.AddDays(-6),
                    CreditAmount = 600,
                    Status = CreditStatus.Paid,
                }); 
                creditList.Add(new Credit()
                {
                    ClientName = "C",
                    CreditDate = DateTime.UtcNow.AddDays(-5),
                    CreditAmount = 800,
                    Status = CreditStatus.AwaitingPayment
                });
                creditList.Add(new Credit()
                {
                    ClientName = "D",
                    CreditDate = DateTime.UtcNow.AddDays(-4),
                    CreditAmount = 1200,
                    Status = CreditStatus.Paid,
                });
                creditList.Add(new Credit()
                {
                    ClientName = "E",
                    CreditDate = DateTime.UtcNow.AddDays(-4),
                    CreditAmount = 1,
                    Status = CreditStatus.Created,
                });

                foreach (var credit in creditList)
                {
                    await repo.AddCredit(credit);
                }

                var totals = await repo.GetPaidAndAwaitingTotals();

                Assert.Equal(1200, totals.TotalAwaiting);
                Assert.Equal(1800, totals.TotalPaid);
                Assert.Equal(40, totals.AwaitingPercentage);
                Assert.Equal(60, totals.PaidPercentage);
            }
        }
    }
}