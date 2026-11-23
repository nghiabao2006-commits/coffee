using System;
using System.Collections.Generic;
using System.Data;
using QuanLyQuanCaPhe.DTO;

namespace QuanLyQuanCaPhe.DAL
{
    /// <summary>Truy vấn ADO.NET cho nhân viên và vai trò.</summary>
    public static class NhanVienDAL
    {
        public static List<NhanVienDTO> LayDanhSach()
        {
            DataTable table = DatabaseHelper.Query(
                "SELECT n.MaNV, n.TenNV, n.TenDangNhap, n.MaVaiTro, v.TenVaiTro, n.DienThoai, " +
                "n.NgayVaoLam, n.TrangThai FROM dbo.NhanVien n " +
                "INNER JOIN dbo.VaiTro v ON v.MaVaiTro = n.MaVaiTro " +
                "ORDER BY n.TrangThai DESC, n.TenNV");

            var result = new List<NhanVienDTO>();
            foreach (DataRow row in table.Rows)
            {
                result.Add(Map(row));
            }
            return result;
        }

        public static DataTable LayVaiTro()
        {
            return DatabaseHelper.Query("SELECT MaVaiTro, TenVaiTro FROM dbo.VaiTro ORDER BY MaVaiTro");
        }

        public static NhanVienDTO LayChiTiet(int maNV)
        {
            DataTable table = DatabaseHelper.Query(
                "SELECT n.MaNV, n.TenNV, n.TenDangNhap, n.MaVaiTro, v.TenVaiTro, n.DienThoai, " +
                "n.DiaChi, n.NgayVaoLam, n.TrangThai FROM dbo.NhanVien n " +
                "INNER JOIN dbo.VaiTro v ON v.MaVaiTro = n.MaVaiTro WHERE n.MaNV = @MaNV",
                DatabaseHelper.P("@MaNV", maNV));
            return table.Rows.Count == 0 ? null : Map(table.Rows[0]);
        }

        public static int DemQuanLyDangLam()
        {
            object value = DatabaseHelper.Scalar(
                "SELECT COUNT(*) FROM dbo.NhanVien WHERE MaVaiTro = 1 AND TrangThai = N'Đang làm'");
            return Convert.ToInt32(value);
        }

        public static void Them(NhanVienDTO nv, string matKhauHash)
        {
            DatabaseHelper.Execute(
                "INSERT INTO dbo.NhanVien (TenNV, TenDangNhap, MatKhau, MaVaiTro, DienThoai, DiaChi, NgayVaoLam, TrangThai) " +
                "VALUES (@TenNV, @TenDangNhap, @MatKhau, @MaVaiTro, @DienThoai, @DiaChi, @NgayVaoLam, @TrangThai)",
                DatabaseHelper.P("@TenNV", nv.TenNV),
                DatabaseHelper.P("@TenDangNhap", nv.TenDangNhap),
                DatabaseHelper.P("@MatKhau", matKhauHash),
                DatabaseHelper.P("@MaVaiTro", nv.MaVaiTro),
                DatabaseHelper.P("@DienThoai", nv.DienThoai),
                DatabaseHelper.P("@DiaChi", nv.DiaChi),
                DatabaseHelper.P("@NgayVaoLam", nv.NgayVaoLam.HasValue ? (object)nv.NgayVaoLam.Value.Date : DBNull.Value),
                DatabaseHelper.P("@TrangThai", nv.TrangThai));
        }

