using System.Collections.Generic;
using System.Data;
using QuanLyQuanCaPhe.DAL;
using QuanLyQuanCaPhe.DTO;

namespace QuanLyQuanCaPhe.BUS
{
    /// <summary>Nghiệp vụ quản lý nhân viên.</summary>
    public static class NhanVienBUS
    {
        public static List<NhanVienDTO> LayDanhSach()
        {
            return NhanVienDAL.LayDanhSach();
        }

        public static DataTable LayVaiTro()
        {
            return NhanVienDAL.LayVaiTro();
        }

        public static NhanVienDTO LayChiTiet(int maNV)
        {
            return NhanVienDAL.LayChiTiet(maNV);
        }

        public static bool LaQuanLyDuyNhat()
        {
            return NhanVienDAL.DemQuanLyDangLam() <= 1;
        }

        public static void Them(NhanVienDTO nhanVien, string matKhau)
        {
            NhanVienDAL.Them(nhanVien, MatKhau.Bam(matKhau));
        }

        /// <summary>Cập nhật nhân viên; để trống mật khẩu mới thì giữ nguyên mật khẩu cũ.</summary>
        public static void Sua(NhanVienDTO nhanVien, string matKhauMoi)
        {
            string hash = string.IsNullOrEmpty(matKhauMoi) ? null : MatKhau.Bam(matKhauMoi);
            NhanVienDAL.Sua(nhanVien, hash);
        }

        public static int DemHoaDon(int maNV)
        {
            return NhanVienDAL.DemHoaDon(maNV);
        }

        public static void ChuyenNghiViec(int maNV)
        {
            NhanVienDAL.ChuyenNghiViec(maNV);
        }

        public static void Xoa(int maNV)
        {
            NhanVienDAL.Xoa(maNV);
        }
    }
}
