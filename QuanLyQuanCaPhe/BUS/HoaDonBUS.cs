using System;
using System.Collections.Generic;
using System.Data;
using QuanLyQuanCaPhe.DAL;
using QuanLyQuanCaPhe.DTO;

namespace QuanLyQuanCaPhe.BUS
{
    /// <summary>Nghiệp vụ tra cứu và hủy hóa đơn.</summary>
    public static class HoaDonBUS
    {
        public static DataTable LayDanhSach(DateTime tuNgay, DateTime denNgay, string keyword, string trangThai)
        {
            return HoaDonDAL.LayDanhSach(tuNgay, denNgay, keyword, trangThai);
        }

        public static HoaDonDTO GetInvoice(int maHD)
        {
            return HoaDonDAL.GetInvoice(maHD);
        }

        public static List<ChiTietHoaDonDTO> GetDetails(int maHD)
        {
            return HoaDonDAL.GetDetails(maHD);
        }

        public static void CancelInvoice(int maHD)
        {
            HoaDonDAL.CancelInvoice(maHD);
        }
    }
}
