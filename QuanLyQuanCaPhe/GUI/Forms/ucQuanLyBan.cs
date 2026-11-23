using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using QuanLyQuanCaPhe.BUS;
using QuanLyQuanCaPhe.GUI.Controls;

namespace QuanLyQuanCaPhe.GUI.Forms
{
    public class ucQuanLyBan : UserControl
    {
        private TabControl _tabs;
        private UiGrid _gridBan;
        private UiGrid _gridKhuVuc;
        private TextBox _txtTenBan;
        private ComboBox _cboKhuVuc;
        private NumericUpDown _numSoCho;
        private Label _lblTrangThaiBan;
        private Button _btnLuuBan;
        private Button _btnXoaBan;
        private TextBox _txtTenKhuVuc;
        private TextBox _txtMoTaKhuVuc;
        private Button _btnLuuKhuVuc;
        private Button _btnXoaKhuVuc;
        private int _maBanDangSua;
        private int _maKhuVucDangSua;

        public ucQuanLyBan()
        {
            BackColor = Theme.Background;
            Font = Theme.Font(10f);
            BuildUi();
            Load += delegate { LoadTatCa(); };
        }

        private void BuildUi()
        {
            _tabs = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = Theme.Bold(10f),
                Padding = new Point(16, 6)
            };

            var pageBan = new TabPage("Quản lý bàn") { BackColor = Theme.Background, Padding = new Padding(6) };
            var pageKhuVuc = new TabPage("Khu vực") { BackColor = Theme.Background, Padding = new Padding(6) };
            pageBan.Controls.Add(BuildBanPage());
            pageKhuVuc.Controls.Add(BuildKhuVucPage());
            _tabs.TabPages.Add(pageBan);
            _tabs.TabPages.Add(pageKhuVuc);
            _tabs.SelectedIndexChanged += delegate
            {
                if (_tabs.SelectedIndex == 0)
                {
                    LoadBan();
                    LoadKhuVucCombo();
                }
                else
                {
                    LoadKhuVuc();
                }
            };
            Controls.Add(_tabs);
        }

