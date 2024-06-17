using FaceDetectionAttendance.MVVM.View;
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

namespace FaceDetectionAttendance.MVVM.Components
{
    /// <summary>
    /// Interaction logic for MessageBoxChangeUI.xaml
    /// </summary>
    public partial class MessageBoxChangeUI : Page
    {
        private int stt;
        private string username;
        public MessageBoxChangeUI(string username,int stt)
        {
            InitializeComponent();
            this.username = username;
            this.stt = stt; 
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (stt == 1)
            {
                MainWindow window1 = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                MessageBoxChange ms = App.Current.Windows.OfType<MessageBoxChange>().FirstOrDefault();
                window1.Start.NavigationService.Navigate(new AdninMenu(username));
                ms.Close();
            }
            else
            {
                MainWindow window1 = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                MessageBoxChange ms = App.Current.Windows.OfType<MessageBoxChange>().FirstOrDefault();
                window1.Start.NavigationService.Navigate(new MenuStaff(username));
                ms.Close();
            }
        }
    }
}
