using System;
using System.Net;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace FitnessApp.UI
{
    public partial class Email : System.Windows.Controls.UserControl
    {
        private OpenFileDialog op;
        private string fileName = "";
        public Email()
        {
            InitializeComponent();
            trash.ImageSource = new BitmapImage(new Uri(Utils.trash));
            email.ImageSource = new BitmapImage(new Uri(Utils.email));
            attachment.ImageSource = new BitmapImage(new Uri(Utils.attachment));
        }

        private void kuld(object sender, System.Windows.RoutedEventArgs e)
        {
            if (to.Text == "" || subject.Text == "" || body.Text == "" || mymail.Text == "" || pass.Password.Length < 1)
            {
                System.Windows.MessageBox.Show("Hiányos mezők!");
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

                    if (fileName.Length > 0)
                    {
                        Attachment attachment = new Attachment(fileName);
                        mailDetails.Attachments.Add(attachment);
                    }

                    clientDetails.Send(mailDetails);
                    System.Windows.MessageBox.Show("E-mail elküldve", "E-mail elküldve", MessageBoxButton.OK, MessageBoxImage.Information);
                    fileName = "";
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
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

        private void attachmentFile(object sender, RoutedEventArgs e)
        {
            try
            {
                op = new OpenFileDialog();
                op.Title = "Válasszon profilképet";
                op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                  "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                  "Portable Network Graphic (*.png)|*.png|Pdf Files|*.pdf|" +
                  "Zip Files|*.zip;*.rar";
                if (op.ShowDialog() == DialogResult.OK)
                {
                    fileName = op.FileName;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
    }
}
