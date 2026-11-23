using System.Windows.Forms;

namespace QuanLyQuanCaPhe.Controls
{
    public class BufferedPanel : Panel
    {
        public BufferedPanel()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
        }
    }
}
