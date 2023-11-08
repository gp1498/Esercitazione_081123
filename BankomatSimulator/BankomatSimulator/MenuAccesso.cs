using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankSimulatorLib;
using Microsoft.Win32;

namespace BankomatSimulator
{
    public class MenuAccesso : Menu
    {

        private GestoreAccesso GestoreAccesso = new GestoreAccesso();
        private SortedList<int, Banca> Banche = null;
        private Banca BancaCorrente = null;
        private Utente utenteCorrente = null;

        public Nullable<long> chiaveBanca
        {
            get
            {
                if (BancaCorrente != null)
                    return BancaCorrente.Id;
                return null;
            }
        }

        public Nullable<long> chiaveUtente
        {
            get
            {
                if (utenteCorrente != null)
                    return utenteCorrente.Id;
                return null;
            }
        }



        public Richiesta Esegui()
        {
            Richiesta Richiesta = Richiesta.SchermataDiBenvenuto;
            do
            {
                switch (Richiesta)
                {
                    case Richiesta.SchermataDiBenvenuto:
                        Richiesta = SchermataDiBenvenuto();
                        break;
                    case Richiesta.Login:
                        Richiesta = Login();
                        break;
                }
            } while (!(Richiesta == Richiesta.Uscita ||
                    Richiesta == Richiesta.MenuPrincipale));
            return Richiesta;
        }

        private Richiesta Login()
        {
            Richiesta richiesta = Richiesta.MenuPrincipale;
            Credenziali credenziali = new Credenziali();
            StampaIntestazione($"Login - {BancaCorrente.Nome}");
            Console.Write("Nome utente: ");
            credenziali.NomeUtente = Console.ReadLine();
            Console.Write("Password: ");
            credenziali.Password = Console.ReadLine();
            Utente utente = null;
            EsitoLogin EsitoLogin = GestoreAccesso.Login(credenziali,
              BancaCorrente, out utente);
            StampaEsitoLogin(EsitoLogin, utente);
            if (EsitoLogin != EsitoLogin.AccessoConsentito)
                richiesta = Richiesta.SchermataDiBenvenuto;

            utenteCorrente = utente;
            return richiesta;
        }

        private void StampaEsitoLogin(EsitoLogin esitoLogin, Utente utente)
        {
            switch (esitoLogin)
            {
                case EsitoLogin.PasswordErrata:
                    Console.WriteLine($"Password errata - " +
                        $"{utente.TentativiDiAccessoResidui} " +
                        @"tentativ{0} residu{0}", utente.TentativiDiAccessoResidui == 1 ? "o" : "i");
                    Console.Write("Premere un tasto per proseguire");
                    Console.ReadKey();
                    break;
                case EsitoLogin.UtentePasswordErrati:
                    Console.WriteLine("Utente o password errati");
                    Console.Write("Premere un tasto per proseguire");
                    Console.ReadKey();
                    break;
                case EsitoLogin.AccountBloccato:
                    Console.WriteLine("*** Account utente bloccato ***");
                    Console.Write("Premere un tasto per proseguire");
                    Console.ReadKey();
                    break;
            }
        }

        private Richiesta SchermataDiBenvenuto()
        {
            int scelta = -1;
            if (Banche == null)
                Banche = LoadBanche();

            while (scelta == -1)
            {
                StampaIntestazione("Selezione banca");

                foreach (var banca in Banche)
                {
                    Console.WriteLine($"{banca.Key.ToString()} - {banca.Value.Nome}");
                }
                Console.WriteLine("0 - Uscita");

                scelta = ScegliVoceMenu(0, Banche.Count);
            }

            Richiesta Richiesta = Richiesta.Uscita;
            if (scelta > 0)
            {
                BancaCorrente = Banche[scelta];
                Richiesta = Richiesta.Login;
            }
            return Richiesta;
        }

        private SortedList<int, Banca> LoadBanche()
        {
            List<Banca> elenco = GestoreAccesso.Banche();
            SortedList<int, Banca> ret = new SortedList<int, Banca>();
            for (int i = 0, chiave = 1; i < elenco.Count; i++, chiave++)
                ret.Add(chiave, elenco[i]);
            return ret;
        }
    }

}
