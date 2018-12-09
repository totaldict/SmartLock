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
        Bitmap bm;
        Bitmap bm2;
        public Form1()
        {            
            InitializeComponent();
            NewPic();
           
        }
        /// <summary>
        /// Очищает поле изображения ключа
        /// </summary>
        private void NewPic()
        {
            PicBox.Image = null;
            bm = new Bitmap(PicBox.Width, PicBox.Height);
            bm2 = new Bitmap(PicBox.Width, PicBox.Height);
            var g = Graphics.FromImage(bm);
            g.Clear(Color.White);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!drawing) return;               //тут 3 процедуры на рисование - движение мыши, нажатая кнопка и отжатая
            var finish = new Point(e.X, e.Y);
            bm2 = new Bitmap(bm);
            PicBox.Image = bm2;
            var pen = new Pen(Color.Black, 5f);
            var g = Graphics.FromImage(bm2);
            g.DrawLine(pen, start, finish);
            g.Dispose();
            PicBox.Invalidate();
        }

        private void PicBox_MouseDown(object sender, MouseEventArgs e)
        {
            start = new Point(e.X, e.Y);        //для рисования
            drawing = true;
        }

        private void PicBox_MouseUp(object sender, MouseEventArgs e)
        {
            var finish = new Point(e.X, e.Y);       //для рисования
            var g = Graphics.FromImage(bm);
            var pen = new Pen(Color.Black, 5f);
            g.DrawLine(pen, start, finish);
            g.Save();
            drawing = false;
            g.Dispose();
            PicBox.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {       //сохраняем рисунок ключа
            string date = DateTime.Now.ToString().Replace(".", "").Replace(":","").Replace(" ","");
            string path =$@"D:\TestKey\test{date}.bmp";
            PicBox.Image.Save(path, System.Drawing.Imaging.ImageFormat.Bmp);
            
            // Считываем в массив цветов (медленно), в след. итерации попробовать Bitmap.LockBits 
            bool[,] pixels = new bool[bm2.Width, bm2.Height];
            for (int i = 0; i < bm2.Width; ++i)
            {
                for (int j = 0; j < bm2.Height; ++j)
                {
                    if (bm2.GetPixel(i, j).Name == "ff000000")
                        pixels[i, j] = true;
                    else
                        pixels[i, j] = false;
                }
            }
        }

        private void btnClr_Click(object sender, EventArgs e)
        {
            NewPic();
        }
    }
}
