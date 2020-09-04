using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBMReportManager.CustomControls
{
  public class DBMProgressBar : ProgressBar
  {
    public DBMProgressBar ()
    {
      this.SetStyle (ControlStyles.UserPaint, true);
    }

    protected override void OnPaint (PaintEventArgs e)
    {
      Rectangle rec = e.ClipRectangle;

      rec.Width = (int)(rec.Width * ((double)Value / Maximum)) - 4;

      if (ProgressBarRenderer.IsSupported)
      {
        ProgressBarRenderer.DrawHorizontalBar (e.Graphics, e.ClipRectangle);
      }

      rec.Height = rec.Height - 4;
      e.Graphics.FillRectangle (Brushes.Crimson, 2, 2, rec.Width, rec.Height);
      e.Graphics.DrawString ($"{this.Value} / {this.Maximum}", SystemFonts.DefaultFont, Brushes.Black, (rec.X + this.Width / 2) - 17 , (rec.Y + this.Height / 2) - 6);
                                                                               
    }
  }
}
