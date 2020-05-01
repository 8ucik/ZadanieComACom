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
            // Obecnie dodałem App.config oraz ustawienia z domyślnymi sciezkami.
            //var defaultDir = @"c:\temp\";
            //var plikWynikowy = @"c:\temp\DniPracownicze.txt";

            var sciezkaZapisu = Directory.GetParent(Properties.Settings.Default.domyslnaSciezkaZapisu).FullName;
            var plikZapisu = Properties.Settings.Default.domyslnyPlikZapisu;
            var plikWyjsciowy = String.Concat(sciezkaZapisu, plikZapisu);

            List<Pracownik> listaPracownikow;
            List<Dzien> listaDni;

            Console.WriteLine("Wybierz jedną z dostępnych opcji." +
                "\n1.Wybierz pliki ręcznie." +
                "\n2.Załaduj pliki z domyślnej lokalizacji: ( {0} )." +
                "\n3.Załaduj pliki z domyślnej lokalizacji: ( {0} ) i wyświetl przetwarzanie na ekranie." +
                "\n4.Wyjście.", sciezkaZapisu);

            int.TryParse(Console.ReadLine(), out int wybierzOpcje);
            switch (wybierzOpcje)
            {
                case 1:
                    Console.WriteLine("Proszę podać pełną ściężę wraz z nazwą do plików wejściowych." +
                        "\nPracownicy (pracownicy.xml)");
                    var wybierzPracownicy = Console.ReadLine();
                    Console.WriteLine("Dni (dni.xml)");
                    var wybierzDni = Console.ReadLine();
                    Console.WriteLine("Podaj pełną ścieżkę wraz z nazwą dla pliku wynikowego.");
                    plikWyjsciowy = Console.ReadLine();
                    var daneWejsciowe = new string[] { wybierzDni, wybierzPracownicy };
                    PobierzDaneWejsciowe(daneWejsciowe, out listaPracownikow, out listaDni);
                    TworzeniePlikuWyjściowego(plikWyjsciowy, listaPracownikow, listaDni);
                    break;

                case 2:
                    daneWejsciowe = Directory.GetFiles(sciezkaZapisu, "*.xml");
                    PobierzDaneWejsciowe(daneWejsciowe, out listaPracownikow, out listaDni);
                    TworzeniePlikuWyjściowego(plikWyjsciowy, listaPracownikow, listaDni);
                    break;

                case 3:
                    daneWejsciowe = Directory.GetFiles(sciezkaZapisu, "*.xml");
                    PobierzDaneWejsciowe(daneWejsciowe, out listaPracownikow, out listaDni);
                    WyswietlPrzetwarzanieNaEkranie(listaPracownikow, listaDni);
                    TworzeniePlikuWyjściowego(plikWyjsciowy, listaPracownikow, listaDni);
                    break;

                case 4:
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("Błąd. Nie rozpoznano opcji.");
                    break;

            }
            Console.ReadKey();
        }

        private static void TworzeniePlikuWyjściowego(string plikWyjsciowy, List<Pracownik> listaPracownikow, List<Dzien> listaDni)
        {
            CzyIstniejeZalegly(plikWyjsciowy);

            Console.WriteLine("{0} Rozpoczęto tworzenie pliku wyjściowego.", DateTime.Now);
            using (TextWriter tw = new StreamWriter(plikWyjsciowy))
            {
                try
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
                catch (Exception ex)
                {
                    throw new Exception("Błąd! Nie udało się utworzyć pliku txt. - \n{0}", ex);
                }

                if (!File.Exists(plikWyjsciowy))
                    Console.WriteLine("Błąd! {0} Plik nie istnieje", plikWyjsciowy);
                else
                    Console.WriteLine("{0} Zakonczono tworzenie pliku wyjściowego." +
                        "\nPlik dostępny pod ścieżką: {1}", DateTime.Now, plikWyjsciowy);
            }
        }

        private static void CzyIstniejeZalegly(string plikWyjsciowy)
        {
            Console.WriteLine("Weryfikacja czy plik wyjsciowy już istnieje: \n( {0} ) ", plikWyjsciowy);
            if (File.Exists(plikWyjsciowy))
            {
                Console.WriteLine("Podany plik istnieje.");
                var zapiszKopie = String.Concat(plikWyjsciowy, DateTime.Now.ToString("yyMMdd_HHmmss"), ".bak");
                File.Move(plikWyjsciowy, zapiszKopie);
                Console.WriteLine("Utworzono kopię pliku: \n( {0} )", zapiszKopie);
            }
            else
                Console.WriteLine("Plik wyjściowy nie istnieje.");
        }

        private static void WyswietlPrzetwarzanieNaEkranie(List<Pracownik> listaPracownikow, List<Dzien> listaDni)
        {
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
        }

        private static void PobierzDaneWejsciowe(string[] plikiWejscia, out List<Pracownik> listaPracownikow, out List<Dzien> listaDni)
        {
            listaPracownikow = new List<Pracownik>();
            listaDni = new List<Dzien>();

            try
            {
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
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd. Podany plik nie istnieje \n{0}", ex);
            }
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
