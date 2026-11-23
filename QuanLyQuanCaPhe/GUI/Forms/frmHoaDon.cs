using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyQuanCaPhe.BUS;
using QuanLyQuanCaPhe.DTO;
using QuanLyQuanCaPhe.GUI.Controls;
using QuanLyQuanCaPhe.GUI.Printing;

namespace QuanLyQuanCaPhe.GUI.Forms
{
    public class frmHoaDon : Form
    {
        private DateTimePicker _dtTuNgay;
        private DateTimePicker _dtDenNgay;
        private ComboBox _cboTrangThai;
        private TextBox _txtTimKiem;
        private UiGrid _grid;
        private UiGrid _gridChiTiet;
        private Label _lblDem;
        private Label _lblDoanhThu;
        private Label _lblThongTin;
        private Label _lblTongTien;
        private Label _lblGiamGia;
        private Label _lblThanhTien;
        private Button _btnIn;
        private Button _btnHuy;
        private int _maHD;
        private string _trangThaiHD;

        public frmHoaDon()
        {
            BackColor = Theme.Background;
            Font = Theme.Font(10f);
            BuildUi();
            Load += delegate { LoadHoaDon(); };
        }

        private void BuildUi()
        {
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Theme.Background,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 100f));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            Panel filterCard = Theme.CreateCard();
            filterCard.Dock = DockStyle.Fill;
            filterCard.Margin = new Padding(6);
            filterCard.Padding = new Padding(12);

