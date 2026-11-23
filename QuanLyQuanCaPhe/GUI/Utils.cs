using System;
using System.Data;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace QuanLyQuanCaPhe.GUI
{
    /// <summary>Các hàm tiện ích dùng chung cho toàn bộ phần mềm.</summary>
    public static class Utils
    {
        private static readonly CultureInfo ViCulture = CultureInfo.GetCultureInfo("vi-VN");

        public static string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password ?? string.Empty));
                var builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static string Money(decimal value)
        {
            return value.ToString("N0", ViCulture) + " đ";
        }

        public static string Money(decimal? value)
        {
            return value.HasValue ? Money(value.Value) : "0 đ";
        }

        /// <summary>Chuyển chuỗi người dùng nhập (1.250.000, 1250000, 1,250,000) thành số tiền.</summary>
        public static decimal ParseMoney(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return 0m;
            }

            string clean = text.Replace("đ", string.Empty)
                               .Replace("VNĐ", string.Empty)
                               .Replace(" ", string.Empty)
                               .Replace(".", string.Empty)
                               .Replace(",", string.Empty)
                               .Trim();

            decimal value;
            if (decimal.TryParse(clean, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                return value;
            }
            return 0m;
        }

        public static string InvoiceCode(int maHD)
        {
            return "HD" + maHD.ToString("D4");
        }

        public static string DateText(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("dd/MM/yyyy") : string.Empty;
        }

        public static string DateTimeText(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty;
        }

        public static string ShortMoney(decimal value)
        {
            if (value >= 1000000m)
            {
                return (value / 1000000m).ToString("0.#", ViCulture) + " tr";
            }
            if (value >= 1000m)
            {
                return (value / 1000m).ToString("0.#", ViCulture) + " k";
            }
            return value.ToString("N0", ViCulture);
        }

        public static bool SameText(string a, string b)
        {
            return string.Equals((a ?? string.Empty).Trim(), (b ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>Đọc dữ liệu từ DataRow an toàn khi gặp giá trị NULL.</summary>
    public static class Row
    {
        public static string Str(DataRow row, string column)
        {
            if (row == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
            {
                return string.Empty;
            }
            return Convert.ToString(row[column]);
        }

        public static int Int(DataRow row, string column)
        {
            if (row == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
            {
                return 0;
            }
            return Convert.ToInt32(row[column]);
        }

        public static int? NullableInt(DataRow row, string column)
        {
            if (row == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
            {
                return null;
            }
            return Convert.ToInt32(row[column]);
        }

        public static decimal Dec(DataRow row, string column)
        {
            if (row == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
            {
                return 0m;
            }
            return Convert.ToDecimal(row[column]);
        }

        public static DateTime? Date(DataRow row, string column)
        {
            if (row == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
            {
                return null;
            }
            return Convert.ToDateTime(row[column]);
        }

        public static bool Bool(DataRow row, string column)
        {
            if (row == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
            {
                return false;
            }
            return Convert.ToBoolean(row[column]);
        }
    }
}
