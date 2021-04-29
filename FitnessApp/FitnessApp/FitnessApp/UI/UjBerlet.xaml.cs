using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class UjBerlet : UserControl
    {
        /*
         A berlet megnevezeseket egy szamkent taroljuk el
         mindegyik szam egy fajta berletet jelol. 
        Berlet a kovetkezo lehet:

        1 - "Napszam" --> ha a nap szam van korlatozva
        2 - "Belepes szam" --> ha a nap szam van korlatozva
        3 - "kombinalt" --> mindketto teljesul
         */
        public int megnevezes = -1;

        public float ar;
        public string ar_str;

        /* 
         Kezdetben az ervenyessegeket -1 ertekkel lassuk el.
        Abban az esetben ha valtozik az ertek akkor tudni fogjuk
        hogy melyik tipusu berlet lepett ervenybe
         
         */
        public int napokErvenyesseg = -1;
        public int belepesekErvenyesseg = -1;
        public bool torolve = false;

        public string hanyOratol_str;
        public string hanyOraig_str;
        public DateTime hanyOraig;
        public DateTime hanyOratol;
        public DateTime letrehozasi_datum;
        public int napiMaxHasznalat;

        /*    
         3 terem van az adatbazisban:
        1-es,2-es,3-as
         */
        public int teremId;

        Regex regex = new Regex("[^0-9]+");


        public UjBerlet()
        {
            InitializeComponent();
        }



        private void BtnSave_click(object sender, RoutedEventArgs e)
        {
            letrehozasi_datum = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            hanyOraig_str = veg.Text;
            hanyOratol_str = kezdet.Text;
            teremId = getTeremId();
            setBelepesek();
            setNapokervenyesseg();
            setAr();
            setMegnevezes();


            //egy kicsi szukites a formnal
            if (napokErvenyesseg < 5 || napokErvenyesseg > 31)
            {
                //error message should be displayed
            }

            if (belepesekErvenyesseg < 5 || belepesekErvenyesseg > 25)
            {
                //error message should be displayed
            }

            if (napiMaxHasznalat < 1)
            {
                //error message should be displayed
            }


             //kesobbre kell ha vissza akarom alakitani datumra
             //hanyOraig = DateTime.ParseExact(hanyOraig_str, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
             //hanyOratol = DateTime.ParseExact(hanyOraig_str, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);


            //deleteRowsIfNeeded();
            insertBerletIntoDataBase(megnevezes, ar, napokErvenyesseg, belepesekErvenyesseg, torolve, teremId, hanyOratol_str, hanyOraig_str, napiMaxHasznalat, letrehozasi_datum);
        }

        private void insertBerletIntoDataBase(int megnevezes, float ar, int napokErvenyesseg, int belepesekErvenyesseg, bool torolve, int teremId, string hanyOratol, string hanyOraig, int napiMaxHasznalat, DateTime letrehozasi_datum)
        {

            SqlConnection sqlCon = new SqlConnection(conString);

            string query = "INSERT INTO Berletek (megnevezes, ar, " +
            "ervenyesseg_nap, ervenyesseg_belepesek_szama, torolve, terem_id, " +
            "hany_oratol, hany_oraig, napi_max_hasznalat,letrehozasi_datum)" +
            " VALUES (@megnevezes, @ar, @ervenyesseg_nap, @ervenyesseg_belepesek_szama, " +
            "@torolve, @terem_id, @hany_oratol, @hany_oraig, @napi_max_hasznalat, @letrehozasi_datum );";

            try
            {
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                sqlCmd.Parameters.AddWithValue("@megnevezes", megnevezes);
                sqlCmd.Parameters.AddWithValue("@ar", ar);
                sqlCmd.Parameters.AddWithValue("@ervenyesseg_nap", napokErvenyesseg);
                sqlCmd.Parameters.AddWithValue("@ervenyesseg_belepesek_szama", belepesekErvenyesseg);
                sqlCmd.Parameters.AddWithValue("@torolve", torolve);
                sqlCmd.Parameters.AddWithValue("@terem_id", teremId);
                sqlCmd.Parameters.AddWithValue("@hany_oratol", hanyOratol);
                sqlCmd.Parameters.AddWithValue("@hany_oraig", hanyOraig);
                sqlCmd.Parameters.AddWithValue("@napi_max_hasznalat", napiMaxHasznalat);
                sqlCmd.Parameters.AddWithValue("@letrehozasi_datum", letrehozasi_datum);

                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }

                int result = sqlCmd.ExecuteNonQuery();


                if (result < 0)
                    System.Windows.MessageBox.Show("Adatbázis hiba új berlet hozzáadásnál");
                else
                    System.Windows.MessageBox.Show("Bérlet sikeresen hozzáadva");
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


        private void setMegnevezes()
        {
            if (napokErvenyesseg == -1)
                megnevezes = 2;
            else if (belepesekErvenyesseg == -1)
                megnevezes = 1;
            else
                megnevezes = 3;


            //hiba kezeles
            if (megnevezes == -1)
                MessageBox.Show("Hiba a berlet tipus kivalasztasanal");

        }

        private void setAr()
        {
            try
            {
                ar = float.Parse(price.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba az ar alakitasnal: " + ex.Message);
            }
        }

        private void setNapokervenyesseg()
        {

            if (NapErvenyesseg.Text != "")
            {
                try
                {
                    napokErvenyesseg = Int32.Parse(NapErvenyesseg.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hiba a napok atalakitasanal: " + ex.Message);
                }
            }
        }

        private void setBelepesek()
        {
            if (BelepesErvenyesseg.Text != "")
            {
                try
                {
                    belepesekErvenyesseg = Int32.Parse(BelepesErvenyesseg.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hiba a belepesek atalakitasanal: " + ex.Message);
                }
            }
        }

        private int getTeremId()
        {
            if ((bool)terem1.IsChecked)
               return  teremId = 1;

            if ((bool)terem2.IsChecked)
               return  teremId = 2;

            if ((bool)terem3.IsChecked)
              return  teremId = 3;

            return -1;
        }

        private void deleteRowsIfNeeded()
        {
            SqlConnection sqlCon = new SqlConnection(conString);
            string query = "DELETE FROM Berletek";

            try
            {
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);

                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }

                int result = sqlCmd.ExecuteNonQuery();


                if (result < 0)
                    System.Windows.MessageBox.Show("hiba a sorok torlesenel");
                else
                    System.Windows.MessageBox.Show("Sorok sikeresen torolv");
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

        //VALIDATION FUNCTION----------------------------

        private void NumberValidationTextBox_3(object sender, TextCompositionEventArgs e)
        {
            e.Handled = regex.IsMatch(e.Text);
            napiMaxHasznalat = Int32.Parse(e.Text);
        }

        private void NumberValidationTextBox_6(object sender, TextCompositionEventArgs e)
        {
            e.Handled = regex.IsMatch(e.Text);
            teremId = Int32.Parse(e.Text);
        }
    }
}
