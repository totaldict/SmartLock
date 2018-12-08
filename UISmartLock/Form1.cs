using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UISmartLock
{
    public partial class Form1 : Form
    {
        

        private Point start;
        private bool drawing = false;
        private Image orig;
        Bitmap bm;
        Bitmap bm2;
        public Form1()
        {            
            InitializeComponent();
            bm = new Bitmap(PicBox.Width, PicBox.Height);
            bm2 = new Bitmap(PicBox.Width, PicBox.Height);

        }
        private void NewPic()
        {
            
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!drawing) return;
            var finish = new Point(e.X, e.Y);
            bm2 = new Bitmap(bm);
            PicBox.Image = bm2;
            var pen = new Pen(Color.Black, 1f);
            var g = Graphics.FromImage(bm2);
            g.DrawLine(pen, start, finish);
            g.Dispose();
            PicBox.Invalidate();
            

        }

        private void PicBox_MouseDown(object sender, MouseEventArgs e)
        {
            start = new Point(e.X, e.Y);
            orig = bm;
            drawing = true;
        }

        private void PicBox_MouseUp(object sender, MouseEventArgs e)
        {
            var finish = new Point(e.X, e.Y);
            var g = Graphics.FromImage(bm);
            var pen = new Pen(Color.Black, 1f);
            g.DrawLine(pen, start, finish);
            g.Save();
            drawing = false;
            g.Dispose();
            PicBox.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //вот тут нормальные имена ключей сделать
            string path = @"D:\TestKey\test"+DateTime.Now.DayOfYear+DateTime.Now.Second+".jpg";
            PicBox.Image.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
            //разобраться с тем что сохраняется чёрная херня вместо изображения
        }
    }
}
