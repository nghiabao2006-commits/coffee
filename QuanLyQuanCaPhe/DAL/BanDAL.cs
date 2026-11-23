using System;
using System.Data;

namespace QuanLyQuanCaPhe.DAL
{
    /// <summary>Truy vấn ADO.NET cho bàn.</summary>
    public static class BanDAL
    {
        public static DataTable LayDanhSach()
        {
            return DatabaseHelper.Query(
                "SELECT b.MaBan, b.TenBan, kv.TenKhuVuc, b.SoCho, b.TrangThai " +
                "FROM dbo.Ban b INNER JOIN dbo.KhuVuc kv ON kv.MaKhuVuc = b.MaKhuVuc " +
                "ORDER BY kv.TenKhuVuc, b.TenBan");
        }

        /// <summary>Lấy bàn theo khu vực; maKhuVuc = 0 nghĩa là tất cả khu vực.</summary>
        public static DataTable LayTheoKhuVuc(int maKhuVuc)
        {
            return DatabaseHelper.Query(
                "SELECT b.MaBan, b.TenBan, b.MaKhuVuc, kv.TenKhuVuc, b.SoCho, b.TrangThai " +
                "FROM dbo.Ban b INNER JOIN dbo.KhuVuc kv ON kv.MaKhuVuc = b.MaKhuVuc " +
                "WHERE (@MaKhuVuc = 0 OR b.MaKhuVuc = @MaKhuVuc) " +
                "ORDER BY kv.TenKhuVuc, b.TenBan",
                DatabaseHelper.P("@MaKhuVuc", maKhuVuc));
        }

        public static void Them(string tenBan, int maKhuVuc, int soCho)
        {
            DatabaseHelper.Execute(
                "INSERT INTO dbo.Ban (TenBan, MaKhuVuc, SoCho, TrangThai) VALUES (@TenBan, @MaKhuVuc, @SoCho, N'Trống')",
                DatabaseHelper.P("@TenBan", tenBan),
                DatabaseHelper.P("@MaKhuVuc", maKhuVuc),
                DatabaseHelper.P("@SoCho", soCho));
        }

        public static void Sua(int maBan, string tenBan, int maKhuVuc, int soCho)
        {
            DatabaseHelper.Execute(
                "UPDATE dbo.Ban SET TenBan = @TenBan, MaKhuVuc = @MaKhuVuc, SoCho = @SoCho WHERE MaBan = @MaBan",
                DatabaseHelper.P("@TenBan", tenBan),
                DatabaseHelper.P("@MaKhuVuc", maKhuVuc),
                DatabaseHelper.P("@SoCho", soCho),
                DatabaseHelper.P("@MaBan", maBan));
        }

        public static void Xoa(int maBan)
        {
            DatabaseHelper.Execute("DELETE FROM dbo.Ban WHERE MaBan = @MaBan",
                DatabaseHelper.P("@MaBan", maBan));
        }

        public static int DemHoaDon(int maBan)
        {
            object value = DatabaseHelper.Scalar(
                "SELECT COUNT(*) FROM dbo.HoaDon WHERE MaBan = @MaBan",
                DatabaseHelper.P("@MaBan", maBan));
            return Convert.ToInt32(value);
        }

        /// <summary>Đồng bộ trạng thái bàn theo các hóa đơn chưa thanh toán.</summary>
        public static void DongBoTrangThai()
        {
            DatabaseHelper.Execute(
                "UPDATE b SET TrangThai = CASE WHEN EXISTS " +
                "(SELECT 1 FROM dbo.HoaDon h WHERE h.MaBan = b.MaBan AND h.TrangThai = N'Chưa thanh toán') " +
                "THEN N'Đang sử dụng' ELSE N'Trống' END FROM dbo.Ban b");
        }
    }
}
