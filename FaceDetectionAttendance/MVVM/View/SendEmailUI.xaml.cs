using DocumentFormat.OpenXml.CustomProperties;
using FaceDetectionAttendance.MVVM.Model;
using FaceDetectionAttendance.MVVM.ViewModel;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for SendEmailUI.xaml
    /// </summary>
    public partial class SendEmailUI : Window
    {
        Dataconnecttion dtc = new Dataconnecttion();
        private void EmailList_Reload()
        {
            string querry = "SELECT * FROM ListEmail";
            if(dtc.GetConnection().State == System.Data.ConnectionState.Closed)
            {
                dtc.GetConnection().Open();
            }
            SqlCommand cmd = new SqlCommand(querry, dtc.GetConnection());
            SqlDataReader reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                string email = reader.GetString(0);
                string describe = reader.GetString(1);
                EmailList_DataGrid.Items.Add( new { email = email, describe = describe}) ;
            }
            reader.Close();
        }
        public SendEmailUI()
        {
            InitializeComponent();
            EmailList_Reload();
        }

        private void EmailList_DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(EmailList_DataGrid.SelectedItems.Count > 1)
            {
                MessageBox.Show("Just choose one email to send , we will update to choose more email in next version","Error",MessageBoxButton.OK);
            }
            else
            {
                Type t = EmailList_DataGrid.SelectedItem.GetType();
                PropertyInfo p = t.GetProperty("email");
                if(p != null)
                {
                    ToEmail_TextBox.Text = p.GetValue(EmailList_DataGrid.SelectedItem).ToString();
                }
            }
        }

        private void ChooseFile_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDiaLog = new OpenFileDialog();
            openDiaLog.Multiselect= true;
            if(openDiaLog.ShowDialog() == true) //DialogResult.OK
            {
                string[] files = openDiaLog.FileNames;
                ChooseFile_TextBlock.Text = string.Join("\n",files);
                ChooseFile_TextBlock.Visibility = Visibility;
            }
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Send_Button_Click(object sender, RoutedEventArgs e)
        {
            string toAddress = ToEmail_TextBox.Text;
            string subject = Subject_TextBox.Text;
            string content = Content_TextBox.Text;
            string[] fileName = ChooseFile_TextBlock.Text.Split('\n', StringSplitOptions.RemoveEmptyEntries); // remove null or "" path in string
            SendEmail send = new SendEmail(); 
            send.SendEmail_FileChoose(toAddress, subject, content, fileName);
        }
    }
}
