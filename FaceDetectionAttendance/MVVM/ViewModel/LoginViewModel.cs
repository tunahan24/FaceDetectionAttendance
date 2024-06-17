using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using FaceDetectionAttendance.MVVM.Model;
using FaceDetectionAttendance.MVVM.View;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Controls;
using Unity;
using OpenCvSharp;
using FaceDetectionAttendance.MVVM.Components;


namespace FaceDetectionAttendance.MVVM.ViewModel
{
    public class LoginViewModel: ViewModelBase
    {
        private Dataconnecttion Dataconnecttion = new Dataconnecttion();
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private Account acc = new Account();
        private object _currentPage { get; set; }
        public object CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }
        public string UserName
        {
            get =>acc.username;
            set
            {
                acc.username = value;
                OnPropertyChanged(nameof(UserName));
            }
        }
        public string Password
        { 
            get => acc.password;
            set
            {
                acc.password= value;
                OnPropertyChanged(nameof(Password));
            }
        }
        public ICommand LoginCommand { get; set; }


        public LoginViewModel()
        {

            LoginCommand = new RelayCommand(Login, CanLogin);
        }
        private bool CanLogin(object parameter)
        {
            return !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password);
        }
        private void Login(object parameter)
        {
            string querry = "Select Count(1) from Account where username =@username and passwords=@password";
            try
            {
                if (Dataconnecttion.GetConnection().State == System.Data.ConnectionState.Closed)
                    Dataconnecttion.GetConnection().Open();

                SqlCommand cmd = new SqlCommand(querry, Dataconnecttion.GetConnection());
                cmd.Parameters.AddWithValue("@username", acc.username);
                cmd.Parameters.AddWithValue("@password", acc.password);
                int check = Convert.ToInt32(cmd.ExecuteScalar());
                if (check == 1)
                {
                    
                    string querry2 = "Select Roles from Account where username=@username";
                    try
                    {
                        SqlCommand cmd2 = new SqlCommand(querry2, Dataconnecttion.GetConnection());
                        cmd2.Parameters.AddWithValue("@username", acc.username);
                        int roles = Convert.ToInt32(cmd2.ExecuteScalar());
                        Dataconnecttion.GetConnection().Close();
                        MessageBoxChange ms = new MessageBoxChange(roles, acc.username);
                        ms.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        ms.Show();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Wrong username or password");
                    MessageBox.Show(acc.password);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
       
    }
}
