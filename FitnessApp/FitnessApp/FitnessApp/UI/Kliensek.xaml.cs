using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
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
using FitnessApp.Model;

namespace FitnessApp.UI
{
    public partial class Kliensek : UserControl
    {
        ObservableCollection<Kliens> users { get; set; } = new ObservableCollection<Kliens>();


        public Kliensek()
        {
            InitializeComponent();
            getUsersFromDatabase();
            DataContext = this;
            KliensGrid.ItemsSource = users;
        }

        private void getUsersFromDatabase()
        {

            ObservableCollection<Kliens> kliensek = new ObservableCollection<Kliens>();
            SqlConnection sqlCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\.Net_Project\FitnessApp\FitnessApp\FitnessApp\FitnessApp\Database\db_local.mdf;Integrated Security=True");
            try
            {
                string query = "SELECT * from Kliensek;";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);

                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }

                using (SqlDataReader reader = sqlCmd.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        Kliens kliens = new Kliens(Int32.Parse(reader["kliens_id"].ToString()),
                                                    reader["nev"].ToString(),
                                                    reader["telefon"].ToString(),
                                                    reader["email"].ToString(),
                                                    bool.Parse(reader["is_deleted"].ToString()),
                                                    reader["photo"].ToString(),
                                                    Convert.ToDateTime(reader["inserted_date"].ToString()),
                                                    reader["szemelyi"].ToString(),
                                                    reader["cim"].ToString(),
                                                    reader["vonalkod"].ToString(),
                                                    reader["megjegyzes"].ToString());
                        kliensek.Add(kliens);
                    }                
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

            users = kliensek;
          
        }
    }
}
