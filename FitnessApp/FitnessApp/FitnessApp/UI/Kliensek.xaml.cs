using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using ClosedXML.Excel;
using FitnessApp.Model;
using static FitnessApp.Utils;

namespace FitnessApp.UI
{
    public partial class Kliensek : System.Windows.Controls.UserControl
    {
        private string export_path_excel;
        public ObservableCollection<Kliens> users { get; set; }
        public object MessageBpx { get; private set; }
        public string photo { get; set; }

        public Kliensek()
        {
            InitializeComponent();
            DataContext = this;
            users = new ObservableCollection<Kliens>();
            users = getUsersFromDatabase();
            this.KliensGrid.ItemsSource = users;
            search.ImageSource = new BitmapImage(new Uri(Utils.search));
            save.ImageSource = new BitmapImage(new Uri(Utils.save));
            excel.ImageSource = new BitmapImage(new Uri(Utils.excel));
        }

        private ObservableCollection<Kliens> getUsersFromDatabase()
        {
            ObservableCollection<Kliens> kliensek = new ObservableCollection<Kliens>();
            SqlConnection sqlCon = new SqlConnection(conString);
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
                        photo = reader["photo"].ToString();
                        Kliens kliens = new Kliens(Int32.Parse(reader["kliens_id"].ToString()),
                                                    reader["nev"].ToString(),
                                                    reader["telefon"].ToString(),
                                                    reader["email"].ToString(),
                                                    bool.Parse(reader["is_deleted"].ToString()),
                                                    photo,
                                                    Convert.ToDateTime(reader["inserted_date"].ToString()),
                                                    reader["szemelyi"].ToString(),
                                                    reader["cim"].ToString(),
                                                    reader["vonalkod"].ToString(),
                                                    reader["megjegyzes"].ToString());

                        //csak akkor jelenitsuk meg ha nincs torolve
                        if (kliens.is_deleted == false)
                            kliensek.Add(kliens);
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
            
            users = kliensek;
            return users;
        }

        private void Delete_User(object sender, RoutedEventArgs e)
        {
            Kliens drv = (Kliens)KliensGrid.SelectedItem;
            String kliens_id = (drv.kliens_id).ToString();

            SqlConnection sqlCon = new SqlConnection(conString);
            string query = "UPDATE Kliensek set is_deleted=1 WHERE kliens_id = @kliens_id;";
            try
            {

                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                sqlCmd.Parameters.AddWithValue("@kliens_id", kliens_id);
                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }

                int result = sqlCmd.ExecuteNonQuery();

                if (result < 0)
                    System.Windows.MessageBox.Show("Adatbázis hiba a kliens torlesenel");
                else
                    System.Windows.MessageBox.Show("Kliens sikeresen torolve");

                Refresh();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Hiba kliens torles: " + ex.Message);
            }
            finally
            {
                sqlCon.Close();
            }
        }

        private void Edit_User(object sender, RoutedEventArgs e)
        {
            saveEditButton.Visibility = Visibility.Visible;
            KliensGrid.IsReadOnly = false;
        }

        private void Save_Edited_Users(object sender, RoutedEventArgs e)
        {
            Kliens kliens = (Kliens)KliensGrid.SelectedItem;

            SqlConnection sqlCon = new SqlConnection(conString);
            string query = "UPDATE Kliensek SET nev=@nev, telefon=@telefon, email=@email, szemelyi=@szemelyi, " +
                "cim=@cim, megjegyzes=@megjegyzes WHERE kliens_id=@kliens_id;";
            try
            {
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                sqlCmd.Parameters.AddWithValue("@kliens_id", kliens.kliens_id);
                sqlCmd.Parameters.AddWithValue("@nev", kliens.nev);
                sqlCmd.Parameters.AddWithValue("@telefon", kliens.telefon);
                sqlCmd.Parameters.AddWithValue("@email", kliens.email);
                sqlCmd.Parameters.AddWithValue("@szemelyi", kliens.szemelyi);
                sqlCmd.Parameters.AddWithValue("@cim", kliens.cim);
                sqlCmd.Parameters.AddWithValue("@megjegyzes", kliens.megjegyzes);

                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }

                int result = sqlCmd.ExecuteNonQuery();

                if (result < 0)
                    System.Windows.MessageBox.Show("Adatbázis hiba a kliens szerkesztésnél");

                Refresh();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Hiba kliens szerkesztésnél: " + ex.Message);
            }
            finally
            {
                sqlCon.Close();
            }

            saveEditButton.Visibility = Visibility.Hidden;
            KliensGrid.IsReadOnly = true;
        }

        private void BtnSaveXls_click(object sender, RoutedEventArgs e)
        {
            export_path_excel = getPath(sender);

            //ha a felhasznalo meggondolja magat akkor nem tortenik semmi
            if (export_path_excel == "")
            {
                return;
            }
            var wb = new XLWorkbook();
            string kliens_string = "Kliensek";
            var ws = wb.Worksheets.Add(kliens_string);
            string temp = export_path_excel + "\\" + kliens_string + ".xlsx";
            try
            {
                ws.Cell(1, 1).InsertData(users);
                wb.SaveAs(temp);
                System.Windows.MessageBox.Show("Sikeres kimentes a valasztott helyre");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Hiba a kimentesnel: " + ex.Message);
            }
        }

        private string getPath(object sender)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    return fbd.SelectedPath;
                }
                if (string.Equals((sender as System.Windows.Controls.Button).Name, @"CloseButton"))
                {
                    return "";
                }
                return "";
            }

        }

        private void Search_Client_Click(object sender, RoutedEventArgs e)
        {
            string search = searchResult.Text;
            ObservableCollection<Kliens> kliensek = new ObservableCollection<Kliens>();
            if (search != "")
            {
                SqlConnection sqlCon = new SqlConnection(conString);
                try
                {
                    string query = "SELECT * from Kliensek WHERE nev like @value";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                    sqlCmd.Parameters.AddWithValue("@value", "%" + search + "%");
                    if (sqlCon.State == ConnectionState.Closed)
                    {
                        sqlCon.Open();
                    }

                    using (SqlDataReader reader = sqlCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            photo = reader["photo"].ToString();
                            Kliens kliens = new Kliens(Int32.Parse(reader["kliens_id"].ToString()),
                                                        reader["nev"].ToString(),
                                                        reader["telefon"].ToString(),
                                                        reader["email"].ToString(),
                                                        bool.Parse(reader["is_deleted"].ToString()),
                                                        photo,
                                                        Convert.ToDateTime(reader["inserted_date"].ToString()),
                                                        reader["szemelyi"].ToString(),
                                                        reader["cim"].ToString(),
                                                        reader["vonalkod"].ToString(),
                                                        reader["megjegyzes"].ToString());

                            //csak akkor jelenitsuk meg ha nincs torolve
                            if (kliens.is_deleted == false)
                            {
                                kliensek.Add(kliens);
                            }
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
                users = kliensek;
            }
            else
            {
                getUsersFromDatabase();
            }
            this.KliensGrid.ItemsSource = users;
            searchResult.Text = "";
        }

        private void Refresh()
        {
            users = getUsersFromDatabase();
            this.KliensGrid.ItemsSource = users;
        }
    }
}
