using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SmartLock;
using System.Runtime.Serialization.Formatters.Binary;

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
            textBox1.Text = null;
        }
        /// <summary>
        /// Сохранение BMP
        /// </summary>
        /// <param name="o">Рисунок, объект Bitmap</param>
        /// <param name="p">Папка сохранения</param>
        public static void SaveBmp(Bitmap o, string p)
        {
            string date = DateTime.Now.ToString().Replace(".", "").Replace(":", "").Replace(" ", "");
            string path = $@"{p}{date}.bmp";
            o.Save(path, System.Drawing.Imaging.ImageFormat.Bmp);
        }
        /// <summary>
        /// Переводит рисунок в матрицу
        /// </summary>
        /// <param name="b">Рисунок Bitmap</param>
        /// <returns></returns>
        public static bool[,] BmpToMatrix(Bitmap b)
        {
            // Считываем в массив заполненных пикселей (медленно), в след. итерации попробовать Bitmap.LockBits 
            bool[,] pixels = new bool[b.Width, b.Height];
            for (int i = 0; i < b.Width; ++i)
            {
                for (int j = 0; j < b.Height; ++j)
                {
                    if (b.GetPixel(i, j).Name == "ff000000")
                        pixels[i, j] = true;
                    else
                        pixels[i, j] = false;
                }
            }
            //создаём массив для сверки
            bool[,] arrFilled = new bool[10, 10];//матрица со значениями bool, нарисовано ли в каждом квадрате 30*25
            int[,] summFilled = new int[10, 10];//матрица количества закрашеных пикселей в каждом квадрате 30*25
            int summ = 0;
            for (int i = 0; i < b.Width / 30; i++)     //НА СЛЕД.ИТЕРАЦИИ СДЕЛАТЬ параметр регулировки шага сетки
            {
                for (int j = 0; j < b.Height / 25; j++)
                {
                    for (int k = i * 30; k < (i + 1) * 30; k++)
                    {
                        for (int h = j * 25; h < (j + 1) * 25; h++)
                        {
                            if (pixels[k, h] == true) summ++;
                        }
                    }
                    summFilled[i, j] = summ;
                    summ = 0;
                }
            }
            for (int i = 0; i < 10; i++)        //отпечаток ключа, ячейки bool
                for (int j = 0; j < 10; j++)
                {
                    if (summFilled[i, j] > 50)      //тут параметр закрашенности ячейки. если больше его - ячейка закрашена
                        arrFilled[i, j] = true;
                    else
                        arrFilled[i, j] = false;
                }
            
            return arrFilled;
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

        private void button1_Click(object sender, EventArgs e)  //проверка ключа
        {
            List<FixedKey> fix = new List<FixedKey>();
            try
            {
                SaveBmp(bm2, @"D:\TestKey\test");//сохраняем рисунок ключа
                bool[,] arrFilled = BmpToMatrix(bm2);//переводим в вид матрицы
                TestKey newKey = new TestKey(DateTime.Now, arrFilled);
                //чтение списка эталонных ключей
                System.IO.FileStream fs = new System.IO.FileStream(@"D:\TestKey\collection.ini", System.IO.FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                fix.Clear();//очищаем коллекцию перед записью из файла
                do
                {
                    fix.Add((FixedKey)bf.Deserialize(fs));
                } while (fs.Position < fs.Length);
                fs.Close();
                int i = 1;
                foreach (FixedKey f in fix)
                {
                    int ck = newKey.CheckTestKey(newKey.matrix, f.matrix);
                    textBox1.Text += $"{i}) {ck}%" + Environment.NewLine;
                    i++;
                }
               // NewPic();   //очистка поля после проверки
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnClr_Click(object sender, EventArgs e)
        {
            NewPic();
        }


        private void authToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 frm2 = new Form2();
            frm2.Show();
        }

        
    }
}
