using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using QuanLyQuanCaPhe.Helpers;

namespace QuanLyQuanCaPhe.Controls
{
    /// <summary>Biểu đồ cột đơn giản vẽ bằng GDI+ cho phần thống kê.</summary>
    public class BarChart : Control
    {
        public class BarItem
        {
            public string Label { get; set; }
            public decimal Value { get; set; }

            public BarItem(string label, decimal value)
            {
                Label = label;
                Value = value;
            }
        }

        private readonly List<BarItem> _items = new List<BarItem>();

        public BarChart()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            BackColor = Color.White;
            Font = Theme.Font(8.5f);
        }

        public void SetData(IEnumerable<BarItem> items)
        {
            _items.Clear();
            if (items != null)
            {
                _items.AddRange(items);
            }
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var plot = new Rectangle(64, 26, Width - 88, Height - 68);
            if (plot.Width < 30 || plot.Height < 30)
            {
                return;
            }

            if (_items.Count == 0)
            {
                using (var brush = new SolidBrush(Theme.Muted))
                using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    g.DrawString("Chưa có dữ liệu trong khoảng thời gian này", Theme.Font(10f), brush,
                        new RectangleF(0, 0, Width, Height), format);
                }
                return;
            }

            decimal max = 0m;
            foreach (BarItem item in _items)
            {
                if (item.Value > max)
                {
                    max = item.Value;
                }
            }
            if (max <= 0m)
            {
                max = 1m;
            }

            using (var gridPen = new Pen(Color.FromArgb(240, 234, 226)))
            using (var textBrush = new SolidBrush(Theme.Muted))
            using (var axisBrush = new SolidBrush(Theme.Border))
            {
                for (int i = 0; i <= 4; i++)
                {
                    int y = plot.Bottom - (plot.Height * i / 4);
                    g.DrawLine(gridPen, plot.Left, y, plot.Right, y);
                    string label = Utils.ShortMoney(max * i / 4m);
                    SizeF size = g.MeasureString(label, Font);
                    g.DrawString(label, Font, textBrush, plot.Left - size.Width - 8, y - size.Height / 2);
                }

                float slot = (float)plot.Width / _items.Count;
                float barWidth = Math.Max(4f, slot * 0.56f);
                int labelStep = Math.Max(1, (int)Math.Ceiling(_items.Count / 10f));

                for (int i = 0; i < _items.Count; i++)
                {
                    BarItem item = _items[i];
                    float height = (float)(item.Value / max) * plot.Height;
                    if (item.Value > 0m && height < 2f)
                    {
                        height = 2f;
                    }

                    float x = plot.Left + i * slot + (slot - barWidth) / 2f;
                    var bar = new RectangleF(x, plot.Bottom - height, barWidth, height);

                    using (var brush = new LinearGradientBrush(
                        new RectangleF(bar.X, bar.Y, Math.Max(bar.Width, 1f), Math.Max(bar.Height, 1f)),
                        Theme.Accent, Color.FromArgb(224, 190, 136), LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(brush, bar);
                    }

                    if (item.Value > 0m && slot >= 34f)
                    {
                        string valueText = Utils.ShortMoney(item.Value);
                        SizeF valueSize = g.MeasureString(valueText, Font);
                        if (valueSize.Width < slot)
                        {
                            g.DrawString(valueText, Font, textBrush,
                                x + barWidth / 2f - valueSize.Width / 2f, bar.Y - valueSize.Height - 2);
                        }
                    }

                    if (i % labelStep == 0)
                    {
                        SizeF labelSize = g.MeasureString(item.Label, Font);
                        g.DrawString(item.Label, Font, textBrush,
                            x + barWidth / 2f - labelSize.Width / 2f, plot.Bottom + 6);
                    }
                }

                g.FillRectangle(axisBrush, plot.Left, plot.Bottom, plot.Width, 1);
            }
        }
    }
}
