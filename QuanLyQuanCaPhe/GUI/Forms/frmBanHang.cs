using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyQuanCaPhe.BUS;
using QuanLyQuanCaPhe.DTO;
using QuanLyQuanCaPhe.GUI.Controls;
using QuanLyQuanCaPhe.GUI.Printing;

namespace QuanLyQuanCaPhe.GUI.Forms
{
    public class frmBanHang : Form
    {
        private ComboBox _cboKhuVuc;
        private FlowLayoutPanel _flowBan;
        private FlowLayoutPanel _flowLoaiMon;
        private FlowLayoutPanel _flowMon;
        private TextBox _txtTimMon;
        private UiGrid _gridChiTiet;
        private Label _lblTenBan;
        private Label _lblThongTinBan;
        private Label _lblTrangThaiDon;
        private Label _lblTongTien;
        private Label _lblGiamGia;
        private Label _lblThanhToan;
        private NumericUpDown _numGiamGia;
        private ComboBox _cboThanhToan;
        private TextBox _txtGhiChu;
        private Button _btnThanhToan;
        private Button _btnTamTinh;
        private Button _btnHuyDon;
        private Button _btnTang;
        private Button _btnGiam;
        private Button _btnXoaMon;

        private DataTable _bang;
        private DataTable _thucDon;
        private DataTable _loaiMon;
        private List<ChiTietHoaDonDTO> _chiTiet;
        private ToolTip _toolTip;
        private int? _maBan;
        private int? _maHD;
        private int _maLoaiDangChon;

        public frmBanHang()
        {
            BackColor = Theme.Background;
            Font = Theme.Font(10f);
            _toolTip = new ToolTip();
            BuildUi();
            Load += delegate { KhoiTaoDuLieu(); };
        }

