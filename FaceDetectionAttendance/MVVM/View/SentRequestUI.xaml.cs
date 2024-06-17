using System;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using FaceDetectionAttendance.MVVM.Model;
using System.Globalization;

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for SendRequestUI.xaml
    /// </summary>
    public partial class SendRequestUI : Page
    {
        private Dataconnecttion dtc = new Dataconnecttion();
        SqlCommand cmd;
        private string _username;
        string FacultyName;
        public SendRequestUI(string username, string fid)
        {
            InitializeComponent();
            setRequestSent(username);
            setComboboxData();
            _username = username;
            FacultyName = fid;
        }
        private void setComboboxData()
        {
            Shiftcbb.Items.Add("1");
            Shiftcbb.Items.Add("2");
        }
        private void setRequestSent(string username)
        {
            string query = "Select detail, states, usernamesent from Request where usernamesent = @username";
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                dtc.GetConnection().Open();
            try
            {
                cmd = new SqlCommand(query, dtc.GetConnection());
                cmd.Parameters.AddWithValue("@username", username);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string detail = reader.GetString(0);
                    string states = reader.GetString(1);
                    string usernamesent = reader.GetString(2);
                    RequestSentdtg.Items.Add(new { detail = detail, states = states, usernamesent = usernamesent });
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            dtc.GetConnection().Close();
        }
        private void SendRequestbtn_Click(object sender, RoutedEventArgs e)
        {
            int attendanceId = getID();
            if(attendanceId != null)
            {
                if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                    dtc.GetConnection().Open();
                string query = "Insert into Request values(@attendanceId, @detail, @states, @usernamesent)";
                try
                {
                    string detail = "Delete worker: " + Nametxb.Text + " Id: " + Idtxb.Text + " At: " + Datetxb.Text + " Shift: " + Shiftcbb.SelectedItem.ToString();
                    cmd = new SqlCommand(query, dtc.GetConnection());
                    cmd.Parameters.AddWithValue("@attendanceId", attendanceId);
                    cmd.Parameters.AddWithValue("@detail", detail);
                    cmd.Parameters.AddWithValue("states", "Idle");
                    cmd.Parameters.AddWithValue("@usernamesent", _username);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Successful sent request");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Wrong worker's information");
            }
           
        }
        private int getID()
        {
            if (Idtxb.Text == null || Datetxb.Text == null || Shiftcbb.SelectedItem == null)
                MessageBox.Show("Please input necessary information");
            else
            {
                DateTime d_m = DateTime.ParseExact($"{Datetxb.Text}", "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string query = "select id from Attendance where id_worker = @id_worker, d_m = @d_m, shift_worked = @shift_worked";
                if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                    dtc.GetConnection().Open();
                try
                {
                    cmd = new SqlCommand(query, dtc.GetConnection());
                    cmd.Parameters.AddWithValue("@id_worker", Idtxb.Text);
                    cmd.Parameters.AddWithValue("@d_m", d_m);
                    cmd.Parameters.AddWithValue("@shift_worked", Shiftcbb.SelectedItem.ToString());
                    dtc.GetConnection().Close();
                    return Int32.Parse(cmd.ExecuteScalar().ToString());
                }
                catch (Exception ex)
                {
                    
                }
            }
            dtc.GetConnection().Close();
            return 0;
        }
        private void clear_Click(object sender, RoutedEventArgs e)
        {
            Datetxb.Text = " ";
            Idtxb.Text = " ";
            Nametxb.Text = " ";
        }

    }
}
