using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using ZadanieComACom.Model;

namespace ZadanieComACom
{
    class Program
    {

        // PRACOWNIK; KOD; IMIE; NAZWISKO;
        // „PRACOWNIK”; „kod”; „nazwisko”; „imię”
        // Potem wszystkie wiersze z godzinami rozpoczęcia pracy dotyczące tego pracownika posortowane według daty
        //„DZIEN”; „data”; „odGodziny”

        static void Main(string[] args)
        {
            Console.WriteLine("Proszę wybrać: \n(1) Zaladuj pliki z domyślnej lokalizacji: " +
                @"c:\temp\*." + "\n(2) Wybierz plik ręcznie.");
            var wybierzOpcje = Console.ReadLine();
            if (String.IsNullOrWhiteSpace(wybierzOpcje))
            {
                Console.WriteLine("Błąd. Nie wybrano żadnej opcji.");
                return;
            }

            switch (Convert.ToInt32(wybierzOpcje))
            {
                case 1:
                    if (!Directory.Exists(@"c:\temp\"))
                        Directory.CreateDirectory(@"c:\temp\");

                    var autoDaneWejscia = Directory.GetFiles(@"c:\temp\", "*.xml");
                    WybierzDaneWejsciaUtworzNowyPlik(autoDaneWejscia);
                    break;

                case 2:
                    Console.WriteLine("Proszę podać pełną ściężę do plików wejściowych.\n pracownicy.xml");
                    var wybierzPracownicy = Console.ReadLine();
                    Console.WriteLine("dni.xml");
                    var wybierzDni = Console.ReadLine();
                    Console.WriteLine("Podaj pełną ścieżkę wraz z nazwą dla pliku wynikowego.");
                    var plikWynikowy = Console.ReadLine();
                    var daneWejsciowe = new string[] { wybierzDni, wybierzPracownicy };
                    WybierzDaneWejsciaUtworzNowyPlik(daneWejsciowe, plikWynikowy);
                    break;
            }
        }

        private static void WybierzDaneWejsciaUtworzNowyPlik(string[] plikiWejscia, string plikWynikowy = @"c:\temp\DniPracownicze.txt")
        {
            #region Pobieranie danych wejściowych
            XmlDocument xmlDoc = new XmlDocument();
            Console.WriteLine("{0} Pobieranie danych wejsciowych START.", DateTime.Now);
            xmlDoc.Load(plikiWejscia[1]);
            var pracownikChildNodes = xmlDoc.DocumentElement.ChildNodes;
            List<Pracownik> listaPracownikow = PobierzListePracownikow(pracownikChildNodes);
            xmlDoc.Load(plikiWejscia[0]);
            var dzienChildNodes = xmlDoc.DocumentElement.ChildNodes;
            List<Dzien> listaDni = PobierzListeDni(dzienChildNodes);
            Console.WriteLine("{0} Zakonczono pobieranie danych wejsciowych.", DateTime.Now);
            Console.WriteLine("Licznik plikow pracownikow: {0}", listaPracownikow.Count);
            Console.WriteLine("Licznik plikow dni: {0}", listaDni.Count);
            #endregion

            #region Algorytm do wyswietlenia danych
            //foreach (var pracownik in listaPracownikow)
            //{
            //    Console.WriteLine(pracownik.GetNaglowek());
            //    foreach (var dzien in listaDni)
            //    {
            //        if (pracownik.GetKod() == dzien.GetKodPracownika())
            //            Console.WriteLine(dzien.GetNaglowek());
            //    }
            //    Console.WriteLine(Environment.NewLine);
            //}
            #endregion

            #region Tworzenie pliku txt
            Console.WriteLine("{0} Tworzenie pliku wyjściowego.", DateTime.Now);
            using (TextWriter tw = new StreamWriter(plikWynikowy))
            {
                foreach (var pracownik in listaPracownikow)
                {
                    tw.WriteLine(pracownik.GetNaglowek());
                    //Console.WriteLine(pracownik.GetNaglowek());
                    foreach (var dzien in listaDni)
                    {
                        if (pracownik.KodPracownika == dzien.GetKodPracownika())
                        {
                            //Console.WriteLine(dzien.GetNaglowek());
                            tw.WriteLine(dzien.GetNaglowek());
                        }
                    }
                    tw.WriteLine(Environment.NewLine);
                }
            }
            Console.WriteLine("{0} Zakonczono tworzenie pliku wyjściowego.", DateTime.Now);
            #endregion
        }

        private static List<Dzien> PobierzListeDni(XmlNodeList dzienChildNodes)
        {
            var listaDni = new List<Dzien>();
            foreach (XmlNode dzienXml in dzienChildNodes)
            {

                var dzien = new Dzien();
                if (dzienXml.HasChildNodes)
                {
                    foreach (XmlNode dzienInfo in dzienXml.ChildNodes)
                    {
                        switch (dzienInfo.Name.ToLower())
                        {
                            case "kodpracownika":
                                    if (!String.IsNullOrEmpty(dzienInfo.InnerText))
                                        dzien.SetKodPracownika(dzienInfo.InnerText);
                                    else
                                        dzien.SetKodPracownika("Brak danych.");
                                break;

                            case "data":
                                    if (!String.IsNullOrEmpty(dzienInfo.InnerText))
                                        dzien.SetData(dzienInfo.InnerText);
                                    else
                                        dzien.SetData("Brak danych.");
                                break;

                            case "odgodziny":
                                    if (!String.IsNullOrEmpty(dzienInfo.InnerText))
                                        dzien.SetOdGodziny(dzienInfo.InnerText);
                                    else
                                        dzien.SetOdGodziny("Brak danych.");
                                break;

                            default:
                                throw new Exception("Błąd pobierania elementu:" + dzienInfo.Name);
                        }
                    }
                }
                //Console.WriteLine(dzien.GetNaglowek());
                listaDni.Add(dzien);

            }
            return listaDni;
        }

        private static List<Pracownik> PobierzListePracownikow(XmlNodeList pracownikChildNodes)
        {
            var listaPracownikow = new List<Pracownik>();
            foreach (XmlNode pracownikXml in pracownikChildNodes)
            {
                //Console.WriteLine(pracownikXml.Name);
                //Console.WriteLine(pracownikXml.Attributes.GetNamedItem("Kod").Name);
                //Console.WriteLine("Pobieranie pracownika o numerze kod: " + pracownikXml.Attributes.GetNamedItem("Kod").Value);

                //Kod nie koniecznie może być jako int.
                var pracownik = new Pracownik();
                pracownik.KodPracownika = pracownikXml.Attributes.GetNamedItem("Kod").Value;

                if (pracownikXml.HasChildNodes)
                {
                    foreach (XmlNode pracownikInfo in pracownikXml.ChildNodes)
                    {
                        switch (pracownikInfo.Name.ToLower())
                        {
                            case "nazwisko":
                                pracownik.SetNazwisko(pracownikInfo.InnerText);
                                break;

                            case "imie":
                                pracownik.SetImie(pracownikInfo.InnerText);
                                break;

                            default:
                                throw new Exception("Błąd pobierania elementu:" + pracownikInfo.Name);
                        }
                    }

                }
                listaPracownikow.Add(pracownik);
            }
            return listaPracownikow;
        }
    }
}
