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
using static SmartLock.XMLFileSettings;

namespace NewUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<FixedKey> fix;
        public string dir;//путь сохранения
        private Props propsOpen = new Props(); //экземпляр класса с настройками 
        public MainWindow()
        {
            InitializeComponent();
            LoadSettXMLMainWindow();
            LogWrite("Запущено окно MainWindow.");  //лог
        }

        public void LoadSettXMLMainWindow()
        {
            //узнаём где находимся, проверяем есть ли папка, если нет - создаём
            dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\TestKey";
            System.IO.Directory.CreateDirectory(dir);
            if (System.IO.File.Exists($@"{dir}\settings.xml"))
                readSetting(); //если есть файл настроек-грузим их.
        }

        //Чтение настроек из xml-файла
        private void readSetting()
        {
            propsOpen.ReadXml();
            string bgimage = propsOpen.Fields.bground;
            dir = propsOpen.Fields.kFolder;
            image.Source = new BitmapImage(new Uri($"{bgimage}"));  //меняем картинку
            LogWrite($"Прочитаны настройки из XML-файла {propsOpen.Fields.XMLFileName}");//лог
        }
        private void LogWrite(string str)   //записывает строку лога
        {
            string logLine = $"{DateTime.Now.ToString()}\t{str}{Environment.NewLine}";
            //if (!System.IO.File.Exists($@"{dir}\log.txt"))  //если файл лога не существует - создаём новый
                //File.Create($@"{dir}\log.txt");
            File.AppendAllText($@"{dir}\log.txt", logLine);
            
        }
        private Bitmap MakeBmpFromInkCanvas()   //Получение рисунка от InkCanvas и сохранение его
        {
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
                return bmp;
        }

        private List<FixedKey> TakeFixKey() //чтение коллекции эталонных ключей из файла
        {
            List<FixedKey> fix = new List<FixedKey>();
            FileStream fs = null;
            //чтение списка эталонных ключей
            try
            {   //пытаемся прочитать коллекцию эталонных ключей
                fs = new System.IO.FileStream($@"{dir}\collection.ini", System.IO.FileMode.Open);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Отсутствует файл эталонных ключей.\n{ex.Message}");
                return fix;
            }
            BinaryFormatter bf = new BinaryFormatter();
            fix.Clear();//очищаем коллекцию перед записью из файла
            do
            {
                fix.Add((FixedKey)bf.Deserialize(fs));
            } while (fs.Position < fs.Length);
            fs.Close();
            return fix;
        }
        private void btnChkKey_Click(object sender, RoutedEventArgs e)  //проверка ключа
        {
            textBox1.Text = null;//окно для вывода % совпадения ключей, потом убрать
            Bitmap bmp = MakeBmpFromInkCanvas();
            bool[,] arrFilled = BmpToMatrix(bmp);//переводим в вид матрицы тестовый ключ
            TestKey newKey = new TestKey(DateTime.Now, arrFilled);
            fix = TakeFixKey();
            if (fix == null) return;//проверяем на наличие ключей
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
