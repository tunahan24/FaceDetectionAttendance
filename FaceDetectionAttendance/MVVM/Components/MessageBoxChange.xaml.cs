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
using FaceDetectionAttendance.MVVM.View;

namespace FaceDetectionAttendance.MVVM.Components
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MessageBoxChange : Window
    {
        MainWindow mainWindow = new MainWindow();
        private int stt;
        private string username;
        public MessageBoxChange(int s, string username)
        {
            InitializeComponent();
            stt=s;
            this.username = username;
            NavToUI.NavigationService.Navigate(new MessageBoxChangeUI(username,stt));
        }
       
    }
}
