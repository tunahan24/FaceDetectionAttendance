using FaceDetectionAttendance.MVVM.Model;
using Microsoft.Data.SqlClient;
using OpenCvSharp.Detail;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
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
    /// Interaction logic for EditLateUI.xaml
    /// </summary>
    public partial class EditLateUI : Page
    {
        private Dataconnecttion dtc = new Dataconnecttion();
        private string faculty;
        private int idLate;
        private SqlCommand cmd = new SqlCommand();


        public void setNameWorker_ComboBox(string fid)
        {
            try
            {
                string querry = "SELECT fullname,id FROM WorkerList WHERE fid = '" + fid + "' ";
                if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                {
                    dtc.GetConnection().Open();
                }
                cmd = new SqlCommand(querry, dtc.GetConnection());
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = reader.GetString(0);
                    item.Tag = reader.GetInt32(1);
                    NameWorker_ComboBox.Items.Add(item);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
        public void setValueLate(int id)
        {
            try
            {
                string querry = "SELECT LateList.id_worker, d_m, shift_worked, detail " +
                                "FROM LateList INNER JOIN WorkerList " +
                                "ON LateList.id_worker = WorkerList.id " +
                                "AND LateList.id = '" + id + "' ";
                if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                {
                    dtc.GetConnection().Open();
                }
                cmd = new SqlCommand(querry, dtc.GetConnection());
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    NameWorker_ComboBox.SelectedValue = reader.GetInt32(0);
                    Date_DatePicker.SelectedDate = reader.GetDateTime(1).Date;
                    Shift_ComboBox.SelectedValue = reader.GetInt32(2);
                    Detail_TextBox.Text = reader.GetString(3);
                }
                reader.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        public EditLateUI(string faculty, int id)
        {
            InitializeComponent();
            this.faculty = faculty;
            Faculty_Header.Text = faculty;
            this.idLate = id;
            setNameWorker_ComboBox(faculty);
            setValueLate(id);
        }
        private bool checkInput()
        {
            if (NameWorker_ComboBox.SelectedIndex == null)
            {
                MessageBox.Show("Please chose worker in list", "Error");
                return false;
            }
            else if (Detail_TextBox.Text.Trim() == "")
            {
                MessageBox.Show("Please enter the detail reason go to work late", "Error");
                return false;
            }
            return true;
        }
        private bool checkAttendanced(int id_worker, DateTime date, int shift)
        {
            try
            {
                string querry = "SELECT id FROM Attendance " +
                                "WHERE id_worker = @id_worker " +
                                "AND d_m = @date " +
                                "AND shift_worked = @shift ";
                if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                {
                    dtc.GetConnection().Open();
                }
                cmd = new SqlCommand(querry, dtc.GetConnection());
                cmd.Parameters.AddWithValue("@id_worker", id_worker);
                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@shift", shift);
                object result = cmd.ExecuteScalar();
                if (result == null)
                {
                    return false;
                }
                MessageBox.Show("The worker Attendanced on time", "Message");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                return false;
            }
        }
        private bool checkExist(int id_worker, DateTime date, int shift)
        {
            try
            {
                string querry = "SELECT id FROM LateList " +
                                "WHERE id_worker = @id_worker " +
                                "AND d_m = @date " +
                                "AND shift_worked = @shift ";
                if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                {
                    dtc.GetConnection().Open();
                }
                cmd = new SqlCommand(querry, dtc.GetConnection());
                cmd.Parameters.AddWithValue("@id_worker", id_worker);
                cmd.Parameters.AddWithValue("@date", date.Date);
                cmd.Parameters.AddWithValue("@shift", shift);
                object result = cmd.ExecuteScalar();
                if (result == null)
                {
                    return false;
                }
                MessageBox.Show("Already exists this data in the database", "Message");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                return false;
            }
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            if (checkInput())
            {
                int id_worker = Convert.ToInt32(NameWorker_ComboBox.SelectedValue);
                DateTime date = Convert.ToDateTime(Date_DatePicker.SelectedDate.Value);
                int shift = Convert.ToInt32(Shift_ComboBox.SelectedValue);
                string detail = Detail_TextBox.Text.Trim();

                if (!checkAttendanced(id_worker, date, shift) && !checkExist(id_worker, date, shift))
                {
                    try
                    {
                        if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                        {
                            dtc.GetConnection().Close();
                        }

                        string querry = "UPDATE LateList SET " +
                                        "id_worker = @id_worker, " +
                                        "id_faculty = @id_faculty, " +
                                        "d_m = @d_m, " +
                                        "shift_worked = @shift_worked, " +
                                        "detail = @detail " +
                                        "WHERE id = @id";
                        cmd = new SqlCommand(querry, dtc.GetConnection());
                        cmd.Parameters.AddWithValue("@id_worker", id_worker);
                        cmd.Parameters.AddWithValue("@id_faculty", this.faculty);
                        cmd.Parameters.AddWithValue("@d_m", date);
                        cmd.Parameters.AddWithValue("@shift_worked", shift);
                        cmd.Parameters.AddWithValue("@detail", detail);
                        cmd.Parameters.AddWithValue("@id", this.idLate);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Successful !", "Message");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "Error");
                    }
                }
            
            }
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}

