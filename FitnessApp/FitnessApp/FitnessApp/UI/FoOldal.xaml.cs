using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static FitnessApp.Utils;

namespace FitnessApp.UI
{
    public partial class FoOldal : UserControl
    {
        private bool isFirst = false;

        private int berlet = -1;
        private int hanyBelepes = -1;
        private int hanyNapig = -1;
        private int megnevezes = -1;
        private int belepesekSzama = -1;
        private int tId = -1;
        private int kId = -1;

        private string hanyOratol = "";
        private string hanyOraig = "";
        private string vonalKod = "";
        private string date_str = "";
        private string nev = "";

        private string berletLetrehozas_str;
        private DateTime berletLetrehozas;
        private DateTime today = DateTime.Now;

        public FoOldal()
        {
            InitializeComponent();

            ok.ImageSource = new BitmapImage(new Uri(Utils.ok));
        }

        private void BtnOk_click(object sender, RoutedEventArgs e)
        {
            beleptetes.Visibility = Visibility.Hidden;
            beleptetes.felkialtojel.Visibility = Visibility.Hidden;
            beleptetes.felkialtojelD.Visibility = Visibility.Hidden;
            beleptetes.lejarat.Visibility = Visibility.Hidden;
            beleptetes.hanyadikhasznalatMezo.Visibility = Visibility.Hidden;
            beleptetes.ervenyessegMezo.Visibility = Visibility.Hidden;

            string vKod = vonalkod.Text;
            string bId = berletId.Text;

            if (vKod != "" && bId != "")
            {
                SqlConnection sqlCon = new SqlConnection(conString);

                if (!isValidVkod(vKod, sqlCon))
                {
                    MessageBox.Show("Ilyen vonalkod nem talalhato!");
                    beleptetes.Visibility = Visibility.Hidden;
                    return;
                }

                if (!isVallidBerletId(bId, sqlCon))
                {
                    MessageBox.Show("Ilyen berlet nem talalhato!");
                    beleptetes.Visibility = Visibility.Hidden;
                    return;
                }

                complexQuery(sqlCon, vKod, bId);
                queryClient(sqlCon, vKod);
                countBarcode(sqlCon, vKod);
                setMegnevezes();

                berletLetrehozas = Convert.ToDateTime(berletLetrehozas_str, new CultureInfo("en-US"));
                DateTime lejarati_datum = berletLetrehozas.AddDays(hanyNapig);
                int kulonbseg = (int)Math.Round((lejarati_datum - today).TotalDays);
                int maradek_belepes = hanyBelepes - belepesekSzama;
                string[] temp1 = hanyOratol.Split(':');
                string[] temp2 = hanyOraig.Split(':');
                int kezdetOra = Int32.Parse(temp1[0]);
                int kezdetPerc = Int32.Parse(temp1[1]);
                int vegOra = Int32.Parse(temp2[0]);
                int vegPerc = Int32.Parse(temp2[1]);
                TimeSpan currentHourMinute;
                currentHourMinute = DateTime.Now.TimeOfDay;

                if (!checkIfClientHasThatAbonament(bId, vKod, sqlCon))
                {
                    MessageBox.Show("Ennek a kliensnek nincs ilyen berlete! ");
                    beleptetes.Visibility = Visibility.Hidden;
                    return;
                }

                if (currentHourMinute < new TimeSpan(kezdetOra, kezdetPerc, 0) || currentHourMinute > new TimeSpan(vegOra, vegPerc, 0))
                {
                    MessageBox.Show("A berlet most nem hasznalhato!");
                    beleptetes.Visibility = Visibility.Hidden;
                    return;
                }

                isFirstEntry(sqlCon, vKod, bId);
                updateKliensekBerletetei(vKod);

                //beleptetes infok megjelenitese
                beleptetes.Visibility = Visibility.Visible;
                //a nev beallitasa
                beleptetes.nevMezo.Content = nev;

                // abban az esetben ha a berletnek megvan szabva hogy hany napig ervenyes
                if (hanyNapig != -1 && hanyBelepes == -1)
                {
                    // ha lejart a berlet
                    if (kulonbseg < 1)
                        beleptetes.lejarat.Visibility = Visibility.Visible;

                    //ha hamarosam lejart
                    else if (kulonbseg >= 1 && kulonbseg <= 2)
                    {
                        beleptetes.felkialtojelD.Visibility = Visibility.Visible;
                        addingANewEntry(sqlCon);
                    }
                    //ha rendben van
                    else
                    {
                        addingANewEntry(sqlCon);
                        beleptetes.ervenyessegMezo.Visibility = Visibility.Visible;
                        beleptetes.ervenyessegMezo.Content = lejarati_datum + " (még " + kulonbseg + " nap)";
                    }
                }
                else if (hanyBelepes != -1 && hanyNapig == -1)
                {
                    beleptetes.ervenyessegMezo.Visibility = Visibility.Hidden;

                    if (maradek_belepes < 0)
                    {
                        beleptetes.lejarat.Visibility = Visibility.Visible;
                        return;
                    }

                    if ((maradek_belepes == 1 || maradek_belepes == 2) || maradek_belepes == 0)
                    {

                        beleptetes.felkialtojel.Visibility = Visibility.Visible;
                        addingANewEntry(sqlCon);
                    }
                    else
                        addingANewEntry(sqlCon);

                    beleptetes.hanyadikhasznalatMezo.Visibility = Visibility.Visible;
                    beleptetes.hanyadikhasznalatMezo.Content = hanyBelepes + "/" + belepesekSzama;
                }
                else
                {
                    if (kulonbseg < 1 || maradek_belepes < 0)
                    {
                        beleptetes.lejarat.Visibility = Visibility.Visible;
                    }

                    else if ((kulonbseg >= 1 && kulonbseg <= 2) || ((maradek_belepes == 1 || maradek_belepes == 2) || maradek_belepes == 0))
                    {
                        beleptetes.hanyadikhasznalatMezo.Visibility = Visibility.Visible;
                        beleptetes.ervenyessegMezo.Visibility = Visibility.Visible;
                        beleptetes.felkialtojelD.Visibility = Visibility.Visible;
                        beleptetes.hanyadikhasznalatMezo.Content = hanyBelepes + "/" + belepesekSzama;
                        addingANewEntry(sqlCon);
                    }

                    //ha rendben van
                    else
                    {
                        addingANewEntry(sqlCon);
                        beleptetes.hanyadikhasznalatMezo.Visibility = Visibility.Visible;
                        beleptetes.ervenyessegMezo.Visibility = Visibility.Visible;
                        beleptetes.ervenyessegMezo.Content = lejarati_datum + " (még " + kulonbseg + " nap)";
                        beleptetes.hanyadikhasznalatMezo.Content = hanyBelepes + "/" + belepesekSzama;
                    }
                }
            }
            else
            {
                MessageBox.Show("Kérem töltse ki a kért mezőket!");
            }
        }

