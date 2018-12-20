using System;
using System.Collections.Generic;
using System.IO;
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
using static SmartLock.XMLFileSettings;

namespace NewUI
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        private Props props = new Props(); //экземпляр класса с настройками
        private string dir;    
        public AuthWindow()
        {
            InitializeComponent();
            ResizeMode = ResizeMode.NoResize;   //фиксируем размеры формы - запрет изменения
            LoadSett();     //загружаем настройки
        }
        private void LogWrite(string str)   //записывает строку лога
        {
            string logLine = $"{DateTime.Now.ToString()}\t{str}{Environment.NewLine}";
            File.AppendAllText($@"{dir}\log.txt", logLine); //если файла лога нет, то создаём новый, если есть - дозаписываем в него и закрываем
        }
        private void LoadSett() //загрузка настроек в окно авторизации
        {
            dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\TestKey";
            System.IO.Directory.CreateDirectory(dir);
            if (System.IO.File.Exists($@"{dir}\settings.xml"))
                props.ReadXml();  //при создании окна читаются настройки из xml-файла, если он существует))

        }
        private void StartSettingsWindow()    //Запуск окна настроек
        {
            LogWrite("Открытие меню настроек.");
            SettingsForm settFtm = new SettingsForm();
            settFtm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            settFtm.Show();
            this.Close();
        }
        private void LogInBtn_Click(object sender, RoutedEventArgs e)   //кнопка проверки логин/пароль
        {
            if (System.IO.File.Exists($@"{dir}\settings.xml"))  //если файл настроек существует - проверяем логин/пароль из него
            {
                if ((props.Fields.login == LoginTBox.Text) && (props.Fields.pass == PassTBox.Text))     //если совпадают - пускает в настройки
                {
                    LogWrite("Введены верные логин/пароль.");
                    StartSettingsWindow();
                }
                else
                {
                    resultLbl.Content = "Неверные логин/пароль.";
                    LogWrite($"Введены неверные логин:{LoginTBox.Text} и пароль:{PassTBox.Text}.");
                }
            }
            else    //если файла настроек нет -просто открываем настройки
            {       //##тут подумать, может если нет настроек - наоборот нельзя пускать
                StartSettingsWindow();
                LogWrite($@"Файл настроек {dir}\settings.xml отсутствует - вход в меню настроек выполнен без проверки логин/пароль.");
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
