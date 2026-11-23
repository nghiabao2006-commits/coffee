using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyQuanCaPhe.BUS;
using QuanLyQuanCaPhe.GUI.Controls;

namespace QuanLyQuanCaPhe.GUI.Forms
{
    public class frmThongKe : Form
    {
        private DateTimePicker _dtTuNgay;
        private DateTimePicker _dtDenNgay;
        private ComboBox _cboNhanh;
        private Label _lblDoanhThu;
        private Label _lblSoHD;
        private Label _lblSoMon;
        private Label _lblTrungBinh;
        private BarChart _chart;
        private UiGrid _gridTopMon;
        private UiGrid _gridThanhToan;
        private Label _lblKyBaoCao;
        private bool _dangKhoiTao;

        public frmThongKe()
        {
            BackColor = Theme.Background;
            Font = Theme.Font(10f);
            _dangKhoiTao = true;
            BuildUi();
            _dangKhoiTao = false;
            Load += delegate { LoadThongKe(); };
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
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 100f));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 106f));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            root.Controls.Add(BuildFilterCard(), 0, 0);
            root.Controls.Add(BuildStatsRow(), 0, 1);
            root.Controls.Add(BuildContent(), 0, 2);
            Controls.Add(root);
        }

        private Control BuildFilterCard()
        {
            Panel card = Theme.CreateCard();
            card.Dock = DockStyle.Fill;
            card.Margin = new Padding(6);
            card.Padding = new Padding(12);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 9,
                RowCount = 1,
                BackColor = Color.White,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 74f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 116f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 76f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 116f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 88f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 154f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 124f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 12f));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            var lblTu = Theme.CreateFieldLabel("Từ ngày");
            lblTu.Dock = DockStyle.Fill;
            lblTu.TextAlign = ContentAlignment.MiddleLeft;

            _dtTuNgay = Theme.CreateDatePicker();
            _dtTuNgay.Value = DateTime.Today.AddDays(-6);
            _dtTuNgay.Dock = DockStyle.Fill;
            _dtTuNgay.Margin = new Padding(0, 12, 6, 12);

            var lblDen = Theme.CreateFieldLabel("Đến ngày");
            lblDen.Dock = DockStyle.Fill;
            lblDen.TextAlign = ContentAlignment.MiddleLeft;

            _dtDenNgay = Theme.CreateDatePicker();
            _dtDenNgay.Value = DateTime.Today;
            _dtDenNgay.Dock = DockStyle.Fill;
            _dtDenNgay.Margin = new Padding(0, 12, 6, 12);

            var lblNhanh = Theme.CreateFieldLabel("Xem nhanh");
            lblNhanh.Dock = DockStyle.Fill;
            lblNhanh.TextAlign = ContentAlignment.MiddleLeft;

            _cboNhanh = Theme.CreateComboBox();
            _cboNhanh.DropDownStyle = ComboBoxStyle.DropDownList;
            _cboNhanh.Items.Add("Tùy chọn");
            _cboNhanh.Items.Add("Hôm nay");
            _cboNhanh.Items.Add("7 ngày gần nhất");
            _cboNhanh.Items.Add("30 ngày gần nhất");
            _cboNhanh.Items.Add("Tháng này");
            _cboNhanh.SelectedIndex = 2;
            _cboNhanh.Dock = DockStyle.Fill;
            _cboNhanh.Margin = new Padding(0, 12, 6, 12);
            _cboNhanh.SelectedIndexChanged += delegate
            {
                if (_dangKhoiTao)
                {
                    return;
                }
                ApDungNhanh();
            };

            var btnXem = Theme.CreateButton("Xem thống kê", true);
            btnXem.Dock = DockStyle.Fill;
            btnXem.Margin = new Padding(0, 12, 6, 12);
            btnXem.Click += delegate { LoadThongKe(); };

            layout.Controls.Add(lblTu, 0, 0);
            layout.Controls.Add(_dtTuNgay, 1, 0);
            layout.Controls.Add(lblDen, 2, 0);
            layout.Controls.Add(_dtDenNgay, 3, 0);
            layout.Controls.Add(lblNhanh, 4, 0);
            layout.Controls.Add(_cboNhanh, 5, 0);
            layout.Controls.Add(btnXem, 7, 0);
            card.Controls.Add(layout);
            return card;
        }

        private void ApDungNhanh()
        {
            string luaChon = Convert.ToString(_cboNhanh.SelectedItem);
            DateTime today = DateTime.Today;
            switch (luaChon)
            {
                case "Hôm nay":
                    _dtTuNgay.Value = today;
                    _dtDenNgay.Value = today;
                    break;
                case "7 ngày gần nhất":
                    _dtTuNgay.Value = today.AddDays(-6);
                    _dtDenNgay.Value = today;
                    break;
                case "30 ngày gần nhất":
                    _dtTuNgay.Value = today.AddDays(-29);
                    _dtDenNgay.Value = today;
                    break;
                case "Tháng này":
                    _dtTuNgay.Value = new DateTime(today.Year, today.Month, 1);
                    _dtDenNgay.Value = today;
                    break;
                default:
                    return;
            }
            LoadThongKe();
        }

        private Control BuildStatsRow()
        {
            var row = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                BackColor = Theme.Background,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            for (int i = 0; i < 4; i++)
            {
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            }
            row.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            _lblDoanhThu = AddStatCard(row, 0, "Doanh thu", "Trong khoảng đã chọn", Theme.AccentDark);
            _lblSoHD = AddStatCard(row, 1, "Hóa đơn", "Đã thanh toán", Theme.Text);
            _lblSoMon = AddStatCard(row, 2, "Món bán ra", "Tổng số lượng", Theme.Text);
            _lblTrungBinh = AddStatCard(row, 3, "Trung bình hóa đơn", "Doanh thu / hóa đơn", Theme.Text);
            return row;
        }

        private static Label AddStatCard(TableLayoutPanel row, int column, string title, string caption, Color valueColor)
        {
            Panel card = Theme.CreateCard();
            card.Dock = DockStyle.Fill;
            card.Margin = new Padding(6, 4, 6, 6);
            card.Padding = new Padding(16, 10, 16, 10);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = Color.White,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 30f));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 45f));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 25f));

            var lblTitle = Theme.CreateBoldLabel(title, 9.5f, Theme.Muted);
            lblTitle.Dock = DockStyle.Fill;
            lblTitle.TextAlign = ContentAlignment.BottomLeft;

            var lblValue = Theme.CreateBoldLabel("0", 16f, valueColor);
            lblValue.Dock = DockStyle.Fill;
            lblValue.TextAlign = ContentAlignment.MiddleLeft;

            var lblCaption = Theme.CreateLabel(caption, 8.5f, Theme.Muted);
            lblCaption.Dock = DockStyle.Fill;
            lblCaption.TextAlign = ContentAlignment.TopLeft;

            layout.Controls.Add(lblTitle, 0, 0);
            layout.Controls.Add(lblValue, 0, 1);
            layout.Controls.Add(lblCaption, 0, 2);
            card.Controls.Add(layout);

            row.Controls.Add(card, column, 0);
            return lblValue;
        }

        private Control BuildContent()
        {
            var content = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Theme.Background,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60f));
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40f));
            content.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            Panel chartCard = Theme.CreateCard();
            chartCard.Dock = DockStyle.Fill;
            chartCard.Margin = new Padding(6);
            chartCard.Padding = new Padding(14);

            var chartLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.White,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            chartLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            chartLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44f));
            chartLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            var chartHeader = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.White,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            chartHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60f));
            chartHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40f));
            chartHeader.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            var chartTitle = Theme.CreateBoldLabel("Doanh thu theo ngày", 12f, Theme.Text);
            chartTitle.Dock = DockStyle.Fill;
            chartTitle.TextAlign = ContentAlignment.MiddleLeft;

            _lblKyBaoCao = Theme.CreateLabel(string.Empty, 8.5f, Theme.Muted);
            _lblKyBaoCao.Dock = DockStyle.Fill;
            _lblKyBaoCao.TextAlign = ContentAlignment.MiddleRight;

            chartHeader.Controls.Add(chartTitle, 0, 0);
            chartHeader.Controls.Add(_lblKyBaoCao, 1, 0);

            _chart = new BarChart { Dock = DockStyle.Fill };
            chartLayout.Controls.Add(chartHeader, 0, 0);
            chartLayout.Controls.Add(_chart, 0, 1);
            chartCard.Controls.Add(chartLayout);

            var right = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Theme.Background,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            right.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            right.RowStyles.Add(new RowStyle(SizeType.Percent, 58f));
            right.RowStyles.Add(new RowStyle(SizeType.Percent, 42f));
            right.Controls.Add(BuildTopMonCard(), 0, 0);
            right.Controls.Add(BuildThanhToanCard(), 0, 1);

            content.Controls.Add(chartCard, 0, 0);
            content.Controls.Add(right, 1, 0);
            return content;
        }

        private Control BuildTopMonCard()
        {
            Panel card = Theme.CreateCard();
            card.Dock = DockStyle.Fill;
            card.Margin = new Padding(6);
            card.Padding = new Padding(14);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.White,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            var title = Theme.CreateBoldLabel("Món bán chạy", 12f, Theme.Text);
            title.Dock = DockStyle.Fill;
            title.TextAlign = ContentAlignment.MiddleLeft;

            _gridTopMon = new UiGrid { Dock = DockStyle.Fill };
            Theme.StyleGrid(_gridTopMon);
            _gridTopMon.AutoGenerateColumns = false;
            _gridTopMon.ReadOnly = true;
            AddColumn(_gridTopMon, "TenMon", "Tên món", 48, true);
            AddColumn(_gridTopMon, "SoLuong", "SL", 14, true);
            AddColumn(_gridTopMon, "DoanhThu", "Doanh thu", 38, true);
            _gridTopMon.Columns["SoLuong"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _gridTopMon.Columns["DoanhThu"].DefaultCellStyle.Format = "N0";
            _gridTopMon.Columns["DoanhThu"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            layout.Controls.Add(title, 0, 0);
            layout.Controls.Add(_gridTopMon, 0, 1);
            card.Controls.Add(layout);
            return card;
        }

        private Control BuildThanhToanCard()
        {
            Panel card = Theme.CreateCard();
            card.Dock = DockStyle.Fill;
            card.Margin = new Padding(6);
            card.Padding = new Padding(14);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.White,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            var title = Theme.CreateBoldLabel("Hình thức thanh toán", 12f, Theme.Text);
            title.Dock = DockStyle.Fill;
            title.TextAlign = ContentAlignment.MiddleLeft;

            _gridThanhToan = new UiGrid { Dock = DockStyle.Fill };
            Theme.StyleGrid(_gridThanhToan);
            _gridThanhToan.AutoGenerateColumns = false;
            _gridThanhToan.ReadOnly = true;
            AddColumn(_gridThanhToan, "PhuongThuc", "Hình thức", 44, true);
            AddColumn(_gridThanhToan, "SoHD", "Số HĐ", 18, true);
            AddColumn(_gridThanhToan, "DoanhThu", "Doanh thu", 38, true);
            _gridThanhToan.Columns["SoHD"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _gridThanhToan.Columns["DoanhThu"].DefaultCellStyle.Format = "N0";
            _gridThanhToan.Columns["DoanhThu"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            layout.Controls.Add(title, 0, 0);
            layout.Controls.Add(_gridThanhToan, 0, 1);
            card.Controls.Add(layout);
            return card;
        }

        private static void AddColumn(DataGridView grid, string property, string header, int weight, bool visible)
        {
            var column = new DataGridViewTextBoxColumn
            {
                Name = property,
                DataPropertyName = property,
                HeaderText = header,
                FillWeight = weight > 0 ? weight : 1,
                Visible = visible,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            grid.Columns.Add(column);
        }

        private void LoadThongKe()
        {
            if (_dtDenNgay.Value.Date < _dtTuNgay.Value.Date)
            {
                MessageBox.Show("Đến ngày phải lớn hơn hoặc bằng từ ngày.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DateTime tu = _dtTuNgay.Value.Date;
            DateTime den = _dtDenNgay.Value.Date.AddDays(1);

            try
            {
                DataRow summary = ThongKeBUS.TongQuan(tu, den);

                int soHD = Row.Int(summary, "SoHD");
                decimal doanhThu = Row.Dec(summary, "DoanhThu");

                int soMon = ThongKeBUS.TongSoLuongMon(tu, den);

                _lblDoanhThu.Text = Utils.Money(doanhThu);
                _lblSoHD.Text = soHD.ToString("N0");
                _lblSoMon.Text = soMon.ToString("N0");
                _lblTrungBinh.Text = Utils.Money(soHD == 0 ? 0m : doanhThu / soHD);
                _lblKyBaoCao.Text = "Từ " + tu.ToString("dd/MM/yyyy") + " đến " +
                                    _dtDenNgay.Value.Date.ToString("dd/MM/yyyy");

                LoadChart(tu, den);
                LoadTopMon(tu, den);
                LoadThanhToan(tu, den);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tải được dữ liệu thống kê. Chi tiết: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadChart(DateTime tu, DateTime den)
        {
            DataTable table = ThongKeBUS.DoanhThuTheoNgay(tu, den);

            var map = new Dictionary<DateTime, decimal>();
            foreach (DataRow row in table.Rows)
            {
                DateTime ngay = Convert.ToDateTime(row["Ngay"]).Date;
                map[ngay] = Convert.ToDecimal(row["DoanhThu"]);
            }

            var items = new List<BarChart.BarItem>();
            int totalDays = (den - tu).Days;
            for (int i = 0; i < totalDays; i++)
            {
                DateTime ngay = tu.AddDays(i);
                decimal value;
                map.TryGetValue(ngay, out value);
                items.Add(new BarChart.BarItem(ngay.ToString("dd/MM"), value));
            }
            _chart.SetData(items);
        }

        private void LoadTopMon(DateTime tu, DateTime den)
        {
            _gridTopMon.DataSource = ThongKeBUS.TopMon(tu, den, 8);
        }

        private void LoadThanhToan(DateTime tu, DateTime den)
        {
            _gridThanhToan.DataSource = ThongKeBUS.HinhThucThanhToan(tu, den);
        }
    }
}
