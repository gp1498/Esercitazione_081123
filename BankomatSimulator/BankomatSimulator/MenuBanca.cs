using BankSimulatorLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankomatSimulator
{
    public class MenuBanca : Menu
    {
        private Banca banca = null;
        private long idUtente;
        GestoreBanca gestoreBanca = new GestoreBanca();

        public MenuBanca(long idBanca , long idUtente)
        {
            this.idUtente = idUtente;
            banca = gestoreBanca.dettaglio(idBanca);
        }

        public Richiesta Esegui()
        {
            int scelta = -1;
            do
            {
                scelta = MenuPrincipale();
                switch (scelta)
                {
                    case (int)Funzionalita.Versamento:
                        if (Versamento())
                        {
                            if (banca.elencoFunzionalita.ContainsValue(Funzionalita.Report_Saldo))
                                ReportSaldo();
                        }
                        break;
                    case (int)Funzionalita.Report_Saldo:
                        ReportSaldo();
                        break;
                    case (int)Funzionalita.Prelievo:
                        if (Prelievo())
                        {
                            if (banca.elencoFunzionalita.ContainsValue(Funzionalita.Report_Saldo))
                                ReportSaldo();
                        }
                        break;
                }
            } while (scelta != 0);
            return Richiesta.Uscita;
        }

        private void ReportSaldo()
        {

            StampaIntestazione($"Report Saldo");

            int saldo  = gestoreBanca.SaldoUtente(idUtente);
           
            Console.WriteLine($"Saldo:                      {saldo}");
            Console.WriteLine($"Data esecuzione report:     {DateTime.Now}");

            Console.Write("Premere un tasto per proseguire");
            Console.ReadKey();

            return;
        }

        private bool Versamento()
        {
            string risposta;
            bool esitoVersamento;


            StampaIntestazione($"Versamento - {banca.Nome}");
            Console.Write("Inserisci importo da versare: ");

            risposta = Console.ReadLine();
            if (!Int32.TryParse(risposta, out int valoreVersamento))
            {
                Console.WriteLine("Operazione annullata - Inserire un numero");
                Console.Write("Premere un tasto per proseguire");
                Console.ReadKey();
                return false;
            }

            esitoVersamento = gestoreBanca.Versamento(valoreVersamento,idUtente);
            if (!esitoVersamento)
            {
                Console.WriteLine("Operazione annullata");
            }
            else
            {
                Console.WriteLine("Versamento effettuato");
            }

            Console.Write("Premere un tasto per proseguire");
            Console.ReadKey();
            return esitoVersamento;
        }

        private int MenuPrincipale()
        {
            int scelta = -1;

            while (scelta == -1)
            {
                StampaIntestazione($"Menu principale - {banca.Nome}");

                var fooList = new List<int>();
                var index = 1;
                Console.WriteLine("0 - Uscita");

                foreach (var funzionalita in banca.elencoFunzionalita)
                {
                    Console.WriteLine($"{(index++).ToString()} - {funzionalita.Value.ToString()}");
                    fooList.Add((int)funzionalita.Key);
                }
                
                int indexVoce = ScegliVoceMenu(0, banca.elencoFunzionalita.Count);
                if(indexVoce == 0)
                {
                    scelta = 0;
                }
                else
                {
                    scelta = fooList[indexVoce-1];
                }
            }

            return scelta;
        }
        private bool Prelievo()
        {
            string risposta;
            bool esitoStoccaggio;

            StampaIntestazione($"Prelievo - {banca.Nome}"); 
            Console.Write("Inserisci importo da prelevare: ");

            risposta = Console.ReadLine();
            if (!Int32.TryParse(risposta, out int importo))
            {
                Console.WriteLine("Operazione annullata - Inserire un numero");
                Console.Write("Premere un tasto per proseguire");
                Console.ReadKey();
                return false;
            }

            esitoStoccaggio = gestoreBanca.Prelievo(importo, idUtente);
            if (!esitoStoccaggio)
                Console.WriteLine("Operazione annullata");
            else
                Console.WriteLine($"Operazione completata - Importo prelevato: " +
                    $"{importo} ");

            Console.Write("Premere un tasto per proseguire");
            Console.ReadKey();
            return esitoStoccaggio;
        }
    }
}
