using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using static FitnessApp.Utils;

namespace FitnessApp.UI
{
    public partial class FoOldal : UserControl
    {
        private int berlet = -1;
        private int hanyBelepes = -1;
        private int hanyNapig = -1;
        private int megnevezes = -1;
        private int tId = -1;
        private int kId = -1;
        private string vonalKod = "";
        private string date_str = "";
        private string nev = "";
        private int belepesekSzama = -1;
        private DateTime berletLetrehozas;

        public FoOldal()
        {
            InitializeComponent();
        }

        private void BtnOk_click(object sender, RoutedEventArgs e)
        {
            string vKod = vonalkod.Text;
            string bId = berletId.Text;

            if (vKod != "" && bId != "")
            {
                SqlConnection sqlCon = new SqlConnection(conString);


                if (!isValidVkod(vKod, sqlCon))
                {
                    MessageBox.Show("Ilyen vonalkod nem talalhato!");
                    return;
                }

                if (!isVallidBerletId(bId, sqlCon))
                {
                    MessageBox.Show("Ilyen berlet nem talalhato!");
                    return;
                }


                complexQuery(sqlCon);
                queryClient(sqlCon, vKod);
                addingANewEntry(sqlCon);
                countBarcode(sqlCon, vKod);



                beleptetes.Visibility = Visibility.Visible;
                switch (megnevezes)
                {
                    case 1:
                        {
                            beleptetes.berletMezo.Content = "Napszam";
                            break;
                        }
                    case 2:
                        {
                            beleptetes.berletMezo.Content = "Belepes szam";
                            break;
                        }
                    case 3:
                        {
                            beleptetes.berletMezo.Content = "Kombinalt";
                            break;
                        }
                    default:
                        {
                            MessageBox.Show("Megnevezes hiba!");
                            break;
                        }
                }


                // abban az esetben ha a berletnek megvan szabva hogy hany napig ervenyes
                if (hanyNapig != -1)
                {
                    DateTime today = DateTime.Now;
                    DateTime lejarati_datum = berletLetrehozas.AddDays(hanyNapig);
                    int kulonbseg = DateTime.Compare(lejarati_datum, today);


                    // ha lejart
                    if (kulonbseg < 0)
                        beleptetes.lejarat.Visibility = Visibility.Visible;

                    //ha meg 1 vagy 2 nap van meg hatra
                    if (kulonbseg >= 1 && kulonbseg <= 2)
                        beleptetes.felkialtojelD.Visibility = Visibility.Visible;

                    //ha negativ ertek akkor az azt jelenti hogy annyi napja van lejarva a berlet
                    beleptetes.ervenyessegMezo.Content = kulonbseg;

                }
                if (hanyBelepes != -1)
                {
                    int b = hanyBelepes - belepesekSzama;
                    if (b == 1 || b == 2)
                        beleptetes.felkialtojel.Visibility = Visibility.Visible;

                    if (hanyBelepes != -1)
                        beleptetes.hanyadikhasznalatMezo.Content = hanyBelepes + "/" + belepesekSzama;
                    else
                        beleptetes.hanyadikhasznalatMezoNev.Visibility = Visibility.Hidden;

                    beleptetes.nevMezo.Content = nev;

                    vonalkod.Text = "";
                    berletId.Text = "";
                }
            }
            else
            {
                MessageBox.Show("Kérem töltse ki a kért mezőket!");
            }

        }

        private bool isVallidBerletId(string bId, SqlConnection sqlCon)
        {
            int my_berlet_id = -1;
            try
            {


                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }

                string query = "select berlet_id from Berletek where berlet_id = @berletId;";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                sqlCmd.Parameters.AddWithValue("@berletId", bId);
                using (SqlDataReader reader = sqlCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        my_berlet_id = Int32.Parse(reader["berlet_id"].ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Hiba berlet ellenorzes: "  + ex.Message);
            }
            finally
            {
                sqlCon.Close();
            }


            if (my_berlet_id == -1)
                return false;
            else
                return true;

        }

        private bool isValidVkod(string vKod, SqlConnection sqlCon)
        {

            int my_kliens_id = -1;
            try
            {

                
                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }

                string query = "select kliens_id from Kliensek where vonalkod = @vkod;" ;
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                sqlCmd.Parameters.AddWithValue("@vkod", vKod);
                using (SqlDataReader reader = sqlCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        my_kliens_id = Int32.Parse(reader["kliens_id"].ToString());                     
                    }
                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Hiba kliens ellenorzesnel: " + ex.Message);
            }
            finally
            {
                sqlCon.Close();
            }


            if (my_kliens_id == -1)
                return false;
            else 
                return true;

        }

