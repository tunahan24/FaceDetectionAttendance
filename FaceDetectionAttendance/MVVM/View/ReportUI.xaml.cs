using Emgu.CV.CvEnum;
using FaceDetectionAttendance.MVVM.Model;
using Microsoft.Data.SqlClient;
using ClosedXML;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.IO;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2013.Excel;
using System.Reflection;
using FaceDetectionAttendance.MVVM.ViewModel;

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for ReportUI.xaml
    /// </summary>    
    public partial class ReportUI : Page
    {
        private Dataconnecttion dtc = new Dataconnecttion();
        private string fid;
        private SqlCommand cmd = new SqlCommand();
        public ReportUI(string fid)
        {
            InitializeComponent();
            this.fid = fid;
            Faculty_TextBlock.Text = fid;
        }
        private void Shift_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedValue = Shift_ComboBox.SelectedValue.ToString();
            if (AttandanceWorkers_DataGrid_1 != null &&
                AbsenteeWorkers_DataGrid_1 != null &&
                LateWorkers_DataGrid_1 != null &&
                AttandanceWorkers_DataGrid_2 != null &&
                AbsenteeWorkers_DataGrid_2 != null &&
                LateWorkers_DataGrid_2 != null )
            {
                if ("Ca 1".Equals(selectedValue))
                {
                    AttandanceWorkers_DataGrid_2.Visibility = Visibility.Collapsed;
                    LateWorkers_DataGrid_2.Visibility = Visibility.Collapsed;
                    AbsenteeWorkers_DataGrid_2.Visibility = Visibility.Collapsed;

                    AttandanceWorkers_DataGrid_1.Visibility = Visibility.Visible;
                    LateWorkers_DataGrid_1.Visibility = Visibility.Visible;
                    AbsenteeWorkers_DataGrid_1.Visibility = Visibility.Visible;
                }
                else
                {
                    AttandanceWorkers_DataGrid_1.Visibility = Visibility.Collapsed;
                    LateWorkers_DataGrid_1.Visibility = Visibility.Collapsed;
                    AbsenteeWorkers_DataGrid_1.Visibility = Visibility.Collapsed;

                    AttandanceWorkers_DataGrid_2.Visibility = Visibility.Visible;
                    LateWorkers_DataGrid_2.Visibility = Visibility.Visible;
                    AbsenteeWorkers_DataGrid_2.Visibility = Visibility.Visible;
                }

            }
        }
        private void Search_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Date_DatePicker.SelectedDate != null)
            {
                DateTime date = Convert.ToDateTime(Date_DatePicker.SelectedDate);

                AttandanceWorkers_DataGrid_1.Items.Clear();
                AttandanceWorkers_DataGrid_2.Items.Clear();
                LateWorkers_DataGrid_1.Items.Clear();
                LateWorkers_DataGrid_2.Items.Clear();
                AbsenteeWorkers_DataGrid_1.Items.Clear();
                AbsenteeWorkers_DataGrid_2.Items.Clear();

                try
                {
                    for (int i = 1; i <= 2; i++) // 2 ca
                    {
                        string query1 = "SELECT id_worker,fullname,d_m " +
                                       "FROM Attendance " +
                                       "INNER JOIN WorkerList " +
                                       "ON Attendance.id_worker = WorkerList.id " +
                                       "WHERE id_faculty = @fid " +
                                       "AND shift_worked = @i " +
                                       "AND CONVERT(date,d_m) = @date";

                        string query2 = "SELECT id_worker,fullname,d_m " +
                                       "FROM LateList " +
                                       "INNER JOIN WorkerList " +
                                       "ON LateList.id_worker = WorkerList.id " +
                                       "WHERE id_faculty = @fid " +
                                       "AND shift_worked = @i " +
                                       "AND CONVERT(date,d_m) = @date";

                        string query3 = "SELECT id, fullname " +
                                        "FROM WorkerList " +
                                        "WHERE id NOT IN (" +
                                            "SELECT id_worker " +
                                            "FROM Attendance " +
                                            "WHERE id_faculty = @fid " +
                                            "AND shift_worked = @i " +
                                            "AND CONVERT(date,d_m) = @date " +
                                        ") " +
                                        "AND id NOT IN (" +
                                            "SELECT id_worker " +
                                            "FROM LateList " +
                                            "WHERE id_faculty = @fid " +
                                            "AND shift_worked = @i " +
                                            "AND CONVERT(date,d_m) = @date " +
                                        ") ";
                        if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                        {
                            dtc.GetConnection().Open();
                        }

                        cmd = new SqlCommand(query1, dtc.GetConnection());
                        cmd.Parameters.AddWithValue("@fid", this.fid);
                        cmd.Parameters.AddWithValue("@i", i);
                        cmd.Parameters.AddWithValue("@date", date.Date);
                        SqlDataReader read1 = cmd.ExecuteReader();
                        while (read1.Read())
                        {
                            int id = read1.GetInt32(0);
                            string name = read1.GetString(1);
                            DateTime dt = read1.GetDateTime(2);
                            if (i == 1)
                            {
                                AttandanceWorkers_DataGrid_1.Items.Add(new { id = id, name = name, dt = dt });
                            }
                            else
                            {
                                AttandanceWorkers_DataGrid_2.Items.Add(new { id = id, name = name, dt = dt });
                            }
                        }
                        read1.Close();

                        cmd = new SqlCommand(query2, dtc.GetConnection());
                        cmd.Parameters.AddWithValue("@fid", this.fid);
                        cmd.Parameters.AddWithValue("@i", i);
                        cmd.Parameters.AddWithValue("@date", date.Date);
                        SqlDataReader read2 = cmd.ExecuteReader();
                        while (read2.Read())
                        {
                            int id = read2.GetInt32(0);
                            string name = read2.GetString(1);
                            DateTime dt = read2.GetDateTime(2);
                            if (i == 1)
                            {
                                LateWorkers_DataGrid_1.Items.Add(new { id = id, name = name, dt = dt });
                            }
                            else
                            {
                                LateWorkers_DataGrid_2.Items.Add(new { id = id, name = name, dt = dt });
                            }
                        }
                        read2.Close();

                        cmd = new SqlCommand(query3, dtc.GetConnection());
                        cmd.Parameters.AddWithValue("@fid", this.fid);
                        cmd.Parameters.AddWithValue("@i", i);
                        cmd.Parameters.AddWithValue("@date", date.Date);
                        SqlDataReader read3 = cmd.ExecuteReader();
                        while (read3.Read())
                        {
                            int id = read3.GetInt32(0);
                            string name = read3.GetString(1);
                            if (i == 1)
                            {
                                AbsenteeWorkers_DataGrid_1.Items.Add(new { id = id, name = name });
                            }
                            else
                            {
                                AbsenteeWorkers_DataGrid_2.Items.Add(new { id = id, name = name });
                            }
                        }
                        read3.Close();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error");
                }
            }
            else MessageBox.Show("Please choose a date", "Message");
        }

        private void ReportMonth_Button_Click(object sender, RoutedEventArgs e)
        {
            var mainWindown = Window.GetWindow(this) as MainWindow;
            if(mainWindown != null)
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

        private void Export_Button_Click(object sender, RoutedEventArgs e)
        {
            if (AttandanceWorkers_DataGrid_1.Items.Count == 0 &&
                LateWorkers_DataGrid_1.Items.Count == 0 &&
                AbsenteeWorkers_DataGrid_1.Items.Count == 0)
            {
                MessageBox.Show("Data in the table is empty", "Error", MessageBoxButton.OK);
            }
            else 
            {
                SaveFileDialog savefile = new SaveFileDialog();
                savefile.DefaultExt = ".xlsx";
                savefile.Filter = "Excel Files|*xlsx;*xls;*xlsm";
                savefile.FileName = "ReportDay.xlsx";
                if (savefile.ShowDialog() == true)
                {
                    try
                    {
                        var workbook = new XLWorkbook();

                        workbook.Style.Font.FontSize = 11;
                        workbook.Style.Font.FontName = "Times New Roman";

                        var sheet1 = workbook.Worksheets.Add("Shift 1");
                        int rowWrite = 1;

                        Type t;
                        PropertyInfo[] p;
                        IXLCell cell;
                        //Attendance
                        cell = sheet1.Cell(rowWrite, 1);
                        cell.Value = "Attandance workers list";
                        cell.Style.Font.Bold= true;
                        cell.Style.Font.FontSize = 14;
                        rowWrite++;
                        for (int col = 0; col < AttandanceWorkers_DataGrid_1.Columns.Count; col++)
                        {
                            cell = sheet1.Cell(rowWrite, col + 1);
                            cell.Value = AttandanceWorkers_DataGrid_1.Columns[col].Header.ToString();
                            cell.Style.Fill.BackgroundColor = XLColor.Aqua;
                            cell.Style.Font.Bold = true;
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }
                        rowWrite++;

                        foreach (var item in AttandanceWorkers_DataGrid_1.Items) 
                        {
                            for (int col = 0; col < AttandanceWorkers_DataGrid_1.Columns.Count; col++)
                            {
                                TextBlock Value = AttandanceWorkers_DataGrid_1.Columns[col].GetCellContent(item) as TextBlock;
                                cell = sheet1.Cell(rowWrite, col + 1);
                                cell.Value = Value.Text;
                                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#F2F2F2");
                                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            }
                            rowWrite++;
                        }
                        rowWrite++;

                        //Late
                        cell = sheet1.Cell(rowWrite, 1);
                        cell.Value = "Late workers list";
                        cell.Style.Font.Bold = true;
                        cell.Style.Font.FontSize = 14;
                        rowWrite++;
                        for (int col = 0; col < LateWorkers_DataGrid_1.Columns.Count; col++)
                        {
                            cell = sheet1.Cell(rowWrite, col + 1);
                            cell.Value = LateWorkers_DataGrid_1.Columns[col].Header.ToString();
                            cell.Style.Fill.BackgroundColor = XLColor.Aqua;
                            cell.Style.Font.Bold = true;
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }
                        rowWrite++;

                        foreach (var item in LateWorkers_DataGrid_1.Items)
                        {
                            for (int col = 0; col < LateWorkers_DataGrid_1.Columns.Count; col++)
                            {
                                TextBlock Value = LateWorkers_DataGrid_1.Columns[col].GetCellContent(item) as TextBlock;
                                cell = sheet1.Cell(rowWrite, col + 1);
                                cell.Value = Value.Text;
                                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#F2F2F2");
                                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            }
                            rowWrite++;
                        }
                        rowWrite++;

                        //Absentee
                        cell = sheet1.Cell(rowWrite, 1);
                        cell.Value = "Absentee workers list";
                        cell.Style.Font.Bold = true;
                        cell.Style.Font.FontSize= 14;
                        rowWrite++;
                        for (int col = 0; col < AbsenteeWorkers_DataGrid_1.Columns.Count; col++)
                        {
                            cell = sheet1.Cell(rowWrite, col + 1);
                            cell.Value = AbsenteeWorkers_DataGrid_1.Columns[col].Header.ToString();
                            cell.Style.Fill.BackgroundColor = XLColor.Aqua;
                            cell.Style.Font.Bold = true;
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }
                        rowWrite++;
                        foreach (var item in AbsenteeWorkers_DataGrid_1.Items)
                        {
                            for (int col = 0; col < AbsenteeWorkers_DataGrid_1.Columns.Count; col++)
                            {
                                TextBlock Value = AbsenteeWorkers_DataGrid_1.Columns[col].GetCellContent(item) as TextBlock;
                                cell = sheet1.Cell(rowWrite, col + 1);
                                cell.Value = Value.Text;
                                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#F2F2F2");
                                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            }
                            rowWrite++;
                        }
                        rowWrite++;
                        sheet1.Columns().AdjustToContents(); // adjust column width auto fit with contents


                        ////////// sheet2 ///////////////////////////////////////////////////////////
                        rowWrite = 1;
                        var sheet2 = workbook.Worksheets.Add("shift 2");
                        //Attandance
                        cell = sheet2.Cell(rowWrite, 1);
                        cell.Value = "Attandance workers list";
                        cell.Style.Font.Bold = true;
                        cell.Style.Font.FontSize = 14;
                        rowWrite++;
                        for (int col = 0; col < AttandanceWorkers_DataGrid_2.Columns.Count; col++)
                        {
                            cell = sheet2.Cell(rowWrite, col + 1);
                            cell.Value = AttandanceWorkers_DataGrid_2.Columns[col].Header.ToString();
                            cell.Style.Fill.BackgroundColor = XLColor.Aqua;
                            cell.Style.Font.Bold = true;
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }
                        rowWrite++;

                        foreach (var item in AttandanceWorkers_DataGrid_2.Items)
                        {
                            for (int col = 0; col < AttandanceWorkers_DataGrid_2.Columns.Count; col++)
                            {
                                TextBlock Value = AttandanceWorkers_DataGrid_2.Columns[col].GetCellContent(item) as TextBlock;
                                cell = sheet2.Cell(rowWrite, col + 1);
                                cell.Value = Value.Text;
                                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#F2F2F2");
                                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            }
                            rowWrite++;
                        }
                        rowWrite++;

                        //Late
                        cell = sheet2.Cell(rowWrite, 1);
                        cell.Value = "Late workers list";
                        cell.Style.Font.Bold = true;
                        cell.Style.Font.FontSize = 14;
                        rowWrite++;
                        for (int col = 0; col < LateWorkers_DataGrid_2.Columns.Count; col++)
                        {
                            cell = sheet2.Cell(rowWrite, col + 1);
                            cell.Value = LateWorkers_DataGrid_2.Columns[col].Header.ToString();
                            cell.Style.Fill.BackgroundColor = XLColor.Aqua;
                            cell.Style.Font.Bold = true;
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }
                        rowWrite++;

                        foreach (var item in LateWorkers_DataGrid_2.Items)
                        {
                            for (int col = 0; col < LateWorkers_DataGrid_2.Columns.Count; col++)
                            {
                                TextBlock Value = LateWorkers_DataGrid_2.Columns[col].GetCellContent(item) as TextBlock;
                                cell = sheet2.Cell(rowWrite, col + 1);
                                cell.Value = Value.Text;
                                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#F2F2F2");
                                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            }
                            rowWrite++;
                        }
                        rowWrite++;
                        //Absentee
                        cell = sheet2.Cell(rowWrite, 1);
                        cell.Value = "Absentee workers list";
                        cell.Style.Font.Bold = true;
                        cell.Style.Font.FontSize = 14;
                        rowWrite++;
                        for (int col = 0; col < AbsenteeWorkers_DataGrid_2.Columns.Count; col++)
                        {
                            cell = sheet2.Cell(rowWrite, col + 1);
                            cell.Value = AbsenteeWorkers_DataGrid_2.Columns[col].Header.ToString();
                            cell.Style.Fill.BackgroundColor = XLColor.Aqua;
                            cell.Style.Font.Bold = true;
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }
                        rowWrite++;
                        foreach (var item in AbsenteeWorkers_DataGrid_2.Items)
                        {
                            for (int col = 0; col < AbsenteeWorkers_DataGrid_2.Columns.Count; col++)
                            {
                                TextBlock Value = AbsenteeWorkers_DataGrid_2.Columns[col].GetCellContent(item) as TextBlock;
                                cell = sheet2.Cell(rowWrite, col + 1);
                                cell.Value = Value.Text;
                                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#F2F2F2");
                                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            }
                            rowWrite++;
                        }
                        rowWrite++;

                        sheet2.Columns().AdjustToContents();

                        workbook.SaveAs(savefile.FileName);
                        MessageBox.Show("Done", "Message", MessageBoxButton.OK);
                        workbook.Dispose();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void Send_Button_Click(object sender, RoutedEventArgs e)
        {
            SendEmailUI win = new SendEmailUI();
            win.Show();

        }
    }
}
