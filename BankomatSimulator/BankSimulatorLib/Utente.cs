using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSimulatorLib
{
    public class Utente
    {
        private int _tentativiDiAccessoErrati = 0;
        private bool _bloccato = false;
        private const int _tentativiDiAccessoPermessi = 3;
        public string NomeUtente { get; internal set; }
        public string Password { get; internal set; }
        public bool Bloccato { get => _bloccato; internal set => _bloccato = value; }
        public long Id { get; internal set; }

        public int TentativiDiAccessoResidui
        {
            get
            {
                return _tentativiDiAccessoPermessi - _tentativiDiAccessoErrati;
            }
        }

        public int TentativiDiAccessoErrati
        {
            get => _tentativiDiAccessoErrati;
            internal set
            {
                _tentativiDiAccessoErrati = value;
                if (_tentativiDiAccessoErrati >= _tentativiDiAccessoPermessi)
                {
                    _bloccato = true;
                }
            }
        }
    }
}
