using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSimulatorLib
{
    public class Banca
    {
        public long Id { get; internal set; }
        public string Nome { get; internal set; }
        public SortedList<long,Funzionalita> elencoFunzionalita { get; internal set; }
    }
}