        private void BuildUi()
        {
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = Theme.Background,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 24f));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 44f));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32f));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            root.Controls.Add(BuildBanCard(), 0, 0);
            root.Controls.Add(BuildMenuCard(), 1, 0);
            root.Controls.Add(BuildOrderCard(), 2, 0);
            Controls.Add(root);
        }

        private Control BuildBanCard()
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
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28f));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34f));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24f));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28f));

            var title = Theme.CreateBoldLabel("Khu vực & Bàn", 11f, Theme.Text);
            title.Dock = DockStyle.Fill;

            _cboKhuVuc = Theme.CreateComboBox();
            _cboKhuVuc.Dock = DockStyle.Fill;
            _cboKhuVuc.Margin = new Padding(0, 0, 0, 6);

            var lblChonBan = Theme.CreateBoldLabel("Chọn bàn để gọi món", 8.5f, Theme.Muted);
            lblChonBan.Dock = DockStyle.Fill;

            _flowBan = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.White,
                Margin = new Padding(0, 4, 0, 4),
                Padding = new Padding(0)
            };

            var legend = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.White,
                Margin = Padding.Empty,
                Padding = new Padding(0, 4, 0, 0)
            };
            legend.Controls.Add(CreateLegendDot(Color.FromArgb(238, 230, 219)));
            legend.Controls.Add(Theme.CreateLabel("Trống", 8.5f, Theme.Muted));
            legend.Controls.Add(CreateLegendDot(Theme.Accent));
            legend.Controls.Add(Theme.CreateLabel("Đang dùng", 8.5f, Theme.Muted));

            layout.Controls.Add(title, 0, 0);
            layout.Controls.Add(_cboKhuVuc, 0, 1);
            layout.Controls.Add(lblChonBan, 0, 2);
            layout.Controls.Add(_flowBan, 0, 3);
            layout.Controls.Add(legend, 0, 4);

            card.Controls.Add(layout);
            return card;
        }

        private static Panel CreateLegendDot(Color color)
        {
            return new Panel
            {
                Width = 12,
                Height = 12,
                BackColor = color,
                Margin = new Padding(0, 6, 4, 0)
            };
        }

        private Control BuildMenuCard()
        {
            Panel card = Theme.CreateCard();
            card.Dock = DockStyle.Fill;
            card.Margin = new Padding(6);
            card.Padding = new Padding(14);

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
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34f));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 72f));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            var searchPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.White,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 66f));
            searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            var lblSearch = Theme.CreateBoldLabel("Tìm món", 9f, Theme.Muted);
            lblSearch.Dock = DockStyle.Fill;
            lblSearch.TextAlign = ContentAlignment.MiddleLeft;

            _txtTimMon = Theme.CreateTextBox();
            _txtTimMon.Dock = DockStyle.Fill;
            _txtTimMon.Margin = new Padding(0, 3, 0, 3);
            _txtTimMon.TextChanged += delegate { RenderMenu(); };

            searchPanel.Controls.Add(lblSearch, 0, 0);
            searchPanel.Controls.Add(_txtTimMon, 1, 0);

            _flowLoaiMon = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = false,
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.White,
                Margin = new Padding(0, 2, 0, 4),
                Padding = new Padding(0)
            };

            _flowMon = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.White,
                Margin = Padding.Empty,
                Padding = new Padding(0, 2, 0, 0)
            };

            layout.Controls.Add(searchPanel, 0, 0);
            layout.Controls.Add(_flowLoaiMon, 0, 1);
            layout.Controls.Add(_flowMon, 0, 2);
            card.Controls.Add(layout);
            return card;
        }

        private Control BuildOrderCard()
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
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58f));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36f));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 166f));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 102f));

            var header = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            _lblTenBan = Theme.CreateBoldLabel("Chưa chọn bàn", 13f, Theme.Text);
            _lblTenBan.Location = new Point(0, 0);
            _lblThongTinBan = Theme.CreateLabel("Chọn một bàn ở cột bên trái để bắt đầu.", 8.5f, Theme.Muted);
            _lblThongTinBan.Location = new Point(2, 30);
            _lblTrangThaiDon = new Label
            {
                Text = string.Empty,
                Font = Theme.Bold(8.5f),
                ForeColor = Theme.Muted,
                AutoSize = false,
                Size = new Size(110, 22),
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.White
            };
            header.Resize += delegate
            {
                _lblTrangThaiDon.Location = new Point(Math.Max(0, header.Width - _lblTrangThaiDon.Width), 4);
            };
            header.Controls.Add(_lblTenBan);
            header.Controls.Add(_lblThongTinBan);
            header.Controls.Add(_lblTrangThaiDon);

            _gridChiTiet = new UiGrid { Dock = DockStyle.Fill };
            Theme.StyleGrid(_gridChiTiet);
            _gridChiTiet.AutoGenerateColumns = false;
            _gridChiTiet.RowTemplate.Height = 32;
            _gridChiTiet.DefaultCellStyle.Font = Theme.Font(9f);
            _gridChiTiet.ColumnHeadersHeight = 32;
            AddColumn(_gridChiTiet, "MaCT", string.Empty, 0, false);
            AddColumn(_gridChiTiet, "TenMon", "Món", 44, true);
            AddColumn(_gridChiTiet, "SoLuong", "SL", 12, true);
            AddColumn(_gridChiTiet, "DonGia", "Đơn giá", 22, true);
            AddColumn(_gridChiTiet, "ThanhTien", "T.Tiền", 22, true);
            _gridChiTiet.Columns["SoLuong"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _gridChiTiet.Columns["DonGia"].DefaultCellStyle.Format = "N0";
            _gridChiTiet.Columns["DonGia"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            _gridChiTiet.Columns["ThanhTien"].DefaultCellStyle.Format = "N0";
            _gridChiTiet.Columns["ThanhTien"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            var actions = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.White,
                Margin = new Padding(0, 2, 0, 2),
                Padding = new Padding(0)
            };

            _btnGiam = Theme.CreateSmallButton("−", false);
            _btnGiam.Width = 38;
            _btnGiam.Margin = new Padding(0, 0, 4, 0);
            _btnGiam.Click += delegate { ChangeQuantity(-1); };

            _btnTang = Theme.CreateSmallButton("+", false);
            _btnTang.Width = 38;
            _btnTang.Margin = new Padding(0, 0, 4, 0);
            _btnTang.Click += delegate { ChangeQuantity(1); };

            _btnXoaMon = Theme.CreateSmallButton("Xóa món", false);
            _btnXoaMon.Width = 88;
            _btnXoaMon.Click += BtnXoaMon_Click;

            actions.Controls.Add(_btnGiam);
            actions.Controls.Add(_btnTang);
            actions.Controls.Add(_btnXoaMon);

            var totals = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 5,
                BackColor = Color.White,
                Margin = Padding.Empty,
                Padding = new Padding(0, 2, 0, 2)
            };
            totals.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42f));
            totals.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58f));
            totals.RowStyles.Add(new RowStyle(SizeType.Absolute, 26f));
            totals.RowStyles.Add(new RowStyle(SizeType.Absolute, 30f));
            totals.RowStyles.Add(new RowStyle(SizeType.Absolute, 36f));
            totals.RowStyles.Add(new RowStyle(SizeType.Absolute, 30f));
            totals.RowStyles.Add(new RowStyle(SizeType.Absolute, 30f));

            var lblTong = Theme.CreateLabel("Tổng tiền", 9.5f, Theme.Muted);
            lblTong.Dock = DockStyle.Fill;
            lblTong.TextAlign = ContentAlignment.MiddleLeft;
            _lblTongTien = Theme.CreateBoldLabel("0 đ", 10f, Theme.Text);
            _lblTongTien.Dock = DockStyle.Fill;
            _lblTongTien.TextAlign = ContentAlignment.MiddleRight;

            var lblGiam = Theme.CreateLabel("Giảm giá (%)", 9.5f, Theme.Muted);
            lblGiam.Dock = DockStyle.Fill;
            lblGiam.TextAlign = ContentAlignment.MiddleLeft;

            var discountPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.White,
                Margin = Padding.Empty,
                Padding = new Padding(0)
            };
            _numGiamGia = Theme.CreateNumeric(0, 100, 0);
            _numGiamGia.Width = 62;
            _numGiamGia.Margin = new Padding(0, 2, 8, 0);
            _numGiamGia.ValueChanged += delegate { UpdateTotals(); };
            _lblGiamGia = Theme.CreateBoldLabel("0 đ", 9f, Theme.Danger);
            _lblGiamGia.Margin = new Padding(0, 6, 0, 0);
            discountPanel.Controls.Add(_numGiamGia);
            discountPanel.Controls.Add(_lblGiamGia);

            var lblThanhToan = Theme.CreateBoldLabel("Thanh toán", 10.5f, Theme.Text);
            lblThanhToan.Dock = DockStyle.Fill;
            lblThanhToan.TextAlign = ContentAlignment.MiddleLeft;
            _lblThanhToan = Theme.CreateBoldLabel("0 đ", 14f, Theme.AccentDark);
            _lblThanhToan.Dock = DockStyle.Fill;
            _lblThanhToan.TextAlign = ContentAlignment.MiddleRight;

            var lblHinhThuc = Theme.CreateLabel("Hình thức", 9.5f, Theme.Muted);
            lblHinhThuc.Dock = DockStyle.Fill;
            lblHinhThuc.TextAlign = ContentAlignment.MiddleLeft;
            _cboThanhToan = Theme.CreateComboBox();
            _cboThanhToan.Dock = DockStyle.Fill;
            _cboThanhToan.Items.Add("Tiền mặt");
            _cboThanhToan.Items.Add("Chuyển khoản");
            _cboThanhToan.SelectedIndex = 0;

            var lblGhiChu = Theme.CreateLabel("Ghi chú", 9.5f, Theme.Muted);
            lblGhiChu.Dock = DockStyle.Fill;
            lblGhiChu.TextAlign = ContentAlignment.MiddleLeft;
            _txtGhiChu = Theme.CreateTextBox();
            _txtGhiChu.Dock = DockStyle.Fill;
            _txtGhiChu.Margin = new Padding(0, 2, 0, 2);
            _txtGhiChu.MaxLength = 200;

            totals.Controls.Add(lblTong, 0, 0);
            totals.Controls.Add(_lblTongTien, 1, 0);
            totals.Controls.Add(lblGiam, 0, 1);
            totals.Controls.Add(discountPanel, 1, 1);
            totals.Controls.Add(lblThanhToan, 0, 2);
            totals.Controls.Add(_lblThanhToan, 1, 2);
            totals.Controls.Add(lblHinhThuc, 0, 3);
            totals.Controls.Add(_cboThanhToan, 1, 3);
            totals.Controls.Add(lblGhiChu, 0, 4);
            totals.Controls.Add(_txtGhiChu, 1, 4);

            var buttons = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.White,
                Margin = Padding.Empty,
                Padding = new Padding(0, 4, 0, 0)
            };
            buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            buttons.RowStyles.Add(new RowStyle(SizeType.Absolute, 48f));
            buttons.RowStyles.Add(new RowStyle(SizeType.Absolute, 38f));

            _btnThanhToan = Theme.CreateButton("THANH TOÁN", true);
            _btnThanhToan.Dock = DockStyle.Fill;
            _btnThanhToan.Margin = new Padding(0, 0, 0, 6);
            _btnThanhToan.Click += BtnThanhToan_Click;

            _btnTamTinh = Theme.CreateButton("In tạm tính", false);
            _btnTamTinh.Dock = DockStyle.Fill;
            _btnTamTinh.Margin = new Padding(0, 0, 4, 0);
            _btnTamTinh.Font = Theme.Bold(9f);
            _btnTamTinh.Click += BtnTamTinh_Click;

            _btnHuyDon = Theme.CreateDangerButton("Hủy đơn");
            _btnHuyDon.Dock = DockStyle.Fill;
            _btnHuyDon.Margin = new Padding(4, 0, 0, 0);
            _btnHuyDon.Font = Theme.Bold(9f);
            _btnHuyDon.Click += BtnHuyDon_Click;

            buttons.Controls.Add(_btnThanhToan, 0, 0);
            buttons.SetColumnSpan(_btnThanhToan, 2);
            buttons.Controls.Add(_btnTamTinh, 0, 1);
            buttons.Controls.Add(_btnHuyDon, 1, 1);

            layout.Controls.Add(header, 0, 0);
            layout.Controls.Add(_gridChiTiet, 0, 1);
            layout.Controls.Add(actions, 0, 2);
            layout.Controls.Add(totals, 0, 3);
            layout.Controls.Add(buttons, 0, 4);
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

        private void KhoiTaoDuLieu()
        {
            try
            {
                BanHangBUS.DongBoTrangThaiBan();
                LoadKhuVuc();
                LoadLoaiMon();
                LoadThucDon();
                LoadBan(true);
                UpdateOrderState();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tải được màn hình bán hàng. Chi tiết: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadKhuVuc()
        {
            DataTable areas = KhuVucBUS.LayCombo();
            DataRow all = areas.NewRow();
            all["MaKhuVuc"] = 0;
            all["TenKhuVuc"] = "Tất cả khu vực";
            areas.Rows.InsertAt(all, 0);

            _cboKhuVuc.SelectedIndexChanged -= CboKhuVuc_SelectedIndexChanged;
            _cboKhuVuc.DataSource = areas;
            _cboKhuVuc.DisplayMember = "TenKhuVuc";
            _cboKhuVuc.ValueMember = "MaKhuVuc";
            _cboKhuVuc.SelectedIndex = 0;
            _cboKhuVuc.SelectedIndexChanged += CboKhuVuc_SelectedIndexChanged;
        }

        private void CboKhuVuc_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadBan(true);
        }

        private void LoadBan(bool autoSelect)
        {
            int maKhuVuc = GetSelectedKhuVuc();
            _bang = BanHangBUS.LayBan(maKhuVuc);

            bool found = false;
            if (_maBan.HasValue)
            {
                foreach (DataRow row in _bang.Rows)
                {
                    if (Row.Int(row, "MaBan") == _maBan.Value)
                    {
                        found = true;
                        break;
                    }
                }
            }

            if (!found)
            {
                _maBan = null;
                _maHD = null;
            }

            if (autoSelect && !_maBan.HasValue && _bang.Rows.Count > 0)
            {
                _maBan = Row.Int(_bang.Rows[0], "MaBan");
            }

            RenderBan();
            if (_maBan.HasValue)
            {
                LoadHoaDonCuaBan();
            }
            else
            {
                UpdateOrderState();
            }
        }

        private void RenderBan()
        {
            ClearFlow(_flowBan);
            foreach (DataRow row in _bang.Rows)
            {
                int maBan = Row.Int(row, "MaBan");
                bool busy = Row.Str(row, "TrangThai") == "Đang sử dụng";
                bool selected = _maBan.HasValue && _maBan.Value == maBan;

                var button = new Button
                {
                    Width = 80,
                    Height = 58,
                    Margin = new Padding(0, 0, 6, 6),
                    FlatStyle = FlatStyle.Flat,
                    Font = Theme.Bold(9f),
                    Text = Row.Str(row, "TenBan") + Environment.NewLine + Row.Int(row, "SoCho") + " chỗ",
                    TextAlign = ContentAlignment.MiddleCenter,
                    Cursor = Cursors.Hand,
                    UseVisualStyleBackColor = false,
                    Tag = maBan,
                    BackColor = selected ? Theme.Sidebar : (busy ? Theme.Accent : Color.FromArgb(250, 246, 239)),
                    ForeColor = selected || busy ? Color.White : Theme.Text
                };
                button.FlatAppearance.BorderSize = selected ? 2 : 1;
                button.FlatAppearance.BorderColor = selected ? Theme.Accent : (busy ? Theme.Accent : Theme.Border);
                button.FlatAppearance.MouseOverBackColor = selected ? Theme.SidebarHover : Theme.AccentLight;
                button.Click += BanButton_Click;
                _toolTip.SetToolTip(button,
                    Row.Str(row, "TenBan") + " • " + Row.Str(row, "TenKhuVuc") + " • " +
                    (busy ? "Đang sử dụng" : "Trống"));
                _flowBan.Controls.Add(button);
            }
        }

        private void BanButton_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null)
            {
                return;
            }

            _maBan = Convert.ToInt32(button.Tag);
            UpdateSelectionColors();
            LoadHoaDonCuaBan();
        }

        private void UpdateSelectionColors()
        {
            foreach (Control control in _flowBan.Controls)
            {
                var button = control as Button;
                if (button == null)
                {
                    continue;
                }

                int maBan = Convert.ToInt32(button.Tag);
                bool selected = _maBan.HasValue && _maBan.Value == maBan;
                bool busy = false;
                if (_bang != null)
                {
                    foreach (DataRow row in _bang.Rows)
                    {
                        if (Row.Int(row, "MaBan") == maBan)
                        {
                            busy = Row.Str(row, "TrangThai") == "Đang sử dụng";
                            break;
                        }
                    }
                }

                button.BackColor = selected ? Theme.Sidebar : (busy ? Theme.Accent : Color.FromArgb(250, 246, 239));
                button.ForeColor = selected || busy ? Color.White : Theme.Text;
                button.FlatAppearance.BorderSize = selected ? 2 : 1;
                button.FlatAppearance.BorderColor = selected ? Theme.Accent : (busy ? Theme.Accent : Theme.Border);
            }
        }

        private void LoadHoaDonCuaBan()
        {
            if (!_maBan.HasValue)
            {
                _maHD = null;
                RenderHeader(null);
                LoadChiTiet();
                return;
            }

            DataRow tableRow = FindTable(_maBan.Value);
            _maHD = BanHangBUS.GetOpenInvoice(_maBan.Value);
            RenderHeader(tableRow);
            LoadChiTiet();
        }

        private void RenderHeader(DataRow tableRow)
        {
            if (tableRow == null)
            {
                _lblTenBan.Text = "Chưa chọn bàn";
                _lblThongTinBan.Text = "Chọn một bàn ở cột bên trái để bắt đầu.";
                _lblTrangThaiDon.Text = string.Empty;
                return;
            }

            _lblTenBan.Text = "Bàn " + Row.Str(tableRow, "TenBan");
            _lblThongTinBan.Text = Row.Str(tableRow, "TenKhuVuc") + "  •  " + Row.Int(tableRow, "SoCho") + " chỗ";
            if (_maHD.HasValue)
            {
                _lblTrangThaiDon.Text = "ĐANG PHỤC VỤ";
                _lblTrangThaiDon.ForeColor = Theme.Success;
            }
            else
            {
                _lblTrangThaiDon.Text = "CHƯA GỌI MÓN";
                _lblTrangThaiDon.ForeColor = Theme.Muted;
            }
        }

        private DataRow FindTable(int maBan)
        {
            if (_bang == null)
            {
                return null;
            }
            foreach (DataRow row in _bang.Rows)
            {
                if (Row.Int(row, "MaBan") == maBan)
                {
                    return row;
                }
            }
            return null;
        }

        private void LoadLoaiMon()
        {
            _loaiMon = BanHangBUS.LayLoaiMon();
            RenderLoaiMon();
        }

        private void RenderLoaiMon()
        {
            ClearFlow(_flowLoaiMon);

            var all = CreateCategoryButton("Tất cả", 0);
            _flowLoaiMon.Controls.Add(all);

            foreach (DataRow row in _loaiMon.Rows)
            {
                _flowLoaiMon.Controls.Add(CreateCategoryButton(Row.Str(row, "TenLoai"), Row.Int(row, "MaLoai")));
            }
        }

        private Button CreateCategoryButton(string text, int maLoai)
        {
            bool active = _maLoaiDangChon == maLoai;
            var button = new Button
            {
                Text = text,
                Tag = maLoai,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Height = 30,
                Font = Theme.Bold(8.5f),
                FlatStyle = FlatStyle.Flat,
                BackColor = active ? Theme.Accent : Color.White,
                ForeColor = active ? Color.White : Theme.Text,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false,
                Padding = new Padding(6, 0, 6, 0),
                Margin = new Padding(0, 0, 4, 0)
            };
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = active ? Theme.Accent : Theme.Border;
            button.FlatAppearance.MouseOverBackColor = active ? Theme.AccentDark : Theme.AccentLight;
            button.Click += delegate
            {
                _maLoaiDangChon = maLoai;
                RenderLoaiMon();
                RenderMenu();
            };
            return button;
        }

        private void LoadThucDon()
        {
            _thucDon = BanHangBUS.LayMonDangBan();
            RenderMenu();
        }

        private void RenderMenu()
        {
            ClearFlow(_flowMon);
            if (_thucDon == null)
            {
                return;
            }

            string keyword = _txtTimMon == null ? string.Empty : _txtTimMon.Text.Trim();
            int count = 0;
            foreach (DataRow row in _thucDon.Rows)
            {
                if (_maLoaiDangChon != 0 && Row.Int(row, "MaLoai") != _maLoaiDangChon)
                {
                    continue;
                }
                if (keyword.Length > 0 &&
                    Row.Str(row, "TenMon").IndexOf(keyword, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }

                int maMon = Row.Int(row, "MaMon");
                var button = new Button
                {
                    Width = 132,
                    Height = 76,
                    Margin = new Padding(0, 0, 6, 6),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(255, 253, 249),
                    ForeColor = Theme.Text,
                    Font = Theme.Bold(9f),
                    Text = Row.Str(row, "TenMon") + Environment.NewLine + Utils.Money(Row.Dec(row, "Gia")),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Cursor = Cursors.Hand,
                    UseVisualStyleBackColor = false,
                    AutoEllipsis = true,
                    Tag = maMon
                };
                button.FlatAppearance.BorderSize = 1;
                button.FlatAppearance.BorderColor = Theme.Border;
                button.FlatAppearance.MouseOverBackColor = Theme.AccentLight;
                button.Click += delegate { AddMon(maMon); };
                _toolTip.SetToolTip(button,
                    Row.Str(row, "TenMon") + Environment.NewLine +
                    "Đơn vị: " + Row.Str(row, "DonViTinh") + Environment.NewLine +
                    Row.Str(row, "MoTa"));
                _flowMon.Controls.Add(button);
                count++;
            }

            if (count == 0)
            {
                _flowMon.Controls.Add(Theme.CreateLabel("Không tìm thấy món phù hợp.", 9.5f, Theme.Muted));
            }
        }

        private void AddMon(int maMon)
        {
            if (!_maBan.HasValue)
            {
                MessageBox.Show("Vui lòng chọn bàn trước khi gọi món.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                if (!_maHD.HasValue)
                {
                    _maHD = BanHangBUS.GetOrCreateInvoice(_maBan.Value, Session.MaNV);
                }
                BanHangBUS.AddItem(_maHD.Value, maMon);
                LoadBan(false);
                LoadChiTiet();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thêm được món. Chi tiết: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadChiTiet()
        {
            try
            {
                if (_maHD.HasValue)
                {
                    _chiTiet = BanHangBUS.GetDetails(_maHD.Value);
                    _gridChiTiet.DataSource = _chiTiet;
                }
                else
                {
                    _chiTiet = null;
                    _gridChiTiet.DataSource = null;
                }
                UpdateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tải được chi tiết hóa đơn. Chi tiết: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTotals()
        {
            decimal total = 0m;
            if (_chiTiet != null)
            {
                foreach (ChiTietHoaDonDTO item in _chiTiet)
                {
                    total += item.ThanhTien;
                }
            }

            decimal percent = _numGiamGia == null ? 0m : _numGiamGia.Value;
            decimal discount = Math.Round(total * percent / 100m, 0, MidpointRounding.AwayFromZero);
            decimal final = total - discount;

            _lblTongTien.Text = Utils.Money(total);
            _lblGiamGia.Text = "− " + Utils.Money(discount);
            _lblThanhToan.Text = Utils.Money(final);

            bool hasItems = total > 0m && _maHD.HasValue;
            _btnThanhToan.Enabled = hasItems;
            _btnTamTinh.Enabled = hasItems;
            _btnHuyDon.Enabled = _maHD.HasValue;
            _btnXoaMon.Enabled = hasItems;
            _btnTang.Enabled = hasItems;
            _btnGiam.Enabled = hasItems;
        }

        private void UpdateOrderState()
        {
            if (!_maBan.HasValue)
            {
                RenderHeader(null);
            }
            else
            {
                DataRow row = FindTable(_maBan.Value);
                if (row != null)
                {
                    RenderHeader(row);
                }
            }
            LoadChiTiet();
        }

        private void ChangeQuantity(int delta)
        {
            int maCT = GetSelectedMaCT();
            if (maCT == 0 || !_maHD.HasValue)
            {
                return;
            }

            try
            {
                bool stillExists = BanHangBUS.ChangeQuantity(maCT, delta);
                if (!stillExists)
                {
                    _maHD = null;
                }
                LoadBan(false);
                LoadChiTiet();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không cập nhật được số lượng. Chi tiết: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnXoaMon_Click(object sender, EventArgs e)
        {
            int maCT = GetSelectedMaCT();
            if (maCT == 0 || !_maHD.HasValue)
            {
                return;
            }

            try
            {
                bool stillExists = BanHangBUS.RemoveItem(maCT);
                if (!stillExists)
                {
                    _maHD = null;
                }
                LoadBan(false);
                LoadChiTiet();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không xóa được món. Chi tiết: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetSelectedMaCT()
        {
            if (_gridChiTiet.CurrentRow == null || _gridChiTiet.CurrentRow.Cells["MaCT"].Value == null)
            {
                return 0;
            }
            return Convert.ToInt32(_gridChiTiet.CurrentRow.Cells["MaCT"].Value);
        }

        private void BtnThanhToan_Click(object sender, EventArgs e)
        {
            if (!_maHD.HasValue)
            {
                return;
            }

            try
            {
                decimal total = BanHangBUS.GetTotal(_maHD.Value);
                if (total <= 0m)
                {
                    MessageBox.Show("Hóa đơn chưa có món nào.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                decimal discount = Math.Round(total * _numGiamGia.Value / 100m, 0, MidpointRounding.AwayFromZero);
                decimal final = total - discount;
                string tableName = _lblTenBan.Text.Replace("Bàn ", string.Empty);
                string message = "Thanh toán " + tableName + "?" + Environment.NewLine + Environment.NewLine +
                                 "Tổng tiền: " + Utils.Money(total) + Environment.NewLine +
                                 "Giảm giá: " + Utils.Money(discount) + Environment.NewLine +
                                 "Khách phải trả: " + Utils.Money(final);

                if (MessageBox.Show(message, "Xác nhận thanh toán",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }

                int maHD = _maHD.Value;
                BanHangBUS.PayInvoice(maHD, discount, Convert.ToString(_cboThanhToan.SelectedItem),
                    _txtGhiChu.Text);

                var printer = new InvoicePrinter(maHD, false);

                _maHD = null;
                _numGiamGia.Value = 0;
                _txtGhiChu.Clear();
                _cboThanhToan.SelectedIndex = 0;
                LoadBan(false);
                LoadChiTiet();

                if (MessageBox.Show("Thanh toán thành công " + Utils.Money(final) + "." +
                                    Environment.NewLine + "Bạn có muốn in hóa đơn không?",
                        "Thanh toán thành công", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    printer.ShowPreview(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Thanh toán thất bại. Chi tiết: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnTamTinh_Click(object sender, EventArgs e)
        {
            if (!_maHD.HasValue)
            {
                return;
            }
            try
            {
                var printer = new InvoicePrinter(_maHD.Value, true);
                printer.ShowPreview(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không in được phiếu tạm tính. Chi tiết: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnHuyDon_Click(object sender, EventArgs e)
        {
            if (!_maHD.HasValue || !_maBan.HasValue)
            {
                return;
            }

            if (MessageBox.Show("Hủy toàn bộ đơn của " + _lblTenBan.Text + "? Hóa đơn sẽ bị xóa.",
                    "Hủy đơn", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                BanHangBUS.DeleteOpenInvoice(_maHD.Value);
                _maHD = null;
                _numGiamGia.Value = 0;
                _txtGhiChu.Clear();
                LoadBan(false);
                LoadChiTiet();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không hủy được đơn. Chi tiết: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetSelectedKhuVuc()
        {
            if (_cboKhuVuc == null || _cboKhuVuc.SelectedValue == null)
            {
                return 0;
            }
            return Convert.ToInt32(_cboKhuVuc.SelectedValue);
        }

        private static void ClearFlow(FlowLayoutPanel panel)
        {
            if (panel == null)
            {
                return;
            }
            while (panel.Controls.Count > 0)
            {
                Control control = panel.Controls[0];
                panel.Controls.RemoveAt(0);
                control.Dispose();
            }
        }
    }
}
