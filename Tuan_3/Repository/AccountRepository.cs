
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace HaiChanBank.Repository
{
    public class AccountRepository
    {
        private readonly string _connectionString;

        public AccountRepository()
        {
            _connectionString = Environment.GetEnvironmentVariable("HAICHAN_DB_CONN")
                ?? "Server=localhost\\SQLEXPRESS;Database=HaiChanDB;Integrated Security=True;TrustServerCertificate=True;";
            EnsureDatabaseExists();
            EnsureTablesExist();
        }

        private void EnsureDatabaseExists()
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(_connectionString);
                var dbName = string.IsNullOrWhiteSpace(builder.InitialCatalog) ? "HaiChanDB" : builder.InitialCatalog;

                var masterBuilder = new SqlConnectionStringBuilder(_connectionString)
                {
                    InitialCatalog = "master"
                };

                using var conn = new SqlConnection(masterBuilder.ConnectionString);
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT database_id FROM sys.databases WHERE name = @name;";
                cmd.Parameters.AddWithValue("@name", dbName);
                var exists = cmd.ExecuteScalar() != null;
                if (!exists)
                {
                    using var createCmd = conn.CreateCommand();
                    createCmd.CommandText = $"CREATE DATABASE [{dbName}];";
                    createCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AccountRepository] EnsureDatabaseExists error: {ex.Message}");
                throw;
            }
        }

        private void EnsureTablesExist()
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText =
                    @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Accounts')
                      BEGIN
                        CREATE TABLE Accounts (
                          id NVARCHAR(50) PRIMARY KEY,
                          owner NVARCHAR(200) NOT NULL,
                          balance FLOAT NOT NULL
                        );
                      END

                      IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Transactions')
                      BEGIN
                        CREATE TABLE Transactions (
                          id INT IDENTITY(1,1) PRIMARY KEY,
                          fromId NVARCHAR(50) NULL,
                          toId NVARCHAR(50) NULL,
                          amount FLOAT NOT NULL,
                          type NVARCHAR(20) NOT NULL,
                          createdAt DATETIME2 NOT NULL
                        );
                      END";
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AccountRepository] EnsureTablesExist error: {ex.Message}");
                throw;
            }
        }

        public bool AccountExists(string id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT 1 FROM Accounts WHERE id = @id;";
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteScalar() != null;
        }

        public void CreateAccount(string id, string owner)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "IF NOT EXISTS (SELECT 1 FROM Accounts WHERE id = @id) INSERT INTO Accounts (id, owner, balance) VALUES (@id, @owner, 0);";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@owner", (object)(owner ?? string.Empty));
            cmd.ExecuteNonQuery();
        }

        public double GetBalance(string id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT balance FROM Accounts WHERE id = @id;";
            cmd.Parameters.AddWithValue("@id", id);
            var obj = cmd.ExecuteScalar();
            return obj == null ? 0.0 : Convert.ToDouble(obj);
        }

        public void UpdateBalance(string id, double newBalance)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Accounts SET balance = @balance WHERE id = @id;";
            cmd.Parameters.AddWithValue("@balance", newBalance);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public void AddTransaction(string fromId, string toId, double amount, string type)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Transactions (fromId, toId, amount, type, createdAt) VALUES (@fromId, @toId, @amount, @type, @createdAt);";
            cmd.Parameters.AddWithValue("@fromId", (object?)fromId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@toId", (object?)toId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@type", (object)(type ?? string.Empty));
            cmd.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);
            cmd.ExecuteNonQuery();
        }
    }
}