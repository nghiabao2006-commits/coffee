using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using QuanLyQuanCaPhe.Data;
using QuanLyQuanCaPhe.Helpers;

namespace QuanLyQuanCaPhe.Printing
{
    /// <summary>In hóa đơn thanh toán hoặc phiếu tạm tính.</summary>
    public class InvoicePrinter
    {
        private readonly PrintDocument _document;
        private readonly DataTable _header;
        private readonly DataTable _details;
        private readonly bool _temporary;
        private int _rowIndex;
        private int _pageNumber;

        private string _shopName = "Quán cà phê T1";
        private string _address = "123 Đường Cà Phê, Quận 1";
        private string _phone = "028 1234 5678";
        private string _thanks = "Cảm ơn quý khách và hẹn gặp lại!";

        public InvoicePrinter(int maHD, bool temporary)
        {
            _header = OrderService.GetInvoiceForPrint(maHD);
            _details = OrderService.GetDetails(maHD);
            _temporary = temporary || Row.Str(GetHeaderRow(), "TrangThai") != "Đã thanh toán";

            if (_header.Rows.Count == 0)
            {
                throw new InvalidOperationException("Không tìm thấy hóa đơn để in.");
            }

            LoadShopInfo();

            _document = new PrintDocument();
            _document.DocumentName = "HoaDon_" + Utils.InvoiceCode(maHD);
            _document.DefaultPageSettings.Margins = new Margins(50, 50, 50, 50);
            _document.BeginPrint += delegate
            {
                _rowIndex = 0;
                _pageNumber = 0;
            };
            _document.PrintPage += PrintPage;
        }