        private Control BuildBanPage()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Theme.Background,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35f));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            Panel gridCard = Theme.CreateCard();
            gridCard.Dock = DockStyle.Fill;
            gridCard.Margin = new Padding(6);
            gridCard.Padding = new Padding(14);

            var gridLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.White,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            gridLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            gridLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
            gridLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            var header = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            var title = Theme.CreateBoldLabel("Danh sách bàn", 12f, Theme.Text);
            title.Location = new Point(0, 8);
            var btnDongBo = Theme.CreateSmallButton("Đồng bộ trạng thái", false);
            btnDongBo.Width = 150;
            btnDongBo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDongBo.Location = new Point(header.Width - 150, 4);
            header.Resize += delegate { btnDongBo.Location = new Point(Math.Max(0, header.Width - btnDongBo.Width), 4); };
            btnDongBo.Click += delegate
            {
                try
                {
                    BanBUS.DongBoTrangThai();
                    LoadBan();
                }
                catch (Exception ex)
                {
                    ShowError("Không đồng bộ được trạng thái bàn", ex);
                }
            };
            header.Controls.Add(title);
            header.Controls.Add(btnDongBo);

            _gridBan = new UiGrid { Dock = DockStyle.Fill };
            Theme.StyleGrid(_gridBan);
            _gridBan.AutoGenerateColumns = false;
            AddColumn(_gridBan, "MaBan", string.Empty, 0, false);
            AddColumn(_gridBan, "TenBan", "Tên bàn", 20, true);
            AddColumn(_gridBan, "TenKhuVuc", "Khu vực", 34, true);
            AddColumn(_gridBan, "SoCho", "Số chỗ", 16, true);
            AddColumn(_gridBan, "TrangThai", "Trạng thái", 30, true);
            _gridBan.Columns["SoCho"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _gridBan.Columns["TrangThai"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _gridBan.SelectionChanged += GridBan_SelectionChanged;

            gridLayout.Controls.Add(header, 0, 0);
            gridLayout.Controls.Add(_gridBan, 0, 1);
            gridCard.Controls.Add(gridLayout);

            Panel editorCard = Theme.CreateCard();
            editorCard.Dock = DockStyle.Fill;
            editorCard.Margin = new Padding(6);
            editorCard.Padding = new Padding(16);

            var form = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 8,
                BackColor = Color.White,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            form.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 92f));
            form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 48f));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 48f));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 48f));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 48f));
            form.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 82f));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 42f));

            var editorTitle = Theme.CreateBoldLabel("Thông tin bàn", 12f, Theme.Text);
            editorTitle.Dock = DockStyle.Fill;
            form.Controls.Add(editorTitle, 0, 0);
            form.SetColumnSpan(editorTitle, 2);

            _txtTenBan = Theme.CreateTextBox();
            AddField(form, 1, "Tên bàn", _txtTenBan);

            _cboKhuVuc = Theme.CreateComboBox();
            AddField(form, 2, "Khu vực", _cboKhuVuc);

            _numSoCho = Theme.CreateNumeric(1, 40, 4);
            AddField(form, 3, "Số chỗ", _numSoCho);

            _lblTrangThaiBan = new Label
            {
                Text = "Trống",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = Theme.Bold(10f),
                ForeColor = Theme.Success,
                BackColor = Color.White
            };
            var lblTrangThai = Theme.CreateFieldLabel("Trạng thái");
            lblTrangThai.Dock = DockStyle.Fill;
            lblTrangThai.TextAlign = ContentAlignment.MiddleLeft;
            form.Controls.Add(lblTrangThai, 0, 4);
            form.Controls.Add(_lblTrangThaiBan, 1, 4);

            var buttons = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.White,
                Margin = new Padding(0, 4, 0, 0),
                Padding = Padding.Empty
            };
            buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            buttons.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
            buttons.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));

            var btnThem = Theme.CreateButton("Thêm bàn", true);
            btnThem.Dock = DockStyle.Fill;
            btnThem.Margin = new Padding(2);
            btnThem.Click += BtnThemBan_Click;

            _btnLuuBan = Theme.CreateButton("Lưu thay đổi", true);
            _btnLuuBan.Dock = DockStyle.Fill;
            _btnLuuBan.Margin = new Padding(2);
            _btnLuuBan.Enabled = false;
            _btnLuuBan.Click += BtnLuuBan_Click;

            _btnXoaBan = Theme.CreateDangerButton("Xóa bàn");
            _btnXoaBan.Dock = DockStyle.Fill;
            _btnXoaBan.Margin = new Padding(2);
            _btnXoaBan.Enabled = false;
            _btnXoaBan.Click += BtnXoaBan_Click;

            var btnMoi = Theme.CreateButton("Làm mới", false);
            btnMoi.Dock = DockStyle.Fill;
            btnMoi.Margin = new Padding(2);
            btnMoi.Click += delegate { ClearBanEditor(); };

            buttons.Controls.Add(btnThem, 0, 0);
            buttons.Controls.Add(_btnLuuBan, 1, 0);
            buttons.Controls.Add(_btnXoaBan, 0, 1);
            buttons.Controls.Add(btnMoi, 1, 1);
            form.Controls.Add(buttons, 0, 6);
            form.SetColumnSpan(buttons, 2);

            var note = Theme.CreateLabel(
                "Trạng thái bàn tự cập nhật khi gọi món hoặc thanh toán.", 8.5f, Theme.Muted);
            note.Dock = DockStyle.Fill;
            note.TextAlign = ContentAlignment.MiddleLeft;
            form.Controls.Add(note, 0, 7);
            form.SetColumnSpan(note, 2);

            editorCard.Controls.Add(form);
            layout.Controls.Add(gridCard, 0, 0);
            layout.Controls.Add(editorCard, 1, 0);
            return layout;
        }

        private Control BuildKhuVucPage()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Theme.Background,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35f));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            Panel gridCard = Theme.CreateCard();
            gridCard.Dock = DockStyle.Fill;
            gridCard.Margin = new Padding(6);
            gridCard.Padding = new Padding(14);

            var gridLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.White,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            gridLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            gridLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
            gridLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            var title = Theme.CreateBoldLabel("Danh sách khu vực", 12f, Theme.Text);
            title.Dock = DockStyle.Fill;
            title.TextAlign = ContentAlignment.MiddleLeft;

            _gridKhuVuc = new UiGrid { Dock = DockStyle.Fill };
            Theme.StyleGrid(_gridKhuVuc);
            _gridKhuVuc.AutoGenerateColumns = false;
            AddColumn(_gridKhuVuc, "MaKhuVuc", string.Empty, 0, false);
            AddColumn(_gridKhuVuc, "TenKhuVuc", "Tên khu vực", 32, true);
            AddColumn(_gridKhuVuc, "MoTa", "Mô tả", 54, true);
            AddColumn(_gridKhuVuc, "SoBan", "Số bàn", 14, true);
            _gridKhuVuc.Columns["SoBan"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _gridKhuVuc.SelectionChanged += GridKhuVuc_SelectionChanged;

            gridLayout.Controls.Add(title, 0, 0);
            gridLayout.Controls.Add(_gridKhuVuc, 0, 1);
            gridCard.Controls.Add(gridLayout);

            Panel editorCard = Theme.CreateCard();
            editorCard.Dock = DockStyle.Fill;
            editorCard.Margin = new Padding(6);
            editorCard.Padding = new Padding(16);

            var form = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6,
                BackColor = Color.White,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            form.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 92f));
            form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 48f));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 96f));
            form.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 82f));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 42f));

            var editorTitle = Theme.CreateBoldLabel("Thông tin khu vực", 12f, Theme.Text);
            editorTitle.Dock = DockStyle.Fill;
            form.Controls.Add(editorTitle, 0, 0);
            form.SetColumnSpan(editorTitle, 2);

            _txtTenKhuVuc = Theme.CreateTextBox();
            AddField(form, 1, "Tên khu vực", _txtTenKhuVuc);

            _txtMoTaKhuVuc = Theme.CreateTextBox();
            _txtMoTaKhuVuc.Multiline = true;
            _txtMoTaKhuVuc.ScrollBars = ScrollBars.Vertical;
            _txtMoTaKhuVuc.Dock = DockStyle.Fill;
            _txtMoTaKhuVuc.Margin = new Padding(0, 8, 0, 8);
            var lblMoTa = Theme.CreateFieldLabel("Mô tả");
            lblMoTa.Dock = DockStyle.Fill;
            lblMoTa.TextAlign = ContentAlignment.TopLeft;
            lblMoTa.Padding = new Padding(0, 14, 0, 0);
            form.Controls.Add(lblMoTa, 0, 2);
            form.Controls.Add(_txtMoTaKhuVuc, 1, 2);

            var buttons = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.White,
                Margin = new Padding(0, 4, 0, 0),
                Padding = Padding.Empty
            };
            buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            buttons.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
            buttons.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));

            var btnThem = Theme.CreateButton("Thêm khu vực", true);
            btnThem.Dock = DockStyle.Fill;
            btnThem.Margin = new Padding(2);
            btnThem.Click += BtnThemKhuVuc_Click;

            _btnLuuKhuVuc = Theme.CreateButton("Lưu thay đổi", true);
            _btnLuuKhuVuc.Dock = DockStyle.Fill;
            _btnLuuKhuVuc.Margin = new Padding(2);
            _btnLuuKhuVuc.Enabled = false;
            _btnLuuKhuVuc.Click += BtnLuuKhuVuc_Click;

            _btnXoaKhuVuc = Theme.CreateDangerButton("Xóa khu vực");
            _btnXoaKhuVuc.Dock = DockStyle.Fill;
            _btnXoaKhuVuc.Margin = new Padding(2);
            _btnXoaKhuVuc.Enabled = false;
            _btnXoaKhuVuc.Click += BtnXoaKhuVuc_Click;

            var btnMoi = Theme.CreateButton("Làm mới", false);
            btnMoi.Dock = DockStyle.Fill;
            btnMoi.Margin = new Padding(2);
            btnMoi.Click += delegate { ClearKhuVucEditor(); };

            buttons.Controls.Add(btnThem, 0, 0);
            buttons.Controls.Add(_btnLuuKhuVuc, 1, 0);
            buttons.Controls.Add(_btnXoaKhuVuc, 0, 1);
            buttons.Controls.Add(btnMoi, 1, 1);
            form.Controls.Add(buttons, 0, 4);
            form.SetColumnSpan(buttons, 2);

            var note = Theme.CreateLabel("Không thể xóa khu vực đang có bàn.", 8.5f, Theme.Muted);
            note.Dock = DockStyle.Fill;
            note.TextAlign = ContentAlignment.MiddleLeft;
            form.Controls.Add(note, 0, 5);
            form.SetColumnSpan(note, 2);

            editorCard.Controls.Add(form);
            layout.Controls.Add(gridCard, 0, 0);
            layout.Controls.Add(editorCard, 1, 0);
            return layout;
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

        private static void AddField(TableLayoutPanel form, int row, string labelText, Control control)
        {
            var label = Theme.CreateFieldLabel(labelText);
            label.Dock = DockStyle.Fill;
            label.TextAlign = ContentAlignment.MiddleLeft;
            control.Dock = DockStyle.Fill;
            control.Margin = new Padding(0, 8, 0, 8);
            form.Controls.Add(label, 0, row);
            form.Controls.Add(control, 1, row);
        }

        private void LoadTatCa()
        {
            try
            {
                LoadKhuVucCombo();
                LoadBan();
                LoadKhuVuc();
            }
            catch (Exception ex)
            {
                ShowError("Không tải được dữ liệu bàn", ex);
            }
        }

        private void LoadKhuVucCombo()
        {
            try
            {
                DataTable table = KhuVucBUS.LayCombo();
                int selected = 0;
                if (_cboKhuVuc.SelectedValue != null && _cboKhuVuc.SelectedValue != DBNull.Value)
                {
                    selected = Convert.ToInt32(_cboKhuVuc.SelectedValue);
                }

                _cboKhuVuc.SelectedIndexChanged -= CboKhuVuc_SelectedIndexChanged;
                _cboKhuVuc.DataSource = table;
                _cboKhuVuc.DisplayMember = "TenKhuVuc";
                _cboKhuVuc.ValueMember = "MaKhuVuc";
                if (selected > 0)
                {
                    _cboKhuVuc.SelectedValue = selected;
                }
                _cboKhuVuc.SelectedIndexChanged += CboKhuVuc_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                ShowError("Không tải được khu vực", ex);
            }
        }

        private void CboKhuVuc_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void LoadBan()
        {
            try
            {
                DataTable table = BanBUS.LayDanhSach();
                _gridBan.DataSource = table;
                ClearBanEditor();
            }
            catch (Exception ex)
            {
                ShowError("Không tải được danh sách bàn", ex);
            }
        }

        private void LoadKhuVuc()
        {
            try
            {
                DataTable table = KhuVucBUS.LayDanhSach();
                _gridKhuVuc.DataSource = table;
                ClearKhuVucEditor();
            }
            catch (Exception ex)
            {
                ShowError("Không tải được danh sách khu vực", ex);
            }
        }

        private void GridBan_SelectionChanged(object sender, EventArgs e)
        {
            if (_gridBan.CurrentRow == null)
            {
                return;
            }
            var view = _gridBan.CurrentRow.DataBoundItem as DataRowView;
            if (view == null)
            {
                return;
            }

            _maBanDangSua = Row.Int(view.Row, "MaBan");
            _txtTenBan.Text = Row.Str(view.Row, "TenBan");
            _numSoCho.Value = Math.Max(1, Row.Int(view.Row, "SoCho"));
            string status = Row.Str(view.Row, "TrangThai");
            _lblTrangThaiBan.Text = status;
            _lblTrangThaiBan.ForeColor = status == "Đang sử dụng" ? Theme.Warning : Theme.Success;

            string areaName = Row.Str(view.Row, "TenKhuVuc");
            for (int i = 0; i < _cboKhuVuc.Items.Count; i++)
            {
                DataRowView area = _cboKhuVuc.Items[i] as DataRowView;
                if (area != null && Row.Str(area.Row, "TenKhuVuc") == areaName)
                {
                    _cboKhuVuc.SelectedIndex = i;
                    break;
                }
            }

            _btnLuuBan.Enabled = true;
            _btnXoaBan.Enabled = true;
        }

        private void ClearBanEditor()
        {
            _maBanDangSua = 0;
            _txtTenBan.Clear();
            _numSoCho.Value = 4;
            _lblTrangThaiBan.Text = "Trống";
            _lblTrangThaiBan.ForeColor = Theme.Success;
            if (_cboKhuVuc.Items.Count > 0)
            {
                _cboKhuVuc.SelectedIndex = 0;
            }
            _btnLuuBan.Enabled = false;
            _btnXoaBan.Enabled = false;
            if (_gridBan != null)
            {
                _gridBan.ClearSelection();
            }
        }

        private void GridKhuVuc_SelectionChanged(object sender, EventArgs e)
        {
            if (_gridKhuVuc.CurrentRow == null)
            {
                return;
            }
            var view = _gridKhuVuc.CurrentRow.DataBoundItem as DataRowView;
            if (view == null)
            {
                return;
            }

            _maKhuVucDangSua = Row.Int(view.Row, "MaKhuVuc");
            _txtTenKhuVuc.Text = Row.Str(view.Row, "TenKhuVuc");
            _txtMoTaKhuVuc.Text = Row.Str(view.Row, "MoTa");
            _btnLuuKhuVuc.Enabled = true;
            _btnXoaKhuVuc.Enabled = true;
        }

        private void ClearKhuVucEditor()
        {
            _maKhuVucDangSua = 0;
            _txtTenKhuVuc.Clear();
            _txtMoTaKhuVuc.Clear();
            _btnLuuKhuVuc.Enabled = false;
            _btnXoaKhuVuc.Enabled = false;
            if (_gridKhuVuc != null)
            {
                _gridKhuVuc.ClearSelection();
            }
        }

        private void BtnThemBan_Click(object sender, EventArgs e)
        {
            string tenBan = _txtTenBan.Text.Trim();
            if (tenBan.Length == 0)
            {
                MessageBox.Show("Vui lòng nhập tên bàn.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                _txtTenBan.Focus();
                return;
            }
            if (_cboKhuVuc.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn khu vực cho bàn.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                BanBUS.Them(tenBan, Convert.ToInt32(_cboKhuVuc.SelectedValue), Convert.ToInt32(_numSoCho.Value));
                LoadBan();
                MessageBox.Show("Đã thêm bàn " + tenBan + ".", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    MessageBox.Show("Tên bàn này đã tồn tại trong khu vực đã chọn.", "Trùng tên bàn",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    ShowError("Không thêm được bàn", ex);
                }
            }
        }

        private void BtnLuuBan_Click(object sender, EventArgs e)
        {
            if (_maBanDangSua == 0)
            {
                return;
            }
            string tenBan = _txtTenBan.Text.Trim();
            if (tenBan.Length == 0)
            {
                MessageBox.Show("Vui lòng nhập tên bàn.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                BanBUS.Sua(_maBanDangSua, tenBan,
                    Convert.ToInt32(_cboKhuVuc.SelectedValue), Convert.ToInt32(_numSoCho.Value));
                LoadBan();
                MessageBox.Show("Đã cập nhật bàn.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    MessageBox.Show("Tên bàn này đã tồn tại trong khu vực đã chọn.", "Trùng tên bàn",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    ShowError("Không cập nhật được bàn", ex);
                }
            }
        }

        private void BtnXoaBan_Click(object sender, EventArgs e)
        {
            if (_maBanDangSua == 0)
            {
                return;
            }

            try
            {
                int invoiceCount = BanBUS.DemHoaDon(_maBanDangSua);
                if (invoiceCount > 0)
                {
                    MessageBox.Show("Bàn đã có hóa đơn nên không thể xóa. Hãy giữ lại để bảo toàn dữ liệu.",
                        "Không thể xóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show("Xóa bàn \"" + _txtTenBan.Text.Trim() + "\"?", "Xác nhận xóa",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                {
                    return;
                }

                BanBUS.Xoa(_maBanDangSua);
                LoadBan();
            }
            catch (Exception ex)
            {
                ShowError("Không xóa được bàn", ex);
            }
        }

        private void BtnThemKhuVuc_Click(object sender, EventArgs e)
        {
            string ten = _txtTenKhuVuc.Text.Trim();
            if (ten.Length == 0)
            {
                MessageBox.Show("Vui lòng nhập tên khu vực.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                _txtTenKhuVuc.Focus();
                return;
            }

            try
            {
                KhuVucBUS.Them(ten, _txtMoTaKhuVuc.Text.Trim());
                LoadKhuVuc();
                LoadKhuVucCombo();
                MessageBox.Show("Đã thêm khu vực " + ten + ".", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    MessageBox.Show("Tên khu vực này đã tồn tại.", "Trùng tên khu vực",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    ShowError("Không thêm được khu vực", ex);
                }
            }
        }

        private void BtnLuuKhuVuc_Click(object sender, EventArgs e)
        {
            if (_maKhuVucDangSua == 0)
            {
                return;
            }

            string ten = _txtTenKhuVuc.Text.Trim();
            if (ten.Length == 0)
            {
                MessageBox.Show("Vui lòng nhập tên khu vực.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                KhuVucBUS.Sua(_maKhuVucDangSua, ten, _txtMoTaKhuVuc.Text.Trim());
                LoadKhuVuc();
                LoadBan();
                LoadKhuVucCombo();
                MessageBox.Show("Đã cập nhật khu vực.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    MessageBox.Show("Tên khu vực này đã tồn tại.", "Trùng tên khu vực",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    ShowError("Không cập nhật được khu vực", ex);
                }
            }
        }

        private void BtnXoaKhuVuc_Click(object sender, EventArgs e)
        {
            if (_maKhuVucDangSua == 0)
            {
                return;
            }

            try
            {
                int tableCount = KhuVucBUS.DemBan(_maKhuVucDangSua);
                if (tableCount > 0)
                {
                    MessageBox.Show("Khu vực đang có " + tableCount + " bàn nên không thể xóa. " +
                                    "Hãy chuyển hoặc xóa các bàn trước.",
                        "Không thể xóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show("Xóa khu vực \"" + _txtTenKhuVuc.Text.Trim() + "\"?", "Xác nhận xóa",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                {
                    return;
                }

                KhuVucBUS.Xoa(_maKhuVucDangSua);
                LoadKhuVuc();
                LoadKhuVucCombo();
            }
            catch (Exception ex)
            {
                ShowError("Không xóa được khu vực", ex);
            }
        }

        private static void ShowError(string message, Exception ex)
        {
            MessageBox.Show(message + ". Chi tiết: " + ex.Message, "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
