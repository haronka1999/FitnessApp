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
using static FitnessApp.Utils;

namespace FitnessApp.UI
{
    public partial class Berletek : UserControl
    {

        public ObservableCollection<Berlet> berletek { get; set; }

        public Berletek()
        {
            InitializeComponent();
            DataContext = this;
            berletek = new ObservableCollection<Berlet>();
            berletek = getAbonamentsFromDatabase();
            this.BerletGrid.ItemsSource = berletek;
        }

        private ObservableCollection<Berlet> getAbonamentsFromDatabase()
        {
            ObservableCollection<Berlet> abonaments = new ObservableCollection<Berlet>();

            SqlConnection sqlCon = new SqlConnection(conString);
            try
            {
                string query = "SELECT * from Berletek;";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);

                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }

                using (SqlDataReader reader = sqlCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Berlet berlet = new Berlet(Int32.Parse(reader["berlet_id"].ToString()),
                                                       Int32.Parse(reader["megnevezes"].ToString()),
                                                       float.Parse(reader["ar"].ToString()),
                                                       Int32.Parse(reader["ervenyesseg_nap"].ToString()),
                                                       Int32.Parse(reader["ervenyesseg_belepesek_szama"].ToString()),
                                                       bool.Parse(reader["torolve"].ToString()),
                                                       Int32.Parse(reader["terem_id"].ToString()),
                                                       reader["hany_oratol"].ToString(),
                                                       reader["hany_oraig"].ToString(),
                                                       Int32.Parse(reader["napi_max_hasznalat"].ToString()),
                                                       Convert.ToDateTime(reader["letrehozasi_datum"].ToString()));

                        //csak akkor jelenitsuk meg ha nincs torolve
                        if (berlet.torolve == false)
                            abonaments.Add(berlet);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Hiba read kliensek " + ex.Message);
            }
            finally
            {
                sqlCon.Close();
            }


            berletek = abonaments;
            return berletek;    
        }

        private void Delete_Abonament(object sender, RoutedEventArgs e)
        {
            Berlet drv = (Berlet)BerletGrid.SelectedItem;
            String berlet_id = (drv.berlet_id).ToString();

            SqlConnection sqlCon = new SqlConnection(conString);
            string query = @"UPDATE Berletek set torolve=1 WHERE berlet_id = @berlet_id;";
            try
            {

                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                sqlCmd.Parameters.AddWithValue("@berlet_id", berlet_id);
                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }

                int result = sqlCmd.ExecuteNonQuery();

                if (result < 0)
                    System.Windows.MessageBox.Show("Adatbázis hiba a berlet torlesenel");
                else
                    System.Windows.MessageBox.Show("Berlet sikeresen torolve");

                Refresh();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Hiba berlet torles: " + ex.Message);
            }
            finally
            {
                sqlCon.Close();
            }
        }

        private void Save_Edited_Abonament(object sender, RoutedEventArgs e)
        {

        }

        private void Edit_Abonament(object sender, RoutedEventArgs e)
        {

        }
        private void Refresh()
        {
            berletek = getAbonamentsFromDatabase();
            this.BerletGrid.ItemsSource = berletek;
        }
    }
}
