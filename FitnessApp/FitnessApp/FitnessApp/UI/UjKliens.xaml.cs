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

namespace FitnessApp.UI
{
    /// <summary>
    /// Interaction logic for UjKliens.xaml
    /// </summary>
    public partial class UjKliens : System.Windows.Controls.UserControl
    {

        //az infok a form-rol
        private string name;
        private string phone;
        private string email;
        private string cnp;
        private string cim;
        private string berletType;
        private Image photo;
        private string comment;


        //egyeb valtozok
        private string date;

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

            SqlConnection sqlCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\.Net_Project\FitnessApp\FitnessApp\FitnessApp\FitnessApp\Database\db_local.mdf;Integrated Security=True");

            try
            {
                if(sqlCon.State == ConnectionState.Closed)
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

        private void BtnCancel_click(object sender, RoutedEventArgs e)
        {

        }
    }
}
