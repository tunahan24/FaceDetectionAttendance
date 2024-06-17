using FaceDetectionAttendance.MVVM.Model;
using FaceDetectionAttendance.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
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
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.IdentityModel.Logging;

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for ReportMonthUI.xaml
    /// </summary>
    
    public partial class ReportMonthUI : Page
    {
        private Dataconnecttion dtc = new Dataconnecttion();
        private string fid;
        SqlCommand cmd = new SqlCommand();
        public ReportMonthUI(string fid)
        {
            InitializeComponent();
            this.fid = fid;
            Faculty_TextBlock.Text = fid;
        }

        private void ReportMonth_Button_Click(object sender, RoutedEventArgs e)
        {
            var mainWindown = Window.GetWindow(this) as MainWindow;
            if (mainWindown != null)
            {
                NavigationService.Navigate(new ReportMonthUI(fid));
            }
        }

        private void ReportDay_Button_Click(object sender, RoutedEventArgs e)
        {
            var mainWindown = Window.GetWindow(this) as MainWindow;
            if (mainWindown != null)
            {
                NavigationService.Navigate(new ReportUI(fid));
            }
        }

        private void TextBlock_OnlyNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
                Message_Label.Visibility = Visibility.Visible;
            }
            else Message_Label.Visibility = Visibility.Collapsed;
        }
        public bool Check_DMY()
        { //DayMonthYear
            if (Month_TextBox.Text.ToString() == "" || Year_TextBox.ToString() == "")
            {
                MessageBox.Show("Please enter full information", "Error", MessageBoxButton.OK);
                return false;
            }
            else
            {
                int month = int.Parse(Month_TextBox.Text.ToString());
                int year = int.Parse(Year_TextBox.Text.ToString());

                if (month <= 0 || month >= 13)
                {
                    MessageBox.Show("Just have 12 month a year, please re-enter", "Error", MessageBoxButton.OK);
                    return false;
                }
            }
            return true;
        }

        private void Search_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Check_DMY())
            {
                MonthCount_DataGrid.Items.Clear();
                int month = int.Parse(Month_TextBox.Text.ToString());
                int year = int.Parse(Year_TextBox.Text.ToString());
                int CountDay = DateTime.DaysInMonth(year, month);
                try
                {
                    if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                    {
                        dtc.GetConnection().Open();
                    }
                    string querry = "SELECT * FROM WorkerList WHERE fid = '" + this.fid + "' ";
                    cmd = new SqlCommand(querry, dtc.GetConnection());
                    List<WorkerList> listWorker = new List<WorkerList>();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        WorkerList item = new WorkerList();
                        item.Id = reader.GetInt32(0);
                        item.Fullname = reader.GetString(1);
                        item.Birth = reader.GetDateTime(2);
                        item.Salary = reader.GetInt32(3);//fix
                        item.Images= reader.GetString(4);
                        item.Fid = reader.GetString(5);
                        listWorker.Add(item);
                    }
                    reader.Close();

                    string monthYear = month+"/"+ year;
                    foreach (WorkerList item in listWorker)
                    {
                        string querryAttend = "SELECT COUNT(shift_worked) FROM Attendance " +
                                              "WHERE id_worker = @id_worker " +
                                              "AND MONTH(d_m) = @month " +
                                              "AND YEAR(d_m) = @year ";
                        cmd = new SqlCommand(querryAttend, dtc.GetConnection());
                        cmd.Parameters.AddWithValue("@id_worker", item.Id);
                        cmd.Parameters.AddWithValue("@month", month);
                        cmd.Parameters.AddWithValue("@year", year);
                        int attend = Convert.ToInt32(cmd.ExecuteScalar());

                        string querryLate = "SELECT COUNT(shift_worked) FROM LateList " +
                                            "WHERE id_worker = @id_worker " +
                                            "AND MONTH(d_m) = @month " +
                                            "AND YEAR(d_m) = @year ";
                        cmd = new SqlCommand(querryLate, dtc.GetConnection());
                        cmd.Parameters.AddWithValue("@id_worker", item.Id);
                        cmd.Parameters.AddWithValue("@month", month);
                        cmd.Parameters.AddWithValue("@year", year);
                        int late = Convert.ToInt32(cmd.ExecuteScalar());

                        // all of shift in month - all of worked = absent (shift not working)
                        int absent = CountDay * 2 - (attend + late);
                        MonthCount_DataGrid.Items.Add(new {id = item.Id , name = item.Fullname , monthYear = monthYear , worked = attend+late , late = late, absent = absent});
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error");
                }

            }
        }

        private void Export_Button_Click(object sender, RoutedEventArgs e)
        {
            ExportExcel ex = new ExportExcel();
            ex.ExportExcel_DataGrid(MonthCount_DataGrid);
        }

        private void Send_Button_Click(object sender, RoutedEventArgs e)
        {
            SendEmailUI win = new SendEmailUI();
            win.Show();
        }
    }
}
