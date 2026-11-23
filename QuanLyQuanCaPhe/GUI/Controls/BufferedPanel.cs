using System.Windows.Forms;

namespace QuanLyQuanCaPhe.GUI.Controls
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
