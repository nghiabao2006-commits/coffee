using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using QuanLyQuanCaPhe.DTO;

namespace QuanLyQuanCaPhe.DAL
{
    /// <summary>Truy vấn ADO.NET cho hóa đơn: gọi món, tính tiền, thanh toán và tra cứu.</summary>
    public static class HoaDonDAL
    {
        public static int? GetOpenInvoice(int maBan)
        {
            object value = DatabaseHelper.Scalar(
                "SELECT TOP 1 MaHD FROM dbo.HoaDon WHERE MaBan = @MaBan AND TrangThai = N'Chưa thanh toán' ORDER BY MaHD DESC",
                DatabaseHelper.P("@MaBan", maBan));
            return value == null ? (int?)null : Convert.ToInt32(value);
        }

        public static int GetOrCreateInvoice(int maBan, int maNV)
        {
            using (var connection = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int? existing = ScalarInt(connection, transaction,
                            "SELECT TOP 1 MaHD FROM dbo.HoaDon WITH (UPDLOCK, HOLDLOCK) " +
                            "WHERE MaBan = @MaBan AND TrangThai = N'Chưa thanh toán' ORDER BY MaHD DESC",
                            new SqlParameter("@MaBan", maBan));

                        if (existing.HasValue)
                        {
                            UpdateTableStatus(connection, transaction, maBan, true);
                            transaction.Commit();
                            return existing.Value;
                        }

                        int maHD;
                        using (var command = new SqlCommand(
                            "INSERT INTO dbo.HoaDon (MaBan, MaNV, NgayLap, TrangThai) " +
                            "VALUES (@MaBan, @MaNV, GETDATE(), N'Chưa thanh toán'); " +
                            "SELECT CAST(SCOPE_IDENTITY() AS INT);", connection, transaction))
                        {
                            command.Parameters.AddWithValue("@MaBan", maBan);
                            command.Parameters.AddWithValue("@MaNV", maNV);
                            maHD = Convert.ToInt32(command.ExecuteScalar());
                        }

                        UpdateTableStatus(connection, transaction, maBan, true);
                        transaction.Commit();
                        return maHD;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public static void AddItem(int maHD, int maMon)
        {
            using (var connection = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int maBan = RequireOpenInvoice(connection, transaction, maHD);

                        decimal price;
                        using (var command = new SqlCommand(
                            "SELECT Gia FROM dbo.Mon WHERE MaMon = @MaMon AND TrangThai = 1", connection, transaction))
                        {
                            command.Parameters.AddWithValue("@MaMon", maMon);
                            object value = command.ExecuteScalar();
                            if (value == null)
                            {
                                throw new InvalidOperationException("Món này đã ngừng bán.");
                            }
                            price = Convert.ToDecimal(value);
                        }

                        int existing = Convert.ToInt32(ExecuteScalar(connection, transaction,
                            "SELECT COUNT(*) FROM dbo.ChiTietHoaDon WHERE MaHD = @MaHD AND MaMon = @MaMon",
                            new SqlParameter("@MaHD", maHD), new SqlParameter("@MaMon", maMon)));

                        if (existing > 0)
                        {
                            using (var command = new SqlCommand(
                                "UPDATE dbo.ChiTietHoaDon SET SoLuong = SoLuong + 1 WHERE MaHD = @MaHD AND MaMon = @MaMon",
                                connection, transaction))
                            {
                                command.Parameters.AddWithValue("@MaHD", maHD);
                                command.Parameters.AddWithValue("@MaMon", maMon);
                                command.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            using (var command = new SqlCommand(
                                "INSERT INTO dbo.ChiTietHoaDon (MaHD, MaMon, SoLuong, DonGia) " +
                                "VALUES (@MaHD, @MaMon, 1, @DonGia)", connection, transaction))
                            {
                                command.Parameters.AddWithValue("@MaHD", maHD);
                                command.Parameters.AddWithValue("@MaMon", maMon);
                                command.Parameters.AddWithValue("@DonGia", price);
                                command.ExecuteNonQuery();
                            }
                        }

                        RecalculateTotal(connection, transaction, maHD);
                        UpdateTableStatus(connection, transaction, maBan, true);
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

        /// <summary>Tăng/giảm số lượng. Trả về false nếu hóa đơn đã bị xóa vì hết món.</summary>
        public static bool ChangeQuantity(int maCT, int delta)
        {
            if (delta == 0)
            {
                return true;
            }

            using (var connection = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int current;
                        using (var command = new SqlCommand(
                            "SELECT SoLuong FROM dbo.ChiTietHoaDon WHERE MaCT = @MaCT", connection, transaction))
                        {
                            command.Parameters.AddWithValue("@MaCT", maCT);
                            object value = command.ExecuteScalar();
                            if (value == null)
                            {
                                transaction.Rollback();
                                return true;
                            }
                            current = Convert.ToInt32(value);
                        }

                        bool result = ApplyQuantity(connection, transaction, maCT, current + delta);
                        transaction.Commit();
                        return result;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public static bool RemoveItem(int maCT)
        {
            using (var connection = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        bool result = ApplyQuantity(connection, transaction, maCT, 0);
                        transaction.Commit();
                        return result;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private static bool ApplyQuantity(SqlConnection connection, SqlTransaction transaction, int maCT, int newQuantity)
        {
            int maHD;
            using (var command = new SqlCommand(
                "SELECT MaHD FROM dbo.ChiTietHoaDon WHERE MaCT = @MaCT", connection, transaction))
            {
                command.Parameters.AddWithValue("@MaCT", maCT);
                object value = command.ExecuteScalar();
                if (value == null)
                {
                    return true;
                }
                maHD = Convert.ToInt32(value);
            }

            int maBan = RequireOpenInvoice(connection, transaction, maHD);

            if (newQuantity <= 0)
            {
                using (var command = new SqlCommand(
                    "DELETE FROM dbo.ChiTietHoaDon WHERE MaCT = @MaCT", connection, transaction))
                {
                    command.Parameters.AddWithValue("@MaCT", maCT);
                    command.ExecuteNonQuery();
                }
            }
            else
            {
                using (var command = new SqlCommand(
                    "UPDATE dbo.ChiTietHoaDon SET SoLuong = @SoLuong WHERE MaCT = @MaCT", connection, transaction))
                {
                    command.Parameters.AddWithValue("@SoLuong", newQuantity);
                    command.Parameters.AddWithValue("@MaCT", maCT);
                    command.ExecuteNonQuery();
                }
            }

            int remaining = Convert.ToInt32(ExecuteScalar(connection, transaction,
                "SELECT COUNT(*) FROM dbo.ChiTietHoaDon WHERE MaHD = @MaHD",
                new SqlParameter("@MaHD", maHD)));

            if (remaining == 0)
            {
                using (var command = new SqlCommand(
                    "DELETE FROM dbo.HoaDon WHERE MaHD = @MaHD", connection, transaction))
                {
                    command.Parameters.AddWithValue("@MaHD", maHD);
                    command.ExecuteNonQuery();
                }
                UpdateTableStatus(connection, transaction, maBan, false);
                return false;
            }

            RecalculateTotal(connection, transaction, maHD);
            return true;
        }

        public static void PayInvoice(int maHD, decimal giamGia, string phuongThuc, string ghiChu)
        {
            using (var connection = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int maBan = RequireOpenInvoice(connection, transaction, maHD);

                        decimal total = Convert.ToDecimal(ExecuteScalar(connection, transaction,
                            "SELECT ISNULL(SUM(ThanhTien), 0) FROM dbo.ChiTietHoaDon WHERE MaHD = @MaHD",
                            new SqlParameter("@MaHD", maHD)));

                        if (giamGia < 0)
                        {
                            giamGia = 0;
                        }
                        if (giamGia > total)
                        {
                            giamGia = total;
                        }

                        using (var command = new SqlCommand(
                            "UPDATE dbo.HoaDon SET TongTien = @TongTien, GiamGia = @GiamGia, " +
                            "PhuongThucThanhToan = @PhuongThuc, GhiChu = @GhiChu, " +
                            "NgayThanhToan = GETDATE(), TrangThai = N'Đã thanh toán' WHERE MaHD = @MaHD",
                            connection, transaction))
                        {
                            command.Parameters.AddWithValue("@TongTien", total);
                            command.Parameters.AddWithValue("@GiamGia", giamGia);
                            command.Parameters.AddWithValue("@PhuongThuc", (object)phuongThuc ?? DBNull.Value);
                            command.Parameters.AddWithValue("@GhiChu", string.IsNullOrWhiteSpace(ghiChu) ? (object)DBNull.Value : ghiChu.Trim());
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

        public static void DeleteOpenInvoice(int maHD)
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
                            "DELETE FROM dbo.HoaDon WHERE MaHD = @MaHD AND TrangThai = N'Chưa thanh toán'",
                            connection, transaction))
                        {
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

        public static decimal GetTotal(int maHD)
        {
            object value = DatabaseHelper.Scalar(
                "SELECT ISNULL(SUM(ThanhTien), 0) FROM dbo.ChiTietHoaDon WHERE MaHD = @MaHD",
                DatabaseHelper.P("@MaHD", maHD));
            return value == null ? 0m : Convert.ToDecimal(value);
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

        private static void RecalculateTotal(SqlConnection connection, SqlTransaction transaction, int maHD)
        {
            using (var command = new SqlCommand(
                "UPDATE dbo.HoaDon SET TongTien = " +
                "(SELECT ISNULL(SUM(ThanhTien), 0) FROM dbo.ChiTietHoaDon WHERE MaHD = @MaHD) " +
                "WHERE MaHD = @MaHD", connection, transaction))
            {
                command.Parameters.AddWithValue("@MaHD", maHD);
                command.ExecuteNonQuery();
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

        private static object ExecuteScalar(SqlConnection connection, SqlTransaction transaction, string sql,
            params SqlParameter[] parameters)
        {
            using (var command = new SqlCommand(sql, connection, transaction))
            {
                foreach (SqlParameter parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
                object value = command.ExecuteScalar();
                return value == DBNull.Value ? null : value;
            }
        }

        private static int? ScalarInt(SqlConnection connection, SqlTransaction transaction, string sql,
            params SqlParameter[] parameters)
        {
            object value = ExecuteScalar(connection, transaction, sql, parameters);
            return value == null ? (int?)null : Convert.ToInt32(value);
        }
    }
}
