using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
using Emgu.CV;
using FaceDetectionAttendance.MVVM.Model;
using Microsoft.Data.SqlClient;
using Unity.Policy;

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for ManageAccountUI.xaml
    /// </summary>
    public partial class ManageAccountUI : Page
    {
        private Dataconnecttion dtc = new Dataconnecttion();
       
        SqlCommand command;
        //Lay du lieu tu sql
        public ManageAccountUI()
        {
            InitializeComponent();
            Loaddata();
        }
        private void Loaddata()
        {
            string querry = "Select username,passwords,fid, gmail,roles From Account";
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                dtc.GetConnection().Open();
            try
            {
                command = new SqlCommand(querry, dtc.GetConnection());
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int temp = reader.GetInt32(4);
                    string role;
                    if (temp == 1)
                    {
                        role = "Admin";
                    }
                    else
                    {
                        role = "Staff";
                    }

                    string Username = reader.GetString(0);
                    string Passwords = reader.GetString(1);
                    string Fid = reader.GetString(2);
                    string Gmail = reader.GetString(3);
                    string Roles = role;
                    Accountdtg.Items.Add(new { username = Username, password = Passwords, fid = Fid, gmail = Gmail, roles = Roles });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Searchbtn_Click(object sender, RoutedEventArgs e)
        {
            Accountdtg.Items.Clear();
            string username = "%" + Usernametxb.Text + "%";

            string query = "SELECT * FROM Account WHERE username LIKE @Username";
            List<Account> accounts = new List<Account>();
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                dtc.GetConnection().Open();
            SqlCommand command = new SqlCommand(query, dtc.GetConnection());
            command.Parameters.AddWithValue("@Username", username);
            try
            {
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int temp = reader.GetInt32(2);
                    string role;
                    if (temp == 1)
                    {
                        role = "Admin";
                    }
                    else
                    {
                        role = "Staff";
                    }

                    string Username = reader.GetString(0);
                    string Passwords = reader.GetString(1);
                    string Fid = reader.GetString(5);
                    string Gmail = reader.GetString(3);
                    string Roles = role;
                    Accountdtg.Items.Add(new { username = Username, password = Passwords, fid = Fid, gmail = Gmail, roles = Roles });
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Addbtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new AddAccountUI());
        }

        private void Editbtn_Click(object sender, RoutedEventArgs e)
        {
           AccountManagement selec = new AccountManagement();
            var temp = Accountdtg.SelectedItem;
            if (temp != null)
            {
                dynamic selectedobject = temp;
                selec.username = selectedobject.username;
                selec.password = selectedobject.password;
                selec.fid = selectedobject.fid;
                selec.roles = selectedobject.roles;
                selec.gmail = selectedobject.gmail;
                this.NavigationService.Navigate(new EditAccountUI(selec));
            }
        }

        private void Delbtn_Click(object sender, RoutedEventArgs e)
        {           
            if (MessageBox.Show("Agree to delete?", "Alert", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    string selectedusername = "";
                    var temp = Accountdtg.SelectedItem;
                    if (temp != null)
                    {
                        dynamic selectedobject = temp;
                        selectedusername = selectedobject.username;
                        string querry = "Delete from Account Where username = @username";
                        if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                            dtc.GetConnection().Open();
                        command = new SqlCommand(querry, dtc.GetConnection());
                        command.Parameters.AddWithValue("username", selectedusername);
                        command.ExecuteNonQuery();
                        Accountdtg.Items.Remove(temp);
                    }
                    else
                    {
                        MessageBox.Show("Select on Account");
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void Usernametxb_TextChanged(object sender, TextChangedEventArgs e)
        {
            Accountdtg.Items.Clear();
            string username = "%" + Usernametxb.Text + "%";

            string query = "SELECT * FROM Account WHERE username LIKE @Username";
            List<Account> accounts = new List<Account>();
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                dtc.GetConnection().Open();
            SqlCommand command = new SqlCommand(query, dtc.GetConnection());
            command.Parameters.AddWithValue("@Username", username);
            try
            {
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int temp = reader.GetInt32(2);
                    string role;
                    if (temp == 1)
                    {
                        role = "Admin";
                    }
                    else
                    {
                        role = "Staff";
                    }

                    string Username = reader.GetString(0);
                    string Passwords = reader.GetString(1);
                    string Fid = reader.GetString(5);
                    string Gmail = reader.GetString(3);
                    string Roles = role;
                    Accountdtg.Items.Add(new { username = Username, password = Passwords, fid = Fid, gmail = Gmail, roles = Roles });
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}

