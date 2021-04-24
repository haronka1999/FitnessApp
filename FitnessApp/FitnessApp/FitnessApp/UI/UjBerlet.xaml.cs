using System;
using System.Collections.Generic;
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

        string text1;
        string text2;
        public UjBerlet()
        {
            InitializeComponent();
        }

        private void NumberValidationTextBox_1(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
            text1 = e.Text;

        }

        private void BtnSave_click(object sender, RoutedEventArgs e)
        {
            if (Int32.Parse(text1) < 5 || Int32.Parse(text1) > 31)
            {
                //error message should be displayed
            }

            if (Int32.Parse(text2) < 5 || Int32.Parse(text2) > 25)
            {
                //error message should be displayed
            }
        }

        private void NumberValidationTextBox_2(object sender, TextCompositionEventArgs e)
        {
            if(text1 != "")
            {
                Regex regex = new Regex("[^0-9]+");
                e.Handled = regex.IsMatch(e.Text);
                text2 = e.Text;
            }
            else
            {
                input_2.IsReadOnly = true;

            }

        }

        private void NumberValidationTextBox_3(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
