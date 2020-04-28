using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZadanieComACom.Model
{
    public class Dzien
    {
        string kodpracownika { get; set; }
        string data { get; set; }
        string odGodziny { get; set; }

        public void SetKodPracownika(string wartosc)
        {
            this.kodpracownika = wartosc;
        }

        public string GetKodPracownika()
        {
            return this.kodpracownika;
        }

        public void SetData(string wartosc)
        {
            this.data = wartosc;
        }

        public string GetData()
        {
            return this.data;
        }
        public void SetOdGodziny(string wartosc)
        {
            this.odGodziny = wartosc;
        }

        public string GetOdGodziny()
        {
            return this.odGodziny;
        }

        //„DZIEN”; „data”; „odGodziny”
        public string GetNaglowek()
        {
            var dzienStr = "\"DZIEN\"; ";
            var dataStr = "\"" + this.data + "\"; ";
            var odGodzinyStr = "\"" + this.odGodziny + "\"; ";

            return String.Concat(dzienStr, dataStr, odGodzinyStr);
        }


    }
}
