using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace QuanLyQuanCaPhe.Data
{
    /// <summary>
    /// Quản lý kết nối SQL Server. Phần mềm ưu tiên LocalDB nhưng vẫn tự dò
    /// các instance phổ biến khác để source tải từ GitHub có thể chạy trên nhiều máy.
    /// </summary>
    public static class DatabaseHelper
    {
        public const string DatabaseName = "QuanLyQuanCaPheT1";

        private static readonly object SyncRoot = new object();
        private static string _connectionString;
        private static string _server;

        public static string Server
        {
            get
            {
                EnsureResolved();
                return _server;
            }
        }

        public static string ConnectionString
        {
            get
            {
                EnsureResolved();
                return _connectionString;
            }
        }

        public static string MasterConnectionString
        {
            get { return BuildConnectionString(Server, "master", 10); }
        }

        public static string OverrideFilePath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "QuanLyQuanCaPheT1", "server.txt");
            }
        }

        private static void EnsureResolved()
        {
            if (!string.IsNullOrEmpty(_connectionString))
            {
                return;
            }

            lock (SyncRoot)
            {
                if (!string.IsNullOrEmpty(_connectionString))
                {
                    return;
                }
                ResolveConnection();
            }
        }

        /// <summary>Thử kết nối lần lượt các server ứng viên và ghi nhớ server thành công.</summary>
        public static void ResolveConnection()
        {
            var errors = new List<string>();
            foreach (string server in GetCandidateServers())
            {
                int timeout = server.IndexOf("localdb", StringComparison.OrdinalIgnoreCase) >= 0 ? 10 : 4;
                try
                {
                    using (var connection = new SqlConnection(BuildConnectionString(server, "master", timeout)))
                    {
                        connection.Open();
                    }

                    _server = server;
                    _connectionString = BuildConnectionString(server, DatabaseName, 15);
                    return;
                }
                catch (Exception ex)
                {
                    errors.Add("- " + server + ": " + ex.Message);
                }
            }

            _connectionString = null;
            _server = null;
            throw new DatabaseConnectionException(
                "Không thể kết nối đến SQL Server. Hãy kiểm tra SQL Server LocalDB đã được cài đặt chưa, " +
                "hoặc bấm \"Cấu hình kết nối\" để nhập tên máy chủ SQL Server trên máy bạn.",
                errors);
        }

        public static void Reset()
        {
            lock (SyncRoot)
            {
                _connectionString = null;
                _server = null;
            }
        }

        public static bool TestServer(string server, out string message)
        {
            try
            {
                using (var connection = new SqlConnection(BuildConnectionString(server, "master", 8)))
                {
                    connection.Open();
                }
                message = "Kết nối thành công đến " + server + ".";
                return true;
            }
            catch (Exception ex)
            {
                message = "Không kết nối được: " + ex.Message;
                return false;
            }
        }

        public static void SaveServerOverride(string server)
        {
            string path = OverrideFilePath;
            string folder = Path.GetDirectoryName(path);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            File.WriteAllText(path, (server ?? string.Empty).Trim());
            Reset();
        }

        public static void ClearServerOverride()
        {
            if (File.Exists(OverrideFilePath))
            {
                File.Delete(OverrideFilePath);
            }
            Reset();
        }

        public static string BuildConnectionString(string server, string database, int timeout)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = server,
                InitialCatalog = database,
                IntegratedSecurity = true,
                MultipleActiveResultSets = true,
                ConnectTimeout = timeout,
                ApplicationName = "QuanLyQuanCaPheT1"
            };
            return builder.ConnectionString;
        }

        private static IEnumerable<string> GetCandidateServers()
        {
            var servers = new List<string>();

            try
            {
                if (File.Exists(OverrideFilePath))
                {
                    string saved = File.ReadAllText(OverrideFilePath).Trim();
                    if (saved.Length > 0)
                    {
                        servers.Add(saved);
                    }
                }
            }
            catch
            {
                // Nếu file cấu hình bị lỗi thì bỏ qua và dò tự động.
            }

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["CoffeeDb"];
            if (settings != null && !string.IsNullOrWhiteSpace(settings.ConnectionString))
            {
                try
                {
                    var builder = new SqlConnectionStringBuilder(settings.ConnectionString);
                    if (!string.IsNullOrWhiteSpace(builder.DataSource))
                    {
                        servers.Add(builder.DataSource);
                    }
                }
                catch
                {
                    // Chuỗi cấu hình sai thì bỏ qua.
                }
            }

            servers.Add(@"(localdb)\MSSQLLocalDB");
            servers.Add(@".\SQLEXPRESS");
            servers.Add(@"(local)\SQLExpress");
            servers.Add(".");
            servers.Add("(local)");

            var result = new List<string>();
            foreach (string server in servers)
            {
                bool exists = false;
                foreach (string item in result)
                {
                    if (string.Equals(item, server, StringComparison.OrdinalIgnoreCase))
                    {
                        exists = true;
                        break;
                    }
                }
                if (!exists && !string.IsNullOrWhiteSpace(server))
                {
                    result.Add(server.Trim());
                }
            }
            return result;
        }

        public static DataTable Query(string sql, params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            using (var adapter = new SqlDataAdapter(command))
            {
                command.CommandTimeout = 60;
                AddParameters(command, parameters);
                var table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        public static int Execute(string sql, params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandTimeout = 60;
                AddParameters(command, parameters);
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public static object Scalar(string sql, params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandTimeout = 60;
                AddParameters(command, parameters);
                connection.Open();
                object value = command.ExecuteScalar();
                return value == DBNull.Value ? null : value;
            }
        }

        public static SqlParameter P(string name, object value)
        {
            return new SqlParameter(name, value ?? DBNull.Value);
        }

        private static void AddParameters(SqlCommand command, SqlParameter[] parameters)
        {
            if (parameters == null)
            {
                return;
            }
            foreach (SqlParameter parameter in parameters)
            {
                if (parameter != null)
                {
                    command.Parameters.Add(parameter);
                }
            }
        }
    }

    public class DatabaseConnectionException : Exception
    {
        public List<string> Details { get; private set; }

        public DatabaseConnectionException(string message, List<string> details) : base(message)
        {
            Details = details ?? new List<string>();
        }

        public string GetDetailedMessage()
        {
            if (Details.Count == 0)
            {
                return Message;
            }
            return Message + Environment.NewLine + Environment.NewLine + "Chi tiết:" + Environment.NewLine +
                   string.Join(Environment.NewLine, Details);
        }
    }
}
