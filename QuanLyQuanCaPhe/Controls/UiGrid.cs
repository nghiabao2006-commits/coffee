using System.Windows.Forms;

namespace QuanLyQuanCaPhe.Controls
{
    /// <summary>DataGridView bật double buffer để cuộn mượt hơn.</summary>
    public class UiGrid : DataGridView
    {
        public UiGrid()
        {
            DoubleBuffered = true;
        }
    }
}