        public void ShowPreview(IWin32Window owner)
        {
            try
            {
                using (var dialog = new PrintPreviewDialog())
                {
                    dialog.Document = _document;
                    dialog.UseAntiAlias = true;
                    dialog.Width = 1000;
                    dialog.Height = 760;
                    dialog.StartPosition = FormStartPosition.CenterParent;
                    dialog.Text = "Xem trước hóa đơn";
                    dialog.ShowDialog(owner);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không mở được chế độ xem trước khi in. Chi tiết: " + ex.Message,
                    "Lỗi in hóa đơn", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataRow GetHeaderRow()
        {
            return _header.Rows.Count > 0 ? _header.Rows[0] : null;
        }

        private void LoadShopInfo()
        {
            try
            {
                DataTable settings = DatabaseHelper.Query("SELECT Khoa, GiaTri FROM dbo.CaiDat");
                foreach (DataRow row in settings.Rows)
                {
                    string key = Row.Str(row, "Khoa");
                    string value = Row.Str(row, "GiaTri");
                    if (key == "TenQuan" && value.Length > 0) _shopName = value;
                    else if (key == "DiaChi" && value.Length > 0) _address = value;
                    else if (key == "DienThoai" && value.Length > 0) _phone = value;
                    else if (key == "LoiChao" && value.Length > 0) _thanks = value;
                }
            }
            catch
            {
                // Dùng thông tin mặc định nếu không đọc được bảng CaiDat.
            }
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            _pageNumber++;
            Graphics graphics = e.Graphics;
            Rectangle bounds = e.MarginBounds;
            float left = bounds.Left;
            float width = bounds.Width;
            float y = bounds.Top;

            using (var brandFont = new Font("Segoe UI", 17f, FontStyle.Bold))
            using (var titleFont = new Font("Segoe UI", 13f, FontStyle.Bold))
            using (var normalFont = new Font("Segoe UI", 9f))
            using (var boldFont = new Font("Segoe UI", 9f, FontStyle.Bold))
            using (var smallFont = new Font("Segoe UI", 8f))
            using (var totalFont = new Font("Segoe UI", 12f, FontStyle.Bold))
            using (var darkBrush = new SolidBrush(Color.FromArgb(40, 32, 28)))
            using (var mutedBrush = new SolidBrush(Color.FromArgb(100, 92, 86)))
            using (var linePen = new Pen(Color.FromArgb(150, 140, 130), 0.8f))
            using (var lightBrush = new SolidBrush(Color.FromArgb(247, 243, 236)))
            {
                DrawCentered(graphics, _shopName.ToUpperInvariant(), brandFont, darkBrush, left, y, width);
                y += 26;
                DrawCentered(graphics, _address + "  •  ĐT: " + _phone, smallFont, mutedBrush, left, y, width);
                y += 18;

                string title = _temporary ? "PHIẾU TẠM TÍNH" : "HÓA ĐƠN THANH TOÁN";
                DrawCentered(graphics, title, titleFont, darkBrush, left, y, width);
                y += 28;

                if (_pageNumber > 1)
                {
                    DrawCentered(graphics, "(Trang " + _pageNumber + ")", smallFont, mutedBrush, left, y, width);
                    y += 16;
                }

                DataRow header = GetHeaderRow();
                string code = Utils.InvoiceCode(Row.Int(header, "MaHD"));
                string table = Row.Str(header, "TenBan");
                string area = Row.Str(header, "TenKhuVuc");
                string staff = Row.Str(header, "TenNV");
                DateTime? date = Row.Date(header, "NgayHienThi");
                string payment = Row.Str(header, "PhuongThucThanhToan");
                string status = Row.Str(header, "TrangThai");

                graphics.DrawString("Mã hóa đơn: " + code, boldFont, darkBrush, left, y);
                graphics.DrawString("Thời gian: " + Utils.DateTimeText(date), normalFont, darkBrush, left + width * 0.46f, y);
                y += 19;
                graphics.DrawString("Bàn: " + table + " (" + area + ")", normalFont, darkBrush, left, y);
                graphics.DrawString("Nhân viên: " + staff, normalFont, darkBrush, left + width * 0.46f, y);
                y += 19;
                if (!_temporary && payment.Length > 0)
                {
                    graphics.DrawString("Thanh toán: " + payment, normalFont, darkBrush, left, y);
                }
                graphics.DrawString("Trạng thái: " + status, normalFont, darkBrush, left + width * 0.46f, y);
                y += 24;

                float[] ratios = { 0.07f, 0.45f, 0.09f, 0.18f, 0.21f };
                string[] headers = { "STT", "Tên món", "SL", "Đơn giá", "Thành tiền" };
                float[] widths = new float[ratios.Length];
                float[] xs = new float[ratios.Length];
                float cursor = left;
                for (int i = 0; i < ratios.Length; i++)
                {
                    widths[i] = width * ratios[i];
                    xs[i] = cursor;
                    cursor += widths[i];
                }

                float rowHeight = 24f;
                graphics.FillRectangle(lightBrush, left, y, width, rowHeight);
                for (int i = 0; i < headers.Length; i++)
                {
                    var align = i == 1 ? StringAlignment.Near : (i == 0 ? StringAlignment.Center : StringAlignment.Far);
                    DrawCell(graphics, headers[i], boldFont, darkBrush,
                        new RectangleF(xs[i], y, widths[i], rowHeight), align);
                }
                graphics.DrawLine(linePen, left, y + rowHeight, left + width, y + rowHeight);
                y += rowHeight;

                int stt = 0;
                while (_rowIndex < _details.Rows.Count)
                {
                    if (y + rowHeight + 150 > bounds.Bottom)
                    {
                        e.HasMorePages = true;
                        return;
                    }

                    DataRow row = _details.Rows[_rowIndex];
                    stt++;
                    string name = Row.Str(row, "TenMon");
                    int quantity = Row.Int(row, "SoLuong");
                    decimal price = Row.Dec(row, "DonGia");
                    decimal amount = Row.Dec(row, "ThanhTien");

                    DrawCell(graphics, stt.ToString(), normalFont, darkBrush,
                        new RectangleF(xs[0], y, widths[0], rowHeight), StringAlignment.Center);
                    DrawCell(graphics, name, normalFont, darkBrush,
                        new RectangleF(xs[1], y, widths[1], rowHeight), StringAlignment.Near);
                    DrawCell(graphics, quantity.ToString(), normalFont, darkBrush,
                        new RectangleF(xs[2], y, widths[2], rowHeight), StringAlignment.Center);
                    DrawCell(graphics, price.ToString("N0"), normalFont, darkBrush,
                        new RectangleF(xs[3], y, widths[3], rowHeight), StringAlignment.Far);
                    DrawCell(graphics, amount.ToString("N0"), normalFont, darkBrush,
                        new RectangleF(xs[4], y, widths[4], rowHeight), StringAlignment.Far);

                    graphics.DrawLine(new Pen(Color.FromArgb(238, 232, 224), 0.5f),
                        left, y + rowHeight, left + width, y + rowHeight);
                    y += rowHeight;
                    _rowIndex++;
                }

                if (y + 140 > bounds.Bottom)
                {
                    e.HasMorePages = true;
                    return;
                }

                decimal total = Row.Dec(header, "TongTien");
                decimal discount = Row.Dec(header, "GiamGia");
                decimal final = Row.Dec(header, "ThanhTien");
                if (_details.Rows.Count > 0)
                {
                    decimal sum = 0m;
                    foreach (DataRow row in _details.Rows)
                    {
                        sum += Row.Dec(row, "ThanhTien");
                    }
                    total = sum;
                    final = total - discount;
                }

                y += 12;
                float labelX = left + width * 0.58f;
                graphics.DrawString("Tổng tiền:", normalFont, darkBrush, labelX, y);
                DrawRight(graphics, total.ToString("N0") + " đ", normalFont, darkBrush, left, y, width);
                y += 20;
                graphics.DrawString("Giảm giá:", normalFont, darkBrush, labelX, y);
                DrawRight(graphics, discount.ToString("N0") + " đ", normalFont, darkBrush, left, y, width);
                y += 24;
                graphics.DrawLine(linePen, left + width * 0.56f, y, left + width, y);
                y += 8;
                graphics.DrawString("THANH TOÁN:", totalFont, darkBrush, labelX, y - 2);
                DrawRight(graphics, final.ToString("N0") + " đ", totalFont, darkBrush, left, y - 2, width);
                y += 38;

                if (_temporary)
                {
                    DrawCentered(graphics, "Phiếu tạm tính – hóa đơn chưa thanh toán", smallFont, mutedBrush, left, y, width);
                    y += 18;
                }

                if (Row.Str(header, "GhiChu").Length > 0)
                {
                    graphics.DrawString("Ghi chú: " + Row.Str(header, "GhiChu"), smallFont, mutedBrush, left, y);
                    y += 18;
                }

                graphics.DrawLine(linePen, left, y, left + width, y);
                y += 10;
                DrawCentered(graphics, _thanks, normalFont, darkBrush, left, y, width);
                y += 18;
                DrawCentered(graphics, "In lúc: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), smallFont, mutedBrush, left, y, width);
            }

            e.HasMorePages = false;
        }

        private static void DrawCentered(Graphics graphics, string text, Font font, Brush brush,
            float left, float y, float width)
        {
            using (var format = new StringFormat { Alignment = StringAlignment.Center })
            {
                graphics.DrawString(text, font, brush, new RectangleF(left, y, width, font.Height + 4), format);
            }
        }

        private static void DrawRight(Graphics graphics, string text, Font font, Brush brush,
            float left, float y, float width)
        {
            using (var format = new StringFormat { Alignment = StringAlignment.Far })
            {
                graphics.DrawString(text, font, brush, new RectangleF(left, y, width, font.Height + 4), format);
            }
        }

        private static void DrawCell(Graphics graphics, string text, Font font, Brush brush,
            RectangleF rectangle, StringAlignment alignment)
        {
            using (var format = new StringFormat
            {
                Alignment = alignment,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap
            })
            {
                rectangle.X += 3;
                rectangle.Width -= 6;
                graphics.DrawString(text, font, brush, rectangle, format);
            }
        }
    }
}
