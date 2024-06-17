using Emgu.CV;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ClosedXML;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2013.Excel;
using System.Reflection;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data;

namespace FaceDetectionAttendance.MVVM.ViewModel
{
    internal class ExportExcel
    {
        public void ExportExcel_DataGrid(DataGrid temp)
        {
            if (temp.Items.Count > 0)
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.DefaultExt= ".xlsx";
                saveDialog.Filter = "Excel Files|*xlsx;*xls;*xlsm";
                saveDialog.FileName = "Report.xlsx";
                if(saveDialog.ShowDialog()==true)
                {
                    try
                    {
                        var workbook = new XLWorkbook();
                        var sheet = workbook.Worksheets.Add(temp.Name);
                        int rowWrite = 1;
                        IXLCell cell;
                        cell = sheet.Cell(rowWrite, 1);
                        cell.Value= temp.Name;
                        cell.Style.Font.Bold = true;
                        cell.Style.Font.FontSize = 14;
                        rowWrite++;
                        for(int col = 0; col< temp.Columns.Count; col++)
                        {
                            cell = sheet.Cell(rowWrite, col + 1);
                            cell.Value = temp.Columns[col].Header.ToString();
                            cell.Style.Fill.BackgroundColor = XLColor.Aqua;
                            cell.Style.Font.Bold = true;
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }
                        rowWrite++;

                        foreach (var item in temp.Items)
                        {
                            for(int col = 0; col < temp.Columns.Count; col++)
                            {
                                TextBlock Value = temp.Columns[col].GetCellContent(item) as TextBlock;
                                cell = sheet.Cell(rowWrite, col + 1);
                                cell.Value = Value.Text;
                                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#F2F2F2");
                                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            }
                            rowWrite++;
                        }

                        workbook.SaveAs(saveDialog.FileName);
                        MessageBox.Show("Export to excel successfull", "Message");
                        workbook.Dispose();
                        
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
            else
            {
                MessageBox.Show("Data in Table is nothing","Error",MessageBoxButton.OK);
            }
        }
    }
}
