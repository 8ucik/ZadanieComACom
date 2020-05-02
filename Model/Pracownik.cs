using System;

namespace ZadanieComACom.Model
{
    public class Pracownik
    {
        string nazwisko { get; set; }
        string imie { get; set; }
        private string kodPracownika;

        //public void SetKod(string wartosc)
        //{
        //    this.kod = wartosc;
        //}
        //public string GetKod()
        //{
        //    return this.kod;
        //}
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
            var kodStr = "\"" + kodPracownika + "\"; ";
            var imieStr = "\"" + imie + "\"; ";
            var nazwiskoStr = "\"" + nazwisko + "\"; ";

            return String.Concat(pracownikStr, kodStr, imieStr, nazwiskoStr);
        }

        public string KodPracownika
        {
            get { return kodPracownika; }
            set { kodPracownika = value; }
        }
    }
}
