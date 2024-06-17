using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using FaceDetectionAttendance.MVVM.Model;
using System.Linq;
using Microsoft.Data.SqlClient;
using Emgu.CV.XImgproc;

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for RequestUI.xaml
    /// </summary>
    public partial class RequestUI : Page
    {
        Dataconnecttion dtc = new Dataconnecttion();
        SqlCommand cmd = new SqlCommand();
        List<Request> requests = new List<Request>();
        public RequestUI()
        {
            InitializeComponent();
            setStatusData();
        }
        private void setStatusData()
        {
            Statuscbb.Items.Add("Idle");
            Statuscbb.Items.Add("Accepted");
            Statuscbb.Items.Add("Denied");
            Statuscbb.Items.Add("All");
            Statuscbb.SelectedItem = "All";
        }
        private void setData()
        {
            string query = "select * from Request";
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
            {
                dtc.GetConnection().Open();
            }
            try
            {
                cmd = new SqlCommand(query, dtc.GetConnection());
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Request request = new Request();
                    request.id = reader.GetInt32(0);
                    request.id_attendance = reader.GetInt32(1);
                    request.detail = reader.GetString(2);
                    request.states = reader.GetString(3);
                    request.usernamesent = reader.GetString(4);
                    Requestdtg.Items.Add(new {id = request.id ,id_attendance = request.id_attendance, detail = request.detail, status = request.states, usernamesent = request.usernamesent});
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void setCusomData(string states)
        {
            string query = "select * from Request where states = @states";
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
            {
                dtc.GetConnection().Open();
            }
            try
            {
                cmd = new SqlCommand(query, dtc.GetConnection());
                cmd.Parameters.AddWithValue("@states", states);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Request request = new Request();
                    request.id = reader.GetInt32(0);
                    request.id_attendance = reader.GetInt32(1);
                    request.detail = reader.GetString(2);
                    request.states = reader.GetString(3);
                    request.usernamesent = reader.GetString(4);
                    Requestdtg.Items.Add(new { id = request.id, id_attendance = request.id_attendance, detail = request.detail, status = request.states, usernamesent = request.usernamesent });
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Acceptbtn_Click(object sender, RoutedEventArgs e)
        {
            var temp = Requestdtg.SelectedItem;
            int id = 0;
            int idA = 0;
            if (temp != null)
            {
                dynamic selected = temp;
                id = selected.id;
                idA = selected.id_attendance;
            }
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                dtc.GetConnection().Open();
            string query = "Update Request set states = 'Accepted' where id = @id";
            try
            {
                cmd = new SqlCommand(query, dtc.GetConnection());
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                Requestdtg.Items.Refresh();
                string query2 = "Delete from Attendance where id = @id";
                if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                    dtc.GetConnection().Open();
                try
                {
                    cmd = new SqlCommand(query2, dtc.GetConnection());
                    cmd.Parameters.AddWithValue("@id", idA);
                    cmd.ExecuteNonQuery();
                    dtc.GetConnection().Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                Requestdtg.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            dtc.GetConnection().Close();
        }

        private void Denybtn_Click(object sender, RoutedEventArgs e)
        {
            var temp = Requestdtg.SelectedItem;
            int id = 0;
            if (temp != null)
            {
                dynamic selected = temp;
                id = selected.id;
            }
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                dtc.GetConnection().Open();
            string query = "Update Request set states = 'Denied' where id = @id";
            try
            {
                cmd = new SqlCommand(query, dtc.GetConnection());
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                dtc.GetConnection().Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            dtc.GetConnection().Close();
        }

        private void Statuscbb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Statuscbb.SelectedItem != null && Statuscbb.SelectedItem != "All")
            {
                Requestdtg.Items.Clear();
                setCusomData(Statuscbb.SelectedItem.ToString());
                Requestdtg.Items.Refresh();  
            }
            else if (Statuscbb.SelectedItem != null && Statuscbb.SelectedItem == "All")
            {
                Requestdtg.Items.Clear();
                setData();
            }
        }
    }
}
