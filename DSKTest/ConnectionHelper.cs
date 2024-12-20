using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKTest
{
    public static class ConnectionHelper
    {
        public static IDbConnection GetConnection()
        {
            var connection = new SqliteConnection("Data Source = :memory:;foreign keys = true;");
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                CREATE TABLE Credits (
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    ClientName TEXT NOT NULL,
                    CreditAmount REAL NOT NULL,
                    CreditDate TEXT NOT NULL,
                    Status INTEGER NOT NULL);
                CREATE TABLE Invoices (
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    CreditId INTEGER,
                    Amount REAL NOT NULL,
                    FOREIGN KEY(CreditId) References Credits(Id) ON DELETE Cascade);";
                command.ExecuteNonQuery();
            }
            return connection;
        }
    }
}
