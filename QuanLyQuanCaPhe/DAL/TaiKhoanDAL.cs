using System;
using System.Data;
using QuanLyQuanCaPhe.DTO;

namespace QuanLyQuanCaPhe.DAL
{
    /// <summary>Truy vấn ADO.NET cho việc đăng nhập.</summary>
    public static class TaiKhoanDAL
    {
        public static TaiKhoanDTO KiemTraDangNhap(string tenDangNhap, string matKhauHash)
        {
            DataTable table = DatabaseHelper.Query(
                "SELECT nv.MaNV, nv.TenNV, nv.TenDangNhap, nv.MaVaiTro, vt.TenVaiTro " +
                "FROM dbo.NhanVien nv INNER JOIN dbo.VaiTro vt ON vt.MaVaiTro = nv.MaVaiTro " +
                "WHERE nv.TenDangNhap = @TenDangNhap AND nv.MatKhau = @MatKhau AND nv.TrangThai = N'Đang làm'",
                DatabaseHelper.P("@TenDangNhap", tenDangNhap),
                DatabaseHelper.P("@MatKhau", matKhauHash));

            if (table.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = table.Rows[0];
            return new TaiKhoanDTO
            {
                MaNV = Convert.ToInt32(row["MaNV"]),
                TenNV = Convert.ToString(row["TenNV"]),
                TenDangNhap = Convert.ToString(row["TenDangNhap"]),
                MaVaiTro = Convert.ToInt32(row["MaVaiTro"]),
                TenVaiTro = Convert.ToString(row["TenVaiTro"])
            };
        }
    }
}
