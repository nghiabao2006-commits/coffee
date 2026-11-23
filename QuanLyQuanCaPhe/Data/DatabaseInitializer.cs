using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace QuanLyQuanCaPhe.Data
{
    /// <summary>
    /// Tự động tạo database, bảng và dữ liệu mẫu ở lần chạy đầu tiên.
    /// Nhờ vậy người khác chỉ cần tải source về là có thể chạy ngay.
    /// </summary>
    public static class DatabaseInitializer
    {
        private static readonly object SyncRoot = new object();
        private static bool _isReady;

        public static bool IsReady
        {
            get { return _isReady; }
        }

        public static void EnsureCreated()
        {
            if (_isReady)
            {
                return;
            }

            lock (SyncRoot)
            {
                if (_isReady)
                {
                    return;
                }

                DatabaseHelper.ResolveConnection();
                CreateDatabaseIfNeeded();

                using (var connection = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    connection.Open();

                    string schema = ReadResource("QuanLyQuanCaPhe.Data.Schema.sql");
                    ExecuteScript(connection, null, schema, 120);

                    object seededValue = ExecuteScalar(connection, null,
                        "SELECT COUNT(*) FROM dbo.CaiDat WHERE Khoa = N'DaKhoiTao'");
                    int seeded = Convert.ToInt32(seededValue);

                    if (seeded == 0)
                    {
                        using (var transaction = connection.BeginTransaction())
                        {
                            try
                            {
                                string seed = ReadResource("QuanLyQuanCaPhe.Data.Seed.sql");
                                ExecuteScript(connection, transaction, seed, 180);

                                using (var command = new SqlCommand(
                                    "INSERT INTO dbo.CaiDat (Khoa, GiaTri) VALUES (N'DaKhoiTao', N'1')",
                                    connection, transaction))
                                {
                                    command.ExecuteNonQuery();
                                }

                                transaction.Commit();
                            }
                            catch
                            {
                                try
                                {
                                    transaction.Rollback();
                                }
                                catch
                                {
                                    // Bỏ qua lỗi rollback, ném lại lỗi gốc bên dưới.
                                }
                                throw;
                            }
                        }
                    }
                }

                _isReady = true;
            }
        }

        public static void Reset()
        {
            lock (SyncRoot)
            {
                _isReady = false;
            }
            DatabaseHelper.Reset();
        }

        private static void CreateDatabaseIfNeeded()
        {
            using (var connection = new SqlConnection(DatabaseHelper.MasterConnectionString))
            using (var command = new SqlCommand(
                "IF DB_ID(N'" + DatabaseHelper.DatabaseName + "') IS NULL " +
                "CREATE DATABASE [" + DatabaseHelper.DatabaseName + "]", connection))
            {
                command.CommandTimeout = 60;
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private static string ReadResource(string name)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(name))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException("Không tìm thấy tài nguyên CSDL: " + name);
                }
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private static void ExecuteScript(SqlConnection connection, SqlTransaction transaction, string script, int timeout)
        {
            string[] batches = Regex.Split(script, @"^\s*GO\s*;?\s*$",
                RegexOptions.Multiline | RegexOptions.IgnoreCase);

            foreach (string batch in batches)
            {
                string sql = batch.Trim();
                if (sql.Length == 0)
                {
                    continue;
                }

                using (var command = new SqlCommand(sql, connection, transaction))
                {
                    command.CommandTimeout = timeout;
                    command.ExecuteNonQuery();
                }
            }
        }

        private static object ExecuteScalar(SqlConnection connection, SqlTransaction transaction, string sql)
        {
            using (var command = new SqlCommand(sql, connection, transaction))
            {
                command.CommandTimeout = 60;
                object value = command.ExecuteScalar();
                return value == DBNull.Value ? null : value;
            }
        }
    }
}