        /// <summary>Cập nhật nhân viên. Nếu matKhauHash rỗng thì giữ nguyên mật khẩu cũ.</summary>
        public static void Sua(NhanVienDTO nv, string matKhauHash)
        {
            if (string.IsNullOrEmpty(matKhauHash))
            {
                DatabaseHelper.Execute(
                    "UPDATE dbo.NhanVien SET TenNV = @TenNV, TenDangNhap = @TenDangNhap, " +
                    "MaVaiTro = @MaVaiTro, DienThoai = @DienThoai, DiaChi = @DiaChi, NgayVaoLam = @NgayVaoLam, " +
                    "TrangThai = @TrangThai WHERE MaNV = @MaNV",
                    DatabaseHelper.P("@TenNV", nv.TenNV),
                    DatabaseHelper.P("@TenDangNhap", nv.TenDangNhap),
                    DatabaseHelper.P("@MaVaiTro", nv.MaVaiTro),
                    DatabaseHelper.P("@DienThoai", nv.DienThoai),
                    DatabaseHelper.P("@DiaChi", nv.DiaChi),
                    DatabaseHelper.P("@NgayVaoLam", nv.NgayVaoLam.HasValue ? (object)nv.NgayVaoLam.Value.Date : DBNull.Value),
                    DatabaseHelper.P("@TrangThai", nv.TrangThai),
                    DatabaseHelper.P("@MaNV", nv.MaNV));
            }
            else
            {
                DatabaseHelper.Execute(
                    "UPDATE dbo.NhanVien SET TenNV = @TenNV, TenDangNhap = @TenDangNhap, MatKhau = @MatKhau, " +
                    "MaVaiTro = @MaVaiTro, DienThoai = @DienThoai, DiaChi = @DiaChi, NgayVaoLam = @NgayVaoLam, " +
                    "TrangThai = @TrangThai WHERE MaNV = @MaNV",
                    DatabaseHelper.P("@TenNV", nv.TenNV),
                    DatabaseHelper.P("@TenDangNhap", nv.TenDangNhap),
                    DatabaseHelper.P("@MatKhau", matKhauHash),
                    DatabaseHelper.P("@MaVaiTro", nv.MaVaiTro),
                    DatabaseHelper.P("@DienThoai", nv.DienThoai),
                    DatabaseHelper.P("@DiaChi", nv.DiaChi),
                    DatabaseHelper.P("@NgayVaoLam", nv.NgayVaoLam.HasValue ? (object)nv.NgayVaoLam.Value.Date : DBNull.Value),
                    DatabaseHelper.P("@TrangThai", nv.TrangThai),
                    DatabaseHelper.P("@MaNV", nv.MaNV));
            }
        }

        public static int DemHoaDon(int maNV)
        {
            object value = DatabaseHelper.Scalar(
                "SELECT COUNT(*) FROM dbo.HoaDon WHERE MaNV = @MaNV",
                DatabaseHelper.P("@MaNV", maNV));
            return Convert.ToInt32(value);
        }

        public static void ChuyenNghiViec(int maNV)
        {
            DatabaseHelper.Execute(
                "UPDATE dbo.NhanVien SET TrangThai = N'Nghỉ việc' WHERE MaNV = @MaNV",
                DatabaseHelper.P("@MaNV", maNV));
        }

        public static void Xoa(int maNV)
        {
            DatabaseHelper.Execute("DELETE FROM dbo.NhanVien WHERE MaNV = @MaNV",
                DatabaseHelper.P("@MaNV", maNV));
        }

        private static NhanVienDTO Map(DataRow row)
        {
            return new NhanVienDTO
            {
                MaNV = Convert.ToInt32(row["MaNV"]),
                TenNV = Convert.ToString(row["TenNV"]),
                TenDangNhap = Convert.ToString(row["TenDangNhap"]),
                MaVaiTro = Convert.ToInt32(row["MaVaiTro"]),
                TenVaiTro = Convert.ToString(row["TenVaiTro"]),
                DienThoai = Convert.ToString(row["DienThoai"]),
                DiaChi = row.Table.Columns.Contains("DiaChi") ? Convert.ToString(row["DiaChi"]) : string.Empty,
                NgayVaoLam = row["NgayVaoLam"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["NgayVaoLam"]),
                TrangThai = Convert.ToString(row["TrangThai"])
            };
        }
    }
}
