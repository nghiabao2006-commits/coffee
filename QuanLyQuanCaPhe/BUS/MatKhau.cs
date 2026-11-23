using System.Security.Cryptography;
using System.Text;

namespace QuanLyQuanCaPhe.BUS
{
    /// <summary>Băm mật khẩu bằng SHA-256 trước khi lưu hoặc so sánh trong CSDL.</summary>
    public static class MatKhau
    {
        public static string Bam(string password)
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
    }
}
