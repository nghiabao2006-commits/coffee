using System.Data;
using QuanLyQuanCaPhe.DAL;

namespace QuanLyQuanCaPhe.BUS
{
    /// <summary>Nghiệp vụ quản lý thực đơn (món và loại món).</summary>
    public static class ThucDonBUS
    {
        public static DataTable LayMon()
        {
            return ThucDonDAL.LayMon();
        }

        public static DataTable LayLoai()
        {
            return ThucDonDAL.LayLoai();
        }

        public static DataTable LayLoaiCombo()
        {
            return ThucDonDAL.LayLoaiCombo();
        }

        public static DataTable LayMonDangBan()
        {
            return ThucDonDAL.LayMonDangBan();
        }

        public static void ThemMon(string tenMon, int maLoai, decimal gia, string donViTinh, bool dangBan, string moTa)
        {
            ThucDonDAL.ThemMon(tenMon, maLoai, gia, donViTinh, dangBan, moTa);
        }

        public static void SuaMon(int maMon, string tenMon, int maLoai, decimal gia, string donViTinh, bool dangBan, string moTa)
        {
            ThucDonDAL.SuaMon(maMon, tenMon, maLoai, gia, donViTinh, dangBan, moTa);
        }

        public static int DemChiTietHoaDon(int maMon)
        {
            return ThucDonDAL.DemChiTietHoaDon(maMon);
        }

        public static void NgungBanMon(int maMon)
        {
            ThucDonDAL.NgungBanMon(maMon);
        }

        public static void XoaMon(int maMon)
        {
            ThucDonDAL.XoaMon(maMon);
        }

        public static void ThemLoai(string tenLoai, string moTa)
        {
            ThucDonDAL.ThemLoai(tenLoai, moTa);
        }

        public static void SuaLoai(int maLoai, string tenLoai, string moTa)
        {
            ThucDonDAL.SuaLoai(maLoai, tenLoai, moTa);
        }

        public static int DemMonTrongLoai(int maLoai)
        {
            return ThucDonDAL.DemMonTrongLoai(maLoai);
        }

        public static void XoaLoai(int maLoai)
        {
            ThucDonDAL.XoaLoai(maLoai);
        }
    }
}
