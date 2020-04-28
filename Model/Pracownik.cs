using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZadanieComACom.Model
{
    public class Pracownik
    {
        string kod { get; set; }
        string nazwisko { get; set; }
        string imie { get; set; }

        public void SetKod(string wartosc)
        {
            this.kod = wartosc;
        }
        public string GetKod()
        {
            return this.kod;
        }
        public void SetNazwisko(string wartosc)
        {
            this.nazwisko = wartosc;
        }
        public string GetNazwisko()
        {
            return this.nazwisko;
        }
        public void SetImie(string wartosc)
        {
            this.imie = wartosc;
        }
        public string GetImie()
        {
            return this.imie;
        }

        // PRACOWNIK; KOD; IMIE; NAZWISKO;
        // „PRACOWNIK”; „kod”; „nazwisko”; „imię”
        public string GetNaglowek()
        {
            var pracownikStr = "\"PRACOWNIK\"; ";
            var kodStr = "\"" + kod + "\"; ";
            var imieStr = "\"" + imie + "\"; ";
            var nazwiskoStr = "\"" + nazwisko + "\"; ";

            return String.Concat(pracownikStr, kodStr, imieStr, nazwiskoStr);
        }
    }
}
