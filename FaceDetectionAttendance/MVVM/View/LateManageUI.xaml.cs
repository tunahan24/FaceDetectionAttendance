
﻿using FaceDetectionAttendance.MVVM.Model;
using Microsoft.Data.SqlClient;
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

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for LateManageUI.xaml
    /// </summary>
    public partial class LateManageUI : Page
    {

    
        private Dataconnecttion dtc = new Dataconnecttion();
        SqlCommand cmd = new SqlCommand();
        private string faculty;

        public void setLate_DataGrid()
        {
            try
            {
                Late_DataGrid.Items.Clear();
                string querry = "SELECT LateList.id, LateList.id_worker, " +
                                "fullname, d_m, shift_worked, detail " +
                                "FROM LateList INNER JOIN WorkerList ON " +
                                "LateList.id_worker = WorkerList.id " +
                                "AND id_faculty = '" + this.faculty + "' ";
                if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                {
                    dtc.GetConnection().Open();
                }
                cmd = new SqlCommand(querry, dtc.GetConnection());
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int id_worker = reader.GetInt32(1);
                    string name_worker = reader.GetString(2);
                    DateTime date = reader.GetDateTime(3);
                    int shift = reader.GetInt32(4);
                    string detail = reader.GetString(5);
                    Late_DataGrid.Items.Add(new { Id = id, IdWorker = id_worker, NameWorker = name_worker, DateTime = date.ToString(), Shift = shift, Detail = detail });
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }
        public LateManageUI(string faculty)
        {
            InitializeComponent();
            this.faculty = faculty;
            Faculty_Header.Text = faculty;
            for (int i = 1; i <= 12; i++)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = i;
                Month_ComboBox.Items.Add(item);
            }
            setLate_DataGrid();
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new AddLateUI(faculty));
        }

        private void Edit_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Late_DataGrid.SelectedItems.Count == 1)
            {
                dynamic temp = Late_DataGrid.SelectedItem;
                if (temp != null)
                {
                    dynamic Idselected = temp.Id;
                    this.NavigationService.Navigate(new EditLateUI(this.faculty, Idselected));
                }
            }
            else
            {
                MessageBox.Show("Please choose one to edit", "Message");
            }
        }

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Late_DataGrid.SelectedItems.Count == 1)
            {
                MessageBoxResult result = MessageBox.Show("Delete this data from database ?", "Warning", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    dynamic temp = Late_DataGrid.SelectedItem;
                    if (temp != null)
                    {
                        dynamic Idselected = temp.Id;
                        string querry = "DELETE FROM LateList WHERE id = '" + Idselected + "'";
                        if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                        {
                            dtc.GetConnection().Open();
                        }
                        cmd = new SqlCommand(querry, dtc.GetConnection());
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Successful", "Message");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please choose one to edit", "Message");
            }
        }

        private void Date_DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Date_DatePicker.SelectedDate != null)
            {
                try
                {
                    Late_DataGrid.Items.Clear();
                    DateTime dateSearch = Convert.ToDateTime(Date_DatePicker.SelectedDate.Value);
                    string querry = "SELECT LateList.id, LateList.id_worker, " +
                                    "fullname, d_m, shift_worked, detail " +
                                    "FROM LateList INNER JOIN WorkerList ON " +
                                    "LateList.id_worker = WorkerList.id " +
                                    "AND id_faculty = @faculty " +
                                    "WHERE d_m = @date ";
                    if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                    {
                        dtc.GetConnection().Open();
                    }
                    cmd = new SqlCommand(querry, dtc.GetConnection());
                    cmd.Parameters.AddWithValue("@date", dateSearch.Date);
                    cmd.Parameters.AddWithValue("@faculty", this.faculty);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        int id_worker = reader.GetInt32(1);
                        string name_worker = reader.GetString(2);
                        DateTime date = reader.GetDateTime(3);
                        int shift = reader.GetInt32(4);
                        string detail = reader.GetString(5);
                        Late_DataGrid.Items.Add(new { Id = id, IdWorker = id_worker, NameWorker = name_worker, DateTime = date.ToString(), Shift = shift, Detail = detail });
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error");
                }
            }
            else setLate_DataGrid();
        }

        private void Search_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Search_TextBox.Text.Trim() != "" &&
                Month_ComboBox.SelectedItem != null &&
                Year_TextBox.Text.Trim() != "")
            {
                string name = Search_TextBox.Text.Trim();
                int month = Convert.ToInt32(Month_ComboBox.SelectedValue);
                int year = Int32.Parse(Year_TextBox.Text.Trim());
                try
                {
                    Late_DataGrid.Items.Clear();
                    string querry = "SELECT LateList.id, LateList.id_worker, " +
                                    "fullname, d_m, shift_worked, detail " +
                                    "FROM LateList INNER JOIN WorkerList ON " +
                                    "LateList.id_worker = WorkerList.id " +
                                    "AND id_faculty = @faculty " +
                                    "WHERE fullname = @fullname " +
                                    "AND MONTH(d_m) = @month " +
                                    "AND YEAR(d_m) = @year ";
                    if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                    {
                        dtc.GetConnection().Open();
                    }
                    cmd = new SqlCommand(querry, dtc.GetConnection());
                    cmd.Parameters.AddWithValue("@faculty", this.faculty);
                    cmd.Parameters.AddWithValue("@fullname", name);
                    cmd.Parameters.AddWithValue("@month", month);
                    cmd.Parameters.AddWithValue("@year", year);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        int id_worker = reader.GetInt32(1);
                        string name_worker = reader.GetString(2);
                        DateTime date = reader.GetDateTime(3);
                        int shift = reader.GetInt32(4);
                        string detail = reader.GetString(5);
                        Late_DataGrid.Items.Add(new { Id = id, IdWorker = id_worker, NameWorker = name_worker, DateTime = date.ToString(), Shift = shift, Detail = detail });
                    }
                    reader.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error");
                }
            }
            else MessageBox.Show("Please enter full date in text box", "Message");
        }

        private void Year_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
                Message_Label.Visibility = Visibility.Visible;
            }
        }
    }
}
