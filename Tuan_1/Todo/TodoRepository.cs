using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace Todo
{
    public class TodoRepository
    {
        private readonly string _connectionString;

        public TodoRepository()
        {
            // 1. Đọc chuỗi kết nối từ biến môi trường của launchSettings.json
            // 2. Nếu không tìm thấy, sẽ tự động dùng chuỗi kết nối chuẩn bằng quyền Windows tới thực thể MSSQLSERVER02
            _connectionString = Environment.GetEnvironmentVariable("TODO_DB_CONN")
                ?? "Server=LAPTOP-M5K0D1B8\\MSSQLSERVER02;Database=todo_db;Integrated Security=True;TrustServerCertificate=True;";

            EnsureDatabaseExists();
            EnsureTableExists();
        }

        private void EnsureDatabaseExists()
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(_connectionString);
                var dbName = string.IsNullOrWhiteSpace(builder.InitialCatalog) ? "todo_db" : builder.InitialCatalog;

                // Tạo chuỗi kết nối trung gian kết nối tới database hệ thống 'master'
                // Giữ nguyên cơ chế đăng nhập (bằng Windows hoặc sa tùy thuộc vào cấu hình gốc)
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
                Console.WriteLine($"[TodoRepository] EnsureDatabaseExists error: {ex.Message}");
                throw;
            }
        }

        private void EnsureTableExists()
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText =
                    @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'todos')
                      BEGIN
                        CREATE TABLE todos (
                          id INT IDENTITY(1,1) PRIMARY KEY,
                          title NVARCHAR(MAX) NOT NULL,
                          is_success BIT NOT NULL
                        );
                      END";
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TodoRepository] EnsureTableExists error: {ex.Message}");
                throw;
            }
        }

        public List<Todo> GetAll()
        {
            var result = new List<Todo>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT id, title, is_success FROM todos ORDER BY id;";
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new Todo
                    {
                        Id = reader.GetInt32(0),
                        Title = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                        IsSuccess = reader.GetBoolean(2)
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TodoRepository] GetAll error: {ex.Message}");
                throw;
            }
            return result;
        }

        public Todo Add(string title)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO todos (title, is_success) OUTPUT INSERTED.id VALUES (@title, 0);";
                cmd.Parameters.AddWithValue("@title", (object)(title ?? string.Empty));
                var idObj = cmd.ExecuteScalar();
                var id = Convert.ToInt32(idObj);
                return new Todo { Id = id, Title = title, IsSuccess = false };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TodoRepository] Add error: {ex.Message}");
                throw;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM todos WHERE id = @id;";
                cmd.Parameters.AddWithValue("@id", id);
                var rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TodoRepository] Delete error: {ex.Message}");
                throw;
            }
        }

        public bool Update(int id, string title)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE todos SET title = @title WHERE id = @id;";
                cmd.Parameters.AddWithValue("@title", (object)(title ?? string.Empty));
                cmd.Parameters.AddWithValue("@id", id);
                var rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TodoRepository] Update error: {ex.Message}");
                throw;
            }
        }

        public bool Toggle(int id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE todos SET is_success = CASE WHEN is_success = 1 THEN 0 ELSE 1 END WHERE id = @id;";
                cmd.Parameters.AddWithValue("@id", id);
                var rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TodoRepository] Toggle error: {ex.Message}");
                throw;
            }
        }
    }
}