using System;
using System.Net;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FitnessApp.UI
{
    public partial class Email : UserControl
    {
        public Email()
        {
            InitializeComponent();
            trash.ImageSource = new BitmapImage(new Uri(Utils.trash));
            email.ImageSource = new BitmapImage(new Uri(Utils.email));
        }

        private void kuld(object sender, System.Windows.RoutedEventArgs e)
        {
            if (to.Text == "" || subject.Text == "" || body.Text == "" || mymail.Text == "" || pass.Password.Length < 1)
            {
                MessageBox.Show("Hiányos mezők!");
            }
            else
            {
                try
                {
                    SmtpClient clientDetails = new SmtpClient();
                    clientDetails.Port = 587;
                    clientDetails.Host = "smtp.gmail.com"; // smtp.mail.yahoo.com
                    clientDetails.EnableSsl = true;
                    clientDetails.DeliveryMethod = SmtpDeliveryMethod.Network;
                    clientDetails.UseDefaultCredentials = true;
                    clientDetails.Credentials = new NetworkCredential(mymail.Text.Trim(), pass.Password.Trim());

                    MailMessage mailDetails = new MailMessage();
                    mailDetails.From = new MailAddress(mymail.Text.Trim());
                    mailDetails.To.Add(to.Text.Trim());
                    mailDetails.Subject = subject.Text.Trim();
                    mailDetails.IsBodyHtml = true;
                    mailDetails.Body = body.Text.Trim();

                    clientDetails.Send(mailDetails);
                    MessageBox.Show("E-mail elküldve", "E-mail elküldve", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void torol(object sender, RoutedEventArgs e)
        {
            mymail.Text = "";
            pass.Password = "";
            to.Text = "";
            subject.Text = "";
            body.Text = "";
        }
    }
}
