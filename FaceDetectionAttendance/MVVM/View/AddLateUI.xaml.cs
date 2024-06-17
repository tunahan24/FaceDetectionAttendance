using FaceDetectionAttendance.MVVM.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for AddLateUI.xaml
    /// </summary>
    public partial class AddLateUI : Page
    {
        private Dataconnecttion dtc = new Dataconnecttion();
        private string faculty;
        private SqlCommand cmd = new SqlCommand();
        private void setNameWorker_ComboBox(string fid)
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
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error");
            }
        }
        public AddLateUI(string faculty)
        {
            InitializeComponent();
            this.faculty = faculty;
            Faculty_Header.Text = faculty;
            Shift_ComboBox.SelectedIndex = 0;
            Date_DatePicker.SelectedDate = DateTime.Now.Date;
            setNameWorker_ComboBox(faculty);
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
                MessageBox.Show("Please enter the detail reason go to work late","Error");
                return false;
            }
            return true;
        }
        private bool checkAttendanced(int id_worker, DateTime date,int shift)
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
            }catch(Exception ex)
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
        private void IdWorker_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
                Message_Label.Visibility = Visibility.Visible;
            }
        }
        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            if (checkInput())
            {
                int id_worker = Convert.ToInt32(NameWorker_ComboBox.SelectedValue);
                DateTime date= Convert.ToDateTime(Date_DatePicker.SelectedDate.Value);
                int shift = Convert.ToInt32(Shift_ComboBox.SelectedValue);
                string detail = Detail_TextBox.Text.Trim();

                if (!checkAttendanced(id_worker, date, shift) && !checkExist(id_worker, date, shift))
                {
                    try
                    {
                        if(dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                        {
                            dtc.GetConnection().Close();
                        }
                        string querry = "INSERT INTO LateList VALUES (@id_worker, @id_faculty, @d_m , @shift_worked , @detail )";
                        cmd = new SqlCommand(querry, dtc.GetConnection());
                        cmd.Parameters.AddWithValue("@id_worker", id_worker);
                        cmd.Parameters.AddWithValue("@id_faculty", this.faculty);
                        cmd.Parameters.AddWithValue("@d_m", date.Date);
                        cmd.Parameters.AddWithValue("@shift_worked", shift);
                        cmd.Parameters.AddWithValue("@detail", detail);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Successful !", "Message");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
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
