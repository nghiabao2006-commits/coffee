using System.Collections.Generic;
using System.Data;
using QuanLyQuanCaPhe.DAL;
using QuanLyQuanCaPhe.DTO;

namespace QuanLyQuanCaPhe.BUS
{
    /// <summary>Nghiệp vụ bán hàng: chọn bàn, gọi món và thanh toán.</summary>
    public static class BanHangBUS
    {
        public static void DongBoTrangThaiBan()
        {
            BanDAL.DongBoTrangThai();
        }

        public static DataTable LayBan(int maKhuVuc)
        {
            return BanDAL.LayTheoKhuVuc(maKhuVuc);
        }

        public static DataTable LayLoaiMon()
        {
            return ThucDonDAL.LayLoaiCombo();
        }

        public static DataTable LayMonDangBan()
        {
            return ThucDonDAL.LayMonDangBan();
        }

        public static int? GetOpenInvoice(int maBan)
        {
            return HoaDonDAL.GetOpenInvoice(maBan);
        }

        public static int GetOrCreateInvoice(int maBan, int maNV)
        {
            return HoaDonDAL.GetOrCreateInvoice(maBan, maNV);
        }

        public static void AddItem(int maHD, int maMon)
        {
            HoaDonDAL.AddItem(maHD, maMon);
        }

        public static bool ChangeQuantity(int maCT, int delta)
        {
            return HoaDonDAL.ChangeQuantity(maCT, delta);
        }

        public static bool RemoveItem(int maCT)
        {
            return HoaDonDAL.RemoveItem(maCT);
        }

        public static List<ChiTietHoaDonDTO> GetDetails(int maHD)
        {
            return HoaDonDAL.GetDetails(maHD);
        }

        public static decimal GetTotal(int maHD)
        {
            return HoaDonDAL.GetTotal(maHD);
        }

        public static void PayInvoice(int maHD, decimal giamGia, string phuongThuc, string ghiChu)
        {
            HoaDonDAL.PayInvoice(maHD, giamGia, phuongThuc, ghiChu);
        }

        public static void DeleteOpenInvoice(int maHD)
        {
            HoaDonDAL.DeleteOpenInvoice(maHD);
        }
    }
}
