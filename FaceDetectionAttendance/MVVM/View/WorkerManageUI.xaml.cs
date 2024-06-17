using FaceDetectionAttendance.MVVM.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for WorkerManageUI.xaml
    /// </summary>
    public partial class WorkerManageUI : Page
    {
        Dataconnecttion dtc = new Dataconnecttion();
        SqlCommand cmd = new SqlCommand();
        private string _faculty;
        public WorkerManageUI(string username)
        {
            InitializeComponent();
            getFid(username);
            FacultyText.Text = _faculty;
            setData();
        }
        private void getFid(string username)
        {
            string query = "Select fid from Account where username = @username";
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                dtc.GetConnection().Open();
            cmd = new SqlCommand(query, dtc.GetConnection());
            try
            {
                cmd.Parameters.AddWithValue("@username", username);
                this._faculty = cmd.ExecuteScalar().ToString();
                dtc.GetConnection().Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void setData()
        {
            string query = "Select id,fullname, birth, salary ,fid from WorkerList Where fid = @fid";
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                dtc.GetConnection().Open();
            cmd = new SqlCommand(query, dtc.GetConnection());
            try
            {
                cmd.Parameters.AddWithValue("@fid", _faculty);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader.GetInt32(0));
                    string fullname = reader.GetString(1);
                    DateTime dob = reader.GetDateTime(2);
                    int salary = reader.GetInt32(3);
                    string fid = reader.GetString(4);
                    Workertxt.Items.Add(new { id = id, fullname = fullname, dob = dob,salary = salary, fid = fid });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new AddWorkerUI(_faculty));
        }


        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //edit
            WorkerList worker = new WorkerList();
            var temp = Workertxt.SelectedItem;
            if (temp != null)
            {
                dynamic selected = temp;
                worker.Fullname = selected.fullname;
                worker.Birth = selected.dob;
                worker.Fid = selected.fid;
                worker.Salary = selected.salary;
                this.NavigationService.Navigate(new EditWorkerUI(worker));
            }
            else
            {
                MessageBox.Show("Select a worker please");
            }

        }
        private void Search_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Search_TextBox.Text == "")
            {
                MessageBox.Show("Please enter the name or id of worker", "Error", MessageBoxButton.OK);
            }
            else
            {
                Workertxt.Items.Clear();
                //Loai khoang trang
                string searchString = Search_TextBox.Text.Trim();

                int id_worker;
                bool result = Int32.TryParse(searchString, out id_worker);

                string querry;
                if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                {
                    dtc.GetConnection().Open();
                }
                try
                {

                    if (result)
                    {
                        querry = "SELECT id, fullname, birth, fid FROM WorkerList WHERE id = @id";
                        cmd = new SqlCommand(querry, dtc.GetConnection());
                        cmd.Parameters.AddWithValue("@id", Int32.Parse(searchString));
                    }
                    else
                    {
                        querry = "SELECT id, fullname, birth, fid FROM WorkerList WHERE fullname = @searchString";
                        cmd = new SqlCommand(querry, dtc.GetConnection());
                        cmd.Parameters.AddWithValue("@searchString", searchString);
                    }
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader.GetInt32(0));
                        string fullname = reader.GetString(1);
                        DateTime dob = reader.GetDateTime(2);
                        string fid = reader.GetString(3);
                        Workertxt.Items.Add(new { id = id, fullname = fullname, dob = dob.Date, fid = fid });
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            if (Workertxt.Items.Count == 0) MessageBox.Show("Can't find " + Search_TextBox.Text + " in Database");
        }

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            var temp = Workertxt.SelectedItem;
            if (temp != null)
            {
                dynamic selected = temp;
                int id = selected.id;
                string fullname = selected.fullname;
                MessageBoxResult result = MessageBox.Show("Do you want to delete worker Id:" + id + ", Name:" + fullname + " ?", "Message", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    //delete picture
                    deleteWorkerImage(id);

                    //delete attendance
                    string querry1 = "Delete from Attndance where id =" + id;
                    if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                    {
                        dtc.GetConnection().Open();
                    }
                    try
                    {
                        cmd = new SqlCommand(querry1, dtc.GetConnection());
                        cmd.ExecuteNonQuery();
                        dtc.GetConnection().Close();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    //delete in database
                    string querry = "DELETE FROM WorkerList WHERE id = " + id;
                    if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                    {
                        dtc.GetConnection().Open();
                    }
                    cmd = new SqlCommand(querry, dtc.GetConnection());
                    cmd.ExecuteNonQuery();
                    deleteWorkerImage(id);
                    Workertxt.Items.Clear();
                    MessageBox.Show("Done!", "Message");
                    setData();
                }
            }
            else
            {
                MessageBox.Show("Select a worker please");
            }
        }
        private void deleteWorkerImage(int id)
        {
            string binFolderPath = System.IO.Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            string projectFolderPath = Directory.GetParent(binFolderPath).FullName;
            string fix = projectFolderPath.Remove(projectFolderPath.Length - 9);
            string resourceFolderPath = System.IO.Path.Combine(fix, "Resource");

            try
            {
                string querry = "SELECT images FROM WorkerList WHERE id = " + id;
                if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                {
                    dtc.GetConnection().Open();
                }
                cmd = new SqlCommand(querry, dtc.GetConnection());
                SqlDataReader reader = cmd.ExecuteReader();
                string nameimg = "";
                while (reader.Read())
                {
                    nameimg = reader.GetString(0);
                }
                reader.Close();
                string imagePath = $"{resourceFolderPath}\\WorkerImage\\{FacultyText.Text}\\{nameimg}.png";
                File.Delete(imagePath);
                MessageBox.Show("Delete image done!", "Message");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new AddWorkerUI(_faculty));
        }
    }
}
