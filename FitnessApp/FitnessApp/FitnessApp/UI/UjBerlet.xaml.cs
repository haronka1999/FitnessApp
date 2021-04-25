using System;
using System.Collections.Generic;
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
         
         */
        public string megnevezes;

        public float ar;

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
            if (napokErvenyesseg < 5 ||napokErvenyesseg > 31)
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

            try
            {
                hanyOraig = DateTime.ParseExact(hanyOraig_str, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba hanyoraig: " + ex.Message);
            }

            try
            {
                hanyOratol = DateTime.ParseExact(hanyOraig_str, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba hanyOratol: " + ex.Message);
            }




            MessageBox.Show("teremId: " + teremId);

          //  insertBerletIntoDataBase(megnevezes, ar, napokErvenyesseg, belepesekErvenyesseg, torolve, teremId, hanyOratol, hanyOraig, napiMaxHasznalat, letrehozasi_datum);
        }

        private void insertBerletIntoDataBase(string megnevezes, float ar, int napokErvenyesseg, int belepesekErvenyesseg, bool torolve, int teremId, DateTime hanyOratol, DateTime hanyOraig, int napiMaxHasznalat, DateTime letrehozasi_datum)
        {
            SqlConnection sqlCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\.Net_Project\FitnessApp\FitnessApp\FitnessApp\FitnessApp\Database\db_local.mdf;Integrated Security=True");

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

            }
            catch (Exception ex)
            {

            }
            finally
            {

            }



        }


        //VALIDATION!!!

        private void NumberValidationTextBox_1(object sender, TextCompositionEventArgs e)
        {
            e.Handled = regex.IsMatch(e.Text);
            napokErvenyesseg = Int32.Parse(e.Text);
        }



        private void NumberValidationTextBox_2(object sender, TextCompositionEventArgs e)
        {
             e.Handled = regex.IsMatch(e.Text);
             belepesekErvenyesseg = Int32.Parse(e.Text);       
        }

        private void NumberValidationTextBox_3(object sender, TextCompositionEventArgs e)
        {
            e.Handled = regex.IsMatch(e.Text);
            napiMaxHasznalat = Int32.Parse(e.Text);
        }

        private void NumberValidationTextBox_4(object sender, TextCompositionEventArgs e)
        {
            hanyOratol_str = e.Text;
        }

        private void NumberValidationTextBox_5(object sender, TextCompositionEventArgs e)
        {
            hanyOraig_str = e.Text;
        }

        private void NumberValidationTextBox_6(object sender, TextCompositionEventArgs e)
        {
            e.Handled = regex.IsMatch(e.Text);
            teremId = Int32.Parse(e.Text);
        }

        private void Terem1_Clicked(object sender, RoutedEventArgs e)
        {
         
            if ((bool)terem1.IsChecked)
            {
                teremId = 1;
            }
            
        }

        private void Terem2_Clicked(object sender, RoutedEventArgs e)
        {

            if ((bool)terem2.IsChecked)
            {
                teremId = 2;
            }
        }

        private void Terem3_Clicked(object sender, RoutedEventArgs e)
        {

            if ((bool)terem3.IsChecked)
            {
                teremId = 3;
            }
        }
    }
}
