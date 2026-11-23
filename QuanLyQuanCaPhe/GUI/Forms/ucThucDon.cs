using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using QuanLyQuanCaPhe.BUS;
using QuanLyQuanCaPhe.GUI.Controls;

namespace QuanLyQuanCaPhe.GUI.Forms
{
    public class ucThucDon : UserControl
    {
        private TabControl _tabs;
        private UiGrid _gridMon;
        private UiGrid _gridLoai;
        private TextBox _txtTenMon;
        private ComboBox _cboLoai;
        private TextBox _txtGia;
        private TextBox _txtDonVi;
        private CheckBox _chkTrangThai;
        private TextBox _txtMoTaMon;
        private Button _btnLuuMon;
        private Button _btnXoaMon;
        private TextBox _txtTenLoai;
        private TextBox _txtMoTaLoai;
        private Button _btnLuuLoai;
        private Button _btnXoaLoai;
        private int _maMonDangSua;
        private int _maLoaiDangSua;

        public ucThucDon()
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

            var pageMon = new TabPage("Thực đơn") { BackColor = Theme.Background, Padding = new Padding(6) };
            var pageLoai = new TabPage("Loại món") { BackColor = Theme.Background, Padding = new Padding(6) };
            pageMon.Controls.Add(BuildMonPage());
            pageLoai.Controls.Add(BuildLoaiPage());
            _tabs.TabPages.Add(pageMon);
            _tabs.TabPages.Add(pageLoai);
            _tabs.SelectedIndexChanged += delegate
            {
                if (_tabs.SelectedIndex == 0)
                {
                    LoadMon();
                    LoadLoaiCombo();
                }
                else
                {
                    LoadLoai();
                }
            };
            Controls.Add(_tabs);
        }

