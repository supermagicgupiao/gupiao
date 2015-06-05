using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;

namespace Stock.Controller.DrawController
{
    public class DrawDataController
    {
        Bitmap bmp;
        Graphics g;
        int segtime;
        int seglong;
        int dateLine;
        float fontSize;
        public DrawDataController(int width, int height)
        {
            bmp = new Bitmap(width, height);
            g = Graphics.FromImage(bmp);
            dateLine = bmp.Height - 30;
            fontSize = Math.Min(width, height) / 20;
            segtime = 10;
            seglong = width / segtime;
            SetBackground(Color.White);
        }
        public Bitmap GetImage()
        {
            return bmp;
        }
        public void SetBackground(Color color)
        {
            SolidBrush brush = new SolidBrush(color);
            g.FillRectangle(brush, 0, 0, bmp.Width, bmp.Height);
        }
        private void DrawDate(DateTime date, int day)
        {
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            SolidBrush brush = new SolidBrush(Color.Black);
            Font font = new Font("Times New Roman", fontSize);
            Pen pen = new Pen(brush);
            g.DrawLine(pen, 0, dateLine, bmp.Width, dateLine);
            int l = day / (segtime - 1);
            for (int i = 0; i < segtime - 1; i++)
            {
                g.DrawString(date.AddDays(l * i).ToString("MM/dd"), font, brush, new PointF(seglong * i, dateLine));
            }
            g.DrawString(date.AddDays(day).ToString("MM/dd"), font, brush, new PointF(seglong * (segtime - 1), dateLine));
        }
        public void DrawData(Dictionary<Color, List<DrawDataEntity>> dict,double max)
        {
            if (max == 0)
                return;
            Point[] p;
            int days;
            Brush brush = new SolidBrush(Color.Red);
            Font font = new Font("Times New Roman", fontSize);
            g.DrawString("最高:" + (max / 1000).ToString() + "k", font, brush, new PointF(0, 0));
            int h = dateLine * 3 / 4;
            if(dict.Values.Count>0)
            {
                if (dict.First().Value.Count > 0) 
                {
                    DateTime dt1 = dict.First().Value.First().date;
                    DateTime dt2 = dict.First().Value.Last().date;
                    TimeSpan ts = dt2 - dt1;
                    days = ts.Days;
                    DrawDate(dt1, days);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
            foreach(var x in dict)
            {
                brush = new SolidBrush(x.Key);
                p = new Point[x.Value.Count];
                for (int i = 0; i < x.Value.Count; i++)
                {
                    p[i].X = bmp.Width * i / days;
                    p[i].Y = dateLine - Convert.ToInt32(h * x.Value[i].money / max);
                    //if (p[i].Y > dateLine)
                    //    p[i].Y = dateLine;
                }
                Pen pen = new Pen(brush);
                g.DrawLines(pen, p);
            }
        }
        public void DrawPieChart(List<DrawPieEntity> DPEL)
        {
            Brush brush = new SolidBrush(Color.Black);
            Font font = new Font("Times New Roman", 8);
            Pen pen = new Pen(brush);
            float max = 0;
            float w = bmp.Width * 2 / 3;
            float h = bmp.Height * 2 / 3;
            foreach (DrawPieEntity DPE in DPEL)
            {
                max += (float)DPE.money;
            }
            float d=0;
            int p = 0;
            foreach (DrawPieEntity DPE in DPEL)
            {
                if (DPE.money == 0)
                    continue;
                brush = new SolidBrush(DPE.color);
                float temp = (float)DPE.money / max;
                float t = temp * 360;
                g.FillPie(brush, bmp.Width - w - 5, bmp.Height - h - 5, w, h, d, t);
                g.DrawString(DPE.name + ":" + (temp * 100).ToString() + "%", font, brush, new PointF(0, p * 15));
                p++;
                d += t;
            }
        }
        public void DrawData(List<DrawDataEntity> DDEL)
        {
            int days = 0;
            if (DDEL.Count > 0)
            {
                DateTime dt1 = DDEL.First().date;
                DateTime dt2 = DDEL.Last().date;
                TimeSpan ts = dt2 - dt1;
                days = ts.Days;
                DrawDate(dt1, days);
            }
            else
                return;
            double max = 0;
            double min = 100;
            foreach(DrawDataEntity DDE in DDEL)
            {
                if (max < DDE.money)
                    max = DDE.money;
                if (min > DDE.money)
                    min = DDE.money;
            }
            if (max == 0)
                return;
            Point[] p = new Point[days];
            int h = dateLine * 3 / 4;
            for (int i = 0; i < days; i++)
            {
                p[i].X = bmp.Width * i / days;
                p[i].Y = dateLine - Convert.ToInt32(h * (DDEL[i].money - min) / (max - min));
            } 
            Brush brush = new SolidBrush(Color.Black);
            Pen pen = new Pen(brush);
            Font font = new Font("Times New Roman", fontSize);
            if (min < 0)
            {
                int k = (int)(h * min / (max - min));
                g.DrawLine(pen, 0, dateLine + k, bmp.Width, dateLine + k);
                g.DrawString("0", font, brush, new PointF(0, dateLine + k));
            }
            else
            {
                g.DrawString(min.ToString(), font, brush, new PointF(0, dateLine - fontSize - 2));
            }
            brush = new SolidBrush(Color.Blue);
            pen = new Pen(brush);
            g.DrawLines(pen, p);
            brush = new SolidBrush(Color.Red);
            g.DrawString("盈亏:" + (DDEL.Last().money).ToString() + "%", font, brush, new PointF(0, 0));
        }
    }
    public struct DrawDataEntity
    {
        public DateTime date;
        public double money;
    }
    public struct DrawPieEntity
    {
        public string name;
        public double money;
        public Color color;
    }
}
