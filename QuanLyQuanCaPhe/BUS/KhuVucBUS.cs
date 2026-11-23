using System.Data;
using QuanLyQuanCaPhe.DAL;

namespace QuanLyQuanCaPhe.BUS
{
    /// <summary>Nghiệp vụ quản lý khu vực.</summary>
    public static class KhuVucBUS
    {
        public static DataTable LayDanhSach()
        {
            return KhuVucDAL.LayDanhSach();
        }

        public static DataTable LayCombo()
        {
            return KhuVucDAL.LayCombo();
        }

        public static void Them(string tenKhuVuc, string moTa)
        {
            KhuVucDAL.Them(tenKhuVuc, moTa);
        }

        public static void Sua(int maKhuVuc, string tenKhuVuc, string moTa)
        {
            KhuVucDAL.Sua(maKhuVuc, tenKhuVuc, moTa);
        }

        public static void Xoa(int maKhuVuc)
        {
            KhuVucDAL.Xoa(maKhuVuc);
        }

        public static int DemBan(int maKhuVuc)
        {
            return KhuVucDAL.DemBan(maKhuVuc);
        }
    }
}
