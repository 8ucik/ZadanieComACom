using System;
using System.IO;
using System.Collections.Generic;
using ZadanieComACom.Model;


namespace ZadanieComACom.FlowControl
{
    public class Menu
    {
        public static bool PokazMenu()
        {
            ///<summary>
            /// Z defaultu uzywalem tutaj c:\temp\, obecnie uzywam folderu z katalogu glownego.
            /// Obecnie dodałem App.config oraz ustawienia z domyślnymi sciezkami.
            ///var defaultDir = @"c:\temp\";
            ///var plikWynikowy = @"c:\temp\DniPracownicze.txt";</summary>
            
            var sciezkaZapisu = Directory.GetParent(Properties.Settings.Default.domyslnaSciezkaZapisu).FullName;
            var plikZapisu = Properties.Settings.Default.domyslnyPlikZapisu;
            var plikWyjsciowy = String.Concat(sciezkaZapisu, plikZapisu);

            List<Pracownik> listaPracownikow;
            List<Dzien> listaDni;

            Console.Clear();
            Console.WriteLine("Proszę wybrać jedną z dostępnych opcji:");
            Console.WriteLine("1) Wybierz pliki ręcznie.");
            Console.WriteLine("2) Załaduj pliki z domyślnej lokalizacji ( {0} ).");
            Console.WriteLine("3) Załaduj pliki z domyślnej lokalizacji ( {0} ) i wyświetl ich przetwarzanie na ekranie.");
            Console.WriteLine("4) Wyjście.");

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
                    KontrolaPlikow.PobierzDaneWejsciowe(daneWejsciowe, out listaPracownikow, out listaDni);
                    KontrolaPlikow.TworzeniePlikuWyjściowego(plikWyjsciowy, listaPracownikow, listaDni);
                    return true;

                case 2:
                    daneWejsciowe = Directory.GetFiles(sciezkaZapisu, "*.xml");
                    KontrolaPlikow.PobierzDaneWejsciowe(daneWejsciowe, out listaPracownikow, out listaDni);
                    KontrolaPlikow.TworzeniePlikuWyjściowego(plikWyjsciowy, listaPracownikow, listaDni);
                    return true;

                case 3:
                    daneWejsciowe = Directory.GetFiles(sciezkaZapisu, "*.xml");
                    KontrolaPlikow.PobierzDaneWejsciowe(daneWejsciowe, out listaPracownikow, out listaDni);
                    KontrolaPlikow.WyswietlPrzetwarzanieNaEkranie(listaPracownikow, listaDni);
                    KontrolaPlikow.TworzeniePlikuWyjściowego(plikWyjsciowy, listaPracownikow, listaDni);
                    return true;

                case 4:
                    Environment.Exit(0);
                    return false;

                default:
                    Console.WriteLine("Błąd. Nie rozpoznano opcji.");
                    return true;
            }
        }
    }
}
