using System;
using System.Data;
using System.Drawing;
using System.IO;
using FaceDetectionAttendance.MVVM.Model;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Globalization;
using System.Windows.Navigation;
using Emgu.CV;
using Emgu.CV.UI;
using System.Text.RegularExpressions;
using System.Windows.Media.Media3D;
using Emgu.CV.Structure;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for AddWorkerUI.xaml
    /// </summary>
    public partial class AddWorkerUI : Page
    {

        private Dataconnecttion dtc = new Dataconnecttion();
        private SqlCommand SQLcommand;
        VideoCapture capture;

        private static string binFolderPath = System.IO.Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
        private static string projectFolderPath = Directory.GetParent(binFolderPath).FullName;
        private static string fix = projectFolderPath.Remove(projectFolderPath.Length - 9);
        private static string resourceFolderPath = System.IO.Path.Combine(fix, "Resource");
        private static string XMLFolderPath = System.IO.Path.Combine(fix, "haarcascade_frontalface_default.xml");

        static readonly CascadeClassifier faceDetector = new CascadeClassifier($"{XMLFolderPath}");
       
        Image<Bgr, byte> image = null;
        DispatcherTimer timer = new DispatcherTimer();
        private bool iscapturing = false;
        private string _faculty;

        public AddWorkerUI(string faculty)
        {
            InitializeComponent();
            this._faculty = faculty;
            Add_SetComboBoxData();
            FacultyText.Text = faculty;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FullNametxt.Text) ||
               Dobtxt.Text.Length == 0 ||
               Falcutybox.SelectedItem == null ||
               Salarytxt.Text.Length ==0
               )
            {
                MessageBox.Show("Hãy nhập đủ thông tin.");
                return;
            }
            string fullname = FullNametxt.Text;
            DateTime DoB = DateTime.ParseExact($"{Dobtxt.Text}", "dd/MM/yyyy",
                                        CultureInfo.InvariantCulture); 
            string faculty = Falcutybox.SelectedItem.ToString();
            FullNametxt.Text = "";
            Dobtxt.Text = "";
            Falcutybox.SelectedItem = null;

            /* add du kieu*/
            string binFolderPath = System.IO.Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            string projectFolderPath = Directory.GetParent(binFolderPath).FullName;
            string fix = projectFolderPath.Remove(projectFolderPath.Length - 9);
            string resourceFolderPath = System.IO.Path.Combine(fix, "Resource");
            string avt = resourceFolderPath + @$"\Avatar\{FullNametxt}.png";
            //     File.Copy("C:\\FDA temp\\temp.png", avt);
            string querry = "Insert into WorkerList Values(@fullname, @birth,@salary ,@images, @fid)";
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                dtc.GetConnection().Open();

            int id_worker = getIdWorker_Next();

            try
            {
                SQLcommand = new SqlCommand(querry, dtc.GetConnection());
                SQLcommand.Parameters.Add("@fullname", SqlDbType.NVarChar).Value = fullname;
                SQLcommand.Parameters.Add("@birth", SqlDbType.Date).Value = DoB.Date.ToString();
                SQLcommand.Parameters.Add("@fid", SqlDbType.NVarChar).Value = faculty;
                SQLcommand.Parameters.Add("@images", SqlDbType.NVarChar).Value = fullname+id_worker;
                SQLcommand.Parameters.AddWithValue("@salary", Int32.Parse(Salarytxt.Text));
                SQLcommand.ExecuteNonQuery();
                MessageBox.Show("Worker added successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Backbtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
        private void Add_SetComboBoxData()
        {

            string querry = "Select* from Faculty";
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                dtc.GetConnection().Open();
            SQLcommand = new SqlCommand(querry, dtc.GetConnection());
            SqlDataReader reader = SQLcommand.ExecuteReader();
            while (reader.Read())
            {
                Faculty a = new Faculty();
                a.IdFaculty = reader.GetString(0);
                a.NameFaculty = reader.GetString(1);
                Falcutybox.Items.Add(a.IdFaculty);
            }
            Falcutybox.SelectedIndex = Falcutybox.Items.IndexOf(_faculty);
        }
        private void Startcam_Click(object sender, RoutedEventArgs e)
        {
            capture = new VideoCapture();
            capture.Set(Emgu.CV.CvEnum.CapProp.FrameWidth, 200);
            capture.Set(Emgu.CV.CvEnum.CapProp.FrameHeight, 200);
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(60);
            timer.Start();
        }

        async void Timer_Tick(object? sender, EventArgs e)
        {
            Mat frame = new Mat();
            capture.Read(frame);
            // Convert the image to a BitmapSource that can be displayed in the Image control
            Image<Bgr, byte> images = frame.ToImage<Bgr, byte>();
            //
            Rectangle[] faces = faceDetector.DetectMultiScale(images.Convert<Gray, byte>(), 1.2, 10, System.Drawing.Size.Empty);

            // Draw rectangles around the faces
            foreach (Rectangle face in faces)
            {
                images.Draw(face, new Bgr(System.Drawing.Color.Red), 2);
            }
            BitmapSource bitmap = BitmapSourceConvert.ToBitmapSource(images);
            // Set the Image control's Source property to the BitmapSource
            webcam.Source = bitmap;
        }

        private void Capture_Click(object sender, RoutedEventArgs e)
        {
            //string binFolderPath = System.IO.Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            //string projectFolderPath = Directory.GetParent(binFolderPath).FullName;
            //string fix = projectFolderPath.Remove(projectFolderPath.Length - 9);
            //string resourceFolderPath = Path.Combine(fix, "Resource");
            iscapturing = true;
            using (capture)
            {

                // Capture images from the webcam
                while (true)
                {
                    Mat frame = new Mat();
                    capture.Read(frame);
                    image = frame.ToImage<Bgr, byte>();

                    // Detect faces in the image
                    Rectangle[] faces = faceDetector.DetectMultiScale(image.Convert<Gray, byte>(), 1.2, 10, System.Drawing.Size.Empty);

                    // Draw rectangles around the faces
                    foreach (Rectangle face in faces)
                    {
                        image.Draw(face, new Bgr(System.Drawing.Color.Red), 2);
                    }

                    // Update the image control with the latest image
                    //result.Source = BitmapSourceConvert.ToBitmapSource(image);
                    // Wait for the user to click on the image control
                    if (iscapturing == true)
                    {
                        try
                        {
                            // Crop the face image
                            Rectangle face = faces[0];
                            Image<Gray, byte> faceImage = image.Convert<Gray, byte>().Copy(face);
                            string nameimg = FullNametxt.Text;
                            int idNext = getIdWorker_Next();
                            string imagePath = $"{resourceFolderPath}\\WorkerImage\\{Falcutybox.SelectedItem.ToString()}\\{nameimg}{idNext}.png";

                            if (File.Exists(imagePath))
                            {
                                ResultImg.Source = null;
                                File.Delete(imagePath);
                            }
                            Resize(faceImage).Save(imagePath);

                            BitmapImage bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                            bitmap.EndInit();

                            ResultImg.Source = bitmap;

                            MessageBox.Show("Add worker picture done.");
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show("Please, try add picture again", "Error add picture", MessageBoxButton.OK);
                        }
                        

                    }
                    timer.Stop();
                    capture.Dispose();
                    break;
                }
            }
        }
        private int getIdWorker_Next()
        {
            string querry1 = "SELECT MAX(id) AS MaxID FROM WorkerList";
            SQLcommand = new SqlCommand(querry1, dtc.GetConnection());
            SqlDataReader reader = SQLcommand.ExecuteReader();
            int id_worker = -1;
            while (reader.Read())
            {
                if (reader.IsDBNull(reader.GetOrdinal("MaxID"))) id_worker++;
                else id_worker = reader.GetInt32(0) + 1;
            }
            reader.Close();
            return id_worker;
        }

        Image<Gray, byte> Resize(Image<Gray, byte> image)
        {
            System.Drawing.Size size = new System.Drawing.Size(200, 200);
            Image<Gray, byte> temp = image.Resize(size.Width, size.Height, Emgu.CV.CvEnum.Inter.Cubic);
            return temp;
        }
    } 

        
    

}