        private Control BuildMonPage()
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
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32f));
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

            var title = Theme.CreateBoldLabel("Danh sách món", 12f, Theme.Text);
            title.Dock = DockStyle.Fill;
            title.TextAlign = ContentAlignment.MiddleLeft;

            _gridMon = new UiGrid { Dock = DockStyle.Fill };
            Theme.StyleGrid(_gridMon);
            _gridMon.AutoGenerateColumns = false;
            AddColumn(_gridMon, "MaMon", string.Empty, 0, false);
            AddColumn(_gridMon, "TenMon", "Tên món", 28, true);
            AddColumn(_gridMon, "TenLoai", "Loại", 20, true);
            AddColumn(_gridMon, "Gia", "Giá (đ)", 14, true);
            AddColumn(_gridMon, "DonViTinh", "Đơn vị", 12, true);
            AddColumn(_gridMon, "TrangThaiText", "Trạng thái", 14, true);
            AddColumn(_gridMon, "MoTa", "Mô tả", 22, true);
            _gridMon.Columns["Gia"].DefaultCellStyle.Format = "N0";
            _gridMon.Columns["Gia"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            _gridMon.Columns["TrangThaiText"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _gridMon.SelectionChanged += GridMon_SelectionChanged;

            gridLayout.Controls.Add(title, 0, 0);
            gridLayout.Controls.Add(_gridMon, 0, 1);
            gridCard.Controls.Add(gridLayout);

            Panel editorCard = Theme.CreateCard();
            editorCard.Dock = DockStyle.Fill;
            editorCard.Margin = new Padding(6);
            editorCard.Padding = new Padding(16);

            var form = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 10,
                BackColor = Color.White,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            form.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 82f));
            form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 48f));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 48f));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 48f));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 48f));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 96f));
            form.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 82f));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 38f));

            var editorTitle = Theme.CreateBoldLabel("Thông tin món", 12f, Theme.Text);
            editorTitle.Dock = DockStyle.Fill;
            form.Controls.Add(editorTitle, 0, 0);
            form.SetColumnSpan(editorTitle, 2);

            _txtTenMon = Theme.CreateTextBox();
            AddField(form, 1, "Tên món", _txtTenMon);

            _cboLoai = Theme.CreateComboBox();
            AddField(form, 2, "Loại món", _cboLoai);

            _txtGia = Theme.CreateTextBox();
            _txtGia.TextAlign = HorizontalAlignment.Right;
            AddField(form, 3, "Giá (đ)", _txtGia);

            _txtDonVi = Theme.CreateTextBox();
            AddField(form, 4, "Đơn vị", _txtDonVi);

            var lblTrangThai = Theme.CreateFieldLabel("Trạng thái");
            lblTrangThai.Dock = DockStyle.Fill;
            lblTrangThai.TextAlign = ContentAlignment.MiddleLeft;
            _chkTrangThai = new CheckBox
            {
                Text = "Đang bán",
                Dock = DockStyle.Fill,
                Font = Theme.Font(10f),
                ForeColor = Theme.Text,
                BackColor = Color.White,
                Checked = true
            };
            form.Controls.Add(lblTrangThai, 0, 5);
            form.Controls.Add(_chkTrangThai, 1, 5);

            _txtMoTaMon = Theme.CreateTextBox();
            _txtMoTaMon.Multiline = true;
            _txtMoTaMon.ScrollBars = ScrollBars.Vertical;
            _txtMoTaMon.Dock = DockStyle.Fill;
            _txtMoTaMon.Margin = new Padding(0, 8, 0, 8);
            var lblMoTa = Theme.CreateFieldLabel("Mô tả");
            lblMoTa.Dock = DockStyle.Fill;
            lblMoTa.TextAlign = ContentAlignment.TopLeft;
            lblMoTa.Padding = new Padding(0, 14, 0, 0);
            form.Controls.Add(lblMoTa, 0, 6);
            form.Controls.Add(_txtMoTaMon, 1, 6);

            var buttons = BuildButtons(out _btnLuuMon, out _btnXoaMon, "Thêm món", BtnThemMon_Click, BtnLuuMon_Click, BtnXoaMon_Click);
            form.Controls.Add(buttons, 0, 8);
            form.SetColumnSpan(buttons, 2);

            var note = Theme.CreateLabel(
                "Món đã có trong hóa đơn chỉ có thể chuyển sang Ngừng bán.", 8.5f, Theme.Muted);
            note.Dock = DockStyle.Fill;
            note.TextAlign = ContentAlignment.MiddleLeft;
            form.Controls.Add(note, 0, 9);
            form.SetColumnSpan(note, 2);

            editorCard.Controls.Add(form);
            layout.Controls.Add(gridCard, 0, 0);
            layout.Controls.Add(editorCard, 1, 0);
            return layout;
        }

        private Control BuildLoaiPage()
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
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32f));
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

            var title = Theme.CreateBoldLabel("Danh sách loại món", 12f, Theme.Text);
            title.Dock = DockStyle.Fill;
            title.TextAlign = ContentAlignment.MiddleLeft;

            _gridLoai = new UiGrid { Dock = DockStyle.Fill };
            Theme.StyleGrid(_gridLoai);
            _gridLoai.AutoGenerateColumns = false;
            AddColumn(_gridLoai, "MaLoai", string.Empty, 0, false);
            AddColumn(_gridLoai, "TenLoai", "Tên loại", 30, true);
            AddColumn(_gridLoai, "MoTa", "Mô tả", 54, true);
            AddColumn(_gridLoai, "SoMon", "Số món", 16, true);
            _gridLoai.Columns["SoMon"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _gridLoai.SelectionChanged += GridLoai_SelectionChanged;

            gridLayout.Controls.Add(title, 0, 0);
            gridLayout.Controls.Add(_gridLoai, 0, 1);
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
            form.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 82f));
            form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 48f));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 120f));
            form.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 82f));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 38f));

            var editorTitle = Theme.CreateBoldLabel("Thông tin loại món", 12f, Theme.Text);
            editorTitle.Dock = DockStyle.Fill;
            form.Controls.Add(editorTitle, 0, 0);
            form.SetColumnSpan(editorTitle, 2);

            _txtTenLoai = Theme.CreateTextBox();
            AddField(form, 1, "Tên loại", _txtTenLoai);

            _txtMoTaLoai = Theme.CreateTextBox();
            _txtMoTaLoai.Multiline = true;
            _txtMoTaLoai.ScrollBars = ScrollBars.Vertical;
            _txtMoTaLoai.Dock = DockStyle.Fill;
            _txtMoTaLoai.Margin = new Padding(0, 8, 0, 8);
            var lblMoTa = Theme.CreateFieldLabel("Mô tả");
            lblMoTa.Dock = DockStyle.Fill;
            lblMoTa.TextAlign = ContentAlignment.TopLeft;
            lblMoTa.Padding = new Padding(0, 14, 0, 0);
            form.Controls.Add(lblMoTa, 0, 2);
            form.Controls.Add(_txtMoTaLoai, 1, 2);

            var buttons = BuildButtons(out _btnLuuLoai, out _btnXoaLoai, "Thêm loại", BtnThemLoai_Click, BtnLuuLoai_Click, BtnXoaLoai_Click);
            form.Controls.Add(buttons, 0, 4);
            form.SetColumnSpan(buttons, 2);

            var note = Theme.CreateLabel("Không thể xóa loại món đang có món bên trong.", 8.5f, Theme.Muted);
            note.Dock = DockStyle.Fill;
            note.TextAlign = ContentAlignment.MiddleLeft;
            form.Controls.Add(note, 0, 5);
            form.SetColumnSpan(note, 2);

            editorCard.Controls.Add(form);
            layout.Controls.Add(gridCard, 0, 0);
            layout.Controls.Add(editorCard, 1, 0);
            return layout;
        }

        private TableLayoutPanel BuildButtons(out Button btnLuu, out Button btnXoa, string addText,
            EventHandler addHandler, EventHandler saveHandler, EventHandler deleteHandler)
        {
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

            var btnThem = Theme.CreateButton(addText, true);
            btnThem.Dock = DockStyle.Fill;
            btnThem.Margin = new Padding(2);
            btnThem.Click += addHandler;

            btnLuu = Theme.CreateButton("Lưu thay đổi", true);
            btnLuu.Dock = DockStyle.Fill;
            btnLuu.Margin = new Padding(2);
            btnLuu.Enabled = false;
            btnLuu.Click += saveHandler;

            btnXoa = Theme.CreateDangerButton("Xóa");
            btnXoa.Dock = DockStyle.Fill;
            btnXoa.Margin = new Padding(2);
            btnXoa.Enabled = false;
            btnXoa.Click += deleteHandler;

            var btnMoi = Theme.CreateButton("Làm mới", false);
            btnMoi.Dock = DockStyle.Fill;
            btnMoi.Margin = new Padding(2);
            Button luuRef = btnLuu;
            btnMoi.Click += delegate
            {
                if (luuRef == _btnLuuMon)
                {
                    ClearMonEditor();
                }
                else
                {
                    ClearLoaiEditor();
                }
            };

            buttons.Controls.Add(btnThem, 0, 0);
            buttons.Controls.Add(btnLuu, 1, 0);
            buttons.Controls.Add(btnXoa, 0, 1);
            buttons.Controls.Add(btnMoi, 1, 1);
            return buttons;
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
                LoadLoaiCombo();
                LoadMon();
                LoadLoai();
            }
            catch (Exception ex)
            {
                ShowError("Không tải được dữ liệu thực đơn", ex);
            }
        }

        private void LoadLoaiCombo()
        {
            try
            {
                DataTable table = ThucDonBUS.LayLoaiCombo();
                int selected = 0;
                if (_cboLoai.SelectedValue != null && _cboLoai.SelectedValue != DBNull.Value)
                {
                    selected = Convert.ToInt32(_cboLoai.SelectedValue);
                }

                _cboLoai.DataSource = table;
                _cboLoai.DisplayMember = "TenLoai";
                _cboLoai.ValueMember = "MaLoai";
                if (selected > 0)
                {
                    _cboLoai.SelectedValue = selected;
                }
            }
            catch (Exception ex)
            {
                ShowError("Không tải được loại món", ex);
            }
        }

        private void LoadMon()
        {
            try
            {
                DataTable table = ThucDonBUS.LayMon();
                _gridMon.DataSource = table;
                ClearMonEditor();
            }
            catch (Exception ex)
            {
                ShowError("Không tải được danh sách món", ex);
            }
        }

        private void LoadLoai()
        {
            try
            {
                DataTable table = ThucDonBUS.LayLoai();
                _gridLoai.DataSource = table;
                ClearLoaiEditor();
            }
            catch (Exception ex)
            {
                ShowError("Không tải được danh sách loại món", ex);
            }
        }

        private void GridMon_SelectionChanged(object sender, EventArgs e)
        {
            if (_gridMon.CurrentRow == null)
            {
                return;
            }
            var view = _gridMon.CurrentRow.DataBoundItem as DataRowView;
            if (view == null)
            {
                return;
            }

            _maMonDangSua = Row.Int(view.Row, "MaMon");
            _txtTenMon.Text = Row.Str(view.Row, "TenMon");
            _txtGia.Text = Row.Dec(view.Row, "Gia").ToString("0");
            _txtDonVi.Text = Row.Str(view.Row, "DonViTinh");
            _txtMoTaMon.Text = Row.Str(view.Row, "MoTa");
            _chkTrangThai.Checked = Row.Str(view.Row, "TrangThaiText") == "Đang bán";

            string categoryName = Row.Str(view.Row, "TenLoai");
            for (int i = 0; i < _cboLoai.Items.Count; i++)
            {
                DataRowView category = _cboLoai.Items[i] as DataRowView;
                if (category != null && Row.Str(category.Row, "TenLoai") == categoryName)
                {
                    _cboLoai.SelectedIndex = i;
                    break;
                }
            }

            _btnLuuMon.Enabled = true;
            _btnXoaMon.Enabled = true;
        }

        private void ClearMonEditor()
        {
            _maMonDangSua = 0;
            _txtTenMon.Clear();
            _txtGia.Clear();
            _txtDonVi.Text = "Ly";
            _txtMoTaMon.Clear();
            _chkTrangThai.Checked = true;
            if (_cboLoai.Items.Count > 0)
            {
                _cboLoai.SelectedIndex = 0;
            }
            _btnLuuMon.Enabled = false;
            _btnXoaMon.Enabled = false;
            if (_gridMon != null)
            {
                _gridMon.ClearSelection();
            }
        }

        private void GridLoai_SelectionChanged(object sender, EventArgs e)
        {
            if (_gridLoai.CurrentRow == null)
            {
                return;
            }
            var view = _gridLoai.CurrentRow.DataBoundItem as DataRowView;
            if (view == null)
            {
                return;
            }

            _maLoaiDangSua = Row.Int(view.Row, "MaLoai");
            _txtTenLoai.Text = Row.Str(view.Row, "TenLoai");
            _txtMoTaLoai.Text = Row.Str(view.Row, "MoTa");
            _btnLuuLoai.Enabled = true;
            _btnXoaLoai.Enabled = true;
        }

        private void ClearLoaiEditor()
        {
            _maLoaiDangSua = 0;
            _txtTenLoai.Clear();
            _txtMoTaLoai.Clear();
            _btnLuuLoai.Enabled = false;
            _btnXoaLoai.Enabled = false;
            if (_gridLoai != null)
            {
                _gridLoai.ClearSelection();
            }
        }

        private void BtnThemMon_Click(object sender, EventArgs e)
        {
            string tenMon = _txtTenMon.Text.Trim();
            decimal gia = Utils.ParseMoney(_txtGia.Text);
            if (tenMon.Length == 0)
            {
                MessageBox.Show("Vui lòng nhập tên món.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                _txtTenMon.Focus();
                return;
            }
            if (gia <= 0m)
            {
                MessageBox.Show("Giá món phải lớn hơn 0.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                _txtGia.Focus();
                return;
            }
            if (_cboLoai.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn loại món.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                ThucDonBUS.ThemMon(tenMon,
                    Convert.ToInt32(_cboLoai.SelectedValue),
                    gia,
                    _txtDonVi.Text.Trim().Length == 0 ? "Ly" : _txtDonVi.Text.Trim(),
                    _chkTrangThai.Checked,
                    _txtMoTaMon.Text.Trim());
                LoadMon();
                MessageBox.Show("Đã thêm món " + tenMon + ".", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowError("Không thêm được món", ex);
            }
        }

        private void BtnLuuMon_Click(object sender, EventArgs e)
        {
            if (_maMonDangSua == 0)
            {
                return;
            }

            string tenMon = _txtTenMon.Text.Trim();
            decimal gia = Utils.ParseMoney(_txtGia.Text);
            if (tenMon.Length == 0 || gia <= 0m)
            {
                MessageBox.Show("Vui lòng nhập tên món và giá lớn hơn 0.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                ThucDonBUS.SuaMon(_maMonDangSua,
                    tenMon,
                    Convert.ToInt32(_cboLoai.SelectedValue),
                    gia,
                    _txtDonVi.Text.Trim().Length == 0 ? "Ly" : _txtDonVi.Text.Trim(),
                    _chkTrangThai.Checked,
                    _txtMoTaMon.Text.Trim());
                LoadMon();
                LoadLoai();
                MessageBox.Show("Đã cập nhật món.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowError("Không cập nhật được món", ex);
            }
        }

        private void BtnXoaMon_Click(object sender, EventArgs e)
        {
            if (_maMonDangSua == 0)
            {
                return;
            }

            try
            {
                int used = ThucDonBUS.DemChiTietHoaDon(_maMonDangSua);

                if (used > 0)
                {
                    if (MessageBox.Show("Món này đã xuất hiện trong " + used + " dòng hóa đơn nên không thể xóa. " +
                                        "Bạn có muốn chuyển sang trạng thái Ngừng bán không?",
                            "Không thể xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        ThucDonBUS.NgungBanMon(_maMonDangSua);
                        LoadMon();
                    }
                    return;
                }

                if (MessageBox.Show("Xóa món \"" + _txtTenMon.Text.Trim() + "\"?", "Xác nhận xóa",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                {
                    return;
                }

                ThucDonBUS.XoaMon(_maMonDangSua);
                LoadMon();
                LoadLoai();
            }
            catch (Exception ex)
            {
                ShowError("Không xóa được món", ex);
            }
        }

        private void BtnThemLoai_Click(object sender, EventArgs e)
        {
            string ten = _txtTenLoai.Text.Trim();
            if (ten.Length == 0)
            {
                MessageBox.Show("Vui lòng nhập tên loại món.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                _txtTenLoai.Focus();
                return;
            }

            try
            {
                ThucDonBUS.ThemLoai(ten, _txtMoTaLoai.Text.Trim());
                LoadLoai();
                LoadLoaiCombo();
                MessageBox.Show("Đã thêm loại món " + ten + ".", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    MessageBox.Show("Tên loại món này đã tồn tại.", "Trùng tên",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    ShowError("Không thêm được loại món", ex);
                }
            }
        }

        private void BtnLuuLoai_Click(object sender, EventArgs e)
        {
            if (_maLoaiDangSua == 0)
            {
                return;
            }

            string ten = _txtTenLoai.Text.Trim();
            if (ten.Length == 0)
            {
                MessageBox.Show("Vui lòng nhập tên loại món.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                ThucDonBUS.SuaLoai(_maLoaiDangSua, ten, _txtMoTaLoai.Text.Trim());
                LoadLoai();
                LoadMon();
                LoadLoaiCombo();
                MessageBox.Show("Đã cập nhật loại món.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    MessageBox.Show("Tên loại món này đã tồn tại.", "Trùng tên",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    ShowError("Không cập nhật được loại món", ex);
                }
            }
        }

        private void BtnXoaLoai_Click(object sender, EventArgs e)
        {
            if (_maLoaiDangSua == 0)
            {
                return;
            }

            try
            {
                int count = ThucDonBUS.DemMonTrongLoai(_maLoaiDangSua);
                if (count > 0)
                {
                    MessageBox.Show("Loại món đang có " + count + " món nên không thể xóa. " +
                                    "Hãy chuyển các món sang loại khác trước.",
                        "Không thể xóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show("Xóa loại món \"" + _txtTenLoai.Text.Trim() + "\"?", "Xác nhận xóa",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                {
                    return;
                }

                ThucDonBUS.XoaLoai(_maLoaiDangSua);
                LoadLoai();
                LoadLoaiCombo();
            }
            catch (Exception ex)
            {
                ShowError("Không xóa được loại món", ex);
            }
        }

        private static void ShowError(string message, Exception ex)
        {
            MessageBox.Show(message + ". Chi tiết: " + ex.Message, "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
