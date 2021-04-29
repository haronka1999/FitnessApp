using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FitnessApp.Model
{
    public class Kliens : INotifyPropertyChanged
    {
        public int kliens_id { get; set; }
        public string nev  { get; set; }
        public string telefon { get; set; }
        public string email { get; set; }
        public bool is_deleted { get; set; }
        public string photo { get; set; }
        public DateTime inserted_date { get; set; }
        public string szemelyi { get; set; }
        public string cim { get; set; }
        public string vonalkod { get; set; }
        public string megjegyzes { get; set; }




        public Kliens(int kliens_id, string nev, string telefon, string email, bool is_deleted, string photo, DateTime inserted_date, string szemelyi, string cim, string vonalkod, string megjegyzes)
        {
            this.kliens_id = kliens_id;
            this.nev = nev;
            this.telefon = telefon;
            this.email = email;
            this.is_deleted = is_deleted;
            this.photo = photo;
            this.inserted_date = inserted_date;
            this.szemelyi = szemelyi;
            this.cim = cim;
            this.vonalkod = vonalkod;
            this.megjegyzes = megjegyzes;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private int _count;
        public int Count
        {
            get { return _count; }
            set
            {
                _count = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
            }
        }
        public override string ToString()
        {
            return 
            this.kliens_id  + " " +
            this.nev  + " " +
            this.telefon  + " "+ 
            this.email  + " " +
            this.is_deleted + " "+
            this.photo  + " " +
            this.inserted_date + " " +
            this.szemelyi + " " +
            this.cim + " " +
            this.vonalkod + " " +
            this.megjegyzes + " " ;
        }
    }
}
