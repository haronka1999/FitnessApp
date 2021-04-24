using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FitnessApp.UI
{
    /// <summary>
    /// Interaction logic for UjKliens.xaml
    /// </summary>
    public partial class UjKliens : System.Windows.Controls.UserControl
    {
        public UjKliens()
        {
            InitializeComponent();
        }

        private void BtnUpload_click(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog op = new OpenFileDialog();
            //op.Title = "Select a picture";
            //op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
            //  "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
            //  "Portable Network Graphic (*.png)|*.png";
            //if (op.ShowDialog() == true)
            //{
            //    imgPhoto.Source = new BitmapImage(new Uri(op.FileName));
            //}

        }
    }
}
