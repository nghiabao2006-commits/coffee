using System;
using System.Data;

namespace QuanLyQuanCaPhe.DAL
{
    /// <summary>Truy vấn ADO.NET cho món và loại món (thực đơn).</summary>
    public static class ThucDonDAL
    {
        public static DataTable LayMon()
        {
            return DatabaseHelper.Query(
                "SELECT m.MaMon, m.TenMon, l.TenLoai, m.Gia, m.DonViTinh, " +
                "CASE WHEN m.TrangThai = 1 THEN N'Đang bán' ELSE N'Ngừng bán' END AS TrangThaiText, " +
                "m.MoTa FROM dbo.Mon m INNER JOIN dbo.LoaiMon l ON l.MaLoai = m.MaLoai " +
                "ORDER BY l.TenLoai, m.TenMon");
        }

        public static DataTable LayLoai()
        {
            return DatabaseHelper.Query(
                "SELECT l.MaLoai, l.TenLoai, l.MoTa, " +
                "(SELECT COUNT(*) FROM dbo.Mon m WHERE m.MaLoai = l.MaLoai) AS SoMon " +
                "FROM dbo.LoaiMon l ORDER BY l.TenLoai");
        }

        public static DataTable LayLoaiCombo()
        {
            return DatabaseHelper.Query("SELECT MaLoai, TenLoai FROM dbo.LoaiMon ORDER BY TenLoai");
        }

        public static DataTable LayMonDangBan()
        {
            return DatabaseHelper.Query(
                "SELECT m.MaMon, m.TenMon, m.MaLoai, m.Gia, m.DonViTinh, m.MoTa " +
                "FROM dbo.Mon m WHERE m.TrangThai = 1 ORDER BY m.TenMon");
        }

        public static void ThemMon(string tenMon, int maLoai, decimal gia, string donViTinh, bool dangBan, string moTa)
        {
            DatabaseHelper.Execute(
                "INSERT INTO dbo.Mon (TenMon, MaLoai, Gia, DonViTinh, TrangThai, MoTa) " +
                "VALUES (@TenMon, @MaLoai, @Gia, @DonViTinh, @TrangThai, @MoTa)",
                DatabaseHelper.P("@TenMon", tenMon),
                DatabaseHelper.P("@MaLoai", maLoai),
                DatabaseHelper.P("@Gia", gia),
                DatabaseHelper.P("@DonViTinh", donViTinh),
                DatabaseHelper.P("@TrangThai", dangBan ? 1 : 0),
                DatabaseHelper.P("@MoTa", moTa));
        }

        public static void SuaMon(int maMon, string tenMon, int maLoai, decimal gia, string donViTinh, bool dangBan, string moTa)
        {
            DatabaseHelper.Execute(
                "UPDATE dbo.Mon SET TenMon = @TenMon, MaLoai = @MaLoai, Gia = @Gia, " +
                "DonViTinh = @DonViTinh, TrangThai = @TrangThai, MoTa = @MoTa WHERE MaMon = @MaMon",
                DatabaseHelper.P("@TenMon", tenMon),
                DatabaseHelper.P("@MaLoai", maLoai),
                DatabaseHelper.P("@Gia", gia),
                DatabaseHelper.P("@DonViTinh", donViTinh),
                DatabaseHelper.P("@TrangThai", dangBan ? 1 : 0),
                DatabaseHelper.P("@MoTa", moTa),
                DatabaseHelper.P("@MaMon", maMon));
        }

        public static void XoaMon(int maMon)
        {
            DatabaseHelper.Execute("DELETE FROM dbo.Mon WHERE MaMon = @MaMon",
                DatabaseHelper.P("@MaMon", maMon));
        }

        public static void NgungBanMon(int maMon)
        {
            DatabaseHelper.Execute("UPDATE dbo.Mon SET TrangThai = 0 WHERE MaMon = @MaMon",
                DatabaseHelper.P("@MaMon", maMon));
        }

        public static int DemChiTietHoaDon(int maMon)
        {
            object value = DatabaseHelper.Scalar(
                "SELECT COUNT(*) FROM dbo.ChiTietHoaDon WHERE MaMon = @MaMon",
                DatabaseHelper.P("@MaMon", maMon));
            return Convert.ToInt32(value);
        }

        public static void ThemLoai(string tenLoai, string moTa)
        {
            DatabaseHelper.Execute(
                "INSERT INTO dbo.LoaiMon (TenLoai, MoTa) VALUES (@TenLoai, @MoTa)",
                DatabaseHelper.P("@TenLoai", tenLoai),
                DatabaseHelper.P("@MoTa", moTa));
        }

        public static void SuaLoai(int maLoai, string tenLoai, string moTa)
        {
            DatabaseHelper.Execute(
                "UPDATE dbo.LoaiMon SET TenLoai = @TenLoai, MoTa = @MoTa WHERE MaLoai = @MaLoai",
                DatabaseHelper.P("@TenLoai", tenLoai),
                DatabaseHelper.P("@MoTa", moTa),
                DatabaseHelper.P("@MaLoai", maLoai));
        }

        public static void XoaLoai(int maLoai)
        {
            DatabaseHelper.Execute("DELETE FROM dbo.LoaiMon WHERE MaLoai = @MaLoai",
                DatabaseHelper.P("@MaLoai", maLoai));
        }

        public static int DemMonTrongLoai(int maLoai)
        {
            object value = DatabaseHelper.Scalar(
                "SELECT COUNT(*) FROM dbo.Mon WHERE MaLoai = @MaLoai",
                DatabaseHelper.P("@MaLoai", maLoai));
            return Convert.ToInt32(value);
        }
    }
}
