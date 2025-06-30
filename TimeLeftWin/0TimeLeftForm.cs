using System;
using System.Drawing;
using System.Windows.Forms;

public class TimeLeftForm : Form
{
    private System.Windows.Forms.Timer timer;

    public TimeLeftForm()
    {
        this.Text = "Time Left Today";
        this.ClientSize = new Size(900, 900);
        this.BackColor = Color.Black;

        timer = new System.Windows.Forms.Timer();
        timer.Interval = 1000; // 1秒ごと
        timer.Tick += (s, e) => this.Invalidate();
        timer.Start();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;

        DateTime now = DateTime.Now;
        DateTime endOfDay = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, 999);
        TimeSpan timeLeft = endOfDay - now;
        int hoursLeft = (int)timeLeft.TotalHours;
        int minutesLeft = timeLeft.Minutes;
        int secondsLeft = timeLeft.Seconds;

        // 時間（200px四方、4列）
        using (Brush hourBrush = new SolidBrush(ColorTranslator.FromHtml("#4a90e2")))
        {
            for (int i = 0; i < hoursLeft; i++)
            {
                int row = i / 4;
                int col = i % 4;
                int x = col * 200;
                int y = row * 200;
                g.FillRectangle(hourBrush, x, y, 200, 200);
            }
        }

        // 分（60px四方、15列）
        int minuteStartY = ((hoursLeft + 3) / 4) * 200;
        using (Brush minuteBrush = new SolidBrush(ColorTranslator.FromHtml("#50d2c2")))
        {
            for (int i = 0; i < minutesLeft; i++)
            {
                int row = i / 15;
                int col = i % 15;
                int x = col * 60;
                int y = minuteStartY + (row * 60);
                g.FillRectangle(minuteBrush, x, y, 60, 60);
            }
        }

        // 秒（10px四方、90列）
        int secondStartY = minuteStartY + ((minutesLeft + 14) / 15) * 60;
        using (Brush secondBrush = new SolidBrush(ColorTranslator.FromHtml("#e74c3c")))
        {
            for (int i = 0; i < secondsLeft; i++)
            {
                int row = i / 90;
                int col = i % 90;
                int x = col * 10;
                int y = secondStartY + (row * 10);
                g.FillRectangle(secondBrush, x, y, 10, 10);
            }
        }
    }

    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.Run(new TimeLeftForm());
    }
} 