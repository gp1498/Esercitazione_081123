using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSimulatorLib
{
    public class ContoCorrente
    {
        private double _saldo;
        private long _idContoCorrente;
        private DateTime _dataUltimoVersamento;

        public ContoCorrente()
        {
            DateTime date = DateTime.Now;
            _dataUltimoVersamento = date.AddDays(-1);
        }

        public struct DatiReport
        {
            public double saldo;
            public DateTime dataUltimoVersamento;
            public DateTime dataCorrente;
        }

        public double Saldo { get => _saldo; set => _saldo = value; }

        public long IdContoCorrente { get => _idContoCorrente; set => _idContoCorrente = value; }


        /// <summary>
        /// Versa, nel conto corrente, la quantità indicata
        /// </summary>
        /// <param name="quantita"></param>
        /// <returns></returns>
        public void Versamento(double quantita)
        {
            _saldo += quantita;
            _saldo = Math.Round(_saldo, 4, MidpointRounding.AwayFromZero);
            _dataUltimoVersamento = DateTime.Now;


        }


        /// <summary>
        /// Sottrae dal saldo la quanità indicata.
        /// </summary>
        /// <returns></returns>
        public bool Prelievo(double quantita)
        {
            bool isOk = true;
            _saldo = Math.Round(_saldo, 4, MidpointRounding.AwayFromZero);
            if (_saldo >= quantita)
            {
                _saldo -= quantita;

            }
            else
                isOk = false;

            return isOk;
        }


        /// <summary>
        /// Popola una struttura <see cref="DatiReport"/> con le statistiche
        /// del conto corrente.
        /// </summary>
        /// <returns></returns>
        public DatiReport ReportSaldo()
        {
            DatiReport datiReport = new DatiReport();
            datiReport.saldo = _saldo;
            datiReport.dataUltimoVersamento = _dataUltimoVersamento;
            datiReport.dataCorrente = DateTime.Now;
            return datiReport;
        }
    }
}
