using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using FitnessApp.Model;
using static FitnessApp.Utils;

namespace FitnessApp.UI
{

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
        private List<Berlet> berletek;

        private List<string> message_to_display = new List<string>();

        //egyeb valtozok
        private string date_str;

        public UjKliens()
        {
            InitializeComponent();
            DataContext = this;
            berletek = new List<Berlet>();
            berletek = getBerletekFromDatabase();
            message_to_display = getAbonamentStrings();
        }

        private List<string> getAbonamentStrings()
        {
            List<string> temp_list = new List<string>();

            string temp = "";
            foreach (var berlet in berletek)
            {
                if (berlet.ervenyesseg_belepesek_szama == -1)
                    temp = "Érvényesség: " + berlet.ervenyesseg_nap + " nap, " + "ár: " + berlet.ar;
                else if (berlet.ervenyesseg_nap == -1)
                    temp = "Érvényesség: " + berlet.ervenyesseg_belepesek_szama + " belépés, " + "ár: " + berlet.ar;
                else
                    temp = "Érvényesség: " + berlet.ervenyesseg_belepesek_szama + " belépés, és " + berlet.ervenyesseg_nap + " nap" + "ár: " + berlet.ar;


                //Console.WriteLine(temp);
                temp_list.Add(temp);
            }


            return temp_list;
        }

        private List<Berlet> getBerletekFromDatabase()
        {
            List<Berlet> abonaments = new List<Berlet>();
            SqlConnection sqlCon = new SqlConnection(conString);

            string query = "SELECT * FROM Berletek;";
            try
            {
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
                        abonaments.Add(berlet);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Hiba getBerletek: " + ex.Message);
            }
            finally
            {
                sqlCon.Close();
            }

            return abonaments;
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


            if (name == "" || phone == "" || email == "" || cnp == ""  || my_address == "" || barcode == "" || date_str == "")
            {
                System.Windows.MessageBox.Show("Nem minden mező került kitöltésre!");
                return;
            }

            insertClientIntoDataBase(name, phone, email, deleted, photo, date, cnp, my_address, barcode, comment);

            UserName.Text = "";
            Number.Text = "";
            Email.Text = "";
            CNP.Text = "";
            address.Text = "";
            Comment.Text = "";
        }

        private void insertClientIntoDataBase(string name, string phone, string email, int deleted, string photo, DateTime date, string cnp, string my_address, string barcode, string comment)
        {
            SqlConnection sqlCon = new SqlConnection(conString);
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
                System.Windows.MessageBox.Show("Hiba insert kliensek: " + ex.Message);
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

    }
}
