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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Drawing;
using SmartLock;
using System.Runtime.Serialization.Formatters.Binary;


namespace NewUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public FixedKey fix;
        public string dir;//путь сохранения
        public MainWindow()
        {
            InitializeComponent();
            //узнаём где находимся, проверяем есть ли папка, если нет - создаём
            dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\TestKey";
            System.IO.Directory.CreateDirectory(dir);
        }

        private void btnChkKey_Click(object sender, RoutedEventArgs e)  //проверка ключа
        {
            textBox1.Text = null;
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)inkcanvas.ActualWidth, (int)inkcanvas.ActualHeight, 96d, 96d, PixelFormats.Default);
            rtb.Render(inkcanvas);
            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            string date = DateTime.Now.ToString().Replace(".", "").Replace(":", "").Replace(" ", "");
            string path = $@"{dir}\testkey{date}.bmp";  //путь сохранения
            FileStream fs = File.Open(path, FileMode.Create);//сохраняем рисунок ключа
            encoder.Save(fs);
            Bitmap bmp = new Bitmap(fs);
            fs.Close();

            List<FixedKey> fix = new List<FixedKey>();
            bool[,] arrFilled = BmpToMatrix(bmp);//переводим в вид матрицы
            TestKey newKey = new TestKey(DateTime.Now, arrFilled);
            //чтение списка эталонных ключей
            fs = new System.IO.FileStream($@"{dir}\collection.ini", System.IO.FileMode.Open);
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
            inkcanvas.Strokes.Clear();  //очищаем поле ввода ключа
        }
        public static bool[,] BmpToMatrix(Bitmap b)
        {
            string str = b.GetPixel(1, 1).Name;
            string str2 = b.GetPixel(2, 2).Name;

            // Считываем в массив заполненных пикселей (медленно), в след. итерации попробовать Bitmap.LockBits 
            bool[,] pixels = new bool[b.Width, b.Height];
            for (int i = 0; i < b.Width; ++i)
            {
                for (int j = 0; j < b.Height; ++j)
                {
                    if (b.GetPixel(i, j).Name == "ffb20000")
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
        

        private void btnSettings_Click(object sender, RoutedEventArgs e) //вызов окна настроек
        {
            SettingsForm frm2 = new SettingsForm();
            frm2.Show();
        }
    }
}
