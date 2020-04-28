using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using ZadanieComACom.Model;

namespace ZadanieComACom
{
    class Program
    {
        static void Main(string[] args)
        {
            // Z defaultu uzywalem tutaj c:\temp\, obecnie uzywam folderu z katalogu glownego.
            var defaultDir = @"..\..\temp\";
            //var defaultDir = @"c:\temp\";
            List<Pracownik> listaPracownikow;
            List<Dzien> listaDni;

            Console.WriteLine("Wybirz jedną z dostępnych opcji." +
                "\n1.Załaduj pliki z domyślnej lokalizacji ({0})." +
                "\n2.Wybierz pliki ręcznie." +
                "\n3.Załaduj pliki z domyślnej lokalizacji ({0}) i wyświetl przetwarzanie na ekranie.", defaultDir);

            int.TryParse(Console.ReadLine(), out int wybierzOpcje);
            if (wybierzOpcje == 0)
            {
                Console.WriteLine("Błąd. Nie wybrano żadnej opcji.");
                return;
            }

            switch (wybierzOpcje)
            {
                case 1:
                    //var plikWynikowy = @"c:\temp\DniPracownicze.txt";
                    var plikWynikowy = @"..\..\temp\DniPracownicze.txt";
                    var daneWejsciowe = Directory.GetFiles(defaultDir, "*.xml");
                    PobierzDaneWejsciowe(daneWejsciowe, out listaPracownikow, out listaDni);
                    TworzeniePlikuTekstowego(plikWynikowy, listaPracownikow, listaDni);
                    break;

                case 2:
                    Console.WriteLine("Proszę podać pełną ściężę wraz z nazwą do plików wejściowych." +
                        "\nPracownicy (pracownicy.xml)");
                    var wybierzPracownicy = Console.ReadLine();
                    Console.WriteLine("Dni (dni.xml)");
                    var wybierzDni = Console.ReadLine();
                    Console.WriteLine("Podaj pełną ścieżkę wraz z nazwą dla pliku wynikowego.");
                    plikWynikowy = Console.ReadLine();
                    daneWejsciowe = new string[] { wybierzDni, wybierzPracownicy };
                    PobierzDaneWejsciowe(daneWejsciowe, out listaPracownikow, out listaDni);
                    TworzeniePlikuTekstowego(plikWynikowy, listaPracownikow, listaDni);
                    break;

                case 3:
                    plikWynikowy = @"..\..\temp\DniPracownicze.txt";
                    daneWejsciowe = Directory.GetFiles(defaultDir, "*.xml");
                    PobierzDaneWejsciowe(daneWejsciowe, out listaPracownikow, out listaDni);
                    WyswietlPrzetwarzanieNaEkranie(listaPracownikow, listaDni);
                    TworzeniePlikuTekstowego(plikWynikowy, listaPracownikow, listaDni);
                    break;
            }
        }

        private static void TworzeniePlikuTekstowego(string plikWynikowy, List<Pracownik> listaPracownikow, List<Dzien> listaDni)
        {
            #region Tworzenie pliku txt
            Console.WriteLine("{0} Rozpoczęto tworzenie pliku wyjściowego.", DateTime.Now);
            using (TextWriter tw = new StreamWriter(plikWynikowy))
            {
                foreach (var pracownik in listaPracownikow)
                {
                    tw.WriteLine(pracownik.GetNaglowek());
                    foreach (var dzien in listaDni)
                        if (pracownik.KodPracownika == dzien.GetKodPracownika())
                            tw.WriteLine(dzien.GetNaglowek());

                    tw.WriteLine(Environment.NewLine);
                }
            }

            if (!File.Exists(plikWynikowy))
                Console.WriteLine("Błąd! {0} nie został utworzony.", plikWynikowy);
            else
                Console.WriteLine("{0} Zakonczono tworzenie pliku wyjściowego.", DateTime.Now);
            #endregion
        }

        private static void WyswietlPrzetwarzanieNaEkranie(List<Pracownik> listaPracownikow, List<Dzien> listaDni)
        {
            #region Algorytm do wyswietlenia danych
            Console.WriteLine("{0} Przetwarzanie danych.", DateTime.Now);
            foreach (var pracownik in listaPracownikow)
            {
                Console.WriteLine(pracownik.GetNaglowek());
                foreach (var dzien in listaDni)
                {
                    if (pracownik.KodPracownika == dzien.GetKodPracownika())
                        Console.WriteLine(dzien.GetNaglowek());
                }
                Console.WriteLine(Environment.NewLine);
            }
            Console.WriteLine("{0} Zakończono przetwarzanie danych.", DateTime.Now);
            #endregion
        }

        private static void PobierzDaneWejsciowe(string[] plikiWejscia, out List<Pracownik> listaPracownikow, out List<Dzien> listaDni)
        {
            #region Pobieranie danych wejściowych
            XmlDocument xmlDoc = new XmlDocument();
            Console.WriteLine("{0} Rozpoczęto pobieranie danych wejsciowych.", DateTime.Now);
            xmlDoc.Load(plikiWejscia[1]);
            var pracownikChildNodes = xmlDoc.DocumentElement.ChildNodes;
            listaPracownikow = PobierzListePracownikow(pracownikChildNodes);
            xmlDoc.Load(plikiWejscia[0]);
            var dzienChildNodes = xmlDoc.DocumentElement.ChildNodes;
            listaDni = PobierzListeDni(dzienChildNodes);
            Console.WriteLine("{0} Zakonczono pobieranie danych wejsciowych.", DateTime.Now);
            Console.WriteLine("Liczba wpisów z pracownikami {0}", listaPracownikow.Count);
            Console.WriteLine("Liczba wpisów ze wszystkimi datami {0}", listaDni.Count);
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
