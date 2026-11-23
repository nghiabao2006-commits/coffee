using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using QuanLyQuanCaPhe.DTO;

namespace QuanLyQuanCaPhe.DAL
{
    /// <summary>Truy vấn ADO.NET cho tra cứu hóa đơn: danh sách, chi tiết và hủy hóa đơn.</summary>
    public static class HoaDonDAL
    {
        public static void CancelInvoice(int maHD)
        {
            using (var connection = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int maBan = RequireOpenInvoice(connection, transaction, maHD);
                        using (var command = new SqlCommand(
                            "UPDATE dbo.HoaDon SET TrangThai = N'Đã hủy', GhiChu = @GhiChu WHERE MaHD = @MaHD",
                            connection, transaction))
                        {
                            command.Parameters.AddWithValue("@GhiChu", "Hủy hóa đơn");
                            command.Parameters.AddWithValue("@MaHD", maHD);
                            command.ExecuteNonQuery();
                        }
                        UpdateTableStatus(connection, transaction, maBan, false);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public static List<ChiTietHoaDonDTO> GetDetails(int maHD)
        {
            DataTable table = DatabaseHelper.Query(
                "SELECT ct.MaCT, ct.MaHD, ct.MaMon, m.TenMon, ct.SoLuong, ct.DonGia, ct.ThanhTien " +
                "FROM dbo.ChiTietHoaDon ct INNER JOIN dbo.Mon m ON m.MaMon = ct.MaMon " +
                "WHERE ct.MaHD = @MaHD ORDER BY ct.MaCT",
                DatabaseHelper.P("@MaHD", maHD));

            var result = new List<ChiTietHoaDonDTO>();
            foreach (DataRow row in table.Rows)
            {
                result.Add(new ChiTietHoaDonDTO
                {
                    MaCT = Convert.ToInt32(row["MaCT"]),
                    MaHD = Convert.ToInt32(row["MaHD"]),
                    MaMon = Convert.ToInt32(row["MaMon"]),
                    TenMon = Convert.ToString(row["TenMon"]),
                    SoLuong = Convert.ToInt32(row["SoLuong"]),
                    DonGia = Convert.ToDecimal(row["DonGia"]),
                    ThanhTien = Convert.ToDecimal(row["ThanhTien"])
                });
            }
            return result;
        }

        public static HoaDonDTO GetInvoice(int maHD)
        {
            DataTable table = DatabaseHelper.Query(
                "SELECT h.MaHD, h.MaBan, b.TenBan, kv.TenKhuVuc, h.MaNV, nv.TenNV, " +
                "h.NgayLap, h.NgayThanhToan, h.TongTien, h.GiamGia, h.ThanhTien, " +
                "h.PhuongThucThanhToan, h.TrangThai, h.GhiChu " +
                "FROM dbo.HoaDon h INNER JOIN dbo.Ban b ON b.MaBan = h.MaBan " +
                "INNER JOIN dbo.KhuVuc kv ON kv.MaKhuVuc = b.MaKhuVuc " +
                "INNER JOIN dbo.NhanVien nv ON nv.MaNV = h.MaNV WHERE h.MaHD = @MaHD",
                DatabaseHelper.P("@MaHD", maHD));
            return table.Rows.Count == 0 ? null : MapInvoice(table.Rows[0]);
        }

        /// <summary>Tra cứu danh sách hóa đơn theo ngày lập, từ khóa và trạng thái.</summary>
        public static DataTable LayDanhSach(DateTime tuNgay, DateTime denNgay, string keyword, string trangThai)
        {
            var sql = new StringBuilder();
            sql.Append(
                "SELECT h.MaHD, 'HD' + RIGHT('0000' + CAST(h.MaHD AS varchar(10)), 4) AS MaHoaDon, " +
                "ISNULL(h.NgayThanhToan, h.NgayLap) AS ThoiGian, b.TenBan, nv.TenNV, " +
                "h.TongTien, h.GiamGia, h.ThanhTien, ISNULL(h.PhuongThucThanhToan, N'—') AS PhuongThuc, " +
                "h.TrangThai, (SELECT COUNT(*) FROM dbo.ChiTietHoaDon ct WHERE ct.MaHD = h.MaHD) AS SoMon " +
                "FROM dbo.HoaDon h INNER JOIN dbo.Ban b ON b.MaBan = h.MaBan " +
                "INNER JOIN dbo.NhanVien nv ON nv.MaNV = h.MaNV " +
                "WHERE h.NgayLap >= @TuNgay AND h.NgayLap < @DenNgay");

            var parameters = new List<SqlParameter>();
            parameters.Add(DatabaseHelper.P("@TuNgay", tuNgay));
            parameters.Add(DatabaseHelper.P("@DenNgay", denNgay));

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sql.Append(" AND (('HD' + RIGHT('0000' + CAST(h.MaHD AS varchar(10)), 4)) LIKE @Kw " +
                           "OR b.TenBan LIKE @Kw OR nv.TenNV LIKE @Kw)");
                parameters.Add(DatabaseHelper.P("@Kw", "%" + keyword.Trim() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                sql.Append(" AND h.TrangThai = @TrangThai");
                parameters.Add(DatabaseHelper.P("@TrangThai", trangThai));
            }
            sql.Append(" ORDER BY h.NgayLap DESC, h.MaHD DESC");

            return DatabaseHelper.Query(sql.ToString(), parameters.ToArray());
        }

        private static HoaDonDTO MapInvoice(DataRow row)
        {
            return new HoaDonDTO
            {
                MaHD = Convert.ToInt32(row["MaHD"]),
                MaBan = Convert.ToInt32(row["MaBan"]),
                TenBan = Convert.ToString(row["TenBan"]),
                TenKhuVuc = Convert.ToString(row["TenKhuVuc"]),
                MaNV = Convert.ToInt32(row["MaNV"]),
                TenNV = Convert.ToString(row["TenNV"]),
                NgayLap = AsDate(row["NgayLap"]),
                NgayThanhToan = AsDate(row["NgayThanhToan"]),
                TongTien = Convert.ToDecimal(row["TongTien"]),
                GiamGia = Convert.ToDecimal(row["GiamGia"]),
                ThanhTien = Convert.ToDecimal(row["ThanhTien"]),
                PhuongThucThanhToan = Convert.ToString(row["PhuongThucThanhToan"]),
                TrangThai = Convert.ToString(row["TrangThai"]),
                GhiChu = Convert.ToString(row["GhiChu"])
            };
        }

        private static DateTime? AsDate(object value)
        {
            return value == null || value == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(value);
        }

        private static int RequireOpenInvoice(SqlConnection connection, SqlTransaction transaction, int maHD)
        {
            using (var command = new SqlCommand(
                "SELECT MaBan FROM dbo.HoaDon WHERE MaHD = @MaHD AND TrangThai = N'Chưa thanh toán'",
                connection, transaction))
            {
                command.Parameters.AddWithValue("@MaHD", maHD);
                object value = command.ExecuteScalar();
                if (value == null)
                {
                    throw new InvalidOperationException("Hóa đơn không còn ở trạng thái chưa thanh toán.");
                }
                return Convert.ToInt32(value);
            }
        }

        private static void UpdateTableStatus(SqlConnection connection, SqlTransaction transaction, int maBan, bool inUse)
        {
            using (var command = new SqlCommand(
                "UPDATE dbo.Ban SET TrangThai = @TrangThai WHERE MaBan = @MaBan", connection, transaction))
            {
                command.Parameters.AddWithValue("@TrangThai", inUse ? "Đang sử dụng" : "Trống");
                command.Parameters.AddWithValue("@MaBan", maBan);
                command.ExecuteNonQuery();
            }
        }
    }
}
