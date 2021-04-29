using FitnessApp.Model;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static FitnessApp.Utils;

namespace FitnessApp.UI
{
    /// <summary>
    /// Interaction logic for FoOldal.xaml
    /// </summary>
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
                try
                {
                    if (sqlCon.State == ConnectionState.Closed)
                    {
                        sqlCon.Open();
                    }

                    string query1 = "select be.megnevezes, be.ervenyesseg_nap, be.ervenyesseg_belepesek_szama, be.terem_id, " +
                        "be.berlet_id from Kliensek k " +
                        "join Belepesek b on b.kliens_id = k.kliens_id " +
                        "join Berletek be on be.berlet_id = b.berlet_id " +
                        "where k.vonalkod = b.barcode;";
                    SqlCommand sqlCmd1 = new SqlCommand(query1, sqlCon);

                    using (SqlDataReader reader = sqlCmd1.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            megnevezes = Int32.Parse(reader["megnevezes"].ToString());
                            hanyNapig = Int32.Parse(reader["ervenyesseg_nap"].ToString());
                            hanyBelepes = Int32.Parse(reader["ervenyesseg_belepesek_szama"].ToString());
                            berlet = Int32.Parse(reader["berlet_id"].ToString());
                            tId = Int32.Parse(reader["terem_id"].ToString());
                        }
                    }

                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("1) Hiba read kliensek " + ex.Message);
                }
                finally
                {
                    sqlCon.Close();
                }

                try
                {
                    if (sqlCon.State == ConnectionState.Closed)
                    {
                        sqlCon.Open();
                    }

                    string query2 = "select kliens_id, vonalkod, nev from Kliensek where vonalkod = @vKod;";
                    SqlCommand sqlCmd2 = new SqlCommand(query2, sqlCon);
                    sqlCmd2.Parameters.AddWithValue("@vKod", vKod);

                    using (SqlDataReader reader = sqlCmd2.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            kId = Int32.Parse(reader["kliens_id"].ToString());
                            vonalKod = reader["vonalkod"].ToString();
                            nev = reader["nev"].ToString();
                        }
                    }

                    beleptetes.Visibility = Visibility.Visible;

                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("2) Hiba read kliensek " + ex.Message);
                }
                finally
                {
                    sqlCon.Close();
                }

                // Uj belepes hozzadasas
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
                beleptetes.ervenyessegMezo.Content = hanyNapig;

                beleptetes.hanyadikhasznalatMezo.Content = hanyBelepes;

                beleptetes.nevMezo.Content = nev;

            }
            else
            {
                MessageBox.Show("Kérem töltse ki a kért mezőket!");
            }

        }
       
    }
}
