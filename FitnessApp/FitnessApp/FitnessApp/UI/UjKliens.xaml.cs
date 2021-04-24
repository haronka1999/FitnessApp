using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FitnessApp.Model;

namespace FitnessApp.UI
{
    /// <summary>
    /// Interaction logic for UjKliens.xaml
    /// </summary>
    public partial class UjKliens : System.Windows.Controls.UserControl
    {
        public const int BARCODE_LENGTH = 4;

        //az infok a form-rol
        private string name;
        private string phone;
        private string email;
        private string cnp;
        private string my_address;
        private string barcode;
        private string berletType;
        private string photo = "placeholder";
        private string comment;

        //egyeb valtozok
        private string date_str;

        public UjKliens()
        {
            InitializeComponent();
        }

        private void BtnUpload_click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Válasszon profilképet";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == DialogResult.OK)
            {
                //idejon a tobbi kod 
                imgPhoto.Source = new BitmapImage(new Uri(op.FileName));
            }
        }

        private void BtnOk_click(object sender, RoutedEventArgs e)
        {
            name = UserName.Text;
            phone = Number.Text;
            email = Email.Text;
            cnp = CNP.Text;
            my_address = address.Text;
            barcode = generateRandomString(BARCODE_LENGTH);
            comment = Comment.Text;
            date_str = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            DateTime date = Convert.ToDateTime(date_str);

            Kliens new_kliens = new Kliens(0,name,phone,email,false,photo,date,cnp,my_address,barcode,comment);
          
            System.Windows.MessageBox.Show(new_kliens.ToString());
     
            SqlConnection sqlCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\.Net_Project\FitnessApp\FitnessApp\FitnessApp\FitnessApp\Database\db_local.mdf;Integrated Security=True");

            try
            {
                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlCon.Close();
            }


        }

        private string generateRandomString(int length)
        {
            Random random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void BtnCancel_click(object sender, RoutedEventArgs e)
        {


        }
    }
}
