using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using ZadanieComACom.Model;

namespace ZadanieComACom.FlowControl
{
    public class KontrolaPlikow
    {
        public static void TworzeniePlikuWyjściowego(string plikWyjsciowy, List<Pracownik> listaPracownikow, List<Dzien> listaDni)
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

        public static void WyswietlPrzetwarzanieNaEkranie(List<Pracownik> listaPracownikow, List<Dzien> listaDni)
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

        public static void PobierzDaneWejsciowe(string[] plikiWejscia, out List<Pracownik> listaPracownikow, out List<Dzien> listaDni)
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
