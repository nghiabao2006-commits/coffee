using System;

namespace QuanLyQuanCaPhe.DTO
{
    /// <summary>Thông tin một nhân viên của quán.</summary>
    public class NhanVienDTO
    {
        public int MaNV { get; set; }
        public string TenNV { get; set; }
        public string TenDangNhap { get; set; }
        public int MaVaiTro { get; set; }
        public string TenVaiTro { get; set; }
        public string DienThoai { get; set; }
        public string DiaChi { get; set; }
        public DateTime? NgayVaoLam { get; set; }
        public string TrangThai { get; set; }
    }
}
