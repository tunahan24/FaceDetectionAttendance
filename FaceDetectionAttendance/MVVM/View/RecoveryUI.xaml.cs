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
using System.Net.Mail;
using Microsoft.Data.SqlClient;
using System.Net;
using FaceDetectionAttendance.MVVM.Model;

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for RecoveryUI.xaml
    /// </summary>
    public partial class RecoveryUI : Page
    {
        private Dataconnecttion dataconnecttion = new Dataconnecttion();
        public RecoveryUI()
        {
            InitializeComponent();
        }

        private void RecovBtn_Click(object sender, RoutedEventArgs e)
        {
            string querry = "Select count(1) from Account inner join Faculty on Account.fid = Faculty.id_faculty where id_faculty = @faculty and gmail=@email and username=@username";
            try
            {
                if(dataconnecttion.GetConnection().State == System.Data.ConnectionState.Closed)
                    dataconnecttion.GetConnection().Open();
                SqlCommand cmd = new SqlCommand(querry, dataconnecttion.GetConnection());
                cmd.Parameters.AddWithValue("@username", UsernameBox.Text);
                cmd.Parameters.AddWithValue("@faculty",FalcultyBox.Text);
                cmd.Parameters.AddWithValue("@email",EmailBox.Text);
                int check = Convert.ToInt32(cmd.ExecuteScalar());
                if (check == 1)
                {
                    string querry2 = "select passwords from Account where username =@username";
                    cmd = new SqlCommand(querry2, dataconnecttion.GetConnection());
                    cmd.Parameters.AddWithValue("@username", UsernameBox.Text);
                    //MessageBox.Show(Convert.ToString(cmd.ExecuteScalar()));
                    string to = EmailBox.Text;
                    string from = "imhunggg02@gmail.com";
                    string subject = "Recovery password";
                    string body = @"Your password is: " + Convert.ToString(cmd.ExecuteScalar());
                    string password = "stfhexhuhwbogtjc";
                    MailMessage message = new MailMessage();
                    message.To.Add(to);
                    message.From = new MailAddress(from);
                    message.IsBodyHtml = true;
                    message.Body = body;
                    message.Subject = subject;
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                    smtp.EnableSsl = true;
                    smtp.Port = 587;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(from, password);
                    try
                    {
                        smtp.Send(message);
                        MessageBox.Show("Email sent");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                    MessageBox.Show("Incorrect information, please fill in again");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}
