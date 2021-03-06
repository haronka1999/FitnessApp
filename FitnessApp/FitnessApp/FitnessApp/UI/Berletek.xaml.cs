using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
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

            save.ImageSource = new BitmapImage(new Uri(Utils.save));
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

                DateTime today = DateTime.Now;

                using (SqlDataReader reader = sqlCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTime letrehozasi_datum = Convert.ToDateTime(reader["letrehozasi_datum"].ToString(), new CultureInfo("en-US"));
                        DateTime lejarati_datum = letrehozasi_datum.AddDays(Int32.Parse(reader["ervenyesseg_nap"].ToString()));
                        int kulonbseg = (int)Math.Round((lejarati_datum - today).TotalDays);
                        string ervenyesseg;
                        int napiervenyesseg = Int32.Parse(reader["ervenyesseg_nap"].ToString());
                        if (napiervenyesseg != -1)
                        {
                            if (kulonbseg > 0)
                            {
                                ervenyesseg = "aktív " + kulonbseg + " napig";
                            }
                            else
                            {
                                ervenyesseg = "lejárt " + -kulonbseg + " napja";
                            }
                        }
                        else
                        {
                            ervenyesseg = "aktív";
                        }

                        Berlet berlet = new Berlet(Int32.Parse(reader["berlet_id"].ToString()),
                                                       Int32.Parse(reader["megnevezes"].ToString()),
                                                       float.Parse(reader["ar"].ToString()),
                                                       napiervenyesseg,
                                                       Int32.Parse(reader["ervenyesseg_belepesek_szama"].ToString()),
                                                       bool.Parse(reader["torolve"].ToString()),
                                                       Int32.Parse(reader["terem_id"].ToString()),
                                                       reader["hany_oratol"].ToString(),
                                                       reader["hany_oraig"].ToString(),
                                                       Int32.Parse(reader["napi_max_hasznalat"].ToString()),
                                                       letrehozasi_datum,
                                                       ervenyesseg);

                        instertErvenyesseg(ervenyesseg, Int32.Parse(reader["berlet_id"].ToString()));

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

        private void Save_Edited_Berlet(object sender, RoutedEventArgs e)
        {
            Berlet berlet = (Berlet)BerletGrid.SelectedItem;

            SqlConnection sqlCon = new SqlConnection(conString);
            string query = "UPDATE Berletek set megnevezes=@megnevezes, ar=@ar, ervenyesseg_nap=@ervenyesseg_nap, " +
                "ervenyesseg_belepesek_szama=@ervenyesseg_belepesek_szama, terem_id=@terem_id, hany_oratol=@hany_oratol, " +
                "hany_oraig=@hany_oraig, napi_max_hasznalat=@napi_max_hasznalat WHERE berlet_id = @berlet_id;";
            try
            {
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                sqlCmd.Parameters.AddWithValue("@berlet_id", berlet.berlet_id);
                sqlCmd.Parameters.AddWithValue("@megnevezes", berlet.megnevezes);
                sqlCmd.Parameters.AddWithValue("@ar", berlet.ar);
                sqlCmd.Parameters.AddWithValue("@ervenyesseg_nap", berlet.ervenyesseg_nap);
                sqlCmd.Parameters.AddWithValue("@ervenyesseg_belepesek_szama", berlet.ervenyesseg_belepesek_szama);
                sqlCmd.Parameters.AddWithValue("@terem_id", berlet.terem_id);
                sqlCmd.Parameters.AddWithValue("@hany_oratol", berlet.hany_oratol);
                sqlCmd.Parameters.AddWithValue("@hany_oraig", berlet.hany_oraig);
                sqlCmd.Parameters.AddWithValue("@napi_max_hasznalat", berlet.napi_max_hasznalat);

                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }

                int result = sqlCmd.ExecuteNonQuery();

                if (result < 0)
                    System.Windows.MessageBox.Show("Adatbázis hiba a bérletek szerkesztésnél");
                //else System.Windows.MessageBox.Show("Bérlet sikeresen szerkesztve");

                Refresh();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Hiba bérlet szerkesztésnél: " + ex.Message);
            }
            finally
            {
                sqlCon.Close();
            }

            saveEditButton.Visibility = Visibility.Hidden;
            BerletGrid.IsReadOnly = true;
        }

        private void instertErvenyesseg(string ervenyesseg, int berlet_id)
        {
            SqlConnection sqlCon = new SqlConnection(conString);
            string query = "UPDATE Berletek set ervenyesseg=@ervenyesseg WHERE berlet_id = @berlet_id;";

            if (sqlCon.State == ConnectionState.Closed)
            {
                sqlCon.Open();
            }

            try
            {
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                sqlCmd.Parameters.AddWithValue("@ervenyesseg", ervenyesseg);
                sqlCmd.Parameters.AddWithValue("@berlet_id", berlet_id);

                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }

                int result = sqlCmd.ExecuteNonQuery();

                if (result < 0)
                    System.Windows.MessageBox.Show("Adatbázis hiba új berlet érvényessége hozzáadásnál");
                //else System.Windows.MessageBox.Show("Bérlet érvényessége sikeresen hozzáadva");
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

        private void Refresh()
        {
            berletek = getAbonamentsFromDatabase();
            this.BerletGrid.ItemsSource = berletek;
        }

        private void Delete_Berlet(object sender, RoutedEventArgs e)
        {
            Berlet drv = (Berlet)BerletGrid.SelectedItem;
            String berlet_id = (drv.berlet_id).ToString();

            SqlConnection sqlCon = new SqlConnection(conString);
            string query = "UPDATE Berletek set torolve=1 WHERE berlet_id = @berlet_id;";
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

        private void Edit_Berlet(object sender, RoutedEventArgs e)
        {
            saveEditButton.Visibility = Visibility.Visible;
            BerletGrid.IsReadOnly = false;
        }
    }
}
