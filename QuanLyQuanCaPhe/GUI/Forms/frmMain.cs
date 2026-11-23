using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using QuanLyQuanCaPhe.GUI.Controls;

namespace QuanLyQuanCaPhe.GUI.Forms
{
    public class frmMain : Form
    {
        private readonly Panel _content;
        private readonly Label _lblPageTitle;
        private readonly Label _lblDate;
        private readonly Dictionary<string, Button> _navButtons = new Dictionary<string, Button>();
        private readonly System.Windows.Forms.Timer _clock;
        private Form _currentPage;

        public bool DangXuat { get; private set; }

        public frmMain()
        {
            Text = "Phần mềm quản lý quán cà phê T1";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1024, 680);
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
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 232f));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            root.Controls.Add(BuildSidebar(), 0, 0);

            var right = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            right.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            right.RowStyles.Add(new RowStyle(SizeType.Absolute, 74f));
            right.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            var header = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = Padding.Empty
            };
            header.Paint += delegate(object sender, PaintEventArgs e)
            {
                using (var pen = new Pen(Theme.Border))
                {
                    e.Graphics.DrawLine(pen, 0, header.Height - 1, header.Width, header.Height - 1);
                }
            };

            _lblPageTitle = Theme.CreateBoldLabel("Trang chủ", 17f, Theme.Text);
            _lblPageTitle.Location = new Point(24, 20);

            _lblDate = new Label
            {
                Dock = DockStyle.Right,
                Width = 340,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 24, 0),
                Font = Theme.Font(9.5f),
                ForeColor = Theme.Muted,
                BackColor = Color.Transparent
            };

            header.Controls.Add(_lblPageTitle);
            header.Controls.Add(_lblDate);

            _content = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Theme.Background,
                Padding = new Padding(20),
                Margin = Padding.Empty
            };

            right.Controls.Add(header, 0, 0);
            right.Controls.Add(_content, 0, 1);
            root.Controls.Add(right, 1, 0);
            Controls.Add(root);

            _clock = new System.Windows.Forms.Timer { Interval = 60000 };
            _clock.Tick += delegate { UpdateClock(); };
            _clock.Start();
            UpdateClock();

            ShowPage("Trang chủ");
        }

        private Control BuildSidebar()
        {
            var sidebar = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Theme.Sidebar,
                ColumnCount = 1,
                RowCount = 3,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            sidebar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            sidebar.RowStyles.Add(new RowStyle(SizeType.Absolute, 104f));
            sidebar.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            sidebar.RowStyles.Add(new RowStyle(SizeType.Absolute, 124f));

            var brand = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Theme.Sidebar,
                Margin = Padding.Empty
            };

            var logo = new BufferedPanel
            {
                Location = new Point(20, 22),
                Size = new Size(46, 46),
                BackColor = Theme.Sidebar
            };
            logo.Paint += delegate(object sender, PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(Theme.Accent))
                {
                    e.Graphics.FillEllipse(brush, 0, 0, 45, 45);
                }
                using (var font = new Font("Segoe UI", 12f, FontStyle.Bold))
                using (var textBrush = new SolidBrush(Theme.Sidebar))
                using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    e.Graphics.DrawString("T1", font, textBrush, new RectangleF(0, 0, 46, 46), format);
                }
            };

            var lblBrand = new Label
            {
                Text = "COFFEE T1",
                Font = Theme.Bold(14f),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(76, 26),
                BackColor = Color.Transparent
            };

            var lblBrandSub = new Label
            {
                Text = "QUẢN LÝ QUÁN CÀ PHÊ",
                Font = Theme.Font(7.5f),
                ForeColor = Color.FromArgb(196, 174, 154),
                AutoSize = true,
                Location = new Point(78, 52),
                BackColor = Color.Transparent
            };

            brand.Controls.Add(logo);
            brand.Controls.Add(lblBrand);
            brand.Controls.Add(lblBrandSub);

            var nav = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Theme.Sidebar,
                Padding = new Padding(12, 12, 12, 12),
                Margin = Padding.Empty,
                AutoScroll = false
            };

            AddNavButton(nav, "Trang chủ");
            AddNavButton(nav, "Bán hàng");
            AddNavButton(nav, "Quản lý bàn");
            AddNavButton(nav, "Thực đơn");
            AddNavButton(nav, "Nhân viên");
            AddNavButton(nav, "Hóa đơn");
            AddNavButton(nav, "Thống kê");

            var userPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Theme.Sidebar,
                Margin = Padding.Empty
            };
            userPanel.Paint += delegate(object sender, PaintEventArgs e)
            {
                using (var pen = new Pen(Color.FromArgb(75, 57, 47)))
                {
                    e.Graphics.DrawLine(pen, 16, 0, userPanel.Width - 16, 0);
                }
            };

            var avatar = new BufferedPanel
            {
                Location = new Point(18, 18),
                Size = new Size(42, 42),
                BackColor = Theme.Sidebar
            };
            avatar.Paint += delegate(object sender, PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(Color.FromArgb(92, 66, 52)))
                {
                    e.Graphics.FillEllipse(brush, 0, 0, 41, 41);
                }
                using (var font = new Font("Segoe UI", 10f, FontStyle.Bold))
                using (var textBrush = new SolidBrush(Color.White))
                using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    e.Graphics.DrawString(GetInitials(Session.TenNV), font, textBrush,
                        new RectangleF(0, 0, 42, 42), format);
                }
            };

            var lblName = new Label
            {
                Text = Session.TenNV,
                Font = Theme.Bold(9.5f),
                ForeColor = Color.White,
                AutoSize = false,
                AutoEllipsis = true,
                Location = new Point(70, 18),
                Size = new Size(146, 20),
                BackColor = Color.Transparent
            };

            var lblRole = new Label
            {
                Text = Session.TenVaiTro,
                Font = Theme.Font(8.5f),
                ForeColor = Color.FromArgb(196, 174, 154),
                AutoSize = false,
                Location = new Point(70, 38),
                Size = new Size(146, 18),
                BackColor = Color.Transparent
            };

            var btnLogout = new Button
            {
                Text = "Đăng xuất",
                Location = new Point(18, 74),
                Size = new Size(196, 32),
                FlatStyle = FlatStyle.Flat,
                Font = Theme.Bold(9f),
                BackColor = Theme.SidebarHover,
                ForeColor = Theme.SidebarText,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.FlatAppearance.MouseOverBackColor = Color.FromArgb(82, 58, 46);
            btnLogout.Click += BtnLogout_Click;

            userPanel.Controls.Add(avatar);
            userPanel.Controls.Add(lblName);
            userPanel.Controls.Add(lblRole);
            userPanel.Controls.Add(btnLogout);

            sidebar.Controls.Add(brand, 0, 0);
            sidebar.Controls.Add(nav, 0, 1);
            sidebar.Controls.Add(userPanel, 0, 2);

            if (!Session.IsQuanLy)
            {
                string[] hidden = { "Quản lý bàn", "Thực đơn", "Nhân viên", "Thống kê" };
                foreach (string key in hidden)
                {
                    if (_navButtons.ContainsKey(key))
                    {
                        _navButtons[key].Visible = false;
                    }
                }
            }

            return sidebar;
        }

        private void AddNavButton(FlowLayoutPanel nav, string text)
        {
            var button = new Button
            {
                Text = text,
                Tag = text,
                Width = 208,
                Height = 44,
                Margin = new Padding(0, 0, 0, 5),
                FlatStyle = FlatStyle.Flat,
                Font = Theme.Bold(10f),
                BackColor = Theme.Sidebar,
                ForeColor = Theme.SidebarText,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(18, 0, 0, 0),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Theme.SidebarHover;
            button.Click += delegate { ShowPage((string)button.Tag); };
            _navButtons[text] = button;
            nav.Controls.Add(button);
        }

        private void ShowPage(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            if (!Session.IsQuanLy &&
                (key == "Quản lý bàn" || key == "Thực đơn" || key == "Nhân viên" || key == "Thống kê"))
            {
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                if (_currentPage != null)
                {
                    _content.Controls.Remove(_currentPage);
                    _currentPage.Dispose();
                    _currentPage = null;
                }

                Form page;
                switch (key)
                {
                    case "Bán hàng":
                        page = new frmBanHang();
                        break;
                    case "Quản lý bàn":
                        page = new frmQuanLyBan();
                        break;
                    case "Thực đơn":
                        page = new frmThucDon();
                        break;
                    case "Nhân viên":
                        page = new frmNhanVien();
                        break;
                    case "Hóa đơn":
                        page = new frmHoaDon();
                        break;
                    case "Thống kê":
                        page = new frmThongKe();
                        break;
                    default:
                        var home = new frmTrangChu();
                        home.NavigateRequested += ShowPage;
                        page = home;
                        key = "Trang chủ";
                        break;
                }

                page.TopLevel = false;
                page.FormBorderStyle = FormBorderStyle.None;
                page.Dock = DockStyle.Fill;
                _content.Controls.Add(page);
                page.Show();
                _currentPage = page;
                _lblPageTitle.Text = key;

                foreach (KeyValuePair<string, Button> pair in _navButtons)
                {
                    bool active = pair.Key == key;
                    pair.Value.BackColor = active ? Theme.Accent : Theme.Sidebar;
                    pair.Value.ForeColor = active ? Color.White : Theme.SidebarText;
                    pair.Value.FlatAppearance.MouseOverBackColor = active ? Theme.AccentDark : Theme.SidebarHover;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không mở được màn hình " + key + ". Chi tiết: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc muốn đăng xuất khỏi phần mềm?",
                "Đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
            {
                return;
            }

            DangXuat = true;
            Session.Clear();
            Close();
        }

        private void UpdateClock()
        {
            _lblDate.Text = DateTime.Now.ToString("dddd, dd/MM/yyyy  •  HH:mm",
                CultureInfo.GetCultureInfo("vi-VN"));
        }

        private static string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "?";
            }

            string[] parts = name.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
            {
                return parts[0].Substring(0, 1).ToUpperInvariant();
            }

            return (parts[0].Substring(0, 1) + parts[parts.Length - 1].Substring(0, 1)).ToUpperInvariant();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _clock.Stop();
            _clock.Dispose();
            base.OnFormClosed(e);
        }
    }
}
