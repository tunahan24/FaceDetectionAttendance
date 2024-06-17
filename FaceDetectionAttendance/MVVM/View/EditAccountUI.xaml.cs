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
using Microsoft.Data.SqlClient;
using FaceDetectionAttendance.MVVM.Model;
using Unity.Policy;
using Microsoft.Win32;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Data;
using System.IO;
using System.Net.Http.Headers;

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for EditAccountUI.xaml
    /// </summary>
    public partial class EditAccountUI : Page
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        private Dataconnecttion dtc = new Dataconnecttion();
        private SqlCommand command;
        private DataGridRow selectedRow;
        private string Currentusername;
        public EditAccountUI()
        {
            InitializeComponent();
            setComboBoxData();
        }

        public EditAccountUI(AccountManagement selec)
        {
            InitializeComponent();
            this.selectedRow = selectedRow;
            setComboBoxData();
            Currentinfor(selec);
        }
        private void Currentinfor(AccountManagement selec)
        {
            Usernametxb.Text = selec.username;
            Passwordtxb.Text = selec.password;
            facultycbb.SelectedItem = selec.fid;
            Gmailtxb.Text = selec.gmail;
            Rolecbb.SelectedItem = selec.roles;
            Currentusername = selec.username;
        }

        private void setComboBoxData()
        {
            string querry = "Select* from Faculty";//cau lenh sql
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                dtc.GetConnection().Open();//ket noi den database
            command = new SqlCommand(querry, dtc.GetConnection());
            SqlDataReader reader = command.ExecuteReader();//doc du lieu tu database
            while (reader.Read())
            {
                Faculty a = new Faculty();
                a.IdFaculty = reader.GetString(0);
                a.NameFaculty = reader.GetString(1);
                facultycbb.Items.Add(a.IdFaculty);
            }
            Rolecbb.Items.Add("Admin");
            Rolecbb.Items.Add("Staff");
            //set tiep Faculty- sua trong while va querry
            //lay du lieu trong sql ra
            //tao ra 1 bien kieu Faculty(trong while)

        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        internal void ShowDialog()
        {
            throw new NotImplementedException();
        }

        private void Addimagebtn_Click(object sender, RoutedEventArgs e)
        {

            if (openFileDialog.ShowDialog() == true)
            {
                if (File.Exists("C:\\FDA temp\\avatar.png"))
                {
                    File.Delete("C:\\FDA temp\\avatar.png");
                    File.Copy(openFileDialog.FileName, "C:\\FDA temp\\avatar.png");
                }
                else
                {
                    File.Copy(openFileDialog.FileName, "C:\\FDA temp\\avatar.png");
                }

                Imagebd.Background = new ImageBrush(new BitmapImage(new Uri(openFileDialog.FileName)));
            }
        }

        private void Savebtn_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra xem tất cả các trường thông tin đã được nhập đầy đủ và hợp lệ
            if (string.IsNullOrEmpty(Usernametxb.Text) ||
                Passwordtxb.Text.Length == 0 ||
                Gmailtxb.Text == "" ||
                facultycbb.SelectedItem == null ||
                Rolecbb.SelectedItem == null ||
                Imagebd.Background == null)
            {
                MessageBox.Show("Please enter enough information.");
                return;
            }

            // Lấy thông tin người dùng từ các ô nhập và chọn
            string username = Usernametxb.Text;
            string password = new System.Net.NetworkCredential(string.Empty, Passwordtxb.Text).Password;
            string gmail = Gmailtxb.Text;
            string faculty = facultycbb.SelectedItem.ToString();
            string role = Rolecbb.SelectedItem.ToString();
            BitmapImage image = ((ImageBrush)Imagebd.Background).ImageSource as BitmapImage;

            // Xóa các ô nhập để chuẩn bị cho việc thêm người dùng tiếp theo
            Usernametxb.Text = "";
            Passwordtxb.Clear();
            Gmailtxb.Text = "";
            facultycbb.SelectedItem = null;
            Rolecbb.SelectedItem = null;
            Imagebd.Background = null;


            //luu du lieu
            string binFolderPath = System.IO.Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            string projectFolderPath = Directory.GetParent(binFolderPath).FullName;
            string fix = projectFolderPath.Remove(projectFolderPath.Length - 9);
            string resourceFolderPath = System.IO.Path.Combine(fix, "Resource");
            string avt = resourceFolderPath + @$"\Avatar\{username}.png";
            string querry = "update Account  set username = @username, passwords=@passwords, gmail=@gmail, images=@images, fid=@fid where username = @U ";
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                dtc.GetConnection().Open();
            int temp;

            File.Delete(avt);
            File.Copy("C:\\FDA temp\\avatar.png",avt );
            
            if (role == "Admin")
            {
                temp = 1;
            }
            else
            {
                temp = 0;
            }

            try
            {
                command = new SqlCommand(querry, dtc.GetConnection());
                command.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;
                command.Parameters.Add("@passwords", SqlDbType.NVarChar).Value = password;
                command.Parameters.Add("@fid", SqlDbType.NVarChar).Value = faculty;
                command.Parameters.Add("@gmail", SqlDbType.NVarChar).Value = gmail;
                command.Parameters.Add("@roles", SqlDbType.NVarChar).Value = temp;
                command.Parameters.Add("@images", SqlDbType.NVarChar).Value = username;
                command.Parameters.Add("@U", SqlDbType.NVarChar).Value = Currentusername;
                command.ExecuteNonQuery();
                this.NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
