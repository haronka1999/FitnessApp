using System;

namespace FitnessApp.Model
{
    public class Berlet
    {
        public int berlet_id;
        public int megnevezes;
        public float ar;
        public int ervenyesseg_nap;
        public int ervenyesseg_belepesek_szama;
        public bool torolve;
        public int terem_id;
        public string hany_oratol;
        public string hany_oraig;
        public int napi_max_hasznalat;
        public DateTime letrehozasi_datum;

        public Berlet(int berlet_id, int megnevezes, float ar, int ervenyesseg_nap, int ervenyesseg_belepesek_szama, bool torolve, int terem_id, string hany_oratol, string hany_oraig, int napi_max_hasznalat, DateTime letrehozasi_datum)
        {
            this.berlet_id = berlet_id;
            this.megnevezes = megnevezes;
            this.ar = ar;
            this.ervenyesseg_nap = ervenyesseg_nap;
            this.ervenyesseg_belepesek_szama = ervenyesseg_belepesek_szama;
            this.torolve = torolve;
            this.terem_id = terem_id;
            this.hany_oratol = hany_oratol;
            this.hany_oraig = hany_oraig;
            this.napi_max_hasznalat = napi_max_hasznalat;
            this.letrehozasi_datum = letrehozasi_datum;
        }

        public override string ToString()
        {
            return this.berlet_id  + " " +
            this.megnevezes + " " +
            this.ar + " " +
            this.ervenyesseg_nap + " " +
            this.ervenyesseg_belepesek_szama + " " +
            this.torolve + " " +
            this.terem_id + " " +
            this.hany_oratol + " " +
            this.hany_oraig + " " +
            this.napi_max_hasznalat + " " +
            this.letrehozasi_datum;
        }
    }
}
