using FaceDetectionAttendance.MVVM.Model;
using FaceDetectionAttendance.MVVM.ViewModel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Data.SqlClient;
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

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for AccountantUI.xaml
    public partial class AccountantUI : Page
    {
        private Dataconnecttion dtc = new Dataconnecttion();
        public SqlCommand SQLcmd;
        // test combobox succefull
        public AccountantUI(string faculty)
        {
            InitializeComponent();
            Faculty_Header.Text = faculty;
            Year.Text = DateTime.Now.Year + "";
            Add_SetComboBoxData();
            reloaddatagrid();
        }
        public void Add_SetComboBoxData()
        {
            string querry = "Select* from Faculty";
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                dtc.GetConnection().Open();
            SQLcmd = new SqlCommand(querry, dtc.GetConnection());
            SqlDataReader reader = SQLcmd.ExecuteReader();
            while (reader.Read())
            {
                Faculty a = new Faculty();
                a.IdFaculty = reader.GetString(0);
                a.NameFaculty = reader.GetString(1);
                facultycbb.Items.Add(a.IdFaculty);
            }
            reader.Close();
            foreach (object item in facultycbb.Items)
            {
                if (item.ToString() == Faculty_Header.Text)
                {
                    facultycbb.SelectedItem = item;
                    facultycbb.IsEnabled = false;
                    break;
                }
            }

            for (int i = 1; i <= 12; i++)
            {
                Monthcbb.Items.Add(i + "");
                if (i == DateTime.Now.Month)
                    Monthcbb.SelectedIndex = i - 1;
            }
        }
        private void Monthcbb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            reloaddatagrid();
        }
        private void Year_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
                message_label.Visibility = Visibility.Visible;
            }
        }
        private void Year_TextChanged(object sender, TextChangedEventArgs e)
        {
            reloaddatagrid();
        }
        private void facultycbb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            reloaddatagrid();
        }
        private void reloaddatagrid()
        {
            if (Year.Text.Length == 0)
            {
                WorkersDataGrid.Columns.Clear();
            }
            else
            {
                WorkersDataGrid.Columns.Clear();
                string fid = Convert.ToString(facultycbb.SelectedItem);
                int year = Convert.ToInt32(Year.Text);
                int month = Convert.ToInt32(Monthcbb.SelectedItem);

                // Add DataGrid columns dynamically
                DataGridTextColumn column1 = new DataGridTextColumn();
                column1.Header = "ID";
                column1.Binding = new Binding("ID_Worker");
                WorkersDataGrid.Columns.Add(column1);

                DataGridTextColumn column2 = new DataGridTextColumn();
                column2.Header = "Name";
                column2.Binding = new Binding("WorkerName");
                WorkersDataGrid.Columns.Add(column2);

                int DayOfMonth = 31;
                switch (month)
                {
                    case 1:
                    case 3:
                    case 5:
                    case 7:
                    case 8:
                    case 10:
                    case 12:
                        DayOfMonth = 31;
                        break;
                    case 2:
                        if (year % 4 == 0 && year % 100 != 0 && year % 400 != 0)
                        {
                            DayOfMonth = 29;
                        }
                        else DayOfMonth = 28;
                        break;
                    case 4:
                    case 6:
                    case 9:
                    case 11:
                        DayOfMonth = 30;
                        break;
                }
                for (int i = 1; i <= DayOfMonth; i++)
                {
                    DataGridTextColumn column = new DataGridTextColumn();
                    column.Header = i.ToString() + "/" + Monthcbb.SelectedValue;
                    column.Binding = new Binding($"Shifts[{i - 1}]");
                    WorkersDataGrid.Columns.Add(column);
                }
                DataGridTextColumn column3 = new DataGridTextColumn();
                column3.Header = "Sum";
                column3.Binding = new Binding("Sum");
                WorkersDataGrid.Columns.Add(column3);

                DataGridTextColumn column4 = new DataGridTextColumn();
                column4.Header = "SumLate";
                column4.Binding = new Binding("SumLate");
                WorkersDataGrid.Columns.Add(column4);

                DataGridTextColumn column5 = new DataGridTextColumn();
                column4.Header = "Salary";
                column4.Binding = new Binding("SalaryMonth");
                WorkersDataGrid.Columns.Add(column5);

                // Get Data In Database
                List<Worker> source = new List<Worker>();//items in datagrid

                List<WorkerList> listWorker = new List<WorkerList>();
                try
                {
                    string querry1 = "SELECT * FROM WorkerList WHERE fid = '" + fid + "' ";
                    if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                    {
                        dtc.GetConnection().Open();
                    }
                    SQLcmd = new SqlCommand(querry1, dtc.GetConnection());
                    SqlDataReader reader1 = SQLcmd.ExecuteReader();
                    while (reader1.Read())
                    {
                        WorkerList addWorker = new WorkerList();
                        addWorker.Id = reader1.GetInt32(0);
                        addWorker.Fullname = reader1.GetString(1);
                        DateTime date = reader1.GetDateTime(2);
                        addWorker.Birth = date.Date;
                        addWorker.Salary = reader1.GetInt32(3);
                        addWorker.Images = reader1.GetString(4);
                        addWorker.Fid = reader1.GetString(5);
                        listWorker.Add(addWorker);
                    }
                    reader1.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                for (int i = 0; i < listWorker.Count(); i++)
                {
                    try
                    {
                        List<int> shifts = new List<int>();
                        for (int j = 0; j < DayOfMonth; j++)
                        {
                            string querryAttendance = "SELECT COUNT(shift_worked) FROM Attendance " +
                                             "WHERE id_faculty = @fid " +
                                             "AND YEAR(d_m) = @year " +
                                             "AND MONTH(d_m) = @month " +
                                             "AND DAY(d_m) = @day " +
                                             "AND id_worker = @id ";
                            SQLcmd = new SqlCommand(querryAttendance, dtc.GetConnection());
                            SQLcmd.Parameters.AddWithValue("@fid", fid);
                            SQLcmd.Parameters.AddWithValue("@year", year);
                            SQLcmd.Parameters.AddWithValue("@month", month);
                            SQLcmd.Parameters.AddWithValue("@day", j + 1);
                            SQLcmd.Parameters.AddWithValue("@id", listWorker[i].Id);
                            int shiftAttendance = Convert.ToInt32(SQLcmd.ExecuteScalar());

                            string querryLate = "SELECT COUNT(shift_worked) FROM LateList " +
                                             "WHERE id_faculty = @fid " +
                                             "AND YEAR(d_m) = @year " +
                                             "AND MONTH(d_m) = @month " +
                                             "AND DAY(d_m) = @day " +
                                             "AND id_worker = @id ";
                            SQLcmd = new SqlCommand(querryLate, dtc.GetConnection());
                            SQLcmd.Parameters.AddWithValue("@fid", fid);
                            SQLcmd.Parameters.AddWithValue("@year", year);
                            SQLcmd.Parameters.AddWithValue("@month", month);
                            SQLcmd.Parameters.AddWithValue("@day", j + 1);
                            SQLcmd.Parameters.AddWithValue("@id", listWorker[i].Id);
                            int shiftLate = Convert.ToInt32(SQLcmd.ExecuteScalar());

                            shifts.Add(shiftAttendance + shiftLate);
                        }

                        string querrySum = "SELECT COUNT(shift_worked) FROM Attendance " +
                                         "WHERE id_faculty = @fid " +
                                         "AND YEAR(d_m) = @year " +
                                         "AND MONTH(d_m) = @month " +
                                         "AND id_worker = @id ";
                        SQLcmd = new SqlCommand(querrySum, dtc.GetConnection());
                        SQLcmd.Parameters.AddWithValue("@fid", fid);
                        SQLcmd.Parameters.AddWithValue("@year", year);
                        SQLcmd.Parameters.AddWithValue("@month", month);
                        SQLcmd.Parameters.AddWithValue("@id", listWorker[i].Id);
                        int sum = Convert.ToInt32(SQLcmd.ExecuteScalar());

                        string querrySumLate = "SELECT COUNT(shift_worked) FROM LateList " +
                                         "WHERE id_faculty = @fid " +
                                         "AND YEAR(d_m) = @year " +
                                         "AND MONTH(d_m) = @month " +
                                         "AND id_worker = @id ";
                        SQLcmd = new SqlCommand(querrySumLate, dtc.GetConnection());
                        SQLcmd.Parameters.AddWithValue("@fid", fid);
                        SQLcmd.Parameters.AddWithValue("@year", year);
                        SQLcmd.Parameters.AddWithValue("@month", month);
                        SQLcmd.Parameters.AddWithValue("@id", listWorker[i].Id);
                        int sumLate = Convert.ToInt32(SQLcmd.ExecuteScalar());

                        int salaryMonth = (sum + sumLate) * listWorker[i].Salary;
                        salaryMonth = salaryMonth - (sumLate * 25000);

                        Worker worker = new Worker(listWorker[i].Id, listWorker[i].Fullname, shifts, sum + sumLate, sumLate, salaryMonth);
                        source.Add(worker);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                WorkersDataGrid.ItemsSource = source;
            }
        }

        private void Send_Button_Click(object sender, RoutedEventArgs e)
        {
            SendEmailUI win = new SendEmailUI();
            win.Show();
        }

        private void Excel_Button_Click(object sender, RoutedEventArgs e)
        {
            ExportExcel export = new ExportExcel();
            export.ExportExcel_DataGrid(WorkersDataGrid);
        }   
    }

    public class Worker
    {
        public int ID_Worker { get; set; }
        public string WorkerName { get; set; }
        public List<int> Shifts { get; set; }
        public int Sum { get; set; }
        public int SumLate { get; set; }
        public int SalaryMonth { get; set; }
        public Worker()
        {
            Shifts = new List<int>();
        }
        public Worker(int ID, String Name, List<int> shifts, int sum, int sumLate, int salaryMonth)
        {
            this.ID_Worker = ID;
            this.WorkerName = Name;
            Shifts = shifts;
            this.Sum = sum;
            this.SumLate = sumLate;
            this.SalaryMonth = salaryMonth;
        }
    }

}