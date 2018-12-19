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
using static SmartLock.XMLFileSettings;
using Microsoft.Win32;
using System.Windows.Forms;

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
        private Props props = new Props(); //экземпляр класса с настройками 

        public SettingsForm()
        {
            InitializeComponent();
            LogWrite("Открыто окно настроек.");
            LoadSett();
        }

        private void LoadSett() //загрузка настроек в окно настроек при старте
        {
            dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\TestKey";
            System.IO.Directory.CreateDirectory(dir);
            if (System.IO.File.Exists($@"{dir}\settings.xml"))
                readSetting();  //при создании окна читаются настройки из xml-файла, если он существует))
            
        }
        private void LogWrite(string str)   //записывает строку лога
        {
            string logLine = $"{DateTime.Now.ToString()}\t{str}{Environment.NewLine}";
            File.AppendAllText($@"{dir}\log.txt", logLine); //если файла лога нет, то создаём новый, если есть - дозаписываем в него и закрываем
        }
        private void NewPic()//очистка формы InkCanvas для рисования
        {
            inkcanvasSet.Strokes.Clear();
        }

        private void SaveFixKeys_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Bitmap bmp = MakeBmpFromCanvas();   //Получаем рисунок из InkCanvas
                bool[,] arrFilled = MainWindow.BmpToMatrix(bmp);//переводим в вид матрицы
                SerializeFixKeys(arrFilled);    //создаём новый эталонный ключ и добавляем к нему матрицу
                NewPic();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                LogWrite($"Ошибка при создании эталонного ключа.\n{ex.Message}");
            }
        }

        private Bitmap MakeBmpFromCanvas()
        {   //Созраняем файл эталонного ключа и получаем BMP
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
            LogWrite($@"Ключ fixkey{date}.bmp записан в каталог {dir}");
            return bmp;
        }
        private void SerializeFixKeys(bool[,] arrFilled)
        {   //сохраняем эталонный ключ в коллекцию
            fix = new FixedKey(DateTime.Now, arrFilled);
            coll.Add(fix);        //коллекция вводимых эталонных ключей   
            FileStream fs = new System.IO.FileStream($@"{dir}\collection.ini", System.IO.FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            for (int i = 0; i < coll.LongCount(); i++)  //коллекцию сериализуем в файл collection.ini
                bf.Serialize(fs, coll[i]);
            fs.Close();
            LogWrite($@"Коллекция ключей записана в файл {dir}\collection.ini.");
        }

        private void ClrKey_Click(object sender, RoutedEventArgs e)
        {
            NewPic();
        }

        #region Настройки в XML
        
        //Запись настроек
        private void writeSetting()
        {
            if (bgroundTBox.Text == "" || kFolderTBox.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("Задайте рисунок и папку сохранения ключей.");
                LogWrite("При сохранении настроек возникла ошибка - не задан путь к заднему рисунку/не задана папка сохранения ключей.");
                return;
            }
            //Запись значений login/pass/background/места хранения ключей
            props.Fields.login = loginTBox.Text;
            props.Fields.pass = passTBox.Text;
            props.Fields.bground = bgroundTBox.Text;
            props.Fields.kFolder = kFolderTBox.Text;
            props.WriteXml();
            LogWrite($"Настройки программы записаны в файл {props.Fields.XMLFileName}.");
        }
        //Чтение настроек
        private void readSetting()
        {
            props.ReadXml();
            loginTBox.Text = props.Fields.login;
            passTBox.Text = props.Fields.pass;
            bgroundTBox.Text = props.Fields.bground;
            kFolderTBox.Text = props.Fields.kFolder;
            dir = kFolderTBox.Text;         //меняем папку сохранения эталонных ключей
            image.Source =new BitmapImage(new Uri($"{bgroundTBox.Text}"));  //меняем картинку
            LogWrite($"Настройки программы прочитаны из файла {props.Fields.XMLFileName}.");
        }
        #endregion

        private void SaveSettBtn_Click(object sender, RoutedEventArgs e)//сохранение настроек в xml
        {
            writeSetting();
        }


        private void bgBtn_Click(object sender, RoutedEventArgs e)//выбор другой картинки
        {
            Microsoft.Win32.OpenFileDialog myDialog = new Microsoft.Win32.OpenFileDialog();
            myDialog.Filter = "Картинки(*.JPG;*.GIF)|*.JPG;*.GIF" + "|Все файлы (*.*)|*.* ";
            myDialog.CheckFileExists = true;
            myDialog.Multiselect = false;
            if (myDialog.ShowDialog() == true)
            {
                bgroundTBox.Text = myDialog.FileName;   //меняем путь к картинке
                image.Source = new BitmapImage(new Uri($"{bgroundTBox.Text}"));  //меняем картинку
            }
            LogWrite($"Изменена картинка заднего фона на {bgroundTBox.Text}.");
        }

        private void KeyFolderBtn_Click(object sender, RoutedEventArgs e)   //меняем путь к папке сохранения ключей
        {
            FolderBrowserDialog fldDialog = new FolderBrowserDialog();
            fldDialog.ShowDialog();   
            kFolderTBox.Text = fldDialog.SelectedPath;
            LogWrite($"Изменена папка хранения ключей на {kFolderTBox.Text}.");
        }

    }
}
