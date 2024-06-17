using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Windows;

namespace FaceDetectionAttendance.MVVM.ViewModel
{
    internal class SendEmail
    {
        public void SendEmail_FileChoose(string toAddress, string subject, string content, string[] filename)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtp = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("imhunggg02@gmail.com");
                mail.To.Add(toAddress);
                mail.Subject = subject;
                mail.Body = content;
                foreach (string file in filename)
                {
                    Attachment attachFile = new Attachment(file);
                    mail.Attachments.Add(attachFile);
                }
                smtp.Port = 587;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = new System.Net.NetworkCredential("imhunggg02@gmail.com", "stfhexhuhwbogtjc");//username and password
                smtp.EnableSsl = true;
                smtp.Send(mail);
                MessageBox.Show("Mail send successfully");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
