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
            int deleted = 0;
            insertClientIntoDataBase(name,phone,email,deleted,photo,date,cnp,my_address,barcode,comment);         
        }

        private void insertClientIntoDataBase(string name, string phone, string email, int deleted, string photo, DateTime date, string cnp, string my_address, string barcode, string comment)
        {
            SqlConnection sqlCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=E:\egyetem\felev2\.net\gyakok\projekt\FitnessApp\FitnessApp\FitnessApp\FitnessApp\Database\db_local.mdf;Integrated Security=True");
           
            string query = "INSERT INTO Kliensek (nev, telefon, email, " +
            "is_deleted, photo, inserted_date, szemelyi, cim, vonalkod, megjegyzes)" +
            " VALUES ( @name, @phone, @email, @deleted, @photo, @date, @cnp, @my_address, @barcode, @comment );";
            try
            {


                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                sqlCmd.Parameters.AddWithValue("@name", name);
                sqlCmd.Parameters.AddWithValue("@phone", phone);
                sqlCmd.Parameters.AddWithValue("@email", email);
                sqlCmd.Parameters.AddWithValue("@deleted", deleted);
                sqlCmd.Parameters.AddWithValue("@photo", photo);
                sqlCmd.Parameters.AddWithValue("@date", date);
                sqlCmd.Parameters.AddWithValue("@cnp", cnp);
                sqlCmd.Parameters.AddWithValue("@my_address", my_address);
                sqlCmd.Parameters.AddWithValue("@barcode", barcode);
                sqlCmd.Parameters.AddWithValue("@comment", comment);


                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }

                int result = sqlCmd.ExecuteNonQuery();


                if (result < 0)
                    System.Windows.MessageBox.Show("Adatbázis hiba új kliens hozzáadásnál");
                else
                    System.Windows.MessageBox.Show("Kliens sikeresen hozzáadva");
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

            //this.tab = Visibility.Hidden;

        }
    }
}
