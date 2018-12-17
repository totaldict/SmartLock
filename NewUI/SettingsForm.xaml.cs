using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SmartLock;
using System.IO;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;

namespace NewUI
{
    /// <summary>
    /// Логика взаимодействия для SettingsForm.xaml
    /// </summary>
    public partial class SettingsForm : Window
    {
        string dir;
        public FixedKey fix;       //пока тут создаём обьект эталонного ключа
        List<FixedKey> coll = new List<FixedKey>();
        public SettingsForm()
        {
            InitializeComponent();
            dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\TestKey";
            System.IO.Directory.CreateDirectory(dir);
        }

        private void NewPic()//очистка формы для рисования
        {
            inkcanvasSet.Strokes.Clear();
        }
        private void SaveFixKeys_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)inkcanvasSet.ActualWidth, (int)inkcanvasSet.ActualHeight, 96d, 96d, PixelFormats.Default);
                rtb.Render(inkcanvasSet);
                BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));
                string date = DateTime.Now.ToString().Replace(".", "").Replace(":", "").Replace(" ", "");
                string path = $@"{dir}\fixkey{date}.bmp";  //путь сохранения
                FileStream fs = File.Open(path, FileMode.Create);//сохраняем рисунок ключа
                encoder.Save(fs);
                Bitmap bmp = new Bitmap(fs);
                fs.Close();

                bool[,] arrFilled = MainWindow.BmpToMatrix(bmp);//переводим в вид матрицы
                fix = new FixedKey(DateTime.Now, arrFilled);
                coll.Add(fix);        //коллекция вводимых ключей                          
                //сохраняем параметры ключей в коллекцию
                fs = new System.IO.FileStream($@"{dir}\collection.ini", System.IO.FileMode.Create);
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


        private void ClrKey_Click(object sender, RoutedEventArgs e)
        {
            NewPic();
        }
    }
}
