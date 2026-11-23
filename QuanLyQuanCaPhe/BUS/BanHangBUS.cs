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
            return BanHangDAL.GetOpenInvoice(maBan);
        }

        public static int GetOrCreateInvoice(int maBan, int maNV)
        {
            return BanHangDAL.GetOrCreateInvoice(maBan, maNV);
        }

        public static void AddItem(int maHD, int maMon)
        {
            BanHangDAL.AddItem(maHD, maMon);
        }

        public static bool ChangeQuantity(int maCT, int delta)
        {
            return BanHangDAL.ChangeQuantity(maCT, delta);
        }

        public static bool RemoveItem(int maCT)
        {
            return BanHangDAL.RemoveItem(maCT);
        }

        public static List<ChiTietHoaDonDTO> GetDetails(int maHD)
        {
            return BanHangDAL.GetDetails(maHD);
        }

        public static decimal GetTotal(int maHD)
        {
            return BanHangDAL.GetTotal(maHD);
        }

        public static void PayInvoice(int maHD, decimal giamGia, string phuongThuc, string ghiChu)
        {
            BanHangDAL.PayInvoice(maHD, giamGia, phuongThuc, ghiChu);
        }

        public static void DeleteOpenInvoice(int maHD)
        {
            BanHangDAL.DeleteOpenInvoice(maHD);
        }
    }
}