        private void isFirstEntry(SqlConnection sqlCon, string vKod, string bId)
        {
            isFirst = false;
            try
            {
                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }

                string query = "Select eddigi_belepes_szam from KliensBerletei b  " +
                    "join Kliensek k on k.kliens_id = b.kliens_id " +
                    "where k.vonalkod = @vKod and b.berlet_id= @bId;";

                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                sqlCmd.Parameters.AddWithValue("@vKod", vKod);
                sqlCmd.Parameters.AddWithValue("@bId", bId);

                using (SqlDataReader reader = sqlCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (Int32.Parse(reader["eddigi_belepes_szam"].ToString()) == 0)
                        {
                            isFirst = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Hiba: Select kliensBerleti: " + ex.Message);
            }
            finally
            {
                sqlCon.Close();
            }
        }

        private void updateKliensekBerletetei(string vKod)
        {
            SqlConnection sqlCon = new SqlConnection(conString);

            string query = "";
            if (isFirst)
            {
                query = "UPDATE KliensBerletei set eddigi_belepes_szam = 1, elso_hasznalat_datum = @today WHERE vonalkod = @vKod;";
            }
            else
            {
                query = "UPDATE KliensBerletei set eddigi_belepes_szam = 1  WHERE vonalkod = @vKod;";
            }

            try
            {
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                sqlCmd.Parameters.AddWithValue("@vKod", vKod);
                if (isFirst)
                {
                    sqlCmd.Parameters.AddWithValue("@today", today);
                }
                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }

                int result = sqlCmd.ExecuteNonQuery();

                if (result < 0)
                    System.Windows.MessageBox.Show("Adatbázis hiba a kliensberletei frissitesenel");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Hiba update kliensek berletei: " + ex.Message);
            }
            finally
            {
                sqlCon.Close();
            }
        }

        private bool checkIfClientHasThatAbonament(string bId, string vKod, SqlConnection sqlCon)
        {
            bool hasAbonament = false;
            try
            {
                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }

                string query = "Select * from Kliensek k " +
                    "join KliensBerletei b on k.kliens_id = b.kliens_id " +
                    "where k.vonalkod = @vKod and b.berlet_id= @bId;";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                sqlCmd.Parameters.AddWithValue("@vKod", vKod);
                sqlCmd.Parameters.AddWithValue("@bId", bId);

                using (SqlDataReader reader = sqlCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        hasAbonament = true;
                    }
                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Hiba checkAbonament: " + ex.Message);
            }
            finally
            {
                sqlCon.Close();
            }

            return hasAbonament;
        }

        private void setMegnevezes()
        {
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
        }

        private void complexQuery(SqlConnection sqlCon, string vKod, string bId)
        {
            try
            {
                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }

                string query = "Select kliens_id,vonalkod,nev from Kliensek where vonalkod  =  @vKod;";
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
                System.Windows.MessageBox.Show("1) Hiba complex query kliensek: " + ex.Message);
            }
            finally
            {
                sqlCon.Close();
            }

            //masodik lekerdezes a Berletek ertekere ertekre
            try
            {
                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }

                string query = "Select megnevezes, ervenyesseg_nap, ervenyesseg_belepesek_szama, hany_oratol, hany_oraig, " +
                    "terem_id, letrehozasi_datum, berlet_id " +
                    "from Berletek where berlet_id  =  @bId;";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                sqlCmd.Parameters.AddWithValue("@bId", bId);

                using (SqlDataReader reader = sqlCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        megnevezes = Int32.Parse(reader["megnevezes"].ToString());
                        hanyNapig = Int32.Parse(reader["ervenyesseg_nap"].ToString());
                        hanyBelepes = Int32.Parse(reader["ervenyesseg_belepesek_szama"].ToString());
                        hanyOratol = reader["hany_oratol"].ToString();
                        hanyOraig = reader["hany_oraig"].ToString();
                        tId = Int32.Parse(reader["terem_id"].ToString());
                        berletLetrehozas_str = reader["letrehozasi_datum"].ToString();
                        berlet = Int32.Parse(reader["berlet_id"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("1) Hiba complex query berletek: " + ex.Message);
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
                System.Windows.MessageBox.Show("3) Hiba a belepesek osszegzesenel: " + ex.Message);
            }
            finally
            {
                sqlCon.Close();
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
                System.Windows.MessageBox.Show("Hiba berlet ellenorzes: " + ex.Message);
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

                string query = "select kliens_id from Kliensek where vonalkod = @vkod and is_deleted=0;";
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
    }
}
