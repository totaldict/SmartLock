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

namespace UISmartLock
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void image_MouseMove(object sender, MouseEventArgs e)
        {
            //Color currentColor = Color.Black;     //ПОменять код под WPF
            //Pen blackPen = new Pen(currentColor, 1);

            //// Рисуем 
            //Graphics g = Graphics.FromImage(PicBox.Image);

            
            //g.DrawLine(blackPen, e.Location, e.Location);

            //blackPen.Dispose();
            //g.Dispose();

            //PicBox.Invalidate();
        }
    }
}
