using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using Emgu.CV.Structure;
using Emgu.CV;
using Emgu.CV.Util;
using FaceDetectionAttendance.MVVM.Model;
using Microsoft.Data.SqlClient;
using Emgu.CV.CvEnum;
using System.Windows.Media.Imaging;
using System.Drawing;
using Emgu.CV.Face;
using CascadeClassifier = Emgu.CV.CascadeClassifier;
using System.Windows.Interop;
using System.Linq;
using System.IO;
using System.Threading;
using System.Windows.Threading;

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for AttendanceUI.xaml
    /// </summary>
    public partial class AttendanceUI : Page
    {
        private Emgu.CV.VideoCapture _videoSource;
        private CascadeClassifier _faceClassifier;
        private readonly EigenFaceRecognizer _recognizer = new EigenFaceRecognizer();
        private Dataconnecttion dataconnecttion = new Dataconnecttion();
        private SqlCommand command;
        private List<Image<Gray, byte>> WorkerList = new List<Image<Gray, byte>>();// Worker's Image
        private List<AttendanceWorker> AttendList = new List<AttendanceWorker>(); //Attend datagrid
        private List<WorkerLabel> workerLabels = new List<WorkerLabel>();// Worker's infor
        private DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        private bool _isCapturing;
        private string _username;
        private string _faculty;
        private string querry;
        private int shift = 1;
        public AttendanceUI(string username)
        {
            InitializeComponent();
            _faceClassifier = new CascadeClassifier("haarcascade_frontalface_default.xml");
            _username = username;
            setData();
            FacultyText.Text = _faculty;
            ShiftText.Text = "Shift: " + shift;
        }
        private void setData()
        {
            string binFolderPath = System.IO.Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            string projectFolderPath = Directory.GetParent(binFolderPath).FullName;
            string fix = projectFolderPath.Remove(projectFolderPath.Length - 9);
            List<int> WorkerID = new List<int>();
            querry = "Select fid From Account where username = @username";
            if (dataconnecttion.GetConnection().State == System.Data.ConnectionState.Closed)
                dataconnecttion.GetConnection().Open();
            try
            {
                command = new SqlCommand(querry, dataconnecttion.GetConnection());
                command.Parameters.AddWithValue("@username", _username);
                _faculty = Convert.ToString(command.ExecuteScalar());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            querry = "Select images From WorkerList where fid = @fid";
            try
            {
                command = new SqlCommand(querry, dataconnecttion.GetConnection());
                command.Parameters.AddWithValue("@fid", _faculty);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string imageName = reader.GetString(0) + ".png";
                        string resourceFolderPath = Path.Combine(fix, "Resource");
                        Image<Gray, byte> temp = new Image<Gray, byte>(@$"{resourceFolderPath}\WorkerImage\{_faculty}\{imageName}");
                        WorkerList.Add(temp);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            querry = "Select fullname, id from WorkerList where fid = @fid";
            try
            {
                command = new SqlCommand(querry, dataconnecttion.GetConnection());
                command.Parameters.AddWithValue("@fid", _faculty);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        WorkerLabel wl = new WorkerLabel();
                        wl.Name = reader.GetString(0);
                        wl.Id = reader.GetInt32(1);
                        workerLabels.Add(wl);
                        WorkerID.Add(wl.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            List<Mat> workerImage = new List<Mat>();
            foreach (var image in WorkerList)
            {
                Mat mat = image.Mat;
                workerImage.Add(mat);
            }
            try
            {
                _recognizer.Train(new VectorOfMat(workerImage.ToArray()), new VectorOfInt(WorkerID.ToArray()));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not retrieve worker data! Please add worker data!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private void StartCam_Click(object sender, EventArgs e)
        {
            if (_isCapturing)
            {
                return;
            }
            _videoSource = new VideoCapture();
            _videoSource.Start();
            _isCapturing = true;

            // Start capturing video frames
            timer.Tick += new EventHandler(ProcessFrame);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 60);
            timer.Start();
        }
        private void StopCam_Click(object sender, EventArgs e)
        {
            if (!_isCapturing)
            {
                return;
            }
            timer.Stop();
            _videoSource.Stop();
            _videoSource.Dispose();
            _isCapturing = false;
            var absentWorkers = from workerLabel in workerLabels
                                join workerAttend in AttendList on workerLabel.Id equals workerAttend.id into workerGroup
                                from worker in workerGroup.DefaultIfEmpty()
                                where worker == null
                                select workerLabel;
            Absentee.ItemsSource = absentWorkers;
            AbsenteeTxt.Text = Absentee.Items.Count.ToString();
        }
        private void ProcessFrame(object sender, EventArgs e)
        {
            var frame = _videoSource.QueryFrame().ToImage<Bgr, byte>().Resize(640, 480, Inter.Cubic);
            var grayFrame = frame.Convert<Gray, byte>();

            // Detect faces in current frame
            var faces = _faceClassifier.DetectMultiScale(grayFrame, 1.2, 5);

            foreach (var face in faces)
            {
                // Extract face region of interest
                var faceRect = new Rectangle(face.X, face.Y, face.Width, face.Height);
                var faceImage = grayFrame.Copy(faceRect).Resize(200, 200, Inter.Cubic);

                // Recognize face
                var result = _recognizer.Predict(faceImage);

                // Display result
                var label = result.Label.ToString();

                if (checkId(Int32.Parse(label)))
                {
                    //Display worker name if recognized
                    var worker = workerLabels.FirstOrDefault(w => w.Id == Int32.Parse(label));// take worker id
                    if (worker != null)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            //KHi nhan dien ra cong nhan
                            if (CheckAttandance(Int32.Parse(label),_faculty,shift))
                            {
                                string querry = "INSERT INTO Attendance VALUES (@id_worker, @date, @fid, @shift)";
                               // string querry2 = "SELECT MAX(id) FROM Attendance";
                                if (dataconnecttion.GetConnection().State == System.Data.ConnectionState.Closed)
                                    dataconnecttion.GetConnection().Open();
                                //command = new SqlCommand(querry2, dataconnecttion.GetConnection());
                                //int id;
                                //if (command.ExecuteScalar() != null)
                                //{
                                //    id = Convert.ToInt32(command.ExecuteScalar().ToString()) + 1;
                                //}                                  
                                //else
                                //{
                                //    id = 1;
                                //}                                
                                command = new SqlCommand(querry, dataconnecttion.GetConnection());
                                //command.Parameters.AddWithValue("@id", id);
                                command.Parameters.AddWithValue("@id_worker", Int32.Parse(label));
                                command.Parameters.AddWithValue("@date", DateTime.Now);
                                command.Parameters.AddWithValue("@fid", _faculty);
                                command.Parameters.AddWithValue("@shift", shift);
                                command.ExecuteNonQuery();
                                dataconnecttion.GetConnection().Close();
                            }
                            AttendanceWorker temp = new AttendanceWorker();
                            temp.name = worker.Name;
                            temp.id = Int32.Parse(label);
                            temp.date = DateTime.Now;
                            Attendance.Items.Add(new { id_worker = worker.Id, Name = worker.Name, TimeIn = DateTime.Now });
                            workerLabels.Remove(worker);
                            AttendanceTxt.Text = Attendance.Items.Count.ToString();
                        });
                    }
                }
                // Draw a rectangle around the face
                frame.Draw(faceRect, new Bgr(0, 0, 255), 2);
            }

            // Display the processed frame in the image control
            var bitmap = frame.ToBitmap();
            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            VideoDisplay.Source = bitmapSource;

        }
        private bool checkId(int id)
        {
            for (int i = 0; i < workerLabels.Count; i++)
            {
                if (workerLabels[i].Id == id)
                    return true;
            }
            return false;
        }

        private void Switchbtn_Click(object sender, RoutedEventArgs e)
        {
            if (ShiftText.Text.Equals("Shift: 1")) shift = 2;
            else shift = 1;
            ShiftText.Text = "Shift: " + shift;
            workerLabels.Clear();
            WorkerList.Clear();    
            setData();
            Attendance.Items.Clear();
            Absentee.ItemsSource = null;
            AttendanceTxt.Text = "";
            AbsenteeTxt.Text = "";
        }

        private bool CheckAttandance(int id_worker, string fid, int shift)
        {   // check null or not 
            string querry = "SELECT id " +
                            "FROM Attendance " +
                            "WHERE id_worker = @id_worker " +
                            "AND DAY(d_m) = @day " +
                            "AND MONTH(d_m) = @month " +
                            "AND YEAR(d_m) = @year " +
                            "AND id_faculty = @fid " +
                            "AND shift_worked = @shift ";
            if(dataconnecttion.GetConnection().State == System.Data.ConnectionState.Closed)
            {
                dataconnecttion.GetConnection().Open();
            }
            command = new SqlCommand(querry, dataconnecttion.GetConnection());
            command.Parameters.AddWithValue("@id_worker",id_worker);
            command.Parameters.AddWithValue("@day", DateTime.Now.Day);
            command.Parameters.AddWithValue("@month", DateTime.Now.Month);
            command.Parameters.AddWithValue("@year", DateTime.Now.Year);
            command.Parameters.AddWithValue("@fid", fid);
            command.Parameters.AddWithValue("@shift", shift);

            if (command.ExecuteScalar()== null) return true;
            return false;
        }

    }
}
