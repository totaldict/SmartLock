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
using System.IO.Ports;
using System.Threading;
using System.Net.Mail;
using System.Net;

namespace NewUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<FixedKey> fix;
        public string dir;//путь сохранения ключей
        public string sysDir; //системный путь
        private SerialPort port;
        private Props propsOpen = new Props(); //экземпляр класса с настройками 
        bool stopStatus = false;
        int failsCount;                        //количество неудачных попыток ввода ключа
        public MainWindow()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            if (LoadSettXMLMainWindow())                //если загружены настройки
                PortStatusAsync();                      //поддерживает подключение и сообщает статус
            LogWrite("Запущено окно MainWindow.");      //лог
        }

        public bool LoadSettXMLMainWindow()
        {
            //узнаём где находимся, проверяем есть ли папка, если нет - создаём
            sysDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\TestKey";
            System.IO.Directory.CreateDirectory(sysDir);
            if (System.IO.File.Exists($@"{sysDir}\settings.xml"))
            {
                readSetting(); //если есть файл настроек-грузим их.
                return true;
            }
            else
                return false;
        }

        //Чтение настроек из xml-файла
        private void readSetting()
        {
            try
            {
                propsOpen.ReadXml();
                string bgimage = propsOpen.Fields.bground;
                dir = propsOpen.Fields.kFolder;
                failsCount = propsOpen.Fields.fails;
                OpenPort(propsOpen.Fields.port);   //открываем порт
                image.Source = new BitmapImage(new Uri($"{bgimage}"));  //меняем картинку
                LogWrite($"Прочитаны настройки из XML-файла {propsOpen.Fields.XMLFileName}");//лог
            }
            catch (Exception ex)
            {
                LogWrite("Невозможно прочитать настройки.");
            }
        }
        private void OpenPort(string portNr)    //открываем порт с заданным номером
        {
            try
            {
                port = new SerialPort(portNr, 9600, Parity.None, 8, StopBits.One);   //инициализируем порт с настройками
                port.Open();    //открываем порт
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"В настройках указан неверный порт соединения.\n{ex.Message}");
            }
        }
        private void LogWrite(string str)   //записывает строку лога
        {
            string logLine = $"{DateTime.Now.ToString()}\t{str}{Environment.NewLine}";
            File.AppendAllText($@"{sysDir}\log.txt", logLine); //если файла лога нет, то создаём новый, если есть - дозаписываем в него и закрываем
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
            LogWrite($@"Ключ testkey{date}.bmp записан в каталог {dir}");
            return bmp;
        }

        private List<FixedKey> TakeFixKey() //чтение коллекции эталонных ключей из файла
        {
            List<FixedKey> fix = new List<FixedKey>();
            FileStream fs = null;
            try
            {   //пытаемся прочитать коллекцию эталонных ключей
                fs = new System.IO.FileStream($@"{sysDir}\collection.ini", System.IO.FileMode.Open);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Отсутствует файл эталонных ключей.\n{ex.Message}");
                LogWrite($"Отсутствует файл эталонных ключей.\n{ex.Message}");
                return fix = null;    //если файл ключей не найден - возвращаем пустую коллекцию
            }
            BinaryFormatter bf = new BinaryFormatter();
            fix.Clear();//очищаем коллекцию перед записью из файла
            do
            {
                fix.Add((FixedKey)bf.Deserialize(fs));
            } while (fs.Position < fs.Length);
            fs.Close();
            LogWrite($@"Файл эталонных ключей прочитан из {dir}\collection.ini");
            return fix;
        }
        private void btnChkKey_Click(object sender, RoutedEventArgs e)  //проверка ключа
        {
            if (dir == null)        //если не прочитаны настройки (некуда сохранять ключи) - выходим
            {
                System.Windows.MessageBox.Show($"Невозможно сохранить ключи.\nЛибо не читаются настройки, либо указана некорректная дирректория сохранения.");
                return;
            }
            bool opened = false;
            textBox1.Text = null;//##окно для вывода % совпадения ключей, потом убрать
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
                if (ck >= 98 && !opened)
                {
                    OpenLock();                                //##Подумать над механизмом разрешения!!!
                    opened = true;
                    failsCount = 0;                             //обнуляем счётчик неудачных вводов
                }
                i++;
            }
            LogWrite($"Произведено сравнение ключа c эталонным.");
            if (!opened)
                failsCount++;
            if (failsCount >= 3)            //если неудачных попыток ввода >=3
                AlertMail(failsCount);

            inkcanvas.Strokes.Clear();  //очищаем поле ввода ключа
        }
        public static bool[,] BmpToMatrix(Bitmap b)
        {
            bool[,] pixels = PixelMatrix(b);    //переводим рисунок ключа в матрицу пикселей
            int[,] summFilled = PixelCountInCell(pixels);//матрица количества закрашеных пикселей в каждом квадрате 30*25
            bool[,] arrFilled = DecisionPainted(summFilled);//матрица со значениями bool, нарисовано ли в каждом квадрате 30*25
            return arrFilled;
        }

        public static bool[,] PixelMatrix(Bitmap b)
        {   // ##Считываем в массив заполненных пикселей (медленно), в след. итерации попробовать Bitmap.LockBits
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
            return pixels;
        }
        public static int[,] PixelCountInCell(bool[,] pixels)
        {   //Создаём матрицу количества закрашеных пикселей в каждом квадрате 30*25
            int[,] summFilled = new int[10, 10];
            int summ = 0;
            for (int i = 0; i < pixels.GetLength(0) / 30; i++)     //##НА СЛЕД.ИТЕРАЦИИ СДЕЛАТЬ параметр регулировки шага сетки
            {
                for (int j = 0; j < pixels.GetLength(1) / 25; j++)
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
            return summFilled;
        }
        public static bool[,] DecisionPainted(int[,] summFilled)
        {   //По количеству закрашенных пикселей в каждой ячейке решаем, закрашена ли она
            bool[,] arrFilled = new bool[10, 10];//матрица со значениями bool, нарисовано ли в каждом квадрате 30*25
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    if (summFilled[i, j] > 50)      //##тут параметр закрашенности ячейки. если больше его - ячейка закрашена
                        arrFilled[i, j] = true;
                    else
                        arrFilled[i, j] = false;
                }
            return arrFilled;
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e) //вызов окна авторизации для входа в настройки
        {
            stopStatus = true;
            AuthWindow authFrm = new AuthWindow();
            authFrm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            authFrm.Show();
        }
        private void OpenLock()
        {
            try
            {
                port.Write("#x|#O|#w|"); //Передаём О для открытия на 5 сек, задано в скетче, x-вкл.светодиод, w-выкл. светодиод.
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Невозможно связаться с замком.\n{ex.Message}");
                //##Логирование ошибки
            }
        }

        private async void PortStatusAsync()
        {
            //##Нашёл баг - фоновая задача статуса порта не завершается при закрытии осн. формы и открытии меню настроек. 
            //вместо этого попробовать
            //Thread t = new Thread(new ThreadStart(DoubThread));
            //t.IsBackground = true;
            //t.Start();
            try
            {
                do              //бесконечный цикл
                {
                    PortName.Content = port.PortName;       //имя порта пишем
                    string message = await Task.Factory.StartNew(() => PortStatus(), TaskCreationOptions.LongRunning);
                    if (message == "ok")
                        StatusColor.Fill = new SolidColorBrush(Colors.GreenYellow);
                    else
                        StatusColor.Fill = new SolidColorBrush(Colors.Red);
                } while (!stopStatus);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Невозможно связаться с замком.\n{ex.Message}");
                //##Логирование ошибки
            }
        }
        public string PortStatus()   //функция проверки статуса порта
        {
            Thread.Sleep(1000);     //засыпаем на 5с.
            string status = null;
            if (port.IsOpen)    //если порт открыт
            {
                try
                {
                    status = port.ReadLine();   //читаем порт
                    return status;
                }
                catch (Exception ex)
                {
                    //##Логирование
                }
            }
            else    //если порт закрыт
            {
                status = "close";
                try
                {
                    port.Open();    //если порт закрыт - пытаемся его открыть.
                }
                catch (Exception ex)
                {
                    //##Сюда логирование можно что не удаётся открыть порт
                }
            }
            return status;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            propsOpen.Fields.fails = failsCount;
            propsOpen.WriteXml();
            if (port != null)
                if (port.IsOpen) port.Close();   //закрываем порт если открыт при закрытии формы
            
        }
        //отправляет письмо о неудачных попытках ввода ключа
        private void AlertMail(int fails)
        {
            SendEmailAsync(fails, propsOpen.Fields.mailFrom, 
                                  propsOpen.Fields.mailPass, 
                                  propsOpen.Fields.mailSmtp, 
                                  propsOpen.Fields.mailPort, 
                                  propsOpen.Fields.mailTo).GetAwaiter();
        }
        /// <summary>
        /// Асинхронная отправка сообщения на почту
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private static async Task SendEmailAsync(int n, string mailFrom, string mailPass, string mailSmtp, string mailPort, string mailTo)
        {
            try
            {
                MailAddress from = new MailAddress(mailFrom);
                MailAddress to = new MailAddress(mailTo);
                MailMessage m = new MailMessage(from, to);
                m.Subject = "Введён неверный ключ.";
                m.Body = $"Выполнено {n} не успешных попыток ввода ключа. {DateTime.Now}";
                SmtpClient smtp = new SmtpClient(mailSmtp, int.Parse(mailPort));
                smtp.Credentials = new NetworkCredential(mailFrom, mailPass);
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(m);
            }
            catch (Exception ex)
            {

            }
        }















    }
}
