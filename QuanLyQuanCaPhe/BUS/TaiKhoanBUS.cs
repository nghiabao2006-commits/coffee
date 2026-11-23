using System;
using QuanLyQuanCaPhe.DAL;
using QuanLyQuanCaPhe.DTO;

namespace QuanLyQuanCaPhe.BUS
{
    /// <summary>Nghiệp vụ đăng nhập: kiểm tra dữ liệu vào và so khớp mật khẩu đã băm.</summary>
    public static class TaiKhoanBUS
    {
        public static TaiKhoanDTO DangNhap(string tenDangNhap, string matKhau)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap) || string.IsNullOrEmpty(matKhau))
            {
                return null;
            }
            return TaiKhoanDAL.KiemTraDangNhap(tenDangNhap.Trim(), MatKhau.Bam(matKhau));
        }
    }
}
