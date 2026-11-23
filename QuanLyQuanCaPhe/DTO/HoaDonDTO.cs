using System;

namespace QuanLyQuanCaPhe.DTO
{
    /// <summary>Thông tin một hóa đơn (dùng cho xem chi tiết và in hóa đơn).</summary>
    public class HoaDonDTO
    {
        public int MaHD { get; set; }
        public int MaBan { get; set; }
        public string TenBan { get; set; }
        public string TenKhuVuc { get; set; }
        public int MaNV { get; set; }
        public string TenNV { get; set; }
        public DateTime? NgayLap { get; set; }
        public DateTime? NgayThanhToan { get; set; }
        public decimal TongTien { get; set; }
        public decimal GiamGia { get; set; }
        public decimal ThanhTien { get; set; }
        public string PhuongThucThanhToan { get; set; }
        public string TrangThai { get; set; }
        public string GhiChu { get; set; }

        /// <summary>Ngày hiển thị trên hóa đơn: ngày thanh toán nếu có, ngược lại là ngày lập.</summary>
        public DateTime? NgayHienThi
        {
            get { return NgayThanhToan ?? NgayLap; }
        }

        public string MaHoaDon
        {
            get { return "HD" + MaHD.ToString("D4"); }
        }
    }
}
