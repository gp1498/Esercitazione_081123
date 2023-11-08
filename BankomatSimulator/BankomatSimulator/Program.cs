using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankomatSimulator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MenuAccesso MenuAccesso = new MenuAccesso();
            Richiesta Richiesta = MenuAccesso.Esegui();
            if (Richiesta == Richiesta.MenuPrincipale)
            {
                MenuBanca MenuOperatore =
                  new MenuBanca(MenuAccesso.chiaveBanca.Value,MenuAccesso.chiaveUtente.Value);
                MenuOperatore.Esegui();
            }
        }
    }
}
