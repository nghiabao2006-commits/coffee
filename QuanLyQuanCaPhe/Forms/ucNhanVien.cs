using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using QuanLyQuanCaPhe.Controls;
using QuanLyQuanCaPhe.Data;
using QuanLyQuanCaPhe.Helpers;

namespace QuanLyQuanCaPhe.Forms
{
    public class ucNhanVien : UserControl
    {
        private UiGrid _grid;
        private TextBox _txtHoTen;
        private TextBox _txtTenDangNhap;
        private TextBox _txtMatKhau;
        private ComboBox _cboVaiTro;
        private TextBox _txtDienThoai;
        private TextBox _txtDiaChi;
        private DateTimePicker _dtNgayVao;
        private ComboBox _cboTrangThai;
        private Label _lblMatKhau;
        private Button _btnLuu;
        private Button _btnXoa;
        private int _maNVDangSua;

        public ucNhanVien()
        {
            BackColor = Theme.Background;
            Font = Theme.Font(10f);
            BuildUi();
            Load += delegate { LoadVaiTro(); LoadNhanVien(); };
        }

        private void BuildUi()
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
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 64f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36f));
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

            var title = Theme.CreateBoldLabel("Danh sách nhân viên", 12f, Theme.Text);
            title.Dock = DockStyle.Fill;
            title.TextAlign = ContentAlignment.MiddleLeft;

            _grid = new UiGrid { Dock = DockStyle.Fill };
            Theme.StyleGrid(_grid);
            _grid.AutoGenerateColumns = false;
            AddColumn(_grid, "MaNV", string.Empty, 0, false);
            AddColumn(_grid, "TenNV", "Họ tên", 26, true);
            AddColumn(_grid, "TenDangNhap", "Đăng nhập", 18, true);
            AddColumn(_grid, "TenVaiTro", "Vai trò", 14, true);
            AddColumn(_grid, "DienThoai", "Điện thoại", 16, true);
            AddColumn(_grid, "NgayVaoLam", "Ngày vào", 14, true);
            AddColumn(_grid, "TrangThai", "Trạng thái", 13, true);
            _grid.Columns["NgayVaoLam"].DefaultCellStyle.Format = "dd/MM/yyyy";
            _grid.Columns["TrangThai"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _grid.SelectionChanged += Grid_SelectionChanged;

            gridLayout.Controls.Add(title, 0, 0);
            gridLayout.Controls.Add(_grid, 0, 1);
            gridCard.Controls.Add(gridLayout);

            Panel editorCard = Theme.CreateCard();
            editorCard.Dock = DockStyle.Fill;
            editorCard.Margin = new Padding(6);
            editorCard.Padding = new Padding(16);

            var form = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 13,
                BackColor = Color.White,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            form.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 112f));
            form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 38f));
            for (int i = 0; i < 8; i++)
            {
                form.RowStyles.Add(new RowStyle(SizeType.Absolute, 46f));
            }
            form.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 82f));
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));

            var editorTitle = Theme.CreateBoldLabel("Thông tin nhân viên", 12f, Theme.Text);
            editorTitle.Dock = DockStyle.Fill;
            form.Controls.Add(editorTitle, 0, 0);
            form.SetColumnSpan(editorTitle, 2);

            _txtHoTen = Theme.CreateTextBox();
            AddField(form, 1, "Họ tên", _txtHoTen);

            _txtTenDangNhap = Theme.CreateTextBox();
            AddField(form, 2, "Tên đăng nhập", _txtTenDangNhap);

            _lblMatKhau = Theme.CreateFieldLabel("Mật khẩu");
            _lblMatKhau.Dock = DockStyle.Fill;
            _lblMatKhau.TextAlign = ContentAlignment.MiddleLeft;
            _txtMatKhau = Theme.CreateTextBox();
            _txtMatKhau.UseSystemPasswordChar = true;
            _txtMatKhau.Dock = DockStyle.Fill;
            _txtMatKhau.Margin = new Padding(0, 8, 0, 8);
            form.Controls.Add(_lblMatKhau, 0, 3);
            form.Controls.Add(_txtMatKhau, 1, 3);

            _cboVaiTro = Theme.CreateComboBox();
            AddField(form, 4, "Vai trò", _cboVaiTro);

            _txtDienThoai = Theme.CreateTextBox();
            AddField(form, 5, "Điện thoại", _txtDienThoai);

            _txtDiaChi = Theme.CreateTextBox();
            AddField(form, 6, "Địa chỉ", _txtDiaChi);

            _dtNgayVao = Theme.CreateDatePicker();
            AddField(form, 7, "Ngày vào làm", _dtNgayVao);

            _cboTrangThai = Theme.CreateComboBox();
            _cboTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
            _cboTrangThai.Items.Add("Đang làm");
            _cboTrangThai.Items.Add("Nghỉ việc");
            _cboTrangThai.SelectedIndex = 0;
            AddField(form, 8, "Trạng thái", _cboTrangThai);

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

            var btnThem = Theme.CreateButton("Thêm nhân viên", true);
            btnThem.Dock = DockStyle.Fill;
            btnThem.Margin = new Padding(2);
            btnThem.Click += BtnThem_Click;

            _btnLuu = Theme.CreateButton("Lưu thay đổi", true);
            _btnLuu.Dock = DockStyle.Fill;
            _btnLuu.Margin = new Padding(2);
            _btnLuu.Enabled = false;
            _btnLuu.Click += BtnLuu_Click;

            _btnXoa = Theme.CreateDangerButton("Xóa");
            _btnXoa.Dock = DockStyle.Fill;
            _btnXoa.Margin = new Padding(2);
            _btnXoa.Enabled = false;
            _btnXoa.Click += BtnXoa_Click;

            var btnMoi = Theme.CreateButton("Làm mới", false);
            btnMoi.Dock = DockStyle.Fill;
            btnMoi.Margin = new Padding(2);
            btnMoi.Click += delegate { ClearEditor(); };

            buttons.Controls.Add(btnThem, 0, 0);
            buttons.Controls.Add(_btnLuu, 1, 0);
            buttons.Controls.Add(_btnXoa, 0, 1);
            buttons.Controls.Add(btnMoi, 1, 1);
            form.Controls.Add(buttons, 0, 10);
            form.SetColumnSpan(buttons, 2);

            var note = Theme.CreateLabel(
                "Khi tạo mới, mật khẩu là bắt buộc. Khi sửa, để trống mật khẩu nếu không muốn đổi.",
                8.5f, Theme.Muted);
            note.Dock = DockStyle.Fill;
            note.TextAlign = ContentAlignment.MiddleLeft;
            form.Controls.Add(note, 0, 11);
            form.SetColumnSpan(note, 2);

            editorCard.Controls.Add(form);

            layout.Controls.Add(gridCard, 0, 0);
            layout.Controls.Add(editorCard, 1, 0);
            Controls.Add(layout);
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

        private void LoadVaiTro()
        {
            try
            {
                DataTable table = DatabaseHelper.Query(
                    "SELECT MaVaiTro, TenVaiTro FROM dbo.VaiTro ORDER BY MaVaiTro");
                _cboVaiTro.DataSource = table;
                _cboVaiTro.DisplayMember = "TenVaiTro";
                _cboVaiTro.ValueMember = "MaVaiTro";
            }
            catch (Exception ex)
            {
                ShowError("Không tải được danh sách vai trò", ex);
            }
        }

        private void LoadNhanVien()
        {
            try
            {
                DataTable table = DatabaseHelper.Query(
                    "SELECT n.MaNV, n.TenNV, n.TenDangNhap, v.TenVaiTro, n.DienThoai, " +
                    "n.NgayVaoLam, n.TrangThai FROM dbo.NhanVien n " +
                    "INNER JOIN dbo.VaiTro v ON v.MaVaiTro = n.MaVaiTro " +
                    "ORDER BY n.TrangThai DESC, n.TenNV");
                _grid.DataSource = table;
                ClearEditor();
            }
            catch (Exception ex)
            {
                ShowError("Không tải được danh sách nhân viên", ex);
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

            _maNVDangSua = Row.Int(view.Row, "MaNV");
            _txtHoTen.Text = Row.Str(view.Row, "TenNV");
            _txtTenDangNhap.Text = Row.Str(view.Row, "TenDangNhap");
            _txtMatKhau.Clear();
            _txtDienThoai.Text = Row.Str(view.Row, "DienThoai");
            _cboTrangThai.SelectedItem = Row.Str(view.Row, "TrangThai");

            string role = Row.Str(view.Row, "TenVaiTro");
            for (int i = 0; i < _cboVaiTro.Items.Count; i++)
            {
                DataRowView roleRow = _cboVaiTro.Items[i] as DataRowView;
                if (roleRow != null && Row.Str(roleRow.Row, "TenVaiTro") == role)
                {
                    _cboVaiTro.SelectedIndex = i;
                    break;
                }
            }

            DataRow detail = DatabaseHelper.Query(
                "SELECT DiaChi, NgayVaoLam FROM dbo.NhanVien WHERE MaNV = @MaNV",
                DatabaseHelper.P("@MaNV", _maNVDangSua)).Rows[0];
            _txtDiaChi.Text = Row.Str(detail, "DiaChi");
            DateTime? ngayVao = Row.Date(detail, "NgayVaoLam");
            if (ngayVao.HasValue && ngayVao.Value.Year > 1900)
            {
                _dtNgayVao.Value = ngayVao.Value;
            }

            _lblMatKhau.Text = "Mật khẩu mới";
            _btnLuu.Enabled = true;
            _btnXoa.Enabled = true;
        }

        private void ClearEditor()
        {
            _maNVDangSua = 0;
            _txtHoTen.Clear();
            _txtTenDangNhap.Clear();
            _txtMatKhau.Clear();
            _txtDienThoai.Clear();
            _txtDiaChi.Clear();
            _dtNgayVao.Value = DateTime.Today;
            _cboTrangThai.SelectedIndex = 0;
            if (_cboVaiTro.Items.Count > 0)
            {
                _cboVaiTro.SelectedIndex = _cboVaiTro.Items.Count > 1 ? 1 : 0;
            }
            _lblMatKhau.Text = "Mật khẩu";
            _btnLuu.Enabled = false;
            _btnXoa.Enabled = false;
            if (_grid != null)
            {
                _grid.ClearSelection();
            }
        }

        private bool IsLastManager()
        {
            int managers = Convert.ToInt32(DatabaseHelper.Scalar(
                "SELECT COUNT(*) FROM dbo.NhanVien WHERE MaVaiTro = 1 AND TrangThai = N'Đang làm'"));
            return managers <= 1;
        }

        private bool ValidateInput(bool requirePassword, out string message)
        {
            message = null;
            if (_txtHoTen.Text.Trim().Length == 0)
            {
                message = "Vui lòng nhập họ tên nhân viên.";
                return false;
            }
            if (_txtTenDangNhap.Text.Trim().Length == 0)
            {
                message = "Vui lòng nhập tên đăng nhập.";
                return false;
            }
            if (requirePassword && _txtMatKhau.Text.Length == 0)
            {
                message = "Vui lòng nhập mật khẩu ban đầu cho nhân viên.";
                return false;
            }
            if (_cboVaiTro.SelectedValue == null)
            {
                message = "Vui lòng chọn vai trò.";
                return false;
            }
            return true;
        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            string message;
            if (!ValidateInput(true, out message))
            {
                MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                DatabaseHelper.Execute(
                    "INSERT INTO dbo.NhanVien (TenNV, TenDangNhap, MatKhau, MaVaiTro, DienThoai, DiaChi, NgayVaoLam, TrangThai) " +
                    "VALUES (@TenNV, @TenDangNhap, @MatKhau, @MaVaiTro, @DienThoai, @DiaChi, @NgayVaoLam, @TrangThai)",
                    DatabaseHelper.P("@TenNV", _txtHoTen.Text.Trim()),
                    DatabaseHelper.P("@TenDangNhap", _txtTenDangNhap.Text.Trim()),
                    DatabaseHelper.P("@MatKhau", Utils.HashPassword(_txtMatKhau.Text)),
                    DatabaseHelper.P("@MaVaiTro", Convert.ToInt32(_cboVaiTro.SelectedValue)),
                    DatabaseHelper.P("@DienThoai", _txtDienThoai.Text.Trim()),
                    DatabaseHelper.P("@DiaChi", _txtDiaChi.Text.Trim()),
                    DatabaseHelper.P("@NgayVaoLam", _dtNgayVao.Value.Date),
                    DatabaseHelper.P("@TrangThai", _cboTrangThai.SelectedItem.ToString()));
                LoadNhanVien();
                MessageBox.Show("Đã thêm nhân viên " + _txtHoTen.Text.Trim() + ".", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    MessageBox.Show("Tên đăng nhập này đã được sử dụng.", "Trùng tên đăng nhập",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    ShowError("Không thêm được nhân viên", ex);
                }
            }
        }

        private void BtnLuu_Click(object sender, EventArgs e)
        {
            if (_maNVDangSua == 0)
            {
                return;
            }

            string message;
            if (!ValidateInput(false, out message))
            {
                MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int role = Convert.ToInt32(_cboVaiTro.SelectedValue);
            string status = _cboTrangThai.SelectedItem.ToString();
            try
            {
                if (IsLastManager() && (role != 1 || status != "Đang làm"))
                {
                    MessageBox.Show("Đây là quản lý duy nhất đang hoạt động. Hãy tạo hoặc kích hoạt một quản lý khác trước.",
                        "Không thể thay đổi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_txtMatKhau.Text.Length > 0)
                {
                    DatabaseHelper.Execute(
                        "UPDATE dbo.NhanVien SET TenNV = @TenNV, TenDangNhap = @TenDangNhap, MatKhau = @MatKhau, " +
                        "MaVaiTro = @MaVaiTro, DienThoai = @DienThoai, DiaChi = @DiaChi, NgayVaoLam = @NgayVaoLam, " +
                        "TrangThai = @TrangThai WHERE MaNV = @MaNV",
                        DatabaseHelper.P("@TenNV", _txtHoTen.Text.Trim()),
                        DatabaseHelper.P("@TenDangNhap", _txtTenDangNhap.Text.Trim()),
                        DatabaseHelper.P("@MatKhau", Utils.HashPassword(_txtMatKhau.Text)),
                        DatabaseHelper.P("@MaVaiTro", role),
                        DatabaseHelper.P("@DienThoai", _txtDienThoai.Text.Trim()),
                        DatabaseHelper.P("@DiaChi", _txtDiaChi.Text.Trim()),
                        DatabaseHelper.P("@NgayVaoLam", _dtNgayVao.Value.Date),
                        DatabaseHelper.P("@TrangThai", status),
                        DatabaseHelper.P("@MaNV", _maNVDangSua));
                }
                else
                {
                    DatabaseHelper.Execute(
                        "UPDATE dbo.NhanVien SET TenNV = @TenNV, TenDangNhap = @TenDangNhap, " +
                        "MaVaiTro = @MaVaiTro, DienThoai = @DienThoai, DiaChi = @DiaChi, NgayVaoLam = @NgayVaoLam, " +
                        "TrangThai = @TrangThai WHERE MaNV = @MaNV",
                        DatabaseHelper.P("@TenNV", _txtHoTen.Text.Trim()),
                        DatabaseHelper.P("@TenDangNhap", _txtTenDangNhap.Text.Trim()),
                        DatabaseHelper.P("@MaVaiTro", role),
                        DatabaseHelper.P("@DienThoai", _txtDienThoai.Text.Trim()),
                        DatabaseHelper.P("@DiaChi", _txtDiaChi.Text.Trim()),
                        DatabaseHelper.P("@NgayVaoLam", _dtNgayVao.Value.Date),
                        DatabaseHelper.P("@TrangThai", status),
                        DatabaseHelper.P("@MaNV", _maNVDangSua));
                }

                LoadNhanVien();
                MessageBox.Show("Đã cập nhật thông tin nhân viên.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    MessageBox.Show("Tên đăng nhập này đã được sử dụng.", "Trùng tên đăng nhập",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    ShowError("Không cập nhật được nhân viên", ex);
                }
            }
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (_maNVDangSua == 0)
            {
                return;
            }

            if (_maNVDangSua == Session.MaNV)
            {
                MessageBox.Show("Bạn không thể xóa tài khoản đang đăng nhập. " +
                                "Nếu muốn rời quán, hãy chuyển trạng thái sang Nghỉ việc.",
                    "Không thể xóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (IsLastManager())
                {
                    MessageBox.Show("Đây là quản lý duy nhất đang hoạt động nên không thể xóa.",
                        "Không thể xóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int used = Convert.ToInt32(DatabaseHelper.Scalar(
                    "SELECT COUNT(*) FROM dbo.HoaDon WHERE MaNV = @MaNV",
                    DatabaseHelper.P("@MaNV", _maNVDangSua)));

                if (used > 0)
                {
                    if (MessageBox.Show("Nhân viên này đã lập " + used + " hóa đơn nên không thể xóa. " +
                                        "Bạn có muốn chuyển sang trạng thái Nghỉ việc không?",
                            "Không thể xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        DatabaseHelper.Execute(
                            "UPDATE dbo.NhanVien SET TrangThai = N'Nghỉ việc' WHERE MaNV = @MaNV",
                            DatabaseHelper.P("@MaNV", _maNVDangSua));
                        LoadNhanVien();
                    }
                    return;
                }

                if (MessageBox.Show("Xóa nhân viên \"" + _txtHoTen.Text.Trim() + "\"?", "Xác nhận xóa",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                {
                    return;
                }

                DatabaseHelper.Execute("DELETE FROM dbo.NhanVien WHERE MaNV = @MaNV",
                    DatabaseHelper.P("@MaNV", _maNVDangSua));
                LoadNhanVien();
            }
            catch (Exception ex)
            {
                ShowError("Không xóa được nhân viên", ex);
            }
        }

        private static void ShowError(string message, Exception ex)
        {
            MessageBox.Show(message + ". Chi tiết: " + ex.Message, "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
