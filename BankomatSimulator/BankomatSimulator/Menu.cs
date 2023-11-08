using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankomatSimulator
{
    public enum Richiesta
    {
        Uscita = 0,
        SchermataDiBenvenuto = 1,
        MenuPrincipale = 2,
        Login = 3
    }
    public class Menu
    {
        /// <summary>
        /// Stampa l'intestazione del menu
        /// </summary>
        protected void StampaIntestazione(string titoloMenu)
        {
            Console.Clear();
            Console.WriteLine("**************************************************");
            Console.WriteLine("*              Bankomat Simulator                *");
            Console.WriteLine("**************************************************");
            Console.WriteLine("".PadLeft((50 - titoloMenu.Length) / 2)
                + titoloMenu);
            Console.WriteLine("--------------------------------------------------");
            return;
        }
        /// <summary>
        /// Gestisce la scelta dell'utente. Verfica che l'input sia una scelta
        /// compresa nell'intervallo <paramref name="min"/> - <paramref name="max"/>
        /// </summary>
        /// <returns>Il numero inserito dall'utente, -1 se la scelta non è consentita</returns>
        protected int ScegliVoceMenu(int min, int max)
        {
            string rispostaUtente;


            Console.Write("Scelta: ");
            rispostaUtente = Console.ReadKey().KeyChar.ToString();
            if (!Int32.TryParse(rispostaUtente, out int scelta) ||
                !(min <= scelta && scelta <= max))
            {
                scelta = -1;
                Console.WriteLine("");
                Console.WriteLine($"Scelta non consentita - {rispostaUtente}");
                Console.Write("Premere un tasto per proseguire");
                Console.ReadKey();
            }
            return scelta;
        }
    }
}
