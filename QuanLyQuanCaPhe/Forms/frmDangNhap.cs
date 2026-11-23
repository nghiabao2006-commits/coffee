using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyQuanCaPhe.Controls;
using QuanLyQuanCaPhe.Data;
using QuanLyQuanCaPhe.Helpers;

namespace QuanLyQuanCaPhe.Forms
{
    public class frmDangNhap : Form
    {
        private readonly TextBox _txtTenDangNhap;
        private readonly TextBox _txtMatKhau;
        private readonly CheckBox _chkHienMatKhau;
        private readonly Button _btnDangNhap;
        private readonly Button _btnCauHinh;
        private readonly Label _lblTrangThai;
        private readonly Panel _card;
        private readonly BufferedPanel _panelTrai;
        private readonly Panel _panelPhai;

        private bool _sanSang;
        private bool _dangKhoiTao;

        public frmDangNhap()
        {
            Text = "Đăng nhập – Quản lý quán cà phê T1";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(1180, 720);
            MinimumSize = new Size(1000, 650);
            BackColor = Theme.Background;
            Font = Theme.Font(10f);
            DoubleBuffered = true;

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 56f));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 44f));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            _panelTrai = new BufferedPanel { Dock = DockStyle.Fill, Margin = Padding.Empty };
            _panelTrai.Paint += PanelTrai_Paint;

            _panelPhai = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(251, 248, 243),
                Margin = Padding.Empty
            };
            _panelPhai.Resize += delegate { CenterCard(); };

            _card = new Panel
            {
                Width = 390,
                Height = 552,
                BackColor = Color.White
            };
            _card.Paint += delegate(object sender, PaintEventArgs e)
            {
                using (var pen = new Pen(Theme.Border))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, _card.Width - 1, _card.Height - 1);
                }
            };

            var lblTitle = Theme.CreateBoldLabel("Đăng nhập", 22f, Theme.Text);
            lblTitle.Location = new Point(34, 28);

            var lblSub = Theme.CreateLabel("Chào mừng bạn đến với Coffee T1", 9.5f, Theme.Muted);
            lblSub.Location = new Point(34, 72);

            var lblUser = Theme.CreateFieldLabel("Tên đăng nhập");
            lblUser.Location = new Point(34, 112);

            _txtTenDangNhap = Theme.CreateTextBox();
            _txtTenDangNhap.Location = new Point(34, 134);
            _txtTenDangNhap.Width = 322;
            _txtTenDangNhap.MaxLength = 50;
            _txtTenDangNhap.TabIndex = 0;

            var lblPass = Theme.CreateFieldLabel("Mật khẩu");
            lblPass.Location = new Point(34, 180);

            _txtMatKhau = Theme.CreateTextBox();
            _txtMatKhau.Location = new Point(34, 202);
            _txtMatKhau.Width = 322;
            _txtMatKhau.MaxLength = 100;
            _txtMatKhau.UseSystemPasswordChar = true;
            _txtMatKhau.TabIndex = 1;

            _chkHienMatKhau = new CheckBox
            {
                Text = "Hiện mật khẩu",
                Font = Theme.Font(9.5f),
                ForeColor = Theme.Muted,
                Location = new Point(34, 242),
                Width = 150,
                Height = 24,
                BackColor = Color.Transparent
            };
            _chkHienMatKhau.CheckedChanged += delegate
            {
                _txtMatKhau.UseSystemPasswordChar = !_chkHienMatKhau.Checked;
            };

            _btnDangNhap = Theme.CreateButton("ĐĂNG NHẬP", true);
            _btnDangNhap.Location = new Point(34, 276);
            _btnDangNhap.Width = 322;
            _btnDangNhap.Height = 44;
            _btnDangNhap.Enabled = false;
            _btnDangNhap.TabIndex = 2;
            _btnDangNhap.Click += BtnDangNhap_Click;

            _lblTrangThai = new Label
            {
                Text = "Đang chuẩn bị cơ sở dữ liệu...",
                Font = Theme.Font(9f),
                ForeColor = Theme.Muted,
                Location = new Point(34, 332),
                Width = 322,
                Height = 44,
                BackColor = Color.Transparent
            };
            _lblTrangThai.Click += LblTrangThai_Click;

            var divider = new Panel
            {
                BackColor = Theme.Border,
                Location = new Point(34, 388),
                Width = 322,
                Height = 1
            };

            var lblDemoTitle = Theme.CreateBoldLabel("TÀI KHOẢN DEMO", 8.5f, Theme.Muted);
            lblDemoTitle.Location = new Point(34, 400);

            var lblDemo = Theme.CreateLabel(
                "Quản lý:    admin / 123456\n" +
                "Nhân viên: nhanvien / 123456\n" +
                "Nhân viên 2: nhanvien2 / 123456",
                9f, Theme.Text);
            lblDemo.Location = new Point(34, 420);

            _btnCauHinh = Theme.CreateButton("Cấu hình kết nối SQL Server", false);
            _btnCauHinh.Location = new Point(34, 492);
            _btnCauHinh.Width = 322;
            _btnCauHinh.Height = 34;
            _btnCauHinh.Font = Theme.Bold(9.5f);
            _btnCauHinh.Click += BtnCauHinh_Click;

            _card.Controls.Add(lblTitle);
            _card.Controls.Add(lblSub);
            _card.Controls.Add(lblUser);
            _card.Controls.Add(_txtTenDangNhap);
            _card.Controls.Add(lblPass);
            _card.Controls.Add(_txtMatKhau);
            _card.Controls.Add(_chkHienMatKhau);
            _card.Controls.Add(_btnDangNhap);
            _card.Controls.Add(_lblTrangThai);
            _card.Controls.Add(divider);
            _card.Controls.Add(lblDemoTitle);
            _card.Controls.Add(lblDemo);
            _card.Controls.Add(_btnCauHinh);

            _panelPhai.Controls.Add(_card);
            root.Controls.Add(_panelTrai, 0, 0);
            root.Controls.Add(_panelPhai, 1, 0);
            Controls.Add(root);

            AcceptButton = _btnDangNhap;
            Load += FrmDangNhap_Load;
            Shown += delegate { CenterCard(); };
        }

        private async void FrmDangNhap_Load(object sender, EventArgs e)
        {
            CenterCard();
            if (DatabaseInitializer.IsReady)
            {
                SetSanSang();
                return;
            }
            await ChuanBiCoSoDuLieuAsync();
        }

        private async Task ChuanBiCoSoDuLieuAsync()
        {
            if (_dangKhoiTao)
            {
                return;
            }

            _dangKhoiTao = true;
            _sanSang = false;
            _btnDangNhap.Enabled = false;
            _btnCauHinh.Enabled = false;
            _lblTrangThai.Tag = null;
            _lblTrangThai.Cursor = Cursors.Default;
            SetTrangThai("Đang kết nối và chuẩn bị cơ sở dữ liệu...", Theme.Accent);

            try
            {
                await Task.Run(delegate { DatabaseInitializer.EnsureCreated(); });
                SetSanSang();
            }
            catch (Exception ex)
            {
                _sanSang = false;
                _lblTrangThai.Tag = ex;
                _lblTrangThai.Cursor = Cursors.Hand;
                SetTrangThai("Không kết nối được cơ sở dữ liệu.\nBấm vào đây để xem chi tiết hoặc cấu hình kết nối.", Theme.Danger);
                _btnDangNhap.Enabled = false;
                _btnCauHinh.Enabled = true;
            }
            finally
            {
                _dangKhoiTao = false;
            }
        }

        private void SetSanSang()
        {
            _sanSang = true;
            _lblTrangThai.Tag = null;
            _lblTrangThai.Cursor = Cursors.Default;
            SetTrangThai("Sẵn sàng đăng nhập  •  " + DatabaseHelper.Server, Theme.Success);
            _btnDangNhap.Enabled = true;
            _btnCauHinh.Enabled = true;
            if (_txtTenDangNhap.Text.Length == 0)
            {
                _txtTenDangNhap.Focus();
            }
        }

        private void SetTrangThai(string text, Color color)
        {
            _lblTrangThai.Text = text;
            _lblTrangThai.ForeColor = color;
        }

        private void BtnDangNhap_Click(object sender, EventArgs e)
        {
            if (!_sanSang || _dangKhoiTao)
            {
                return;
            }

            string tenDangNhap = _txtTenDangNhap.Text.Trim();
            string matKhau = _txtMatKhau.Text;

            if (tenDangNhap.Length == 0)
            {
                SetTrangThai("Vui lòng nhập tên đăng nhập.", Theme.Danger);
                _txtTenDangNhap.Focus();
                return;
            }
            if (matKhau.Length == 0)
            {
                SetTrangThai("Vui lòng nhập mật khẩu.", Theme.Danger);
                _txtMatKhau.Focus();
                return;
            }

            UseWaitCursor = true;
            _btnDangNhap.Enabled = false;
            try
            {
                DataTable table = DatabaseHelper.Query(
                    "SELECT nv.MaNV, nv.TenNV, nv.TenDangNhap, nv.MaVaiTro, vt.TenVaiTro " +
                    "FROM dbo.NhanVien nv INNER JOIN dbo.VaiTro vt ON vt.MaVaiTro = nv.MaVaiTro " +
                    "WHERE nv.TenDangNhap = @TenDangNhap AND nv.MatKhau = @MatKhau AND nv.TrangThai = N'Đang làm'",
                    DatabaseHelper.P("@TenDangNhap", tenDangNhap),
                    DatabaseHelper.P("@MatKhau", Utils.HashPassword(matKhau)));

                if (table.Rows.Count == 0)
                {
                    SetTrangThai("Tên đăng nhập hoặc mật khẩu không đúng, hoặc tài khoản đã nghỉ làm.", Theme.Danger);
                    _txtMatKhau.SelectAll();
                    _txtMatKhau.Focus();
                    return;
                }

                DataRow row = table.Rows[0];
                Session.Clear();
                Session.MaNV = Row.Int(row, "MaNV");
                Session.TenNV = Row.Str(row, "TenNV");
                Session.TenDangNhap = Row.Str(row, "TenDangNhap");
                Session.MaVaiTro = Row.Int(row, "MaVaiTro");
                Session.TenVaiTro = Row.Str(row, "TenVaiTro");

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                SetTrangThai("Lỗi đăng nhập: " + ex.Message, Theme.Danger);
            }
            finally
            {
                UseWaitCursor = false;
                if (!IsDisposed)
                {
                    _btnDangNhap.Enabled = _sanSang;
                }
            }
        }

        private void BtnCauHinh_Click(object sender, EventArgs e)
        {
            using (var form = new frmKetNoiCSDL())
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    SetSanSang();
                }
            }
        }

        private void LblTrangThai_Click(object sender, EventArgs e)
        {
            Exception error = _lblTrangThai.Tag as Exception;
            if (error == null)
            {
                return;
            }

            string message = error is DatabaseConnectionException
                ? ((DatabaseConnectionException)error).GetDetailedMessage()
                : error.ToString();

            using (var form = new Form())
            {
                form.Text = "Chi tiết lỗi kết nối";
                form.StartPosition = FormStartPosition.CenterParent;
                form.ClientSize = new Size(760, 420);
                form.MinimizeBox = false;
                form.MaximizeBox = false;
                form.ShowInTaskbar = false;
                form.BackColor = Color.White;

                var text = new TextBox
                {
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Both,
                    Dock = DockStyle.Fill,
                    Font = new Font("Consolas", 9f),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    Text = message
                };

                var close = Theme.CreateButton("Đóng", true);
                close.Dock = DockStyle.Bottom;
                close.Height = 42;
                close.DialogResult = DialogResult.OK;

                form.Controls.Add(text);
                form.Controls.Add(close);
                form.AcceptButton = close;
                form.CancelButton = close;
                form.ShowDialog(this);
            }
        }

        private void CenterCard()
        {
            if (_panelPhai == null || _card == null)
            {
                return;
            }
            _card.Left = Math.Max(16, (_panelPhai.ClientSize.Width - _card.Width) / 2);
            _card.Top = Math.Max(16, (_panelPhai.ClientSize.Height - _card.Height) / 2);
        }

        private void PanelTrai_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = _panelTrai.ClientRectangle;
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                return;
            }

            using (var gradient = new LinearGradientBrush(rect,
                Color.FromArgb(38, 27, 22), Color.FromArgb(96, 63, 45), 25f))
            {
                g.FillRectangle(gradient, rect);
            }

            using (var circleBrush = new SolidBrush(Color.FromArgb(16, 255, 255, 255)))
            {
                g.FillEllipse(circleBrush, rect.Width - 230, -110, 380, 380);
                g.FillEllipse(circleBrush, -120, rect.Height - 240, 300, 300);
            }

            float scale = Math.Min(1.25f, rect.Width / 620f);
            DrawCoffeeCup(g, rect.Width / 2f, rect.Height * 0.40f, scale);

            float left = 56f;
            float bottom = rect.Height;

            using (var titleFont = new Font("Segoe UI", 30f, FontStyle.Bold))
            using (var accentPen = new Pen(Theme.Accent, 3f))
            using (var whiteBrush = new SolidBrush(Color.White))
            using (var softBrush = new SolidBrush(Color.FromArgb(215, 255, 255, 255)))
            using (var smallFont = new Font("Segoe UI", 10f, FontStyle.Bold))
            using (var sloganFont = new Font("Segoe UI", 11.5f, FontStyle.Italic))
            {
                g.DrawString("COFFEE T1", titleFont, whiteBrush, left, bottom - 168);
                g.DrawLine(accentPen, left, bottom - 116, left + 74, bottom - 116);
                g.DrawString("PHẦN MỀM QUẢN LÝ QUÁN CÀ PHÊ", smallFont, softBrush,
                    left, bottom - 100);
                g.DrawString("Một ly cà phê, một ngày trọn vẹn.", sloganFont,
                    new SolidBrush(Color.FromArgb(190, 255, 255, 255)), left, bottom - 70);
            }
        }

        private static void DrawCoffeeCup(Graphics g, float cx, float cy, float scale)
        {
            float cupWidth = 150f * scale;
            float cupHeight = 120f * scale;

            using (var saucerBrush = new SolidBrush(Color.FromArgb(32, 255, 255, 255)))
            using (var saucerPen = new Pen(Color.FromArgb(80, 255, 255, 255), 1.5f * scale))
            {
                var saucer = new RectangleF(cx - cupWidth * 0.78f, cy + cupHeight * 0.52f,
                    cupWidth * 1.56f, cupHeight * 0.28f);
                g.FillEllipse(saucerBrush, saucer);
                g.DrawEllipse(saucerPen, saucer);
            }

            using (var handlePen = new Pen(Color.FromArgb(220, 255, 255, 255), 9f * scale))
            {
                g.DrawArc(handlePen,
                    cx + cupWidth * 0.38f, cy - cupHeight * 0.05f,
                    cupWidth * 0.46f, cupHeight * 0.52f, -80f, 200f);
            }

            using (var steamPen = new Pen(Color.FromArgb(95, 255, 255, 255), 3f * scale))
            {
                g.DrawBezier(steamPen,
                    cx - 24 * scale, cy - cupHeight * 0.78f,
                    cx - 46 * scale, cy - cupHeight * 0.95f,
                    cx - 4 * scale, cy - cupHeight * 1.02f,
                    cx - 24 * scale, cy - cupHeight * 1.24f);
                g.DrawBezier(steamPen,
                    cx + 24 * scale, cy - cupHeight * 0.76f,
                    cx + 46 * scale, cy - cupHeight * 0.94f,
                    cx + 4 * scale, cy - cupHeight * 1.00f,
                    cx + 24 * scale, cy - cupHeight * 1.22f);
            }

            using (var cupPath = new GraphicsPath())
            {
                cupPath.AddLine(cx - cupWidth / 2f, cy - cupHeight / 2f,
                    cx + cupWidth / 2f, cy - cupHeight / 2f);
                cupPath.AddLine(cx + cupWidth / 2f, cy + cupHeight * 0.10f,
                    cx + cupWidth * 0.33f, cy + cupHeight * 0.38f);
                cupPath.AddBezier(
                    cx + cupWidth * 0.33f, cy + cupHeight * 0.38f,
                    cx + cupWidth * 0.15f, cy + cupHeight * 0.56f,
                    cx - cupWidth * 0.15f, cy + cupHeight * 0.56f,
                    cx - cupWidth * 0.33f, cy + cupHeight * 0.38f);
                cupPath.AddLine(cx - cupWidth * 0.33f, cy + cupHeight * 0.38f,
                    cx - cupWidth / 2f, cy + cupHeight * 0.10f);
                cupPath.CloseFigure();

                using (var cupBrush = new SolidBrush(Color.FromArgb(238, 255, 255, 255)))
                using (var cupPen = new Pen(Color.White, 1.5f * scale))
                {
                    g.FillPath(cupBrush, cupPath);
                    g.DrawPath(cupPen, cupPath);
                }
            }

            using (var coffeeBrush = new SolidBrush(Color.FromArgb(255, 79, 50, 36)))
            using (var rimPen = new Pen(Theme.Accent, 2f * scale))
            {
                var coffee = new RectangleF(cx - cupWidth / 2f, cy - cupHeight / 2f - 5f * scale,
                    cupWidth, 12f * scale);
                g.FillEllipse(coffeeBrush, coffee);
                g.DrawEllipse(rimPen, coffee);
            }
        }
    }
}
