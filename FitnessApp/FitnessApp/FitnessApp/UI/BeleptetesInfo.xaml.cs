using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Controls;
using static FitnessApp.Utils;

namespace FitnessApp.UI
{
    public partial class BeleptetesInfo : UserControl
    {
        public string photo { get; set; }
        public BeleptetesInfo()
        {
            InitializeComponent();
        }
    }
}
