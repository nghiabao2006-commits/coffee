using System.Drawing;
using System.Windows.Forms;

namespace QuanLyQuanCaPhe
{
    /// <summary>Bảng màu và các hàm tạo control dùng chung cho giao diện.</summary>
    public static class Theme
    {
        public static readonly Color Background = Color.FromArgb(247, 243, 236);
        public static readonly Color Sidebar = Color.FromArgb(46, 33, 27);
        public static readonly Color SidebarHover = Color.FromArgb(64, 46, 38);
        public static readonly Color SidebarText = Color.FromArgb(233, 221, 207);
        public static readonly Color Accent = Color.FromArgb(198, 151, 79);
        public static readonly Color AccentDark = Color.FromArgb(166, 122, 56);
        public static readonly Color AccentLight = Color.FromArgb(246, 236, 220);
        public static readonly Color Card = Color.White;
        public static readonly Color Text = Color.FromArgb(56, 43, 36);
        public static readonly Color Muted = Color.FromArgb(130, 116, 106);
        public static readonly Color Border = Color.FromArgb(232, 224, 214);
        public static readonly Color Success = Color.FromArgb(63, 143, 99);
        public static readonly Color Danger = Color.FromArgb(196, 82, 74);
        public static readonly Color Warning = Color.FromArgb(210, 150, 60);
        public static readonly Color SoftGreen = Color.FromArgb(232, 244, 237);
        public static readonly Color SoftRed = Color.FromArgb(250, 235, 233);

        public static Font Font(float size)
        {
            return new Font("Segoe UI", size, FontStyle.Regular, GraphicsUnit.Point);
        }

        public static Font Bold(float size)
        {
            return new Font("Segoe UI", size, FontStyle.Bold, GraphicsUnit.Point);
        }

        public static Label CreateLabel(string text, float size, Color color)
        {
            return new Label
            {
                Text = text,
                Font = Font(size),
                ForeColor = color,
                AutoSize = true,
                BackColor = Color.Transparent
            };
        }

        public static Label CreateBoldLabel(string text, float size, Color color)
        {
            return new Label
            {
                Text = text,
                Font = Bold(size),
                ForeColor = color,
                AutoSize = true,
                BackColor = Color.Transparent
            };
        }

        public static Label CreateFieldLabel(string text)
        {
            return new Label
            {
                Text = text,
                Font = Bold(9.5f),
                ForeColor = Text,
                AutoSize = true,
                BackColor = Color.Transparent
            };
        }

        public static Button CreateButton(string text, bool primary)
        {
            var button = new Button
            {
                Text = text,
                Height = 38,
                Font = Bold(10f),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false,
                BackColor = primary ? Accent : Color.White,
                ForeColor = primary ? Color.White : Text,
                TextAlign = ContentAlignment.MiddleCenter
            };
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = primary ? Accent : Border;
            button.FlatAppearance.MouseOverBackColor = primary ? AccentDark : Color.FromArgb(249, 244, 236);
            button.FlatAppearance.MouseDownBackColor = primary ? AccentDark : AccentLight;
            return button;
        }

        public static Button CreateSmallButton(string text, bool primary)
        {
            Button button = CreateButton(text, primary);
            button.Height = 30;
            button.Font = Bold(9f);
            button.Padding = new Padding(8, 0, 8, 0);
            return button;
        }

        public static Button CreateDangerButton(string text)
        {
            Button button = CreateButton(text, false);
            button.ForeColor = Danger;
            button.FlatAppearance.BorderColor = Color.FromArgb(235, 200, 196);
            button.FlatAppearance.MouseOverBackColor = SoftRed;
            return button;
        }

        public static TextBox CreateTextBox()
        {
            return new TextBox
            {
                Font = Font(10.5f),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = Text
            };
        }

        public static ComboBox CreateComboBox()
        {
            return new ComboBox
            {
                Font = Font(10f),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Text
            };
        }

        public static NumericUpDown CreateNumeric(decimal minimum, decimal maximum, decimal value)
        {
            return new NumericUpDown
            {
                Font = Font(10.5f),
                Minimum = minimum,
                Maximum = maximum,
                Value = value,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = Text,
                TextAlign = HorizontalAlignment.Right
            };
        }

        public static DateTimePicker CreateDatePicker()
        {
            return new DateTimePicker
            {
                Font = Font(10f),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Width = 130
            };
        }

        public static Panel CreateCard()
        {
            var panel = new Panel
            {
                BackColor = Card,
                Padding = new Padding(16)
            };
            panel.Paint += delegate(object sender, PaintEventArgs e)
            {
                using (var pen = new Pen(Border))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
                }
            };
            return panel;
        }

        public static void StyleGrid(DataGridView grid)
        {
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Border;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(250, 247, 242),
                ForeColor = Muted,
                Font = Bold(9.5f),
                SelectionBackColor = Color.FromArgb(250, 247, 242),
                SelectionForeColor = Muted,
                Padding = new Padding(6, 0, 6, 0),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                WrapMode = DataGridViewTriState.False
            };
            grid.ColumnHeadersHeight = 38;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Text,
                Font = Font(9.5f),
                SelectionBackColor = AccentLight,
                SelectionForeColor = Text,
                Padding = new Padding(6, 0, 6, 0),
                WrapMode = DataGridViewTriState.False
            };
            grid.RowTemplate.Height = 34;
            grid.RowHeadersVisible = false;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        public static void MakeHeader(Label label)
        {
            label.Font = Bold(16f);
            label.ForeColor = Text;
        }
    }
}
