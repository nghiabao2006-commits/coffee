using System;
using System.Data;

namespace QuanLyQuanCaPhe.DAL
{
    /// <summary>Truy vấn ADO.NET cho khu vực.</summary>
    public static class KhuVucDAL
    {
        public static DataTable LayDanhSach()
        {
            return DatabaseHelper.Query(
                "SELECT kv.MaKhuVuc, kv.TenKhuVuc, kv.MoTa, " +
                "(SELECT COUNT(*) FROM dbo.Ban b WHERE b.MaKhuVuc = kv.MaKhuVuc) AS SoBan " +
                "FROM dbo.KhuVuc kv ORDER BY kv.TenKhuVuc");
        }

        public static DataTable LayCombo()
        {
            return DatabaseHelper.Query("SELECT MaKhuVuc, TenKhuVuc FROM dbo.KhuVuc ORDER BY TenKhuVuc");
        }

        public static void Them(string tenKhuVuc, string moTa)
        {
            DatabaseHelper.Execute(
                "INSERT INTO dbo.KhuVuc (TenKhuVuc, MoTa) VALUES (@TenKhuVuc, @MoTa)",
                DatabaseHelper.P("@TenKhuVuc", tenKhuVuc),
                DatabaseHelper.P("@MoTa", moTa));
        }

        public static void Sua(int maKhuVuc, string tenKhuVuc, string moTa)
        {
            DatabaseHelper.Execute(
                "UPDATE dbo.KhuVuc SET TenKhuVuc = @TenKhuVuc, MoTa = @MoTa WHERE MaKhuVuc = @MaKhuVuc",
                DatabaseHelper.P("@TenKhuVuc", tenKhuVuc),
                DatabaseHelper.P("@MoTa", moTa),
                DatabaseHelper.P("@MaKhuVuc", maKhuVuc));
        }

        public static void Xoa(int maKhuVuc)
        {
            DatabaseHelper.Execute("DELETE FROM dbo.KhuVuc WHERE MaKhuVuc = @MaKhuVuc",
                DatabaseHelper.P("@MaKhuVuc", maKhuVuc));
        }

        public static int DemBan(int maKhuVuc)
        {
            object value = DatabaseHelper.Scalar(
                "SELECT COUNT(*) FROM dbo.Ban WHERE MaKhuVuc = @MaKhuVuc",
                DatabaseHelper.P("@MaKhuVuc", maKhuVuc));
            return Convert.ToInt32(value);
        }
    }
}
