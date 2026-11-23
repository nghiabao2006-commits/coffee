using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyQuanCaPhe.DAL;

namespace QuanLyQuanCaPhe.GUI.Forms
{
    /// <summary>Cho phép nhập tên SQL Server nếu máy không có LocalDB mặc định.</summary>
    public class frmKetNoiCSDL : Form
    {
        private readonly TextBox _txtServer;
        private readonly Label _lblMessage;
        private readonly Button _btnSave;

        public frmKetNoiCSDL()
        {
            Text = "Cấu hình kết nối SQL Server";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(540, 330);
            BackColor = Theme.Background;
            Padding = new Padding(28);

            var title = Theme.CreateBoldLabel("Kết nối cơ sở dữ liệu", 16f, Theme.Text);
            title.Location = new Point(28, 22);

            var description = Theme.CreateLabel(
                "Phần mềm dùng SQL Server LocalDB mặc định. Nếu máy bạn dùng SQL Server Express,\n" +
                "hãy nhập tên máy chủ bên dưới (ví dụ: .\\SQLEXPRESS).",
                9.5f, Theme.Muted);
            description.Location = new Point(28, 58);
            description.MaximumSize = new Size(484, 0);

            var lblServer = Theme.CreateFieldLabel("Tên máy chủ SQL Server");
            lblServer.Location = new Point(28, 108);

            _txtServer = Theme.CreateTextBox();
            _txtServer.Location = new Point(28, 132);
            _txtServer.Width = 484;
            _txtServer.Text = GetCurrentServer();

            var note = Theme.CreateLabel("Xác thực Windows (Integrated Security) được dùng tự động.",
                8.5f, Theme.Muted);
            note.Location = new Point(28, 162);

            _lblMessage = Theme.CreateLabel(string.Empty, 9f, Theme.Muted);
            _lblMessage.Location = new Point(28, 190);
            _lblMessage.MaximumSize = new Size(484, 42);

            var btnTest = Theme.CreateButton("Kiểm tra kết nối", false);
            btnTest.Location = new Point(28, 250);
            btnTest.Width = 150;
            btnTest.Click += BtnTest_Click;

            _btnSave = Theme.CreateButton("Lưu và kết nối", true);
            _btnSave.Location = new Point(188, 250);
            _btnSave.Width = 150;
            _btnSave.Click += BtnSave_Click;

            var btnCancel = Theme.CreateButton("Hủy", false);
            btnCancel.Location = new Point(348, 250);
            btnCancel.Width = 100;
            btnCancel.DialogResult = DialogResult.Cancel;

            Controls.Add(title);
            Controls.Add(description);
            Controls.Add(lblServer);
            Controls.Add(_txtServer);
            Controls.Add(note);
            Controls.Add(_lblMessage);
            Controls.Add(btnTest);
            Controls.Add(_btnSave);
            Controls.Add(btnCancel);

            AcceptButton = _btnSave;
            CancelButton = btnCancel;
        }

        private static string GetCurrentServer()
        {
            try
            {
                return DatabaseHelper.Server;
            }
            catch
            {
                return @"(localdb)\MSSQLLocalDB";
            }
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            string server = _txtServer.Text.Trim();
            if (server.Length == 0)
            {
                SetMessage("Vui lòng nhập tên máy chủ SQL Server.", Theme.Danger);
                return;
            }

            Cursor = Cursors.WaitCursor;
            try
            {
                string message;
                bool ok = DatabaseHelper.TestServer(server, out message);
                SetMessage(message, ok ? Theme.Success : Theme.Danger);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string server = _txtServer.Text.Trim();
            if (server.Length == 0)
            {
                SetMessage("Vui lòng nhập tên máy chủ SQL Server.", Theme.Danger);
                return;
            }

            Cursor = Cursors.WaitCursor;
            _btnSave.Enabled = false;
            try
            {
                string message;
                if (!DatabaseHelper.TestServer(server, out message))
                {
                    SetMessage(message, Theme.Danger);
                    return;
                }

                DatabaseHelper.SaveServerOverride(server);
                DatabaseInitializer.Reset();
                DatabaseInitializer.EnsureCreated();

                SetMessage("Kết nối thành công. Đang mở phần mềm...", Theme.Success);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (DatabaseConnectionException ex)
            {
                SetMessage(ex.GetDetailedMessage(), Theme.Danger);
            }
            catch (Exception ex)
            {
                SetMessage("Không khởi tạo được cơ sở dữ liệu: " + ex.Message, Theme.Danger);
            }
            finally
            {
                Cursor = Cursors.Default;
                _btnSave.Enabled = true;
            }
        }

        private void SetMessage(string text, Color color)
        {
            _lblMessage.Text = text;
            _lblMessage.ForeColor = color;
        }
    }
}
