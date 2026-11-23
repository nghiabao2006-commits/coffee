using System.Data;
using QuanLyQuanCaPhe.DAL;

namespace QuanLyQuanCaPhe.BUS
{
    /// <summary>Nghiệp vụ quản lý bàn.</summary>
    public static class BanBUS
    {
        public static DataTable LayDanhSach()
        {
            return BanDAL.LayDanhSach();
        }

        public static DataTable LayTheoKhuVuc(int maKhuVuc)
        {
            return BanDAL.LayTheoKhuVuc(maKhuVuc);
        }

        public static void Them(string tenBan, int maKhuVuc, int soCho)
        {
            BanDAL.Them(tenBan, maKhuVuc, soCho);
        }

        public static void Sua(int maBan, string tenBan, int maKhuVuc, int soCho)
        {
            BanDAL.Sua(maBan, tenBan, maKhuVuc, soCho);
        }

        public static void Xoa(int maBan)
        {
            BanDAL.Xoa(maBan);
        }

        public static int DemHoaDon(int maBan)
        {
            return BanDAL.DemHoaDon(maBan);
        }

        public static void DongBoTrangThai()
        {
            BanDAL.DongBoTrangThai();
        }
    }
}
