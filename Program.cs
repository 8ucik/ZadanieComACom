using System;
using ZadanieComACom.FlowControl;

namespace ZadanieComACom
{
    class Program
    {
        static void Main(string[] args)
        {
            bool pokazMenu = true;
            while (pokazMenu)
            {
                pokazMenu = Menu.PokazMenu();
                Console.WriteLine("Naciśnij dowolny klawisz aby kontynuować...");
                Console.ReadKey();
            }
        }
    }
}