            var filter = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 9,
                RowCount = 1,
                BackColor = Color.White,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            filter.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 76f));
            filter.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 116f));
            filter.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 78f));
            filter.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 116f));
            filter.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 82f));
            filter.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 128f));
            filter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            filter.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 92f));
            filter.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 92f));
            filter.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            var lblTu = Theme.CreateFieldLabel("Từ ngày");
            lblTu.Dock = DockStyle.Fill;
            lblTu.TextAlign = ContentAlignment.MiddleLeft;
            _dtTuNgay = Theme.CreateDatePicker();
            _dtTuNgay.Value = DateTime.Today.AddDays(-30);
            _dtTuNgay.Dock = DockStyle.Fill;
            _dtTuNgay.Margin = new Padding(0, 12, 6, 12);

            var lblDen = Theme.CreateFieldLabel("Đến ngày");
            lblDen.Dock = DockStyle.Fill;
            lblDen.TextAlign = ContentAlignment.MiddleLeft;
            _dtDenNgay = Theme.CreateDatePicker();
            _dtDenNgay.Value = DateTime.Today;
            _dtDenNgay.Dock = DockStyle.Fill;
            _dtDenNgay.Margin = new Padding(0, 12, 6, 12);

            var lblTrangThai = Theme.CreateFieldLabel("Trạng thái");
            lblTrangThai.Dock = DockStyle.Fill;
            lblTrangThai.TextAlign = ContentAlignment.MiddleLeft;
            _cboTrangThai = Theme.CreateComboBox();
            _cboTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
            _cboTrangThai.Items.Add("Tất cả");
            _cboTrangThai.Items.Add("Đã thanh toán");
            _cboTrangThai.Items.Add("Chưa thanh toán");
            _cboTrangThai.Items.Add("Đã hủy");
            _cboTrangThai.SelectedIndex = 0;
            _cboTrangThai.Dock = DockStyle.Fill;
            _cboTrangThai.Margin = new Padding(0, 12, 6, 12);

            _txtTimKiem = Theme.CreateTextBox();
            _txtTimKiem.Dock = DockStyle.Fill;
            _txtTimKiem.Margin = new Padding(0, 12, 6, 12);
            SetPlaceholder(_txtTimKiem, "Tìm theo mã hóa đơn, bàn, nhân viên...");
            _txtTimKiem.KeyDown += delegate(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadHoaDon();
                }
            };

            var btnTim = Theme.CreateButton("Tìm kiếm", true);
            btnTim.Dock = DockStyle.Fill;
            btnTim.Margin = new Padding(0, 12, 4, 12);
            btnTim.Click += delegate { LoadHoaDon(); };

            var btnMoi = Theme.CreateButton("Làm mới", false);
            btnMoi.Dock = DockStyle.Fill;
            btnMoi.Margin = new Padding(4, 12, 0, 12);
            btnMoi.Click += delegate
            {
                _dtTuNgay.Value = DateTime.Today.AddDays(-30);
                _dtDenNgay.Value = DateTime.Today;
                _cboTrangThai.SelectedIndex = 0;
                _txtTimKiem.Text = _txtTimKiem.Tag as string ?? string.Empty;
                _txtTimKiem.ForeColor = Theme.Muted;
                LoadHoaDon();
            };

            filter.Controls.Add(lblTu, 0, 0);
            filter.Controls.Add(_dtTuNgay, 1, 0);
            filter.Controls.Add(lblDen, 2, 0);
            filter.Controls.Add(_dtDenNgay, 3, 0);
            filter.Controls.Add(lblTrangThai, 4, 0);
            filter.Controls.Add(_cboTrangThai, 5, 0);
            filter.Controls.Add(_txtTimKiem, 6, 0);
            filter.Controls.Add(btnTim, 7, 0);
            filter.Controls.Add(btnMoi, 8, 0);
            filterCard.Controls.Add(filter);

            var main = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Theme.Background,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60f));
            main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40f));
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            main.Controls.Add(BuildListCard(), 0, 0);
            main.Controls.Add(BuildDetailCard(), 1, 0);

            root.Controls.Add(filterCard, 0, 0);
            root.Controls.Add(main, 0, 1);
            Controls.Add(root);
        }

        private Control BuildListCard()
        {
            Panel card = Theme.CreateCard();
            card.Dock = DockStyle.Fill;
            card.Margin = new Padding(6);
            card.Padding = new Padding(14);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                BackColor = Color.White,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40f));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38f));

            var title = Theme.CreateBoldLabel("Danh sách hóa đơn", 12f, Theme.Text);
            title.Dock = DockStyle.Fill;
            title.TextAlign = ContentAlignment.MiddleLeft;

            _lblDem = Theme.CreateLabel("0 hóa đơn", 9f, Theme.Muted);
            _lblDem.Dock = DockStyle.Fill;
            _lblDem.TextAlign = ContentAlignment.MiddleRight;

            _grid = new UiGrid { Dock = DockStyle.Fill };
            Theme.StyleGrid(_grid);
            _grid.AutoGenerateColumns = false;
            AddColumn(_grid, "MaHD", string.Empty, 0, false);
            AddColumn(_grid, "MaHoaDon", "Mã HĐ", 13, true);
            AddColumn(_grid, "ThoiGian", "Thời gian", 19, true);
            AddColumn(_grid, "TenBan", "Bàn", 12, true);
            AddColumn(_grid, "TenNV", "Nhân viên", 20, true);
            AddColumn(_grid, "ThanhTien", "Thanh toán", 16, true);
            AddColumn(_grid, "TrangThai", "Trạng thái", 15, true);
            _grid.Columns["ThoiGian"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            _grid.Columns["ThanhTien"].DefaultCellStyle.Format = "N0";
            _grid.Columns["ThanhTien"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            _grid.Columns["TrangThai"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _grid.Columns["MaHoaDon"].MinimumWidth = 80;
            _grid.Columns["ThoiGian"].MinimumWidth = 138;
            _grid.Columns["TenBan"].MinimumWidth = 60;
            _grid.Columns["TenNV"].MinimumWidth = 140;
            _grid.Columns["ThanhTien"].MinimumWidth = 100;
            _grid.Columns["TrangThai"].MinimumWidth = 100;
            _grid.SelectionChanged += Grid_SelectionChanged;
            _grid.CellFormatting += Grid_CellFormatting;

            _lblDoanhThu = Theme.CreateBoldLabel("Doanh thu đã thanh toán: 0 đ", 9.5f, Theme.Text);
            _lblDoanhThu.Dock = DockStyle.Fill;
            _lblDoanhThu.TextAlign = ContentAlignment.MiddleLeft;

            layout.Controls.Add(title, 0, 0);
            layout.Controls.Add(_lblDem, 1, 0);
            layout.Controls.Add(_grid, 0, 1);
            layout.SetColumnSpan(_grid, 2);
            layout.Controls.Add(_lblDoanhThu, 0, 2);
            layout.SetColumnSpan(_lblDoanhThu, 2);
            card.Controls.Add(layout);
            return card;
        }

        private Control BuildDetailCard()
        {
            Panel card = Theme.CreateCard();
            card.Dock = DockStyle.Fill;
            card.Margin = new Padding(6);
            card.Padding = new Padding(14);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                BackColor = Color.White,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38f));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 104f));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 96f));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50f));

            var title = Theme.CreateBoldLabel("Chi tiết hóa đơn", 12f, Theme.Text);
            title.Dock = DockStyle.Fill;
            title.TextAlign = ContentAlignment.MiddleLeft;

            _lblThongTin = Theme.CreateLabel("Chọn một hóa đơn để xem chi tiết.", 9f, Theme.Muted);
            _lblThongTin.Dock = DockStyle.Fill;
            _lblThongTin.TextAlign = ContentAlignment.TopLeft;

            _gridChiTiet = new UiGrid { Dock = DockStyle.Fill };
            Theme.StyleGrid(_gridChiTiet);
            _gridChiTiet.AutoGenerateColumns = false;
            _gridChiTiet.ReadOnly = true;
            AddColumn(_gridChiTiet, "MaCT", string.Empty, 0, false);
            AddColumn(_gridChiTiet, "TenMon", "Tên món", 46, true);
            AddColumn(_gridChiTiet, "SoLuong", "SL", 12, true);
            AddColumn(_gridChiTiet, "DonGia", "Đơn giá", 20, true);
            AddColumn(_gridChiTiet, "ThanhTien", "Thành tiền", 22, true);
            _gridChiTiet.Columns["SoLuong"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _gridChiTiet.Columns["DonGia"].DefaultCellStyle.Format = "N0";
            _gridChiTiet.Columns["ThanhTien"].DefaultCellStyle.Format = "N0";
            _gridChiTiet.Columns["DonGia"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            _gridChiTiet.Columns["ThanhTien"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            var totals = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                BackColor = Color.White,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            totals.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            totals.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            for (int i = 0; i < 3; i++)
            {
                totals.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33f));
            }

            _lblTongTien = AddTotalRow(totals, 0, "Tổng tiền", Theme.Text);
            _lblGiamGia = AddTotalRow(totals, 1, "Giảm giá", Theme.Muted);
            _lblThanhTien = AddTotalRow(totals, 2, "Thanh toán", Theme.AccentDark);
            _lblThanhTien.Font = Theme.Bold(12f);

            var buttons = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.White,
                Margin = new Padding(0, 6, 0, 0),
                Padding = Padding.Empty
            };
            buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55f));
            buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45f));
            buttons.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            _btnIn = Theme.CreateButton("In hóa đơn", true);
            _btnIn.Dock = DockStyle.Fill;
            _btnIn.Margin = new Padding(0, 2, 4, 2);
            _btnIn.Enabled = false;
            _btnIn.Click += BtnIn_Click;

            _btnHuy = Theme.CreateDangerButton("Hủy đơn");
            _btnHuy.Dock = DockStyle.Fill;
            _btnHuy.Margin = new Padding(4, 2, 0, 2);
            _btnHuy.Enabled = false;
            _btnHuy.Click += BtnHuy_Click;

            buttons.Controls.Add(_btnIn, 0, 0);
            buttons.Controls.Add(_btnHuy, 1, 0);

            layout.Controls.Add(title, 0, 0);
            layout.Controls.Add(_lblThongTin, 0, 1);
            layout.Controls.Add(_gridChiTiet, 0, 2);
            layout.Controls.Add(totals, 0, 3);
            layout.Controls.Add(buttons, 0, 4);
            card.Controls.Add(layout);
            return card;
        }

        private static Label AddTotalRow(TableLayoutPanel totals, int row, string caption, Color color)
        {
            var label = Theme.CreateLabel(caption, 9.5f, Theme.Muted);
            label.Dock = DockStyle.Fill;
            label.TextAlign = ContentAlignment.MiddleLeft;

            var value = Theme.CreateBoldLabel("0 đ", 10f, color);
            value.Dock = DockStyle.Fill;
            value.TextAlign = ContentAlignment.MiddleRight;

            totals.Controls.Add(label, 0, row);
            totals.Controls.Add(value, 1, row);
            return value;
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

        private static void SetPlaceholder(TextBox textBox, string placeholder)
        {
            textBox.Tag = placeholder;
            textBox.Text = placeholder;
            textBox.ForeColor = Theme.Muted;
            textBox.GotFocus += delegate
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = string.Empty;
                    textBox.ForeColor = Theme.Text;
                }
            };
            textBox.LostFocus += delegate
            {
                if (textBox.Text.Trim().Length == 0)
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = Theme.Muted;
                }
            };
        }

        private string GetKeyword()
        {
            string text = _txtTimKiem.Text.Trim();
            return text == (_txtTimKiem.Tag as string ?? string.Empty) ? string.Empty : text;
        }

        private void LoadHoaDon()
        {
            try
            {
                string keyword = GetKeyword();
                string trangThai = _cboTrangThai.SelectedIndex > 0
                    ? _cboTrangThai.SelectedItem.ToString()
                    : null;

                DataTable table = HoaDonBUS.LayDanhSach(_dtTuNgay.Value.Date,
                    _dtDenNgay.Value.Date.AddDays(1), keyword, trangThai);
                _grid.DataSource = table;

                decimal revenue = 0m;
                int paid = 0;
                foreach (DataRow row in table.Rows)
                {
                    if (Row.Str(row, "TrangThai") == "Đã thanh toán")
                    {
                        revenue += Row.Dec(row, "ThanhTien");
                        paid++;
                    }
                }
                _lblDem.Text = table.Rows.Count + " hóa đơn";
                _lblDoanhThu.Text = "Doanh thu đã thanh toán (" + paid + " hóa đơn): " + Utils.Money(revenue);

                _maHD = 0;
                _trangThaiHD = null;
                _lblThongTin.Text = "Chọn một hóa đơn để xem chi tiết.";
                _gridChiTiet.DataSource = null;
                _lblTongTien.Text = "0 đ";
                _lblGiamGia.Text = "0 đ";
                _lblThanhTien.Text = "0 đ";
                _btnIn.Enabled = false;
                _btnHuy.Enabled = false;
                _grid.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tải được danh sách hóa đơn. Chi tiết: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Grid_SelectionChanged(object sender, EventArgs e)
        {
            if (_grid.CurrentRow == null)
            {
                return;
            }
            var view = _grid.CurrentRow.DataBoundItem as DataRowView;
            if (view == null)
            {
                return;
            }

            int maHD = Row.Int(view.Row, "MaHD");
            if (maHD == _maHD)
            {
                return;
            }

            try
            {
                _maHD = maHD;
                HoaDonDTO invoice = HoaDonBUS.GetInvoice(maHD);
                if (invoice == null)
                {
                    return;
                }

                _trangThaiHD = invoice.TrangThai;
                DateTime? thoiGian = invoice.NgayHienThi;
                _lblThongTin.Text =
                    "Mã hóa đơn: " + Utils.InvoiceCode(maHD) + "\r\n" +
                    "Bàn: " + invoice.TenBan + " • " + invoice.TenKhuVuc + "\r\n" +
                    "Nhân viên lập: " + invoice.TenNV + "\r\n" +
                    "Thời gian: " + Utils.DateTimeText(thoiGian) + "\r\n" +
                    "Trạng thái: " + _trangThaiHD +
                    (invoice.PhuongThucThanhToan.Length > 0
                        ? " • " + invoice.PhuongThucThanhToan
                        : string.Empty);

                _gridChiTiet.DataSource = HoaDonBUS.GetDetails(maHD);
                _lblTongTien.Text = Utils.Money(invoice.TongTien);
                _lblGiamGia.Text = Utils.Money(invoice.GiamGia);
                _lblThanhTien.Text = Utils.Money(invoice.ThanhTien);
                _btnIn.Enabled = true;
                _btnHuy.Enabled = _trangThaiHD == "Chưa thanh toán";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tải được chi tiết hóa đơn. Chi tiết: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            string column = _grid.Columns[e.ColumnIndex].Name;
            if (column != "TrangThai")
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

        private void BtnIn_Click(object sender, EventArgs e)
        {
            if (_maHD == 0)
            {
                return;
            }
            try
            {
                bool temporary = _trangThaiHD != "Đã thanh toán";
                new InvoicePrinter(_maHD, temporary).ShowPreview(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không in được hóa đơn. Chi tiết: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnHuy_Click(object sender, EventArgs e)
        {
            if (_maHD == 0)
            {
                return;
            }
            if (MessageBox.Show("Hủy hóa đơn " + Utils.InvoiceCode(_maHD) + "?\r\n" +
                                "Bàn sẽ được chuyển về trạng thái Trống.",
                    "Xác nhận hủy", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                HoaDonBUS.CancelInvoice(_maHD);
                LoadHoaDon();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không hủy được hóa đơn. Chi tiết: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
