namespace QuanLyQuanCaPhe.DTO
{
    /// <summary>Một dòng món trong hóa đơn.</summary>
    public class ChiTietHoaDonDTO
    {
        public int MaCT { get; set; }
        public int MaHD { get; set; }
        public int MaMon { get; set; }
        public string TenMon { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
    }
}
