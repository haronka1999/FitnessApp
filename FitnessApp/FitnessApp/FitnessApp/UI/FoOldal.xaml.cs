﻿using System;
using System.Collections.Generic;
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

namespace FitnessApp.UI
{
    /// <summary>
    /// Interaction logic for FoOldal.xaml
    /// </summary>
    public partial class FoOldal : UserControl
    {
        public FoOldal()
        {
            InitializeComponent();
        }

        private void BtnOk_click(object sender, RoutedEventArgs e)
        {
           // this.Visibility = Visibility.Hidden;
            beleptetes.Visibility = Visibility.Visible;

        }
    }
}