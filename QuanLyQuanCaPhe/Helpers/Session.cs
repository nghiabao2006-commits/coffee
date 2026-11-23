namespace QuanLyQuanCaPhe.Helpers
{
    /// <summary>Lưu thông tin nhân viên đang đăng nhập.</summary>
    public static class Session
    {
        public static int MaNV { get; set; }
        public static string TenNV { get; set; }
        public static string TenDangNhap { get; set; }
        public static string TenVaiTro { get; set; }
        public static int MaVaiTro { get; set; }

        public static bool IsQuanLy
        {
            get { return MaVaiTro == 1; }
        }

        public static void Clear()
        {
            MaNV = 0;
            TenNV = string.Empty;
            TenDangNhap = string.Empty;
            TenVaiTro = string.Empty;
            MaVaiTro = 0;
        }
    }
}
