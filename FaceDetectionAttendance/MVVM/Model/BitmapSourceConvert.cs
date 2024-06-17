using Emgu.CV.Structure;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace FaceDetectionAttendance.MVVM.Model
{
    public static class BitmapSourceConvert
    {
        public static BitmapSource ToBitmapSource(this Image<Bgr, byte> image)
        {
            return BitmapSourceConvert.ToBitmapSource(image.Mat);
        }

        public static BitmapSource ToBitmapSource(this Mat mat)
        {
            return Application.Current.Dispatcher.Invoke(() =>
            {
                return BitmapSourceConvert.ConvertMatToBitmapSource(mat);
            });
        }

        private static BitmapSource ConvertMatToBitmapSource(Mat mat)
        {
            try
            {
                IntPtr ptr = mat.DataPointer;
                int stride = mat.Step;
                int width = mat.Width;
                int height = mat.Height;

                return BitmapSourceConvert.CreateBitmapSourceFromMemory(ptr, width, height, stride, PixelFormats.Bgr24, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        private static BitmapSource CreateBitmapSourceFromMemory(IntPtr data, int width, int height, int stride, PixelFormat format, BitmapPalette palette)
        {
            try
            {
                return BitmapSource.Create(width, height, 96, 96, format, palette, data, stride * height, stride);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
