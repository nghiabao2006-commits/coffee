namespace QuanLyQuanCaPhe.DTO
{
    /// <summary>Thông tin tài khoản đăng nhập (kết quả kiểm tra đăng nhập).</summary>
    public class TaiKhoanDTO
    {
        public int MaNV { get; set; }
        public string TenNV { get; set; }
        public string TenDangNhap { get; set; }
        public int MaVaiTro { get; set; }
        public string TenVaiTro { get; set; }
    }
}
