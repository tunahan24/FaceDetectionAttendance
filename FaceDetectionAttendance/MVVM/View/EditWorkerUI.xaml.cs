using Emgu.CV;
using Emgu.CV.Structure;
using FaceDetectionAttendance.MVVM.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for EditWorkerUI.xaml
    /// </summary>
    public partial class EditWorkerUI : Page
    {
        private string fullname;
        private string _faculty;
        private int id;
        private static string binFolderPath = System.IO.Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
        private static string projectFolderPath = Directory.GetParent(binFolderPath).FullName;
        private static string fix = projectFolderPath.Remove(projectFolderPath.Length - 9);
        private static string resourceFolderPath = System.IO.Path.Combine(fix, "Resource");
        Dataconnecttion dtc = new Dataconnecttion();
        SqlCommand cmd = new SqlCommand();
        VideoCapture capture;
        static readonly CascadeClassifier faceDetector = new CascadeClassifier(fix + "haarcascade_frontalface_default.xml");
        Image<Bgr, byte> image = null;
        DispatcherTimer timer = new DispatcherTimer();
        private bool iscapturing = false;

        public EditWorkerUI(WorkerList worker)
        {
            InitializeComponent();
            fullname = worker.Fullname;
            _faculty = worker.Fid;
            setComboboxData();
            setCurrentInfor(worker);
            FacultyText.Text = worker.Fid;
        }
        private void setComboboxData()
        {
            string query = "Select id_faculty from Faculty ";
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                dtc.GetConnection().Open();
            try
            {
                cmd = new SqlCommand(query, dtc.GetConnection());
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Facultycbb.Items.Add(reader.GetString(0));
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void setCurrentInfor(WorkerList worker)
        {

            FullNametxt.Text = worker.Fullname;
            Dobtxt.Text = worker.Birth.ToString().Remove(worker.Birth.ToString().Length-12);
            Facultycbb.SelectedItem = worker.Fid;
            Salarytxt.Text = worker.Salary.ToString();
            string query = "Select images, id from WorkerList where fullname =@fullname";
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                dtc.GetConnection().Open();
            cmd = new SqlCommand(query, dtc.GetConnection());
            cmd.Parameters.AddWithValue("@fullname", fullname);
            SqlDataReader reader = cmd.ExecuteReader();
            string imageName = " ";
            int idReader = 0;
            while (reader.Read())
            {
                imageName = reader.GetString(0);
                idReader = reader.GetInt32(1);
            }
            reader.Close();
            id = idReader;
            string currentImage = $"{resourceFolderPath}\\WorkerImage\\{_faculty}\\{imageName}.png";
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(currentImage, UriKind.Absolute);
            bitmap.EndInit();
            ResultImg.Source = bitmap;
            dtc.GetConnection().Close();
        }
        private void Backbtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
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
                        // Crop the face image
                        Rectangle face = faces[0];
                        Image<Gray, byte> faceImage = image.Convert<Gray, byte>().Copy(face);
                        string nameimg = FullNametxt.Text;
                        string imagePath = $"D:\\{nameimg}{id}.png";

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

                        MessageBox.Show("Worker added successfully.");

                    }
                    timer.Stop();
                    capture.Dispose();
                    break;
                }
            }
        }
        Image<Gray, byte> Resize(Image<Gray, byte> image)
        {
            System.Drawing.Size size = new System.Drawing.Size(200, 200);
            Image<Gray, byte> temp = image.Resize(size.Width, size.Height, Emgu.CV.CvEnum.Inter.Cubic);
            return temp;
        }

        private void Save_click(object sender, RoutedEventArgs e)
        {
             DateTime DoB = DateTime.ParseExact($"{Dobtxt.Text}", "dd/MM/yyyy",
                                        CultureInfo.InvariantCulture); ;
            string query = "Update WorkerList set fullname = @fullname, Birth = @dob, fid =@fid, images = @image, salary = @salary where id = @id";
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                dtc.GetConnection().Open();
            cmd = new SqlCommand(query, dtc.GetConnection());
            cmd.Parameters.AddWithValue("@fullname", FullNametxt.Text);
            cmd.Parameters.AddWithValue("@dob", DoB);
            cmd.Parameters.AddWithValue("@fid", Facultycbb.SelectedItem.ToString());
            cmd.Parameters.AddWithValue("@image", FullNametxt.Text + id);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@salary", Int32.Parse(Salarytxt.Text.ToString()));
            cmd.ExecuteNonQuery();
            //Image processing
            if (File.Exists($"D:\\{FullNametxt.Text}{id}.png") && FullNametxt.Text != fullname)
            {
                //change everything -> delete old image and replace
                //Delete old image
                string oldpath = $"{resourceFolderPath}\\WorkerImage\\{_faculty}\\{fullname}{id}.png";
                File.Delete(oldpath);
                //Copy new image to directed folder
                string targetpath = $"{resourceFolderPath}\\WorkerImage\\{Facultycbb.SelectedItem.ToString()}\\{FullNametxt.Text}{id}.png";
                File.Copy($"D:\\{FullNametxt.Text}{id}.png", targetpath);
                File.Delete($"D:\\{FullNametxt.Text}{id}.png");
            }
            else if (FullNametxt.Text == fullname && Facultycbb.SelectedItem.ToString() == _faculty)
            {
                //only change dob ->do not thing
            }
            else
            {
                //only change falculty-> Move old iamge to new folder
                string oldpath = $"{resourceFolderPath}\\WorkerImage\\{_faculty}\\{fullname}{id}.png";
                string targetpath = $"{resourceFolderPath}\\WorkerImage\\{Facultycbb.SelectedItem.ToString()}\\{FullNametxt.Text}{id}.png";
                File.Move(oldpath, targetpath);
            }
            MessageBox.Show("Successful change worker's information");
            this.NavigationService.GoBack(); 
        }
    }
}