using System;
using System.Data;

namespace QuanLyQuanCaPhe.DAL
{
    /// <summary>Truy vấn ADO.NET phục vụ trang chủ và báo cáo thống kê.</summary>
    public static class ThongKeDAL
    {
        public static decimal DoanhThu(DateTime tu, DateTime den)
        {
            object value = DatabaseHelper.Scalar(
                "SELECT ISNULL(SUM(ThanhTien), 0) FROM dbo.HoaDon " +
                "WHERE TrangThai = N'Đã thanh toán' AND NgayThanhToan >= @Tu AND NgayThanhToan < @Den",
                DatabaseHelper.P("@Tu", tu), DatabaseHelper.P("@Den", den));
            return value == null ? 0m : Convert.ToDecimal(value);
        }

        public static int SoHoaDon(DateTime tu, DateTime den)
        {
            object value = DatabaseHelper.Scalar(
                "SELECT COUNT(*) FROM dbo.HoaDon " +
                "WHERE TrangThai = N'Đã thanh toán' AND NgayThanhToan >= @Tu AND NgayThanhToan < @Den",
                DatabaseHelper.P("@Tu", tu), DatabaseHelper.P("@Den", den));
            return Convert.ToInt32(value);
        }

        public static int SoBanDangSuDung()
        {
            return Convert.ToInt32(DatabaseHelper.Scalar(
                "SELECT COUNT(*) FROM dbo.Ban WHERE TrangThai = N'Đang sử dụng'"));
        }

        public static int TongSoBan()
        {
            return Convert.ToInt32(DatabaseHelper.Scalar("SELECT COUNT(*) FROM dbo.Ban"));
        }

        public static string MonBanChay(DateTime tu, DateTime den)
        {
            object value = DatabaseHelper.Scalar(
                "SELECT TOP 1 m.TenMon FROM dbo.ChiTietHoaDon ct " +
                "INNER JOIN dbo.HoaDon h ON h.MaHD = ct.MaHD " +
                "INNER JOIN dbo.Mon m ON m.MaMon = ct.MaMon " +
                "WHERE h.TrangThai = N'Đã thanh toán' AND h.NgayThanhToan >= @Tu AND h.NgayThanhToan < @Den " +
                "GROUP BY m.TenMon ORDER BY SUM(ct.SoLuong) DESC",
                DatabaseHelper.P("@Tu", tu), DatabaseHelper.P("@Den", den));
            return value == null ? string.Empty : Convert.ToString(value);
        }

        public static DataTable HoaDonGanDay(int soLuong)
        {
            return DatabaseHelper.Query(
                "SELECT TOP " + soLuong + " 'HD' + RIGHT('0000' + CAST(h.MaHD AS VARCHAR(10)), 4) AS MaHoaDon, " +
                "ISNULL(h.NgayThanhToan, h.NgayLap) AS ThoiGian, b.TenBan, nv.TenNV, " +
                "h.ThanhTien, h.TrangThai " +
                "FROM dbo.HoaDon h INNER JOIN dbo.Ban b ON b.MaBan = h.MaBan " +
                "INNER JOIN dbo.NhanVien nv ON nv.MaNV = h.MaNV " +
                "ORDER BY ISNULL(h.NgayThanhToan, h.NgayLap) DESC, h.MaHD DESC");
        }

        public static DataTable TopMon(DateTime tu, DateTime den, int soLuong)
        {
            return DatabaseHelper.Query(
                "SELECT TOP " + soLuong + " m.TenMon, SUM(ct.SoLuong) AS SoLuong, SUM(ct.ThanhTien) AS DoanhThu " +
                "FROM dbo.ChiTietHoaDon ct INNER JOIN dbo.HoaDon h ON h.MaHD = ct.MaHD " +
                "INNER JOIN dbo.Mon m ON m.MaMon = ct.MaMon " +
                "WHERE h.TrangThai = N'Đã thanh toán' AND h.NgayThanhToan >= @Tu AND h.NgayThanhToan < @Den " +
                "GROUP BY m.TenMon ORDER BY SoLuong DESC, DoanhThu DESC",
                DatabaseHelper.P("@Tu", tu), DatabaseHelper.P("@Den", den));
        }

        public static DataRow TongQuan(DateTime tu, DateTime den)
        {
            DataTable table = DatabaseHelper.Query(
                "SELECT COUNT(*) AS SoHD, ISNULL(SUM(h.ThanhTien), 0) AS DoanhThu " +
                "FROM dbo.HoaDon h WHERE h.TrangThai = N'Đã thanh toán' " +
                "AND h.NgayThanhToan >= @Tu AND h.NgayThanhToan < @Den",
                DatabaseHelper.P("@Tu", tu), DatabaseHelper.P("@Den", den));
            return table.Rows[0];
        }

        public static int TongSoLuongMon(DateTime tu, DateTime den)
        {
            object value = DatabaseHelper.Scalar(
                "SELECT ISNULL(SUM(ct.SoLuong), 0) FROM dbo.ChiTietHoaDon ct " +
                "INNER JOIN dbo.HoaDon h ON h.MaHD = ct.MaHD " +
                "WHERE h.TrangThai = N'Đã thanh toán' AND h.NgayThanhToan >= @Tu AND h.NgayThanhToan < @Den",
                DatabaseHelper.P("@Tu", tu), DatabaseHelper.P("@Den", den));
            return value == null ? 0 : Convert.ToInt32(value);
        }

        public static DataTable DoanhThuTheoNgay(DateTime tu, DateTime den)
        {
            return DatabaseHelper.Query(
                "SELECT CONVERT(date, h.NgayThanhToan) AS Ngay, ISNULL(SUM(h.ThanhTien), 0) AS DoanhThu " +
                "FROM dbo.HoaDon h WHERE h.TrangThai = N'Đã thanh toán' " +
                "AND h.NgayThanhToan >= @Tu AND h.NgayThanhToan < @Den " +
                "GROUP BY CONVERT(date, h.NgayThanhToan) ORDER BY Ngay",
                DatabaseHelper.P("@Tu", tu), DatabaseHelper.P("@Den", den));
        }

        public static DataTable HinhThucThanhToan(DateTime tu, DateTime den)
        {
            return DatabaseHelper.Query(
                "SELECT ISNULL(h.PhuongThucThanhToan, N'Khác') AS PhuongThuc, COUNT(*) AS SoHD, " +
                "SUM(h.ThanhTien) AS DoanhThu FROM dbo.HoaDon h " +
                "WHERE h.TrangThai = N'Đã thanh toán' AND h.NgayThanhToan >= @Tu AND h.NgayThanhToan < @Den " +
                "GROUP BY ISNULL(h.PhuongThucThanhToan, N'Khác') ORDER BY DoanhThu DESC",
                DatabaseHelper.P("@Tu", tu), DatabaseHelper.P("@Den", den));
        }
    }
}
