using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using QuanLyQuanCaPhe.BUS;
using QuanLyQuanCaPhe.DAL;
using QuanLyQuanCaPhe.GUI;

namespace QuanLyQuanCaPhe
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var culture = CultureInfo.GetCultureInfo("vi-VN");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            bool selfTest = false;
            foreach (string arg in args)
            {
                if (string.Equals(arg, "--selftest", StringComparison.OrdinalIgnoreCase))
                {
                    selfTest = true;
                    break;
                }
            }

            if (selfTest)
            {
                RunSelfTest();
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ThreadException += delegate(object sender, ThreadExceptionEventArgs e)
            {
                MessageBox.Show("Đã xảy ra lỗi không mong muốn:\r\n" + e.Exception.Message,
                    "Quản lý quán cà phê T1", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            while (true)
            {
                using (var login = new GUI.Forms.frmDangNhap())
                {
                    if (login.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                }

                using (var main = new GUI.Forms.frmMain())
                {
                    Application.Run(main);
                    if (!main.DangXuat)
                    {
                        return;
                    }
                }

                Session.Clear();
            }
        }

        private static void RunSelfTest()
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "selftest.log");
            var lines = new List<string>();
            try
            {
                lines.Add("Bắt đầu kiểm tra tự động: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                DatabaseInitializer.EnsureCreated();
                lines.Add("Server: " + DatabaseHelper.Server);
                lines.Add("Database: " + DatabaseHelper.DatabaseName);

                lines.Add("VaiTro: " + Count("SELECT COUNT(*) FROM dbo.VaiTro"));
                lines.Add("NhanVien: " + Count("SELECT COUNT(*) FROM dbo.NhanVien"));
                lines.Add("KhuVuc: " + Count("SELECT COUNT(*) FROM dbo.KhuVuc"));
                lines.Add("Ban: " + Count("SELECT COUNT(*) FROM dbo.Ban"));
                lines.Add("LoaiMon: " + Count("SELECT COUNT(*) FROM dbo.LoaiMon"));
                lines.Add("Mon: " + Count("SELECT COUNT(*) FROM dbo.Mon"));
                lines.Add("HoaDon: " + Count("SELECT COUNT(*) FROM dbo.HoaDon"));
                lines.Add("ChiTietHoaDon: " + Count("SELECT COUNT(*) FROM dbo.ChiTietHoaDon"));

                int login = TaiKhoanBUS.DangNhap("admin", "123456") != null ? 1 : 0;
                lines.Add("Đăng nhập admin/123456: " + (login == 1 ? "OK" : "THẤT BẠI"));

                int maBan = Convert.ToInt32(DatabaseHelper.Scalar(
                    "SELECT TOP 1 MaBan FROM dbo.Ban WHERE TrangThai = N'Trống' ORDER BY MaBan"));
                int maMon = Convert.ToInt32(DatabaseHelper.Scalar(
                    "SELECT TOP 1 MaMon FROM dbo.Mon WHERE TrangThai = 1 ORDER BY MaMon"));
                int maNV = Convert.ToInt32(DatabaseHelper.Scalar(
                    "SELECT TOP 1 MaNV FROM dbo.NhanVien WHERE MaVaiTro = 1 ORDER BY MaNV"));

                int maHD = BanHangBUS.GetOrCreateInvoice(maBan, maNV);
                BanHangBUS.AddItem(maHD, maMon);
                BanHangBUS.AddItem(maHD, maMon);
                decimal total = BanHangBUS.GetTotal(maHD);
                lines.Add("Tạo hóa đơn thử: " + Utils.InvoiceCode(maHD) + ", tổng tiền: " + Utils.Money(total));
                BanHangBUS.PayInvoice(maHD, 0m, "Tiền mặt", "Kiểm tra tự động");

                string status = Convert.ToString(DatabaseHelper.Scalar(
                    "SELECT TrangThai FROM dbo.HoaDon WHERE MaHD = @MaHD",
                    DatabaseHelper.P("@MaHD", maHD)));
                string tableStatus = Convert.ToString(DatabaseHelper.Scalar(
                    "SELECT TrangThai FROM dbo.Ban WHERE MaBan = @MaBan",
                    DatabaseHelper.P("@MaBan", maBan)));
                lines.Add("Thanh toán: " + status + " | Trạng thái bàn: " + tableStatus);

                if (status != "Đã thanh toán" || tableStatus != "Trống")
                {
                    throw new InvalidOperationException("Nghiệp vụ thanh toán chưa đúng.");
                }

                DatabaseHelper.Execute("DELETE FROM dbo.HoaDon WHERE MaHD = @MaHD",
                    DatabaseHelper.P("@MaHD", maHD));
                DatabaseHelper.Execute("UPDATE dbo.Ban SET TrangThai = N'Trống' WHERE MaBan = @MaBan",
                    DatabaseHelper.P("@MaBan", maBan));

                lines.Add("KẾT QUẢ: OK");
                File.WriteAllLines(logPath, lines);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                lines.Add("KẾT QUẢ: LỖI");
                lines.Add(ex.ToString());
                try
                {
                    File.WriteAllLines(logPath, lines);
                }
                catch
                {
                    // Không ghi được log thì vẫn thoát với mã lỗi.
                }
                Environment.Exit(1);
            }
        }

        private static int Count(string sql)
        {
            return Convert.ToInt32(DatabaseHelper.Scalar(sql));
        }
    }
}