        private void complexQuery(SqlConnection sqlCon)
        {
            try
            {

                MessageBox.Show("Itt vagy? ");
                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }

                string query = "select be.megnevezes, be.ervenyesseg_nap, be.ervenyesseg_belepesek_szama, be.terem_id, be.letrehozasi_datum, " +
                    "be.berlet_id from Kliensek k " +
                    "join Belepesek b on b.kliens_id = k.kliens_id " +
                    "join Berletek be on be.berlet_id = b.berlet_id " +
                    "where k.vonalkod = b.barcode;";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);

                using (SqlDataReader reader = sqlCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MessageBox.Show("Itt vagy2? ");
                        megnevezes = Int32.Parse(reader["megnevezes"].ToString());
                        MessageBox.Show("Megnevezes: " + megnevezes);
                        hanyNapig = Int32.Parse(reader["ervenyesseg_nap"].ToString());
                        hanyBelepes = Int32.Parse(reader["ervenyesseg_belepesek_szama"].ToString());
                        berlet = Int32.Parse(reader["berlet_id"].ToString());
                        tId = Int32.Parse(reader["terem_id"].ToString());
                        MessageBox.Show(reader["letrehozasi_datum"].ToString());
                        berletLetrehozas = DateTime.ParseExact(reader["letrehozasi_datum"].ToString(), "MM/dd/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
                    }
                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("1) Hiba read " + ex.Message);
            }
            finally
            {
                sqlCon.Close();
            }
        }
        private void queryClient(SqlConnection sqlCon, string vKod)
        {
            try
            {
                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }

                string query = "select kliens_id, vonalkod, nev from Kliensek where vonalkod = @vKod;";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                sqlCmd.Parameters.AddWithValue("@vKod", vKod);

                using (SqlDataReader reader = sqlCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        kId = Int32.Parse(reader["kliens_id"].ToString());
                        vonalKod = reader["vonalkod"].ToString();
                        nev = reader["nev"].ToString();
                    }
                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("2) Hiba read kliensek " + ex.Message);
            }
            finally
            {
                sqlCon.Close();
            }
        }

        private void addingANewEntry(SqlConnection sqlCon)
        {
            try
            {
                date_str = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                DateTime date = Convert.ToDateTime(date_str);

                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }

                string query = "INSERT INTO Belepesek(kliens_id, berlet_id, datum, insertedby_uid, barcode, terem_id) " +
                "VALUES(@kId, @berlet, @date, -1, @vonalKod, @tId); ";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                sqlCmd.Parameters.AddWithValue("@kId", kId);
                sqlCmd.Parameters.AddWithValue("@berlet", berlet);
                sqlCmd.Parameters.AddWithValue("@date", date);
                sqlCmd.Parameters.AddWithValue("@vonalKod", vonalKod);
                sqlCmd.Parameters.AddWithValue("@tId", tId);

                int result = sqlCmd.ExecuteNonQuery();

                if (result < 0)
                    System.Windows.MessageBox.Show("Adatbázis hiba új belepes hozzáadásnál");
                // else System.Windows.MessageBox.Show("Belepes sikeresen hozzáadva");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Hiba insert belepesek: " + ex.Message);
            }
            finally
            {
                sqlCon.Close();
            }
        }

        private void countBarcode(SqlConnection sqlCon, string vKod)
        {
            try
            {
                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }

                string query = "SELECT COUNT(barcode) FROM Belepesek WHERE barcode = @barCode;";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                sqlCmd.Parameters.AddWithValue("@barCode", vKod);

                belepesekSzama = (int)sqlCmd.ExecuteScalar();

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("3) Hiba read kliensek " + ex.Message);
            }
            finally
            {
                sqlCon.Close();
            }
        }

    }
}
