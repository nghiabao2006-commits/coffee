using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using QuanLyQuanCaPhe.BUS;
using QuanLyQuanCaPhe.GUI.Controls;

namespace QuanLyQuanCaPhe.GUI.Forms
{
    public class ucTrangChu : UserControl
    {
        public event Action<string> NavigateRequested;

        private Label _lblDoanhThu;
        private Label _lblHoaDon;
        private Label _lblBan;
        private Label _lblMonHot;
        private UiGrid _gridHoaDon;
        private UiGrid _gridTopMon;

        public ucTrangChu()
        {
            BackColor = Theme.Background;
            Font = Theme.Font(10f);
            BuildUi();
            Load += delegate { LoadData(); };
        }

        private void BuildUi()
        {
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = Theme.Background,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 78f));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 122f));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            root.Controls.Add(BuildWelcome(), 0, 0);

            var cards = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            for (int i = 0; i < 4; i++)
            {
                cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            }
            cards.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            cards.Controls.Add(BuildStatCard("Doanh thu hôm nay", "Hóa đơn đã thanh toán", Theme.Accent, out _lblDoanhThu, 18f), 0, 0);
            cards.Controls.Add(BuildStatCard("Hóa đơn hôm nay", "Số hóa đơn đã thanh toán", Theme.Success, out _lblHoaDon, 18f), 1, 0);
            cards.Controls.Add(BuildStatCard("Bàn đang sử dụng", "Trên tổng số bàn của quán", Theme.Warning, out _lblBan, 18f), 2, 0);
            cards.Controls.Add(BuildStatCard("Món bán chạy hôm nay", "Theo số lượng đã bán", Theme.Danger, out _lblMonHot, 14f), 3, 0);
            root.Controls.Add(cards, 0, 1);

            var middle = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            middle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58f));
            middle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42f));
            middle.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            middle.Controls.Add(BuildRecentCard(), 0, 0);
            middle.Controls.Add(BuildRightColumn(), 1, 0);
            root.Controls.Add(middle, 0, 2);

            Controls.Add(root);
        }

        private Control BuildWelcome()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Background };
            var lblHello = Theme.CreateBoldLabel("Xin chào, " + Session.TenNV + "!", 20f, Theme.Text);
            lblHello.Location = new Point(0, 6);

            string dateText = DateTime.Now.ToString("dddd, dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"));
            var lblSub = Theme.CreateLabel(
                "Hôm nay là " + dateText + "  •  Chúc bạn một ngày làm việc hiệu quả.", 9.5f, Theme.Muted);
            lblSub.Location = new Point(2, 42);

            var btnRefresh = Theme.CreateButton("Làm mới", false);
            btnRefresh.Width = 110;
            btnRefresh.Height = 34;
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefresh.Click += delegate { LoadData(); };
            panel.Resize += delegate
            {
                btnRefresh.Location = new Point(Math.Max(0, panel.Width - btnRefresh.Width), 6);
            };

            panel.Controls.Add(lblHello);
            panel.Controls.Add(lblSub);
            panel.Controls.Add(btnRefresh);
            return panel;
        }

        private Control BuildStatCard(string title, string note, Color accent, out Label valueLabel, float valueFont)
        {
            Panel card = Theme.CreateCard();
            card.Dock = DockStyle.Fill;
            card.Margin = new Padding(6);
            card.Padding = new Padding(20, 14, 14, 12);
            card.Tag = accent;
            card.Paint += delegate(object sender, PaintEventArgs e)
            {
                using (var brush = new SolidBrush((Color)((Panel)sender).Tag))
                {
                    e.Graphics.FillRectangle(brush, 0, 0, 5, card.Height);
                }
            };

            var inner = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = Color.White,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            inner.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            inner.RowStyles.Add(new RowStyle(SizeType.Absolute, 24f));
            inner.RowStyles.Add(new RowStyle(SizeType.Absolute, 38f));
            inner.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            var lblTitle = new Label
            {
                Text = title,
                Font = Theme.Font(9.5f),
                ForeColor = Theme.Muted,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.White
            };

            valueLabel = new Label
            {
                Text = "—",
                Font = Theme.Bold(valueFont),
                ForeColor = Theme.Text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true,
                BackColor = Color.White
            };

            var lblNote = new Label
            {
                Text = note,
                Font = Theme.Font(8.5f),
                ForeColor = Theme.Muted,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.White
            };

            inner.Controls.Add(lblTitle, 0, 0);
            inner.Controls.Add(valueLabel, 0, 1);
            inner.Controls.Add(lblNote, 0, 2);
            card.Controls.Add(inner);
            return card;
        }

        private Control BuildRecentCard()
        {
            Panel card = Theme.CreateCard();
            card.Dock = DockStyle.Fill;
            card.Margin = new Padding(6);
            card.Padding = new Padding(16);

            var title = Theme.CreateBoldLabel("Hóa đơn gần đây", 11f, Theme.Text);
            title.Dock = DockStyle.Top;
            title.Height = 30;

            _gridHoaDon = new UiGrid { Dock = DockStyle.Fill };
            Theme.StyleGrid(_gridHoaDon);
            _gridHoaDon.AutoGenerateColumns = false;
            AddTextColumn(_gridHoaDon, "MaHoaDon", "Mã HĐ", 15);
            AddTextColumn(_gridHoaDon, "ThoiGian", "Thời gian", 24);
            AddTextColumn(_gridHoaDon, "TenBan", "Bàn", 10);
            AddTextColumn(_gridHoaDon, "TenNV", "Nhân viên", 22);
            AddTextColumn(_gridHoaDon, "ThanhTien", "Thanh toán", 18);
            AddTextColumn(_gridHoaDon, "TrangThai", "Trạng thái", 16);
            _gridHoaDon.Columns["ThanhTien"].DefaultCellStyle.Format = "N0";
            _gridHoaDon.Columns["ThanhTien"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            _gridHoaDon.Columns["TrangThai"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _gridHoaDon.CellFormatting += Grid_CellFormatting;

            card.Controls.Add(_gridHoaDon);
            card.Controls.Add(title);
            return card;
        }

        private Control BuildRightColumn()
        {
            var column = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            column.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            column.RowStyles.Add(new RowStyle(SizeType.Percent, 54f));
            column.RowStyles.Add(new RowStyle(SizeType.Percent, 46f));
            column.Controls.Add(BuildTopItemCard(), 0, 0);
            column.Controls.Add(BuildQuickActions(), 0, 1);
            return column;
        }

        private Control BuildTopItemCard()
        {
            Panel card = Theme.CreateCard();
            card.Dock = DockStyle.Fill;
            card.Margin = new Padding(6);
            card.Padding = new Padding(16);

            var title = Theme.CreateBoldLabel("Món bán chạy hôm nay", 11f, Theme.Text);
            title.Dock = DockStyle.Top;
            title.Height = 30;

            _gridTopMon = new UiGrid { Dock = DockStyle.Fill };
            Theme.StyleGrid(_gridTopMon);
            _gridTopMon.AutoGenerateColumns = false;
            AddTextColumn(_gridTopMon, "TenMon", "Tên món", 60);
            AddTextColumn(_gridTopMon, "SoLuong", "SL", 15);
            AddTextColumn(_gridTopMon, "DoanhThu", "Doanh thu", 30);
            _gridTopMon.Columns["SoLuong"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _gridTopMon.Columns["DoanhThu"].DefaultCellStyle.Format = "N0";
            _gridTopMon.Columns["DoanhThu"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            card.Controls.Add(_gridTopMon);
            card.Controls.Add(title);
            return card;
        }

        private Control BuildQuickActions()
        {
            Panel card = Theme.CreateCard();
            card.Dock = DockStyle.Fill;
            card.Margin = new Padding(6);
            card.Padding = new Padding(16);

            var title = Theme.CreateBoldLabel("Thao tác nhanh", 11f, Theme.Text);
            title.Dock = DockStyle.Top;
            title.Height = 28;

            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.White,
                Padding = new Padding(0, 4, 0, 0)
            };

            var btnBanHang = Theme.CreateButton("Mở màn hình bán hàng", true);
            btnBanHang.Width = 260;
            btnBanHang.Margin = new Padding(0, 0, 0, 8);
            btnBanHang.Click += delegate
            {
                if (NavigateRequested != null) NavigateRequested("Bán hàng");
            };

            var btnHoaDon = Theme.CreateButton("Xem danh sách hóa đơn", false);
            btnHoaDon.Width = 260;
            btnHoaDon.Margin = new Padding(0, 0, 0, 8);
            btnHoaDon.Click += delegate
            {
                if (NavigateRequested != null) NavigateRequested("Hóa đơn");
            };

            var btnThongKe = Theme.CreateButton("Xem thống kê doanh thu", false);
            btnThongKe.Width = 260;
            btnThongKe.Visible = Session.IsQuanLy;
            btnThongKe.Click += delegate
            {
                if (NavigateRequested != null) NavigateRequested("Thống kê");
            };

            buttons.Controls.Add(btnBanHang);
            buttons.Controls.Add(btnHoaDon);
            buttons.Controls.Add(btnThongKe);

            card.Controls.Add(buttons);
            card.Controls.Add(title);
            return card;
        }

        private static void AddTextColumn(DataGridView grid, string property, string header, int weight)
        {
            var column = new DataGridViewTextBoxColumn
            {
                Name = property,
                DataPropertyName = property,
                HeaderText = header,
                FillWeight = weight,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            grid.Columns.Add(column);
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (_gridHoaDon.Columns[e.ColumnIndex].DataPropertyName != "TrangThai" || e.Value == null)
            {
                return;
            }

            string status = Convert.ToString(e.Value);
            if (status == "Đã thanh toán")
            {
                e.CellStyle.ForeColor = Theme.Success;
            }
            else if (status == "Chưa thanh toán")
            {
                e.CellStyle.ForeColor = Theme.Warning;
            }
            else if (status == "Đã hủy")
            {
                e.CellStyle.ForeColor = Theme.Danger;
            }
        }

        private void LoadData()
        {
            try
            {
                DateTime today = DateTime.Today;
                DateTime tomorrow = today.AddDays(1);

                decimal revenue = ThongKeBUS.DoanhThu(today, tomorrow);

                int invoiceCount = ThongKeBUS.SoHoaDon(today, tomorrow);

                int busyTables = ThongKeBUS.SoBanDangSuDung();
                int totalTables = ThongKeBUS.TongSoBan();

                string topItem = ThongKeBUS.MonBanChay(today, tomorrow);

                _lblDoanhThu.Text = Utils.Money(revenue);
                _lblHoaDon.Text = invoiceCount.ToString("N0", CultureInfo.GetCultureInfo("vi-VN"));
                _lblBan.Text = busyTables + " / " + totalTables;
                _lblMonHot.Text = topItem.Length == 0 ? "Chưa có dữ liệu" : topItem;

                DataTable recent = ThongKeBUS.HoaDonGanDay(6);
                _gridHoaDon.DataSource = recent;
                _gridHoaDon.Columns["ThoiGian"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";

                DataTable topItems = ThongKeBUS.TopMon(today, tomorrow, 5);
                _gridTopMon.DataSource = topItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tải được dữ liệu trang chủ. Chi tiết: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
