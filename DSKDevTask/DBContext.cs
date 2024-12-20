using Dapper;
using DSKDevTask.Models;
using Microsoft.Data.Sqlite;
using System.Data;

namespace DSKDevTask
{
    public class DBContext
    {

        private readonly SqliteConnection _connection;
        public IDbConnection _dbConnection => _connection;

        public DBContext()
        {
            _connection = new SqliteConnection("Data Source = :memory:;foreign keys = true;");
            _connection.Open();
            CreateDB();
            PopulateDB();
        }

        public  void CreateDB()
        {
            string creditsCommand = @"CREATE TABLE Credits (
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    ClientName TEXT NOT NULL,
                    CreditAmount REAL NOT NULL,
                    CreditDate TEXT NOT NULL,
                    Status INTEGER NOT NULL
                )";

            string invoiceCommand = @"CREATE TABLE Invoices (
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    CreditId INTEGER,
                    Amount REAL NOT NULL,
                    FOREIGN KEY(CreditId) References Credits(Id) ON DELETE Cascade
                )";
            _connection.Execute(creditsCommand);
            _connection.Execute(invoiceCommand);
        }

        private void PopulateDB()
        {
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
                ClientName= "C",
                CreditDate= DateTime.UtcNow.AddDays(-5),
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


            string creditCommand = "insert into Credits (ClientName, CreditDate, CreditAmount, Status) values (@ClientName, @CreditDate, @CreditAmount, @Status)";
            foreach (var credit in creditList)
            {
                _connection.Execute(creditCommand, credit);
            }

            string invoiceCommand = "insert into Invoices (CreditId, Amount) values (@CreditId, @Amount)";
            foreach(var invoice in invoiceList)
            {
                _connection.Execute(invoiceCommand, invoice);
            }
        }
    }
}
