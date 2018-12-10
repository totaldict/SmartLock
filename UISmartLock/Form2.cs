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
using System.IO;

namespace UISmartLock
{
    public partial class Form2 : Form
    {
        private Point start;
        private bool drawing = false;
        Bitmap bm;
        Bitmap bm2;
        public FixedKey fix;       //пока тут создаём обьект эталонного ключа
        List<FixedKey> coll = new List<FixedKey>();
        
        public Form2()
        {
            InitializeComponent();
            NewPic();
            //устанавливаем путь по умолчанию
            SaveTBox.Text= System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\TestKey";
        }
        private void NewPic()
        {
            PicBoxAdmin.Image = null;
            bm = new Bitmap(PicBoxAdmin.Width, PicBoxAdmin.Height);
            bm2 = new Bitmap(PicBoxAdmin.Width, PicBoxAdmin.Height);
            var g = Graphics.FromImage(bm);
            g.Clear(Color.White);       //устанавливает белый задний фон
        }
        private void button1_Click(object sender, EventArgs e)  //сохранение эталонного ключа
        {
            try
            {
                
                Form1.SaveBmp(bm2, $@"{SaveTBox.Text}\fixed");//сохраняем рисунок ключа
                bool[,] arrFilled = Form1.BmpToMatrix(bm2);//переводим в вид матрицы
                fix = new FixedKey(DateTime.Now, arrFilled);
                coll.Add(fix);        //коллекция вводимых ключей                          

                System.IO.FileStream fs = new System.IO.FileStream($@"{SaveTBox.Text}\collection.ini", System.IO.FileMode.Create);
                BinaryFormatter bf = new BinaryFormatter();
                for (int i = 0; i < coll.LongCount(); i++)
                    bf.Serialize(fs, coll[i]);
                fs.Close();
                NewPic();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void PicBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (!drawing) return;               //тут 3 процедуры на рисование - движение мыши, нажатая кнопка и отжатая
            var finish = new Point(e.X, e.Y);
            bm2 = new Bitmap(bm);
            PicBoxAdmin.Image = bm2;
            var pen = new Pen(Color.Black, 5f);
            var g = Graphics.FromImage(bm2);
            g.DrawLine(pen, start, finish);
            g.Dispose();
            PicBoxAdmin.Invalidate();
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
            PicBoxAdmin.Invalidate();
        }

        private void SavePathBtn_Click(object sender, EventArgs e)
        {
            try
            {
                folderBrowserDialog1.ShowDialog();
                SaveTBox.Text = folderBrowserDialog1.SelectedPath;
                using (StreamWriter sw = new StreamWriter($@"{SaveTBox.Text}\data.ini", false, System.Text.Encoding.Default))
                {
                    sw.WriteLine(SaveTBox.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //     for (int i = 0; i< 10; i++)        //ОТЛАДКА, можно добавить в админское меню
        //        {
        //            for (int j = 0; j< 10; j++)
        //            {
        //                if (arrFilled[j, i] == true)
        //                    rsltBox.Text += "*  ";
        //                else
        //                    rsltBox.Text += "   ";
        //            }
        //rsltBox.Text += Environment.NewLine;
        //        }


    }
}
