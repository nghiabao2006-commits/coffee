using System;
using System.Data;
using QuanLyQuanCaPhe.DAL;

namespace QuanLyQuanCaPhe.BUS
{
    /// <summary>Nghiệp vụ tổng hợp số liệu cho trang chủ và báo cáo thống kê.</summary>
    public static class ThongKeBUS
    {
        public static decimal DoanhThu(DateTime tu, DateTime den)
        {
            return ThongKeDAL.DoanhThu(tu, den);
        }

        public static int SoHoaDon(DateTime tu, DateTime den)
        {
            return ThongKeDAL.SoHoaDon(tu, den);
        }

        public static int SoBanDangSuDung()
        {
            return ThongKeDAL.SoBanDangSuDung();
        }

        public static int TongSoBan()
        {
            return ThongKeDAL.TongSoBan();
        }

        public static string MonBanChay(DateTime tu, DateTime den)
        {
            return ThongKeDAL.MonBanChay(tu, den);
        }

        public static DataTable HoaDonGanDay(int soLuong)
        {
            return ThongKeDAL.HoaDonGanDay(soLuong);
        }

        public static DataTable TopMon(DateTime tu, DateTime den, int soLuong)
        {
            return ThongKeDAL.TopMon(tu, den, soLuong);
        }

        public static DataRow TongQuan(DateTime tu, DateTime den)
        {
            return ThongKeDAL.TongQuan(tu, den);
        }

        public static int TongSoLuongMon(DateTime tu, DateTime den)
        {
            return ThongKeDAL.TongSoLuongMon(tu, den);
        }

        public static DataTable DoanhThuTheoNgay(DateTime tu, DateTime den)
        {
            return ThongKeDAL.DoanhThuTheoNgay(tu, den);
        }

        public static DataTable HinhThucThanhToan(DateTime tu, DateTime den)
        {
            return ThongKeDAL.HinhThucThanhToan(tu, den);
        }
    }
}
