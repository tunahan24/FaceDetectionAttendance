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
using FaceDetectionAttendance.MVVM.Model;
using Microsoft.Data.SqlClient;
namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for AdninMenu.xaml
    /// </summary>
    public partial class AdninMenu : Page
    {
        private Dataconnecttion Dataconnecttion = new Dataconnecttion();
        private System.Data.SqlClient.SqlCommand sqlcommand;
        public AdninMenu(string username)
        {
            InitializeComponent();
            setinfor(username);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Content.NavigationService.Navigate(new ManageAccountUI());
        }
        private void Requestbtn_click(object sebnder, RoutedEventArgs e)
        {
            Content.NavigationService.Navigate(new RequestUI());
        }
        void setinfor(string username)
        {
            string querry = "Select images from Account where username = @username";
            if (Dataconnecttion.GetConnection().State == System.Data.ConnectionState.Closed)
                Dataconnecttion.GetConnection().Open();
            SqlCommand cmd = new SqlCommand(querry, Dataconnecttion.GetConnection());
            cmd.Parameters.AddWithValue("@username", username);
            BitmapImage source = new BitmapImage();
            source.BeginInit();
            source.UriSource = new Uri(@"/Resource/Avatar/" + Convert.ToString(cmd.ExecuteScalar()) + ".png", UriKind.RelativeOrAbsolute);
            source.EndInit();
            avt.Source = source;
            Name.Text = username;
        }
    }
}
