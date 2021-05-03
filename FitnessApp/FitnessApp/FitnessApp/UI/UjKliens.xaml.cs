using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
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
        private string photo = "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";
        private string comment;
        private List<Berlet> berletek;
        private string date_str;
        List<string> options = new List<string>();

        public IEnumerable<string> Options
        {
            get { return options; }
        }

        public UjKliens()
        {
            InitializeComponent();
            DataContext = this;
            berletek = new List<Berlet>();
            berletek = getBerletekFromDatabase();             
            options = getAbonamentStrings();
        }

        private List<string> getAbonamentStrings()
        {
            List<string> temp_list = new List<string>();

            string temp = "";
            foreach (var berlet in berletek)
            {
                if (berlet.ervenyesseg_belepesek_szama == -1)
                    temp = berlet.berlet_id + ".), érvényesség: " + berlet.ervenyesseg_nap + " nap, " + "ár: " + berlet.ar;
                else if (berlet.ervenyesseg_nap == -1)
                    temp = berlet.berlet_id + ".), érvényesség: " + berlet.ervenyesseg_belepesek_szama + " belépés, " + "ár: " + berlet.ar;
                else
                    temp = berlet.berlet_id + ".), érvényesség: " + berlet.ervenyesseg_belepesek_szama + " belépés, és " + berlet.ervenyesseg_nap + " nap" + "ár: " + berlet.ar;


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
                imgPhoto.Source = new BitmapImage(new Uri(op.FileName));
                using (Image image = Image.FromFile(op.FileName))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();

                        photo = Convert.ToBase64String(imageBytes);
                    }
                }
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
