using System.Windows.Controls;

namespace FitnessApp.UI
{
    public partial class BeleptetesInfo : UserControl
    {
        private FoOldal uc;
        public BeleptetesInfo()
        {
            InitializeComponent();
            uc = new FoOldal();
        }

        private void BtnTunes_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //uc.beleptetes.Visibility = Visibility.Hidden;

        }
    }
}
