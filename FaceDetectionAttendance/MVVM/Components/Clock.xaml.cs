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
using System.Windows.Threading;

namespace FaceDetectionAttendance.MVVM.Components
{
    /// <summary>
    /// Interaction logic for Clock.xaml
    /// </summary>
    public partial class Clock : UserControl
    {
        private DispatcherTimer timer;

        public Clock()
        {
            InitializeComponent();
            TimeTextBlock.FontSize= 20;
            TimeTextBlock.Height=60;
            TimeTextBlock.Width=120;
            TimeTextBlock.FontWeight= FontWeights.Bold;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeTextBlock.Text = DateTime.Now.ToString("dd/MM/yyyy\n     hh:m:ss");
        }
    }
}
