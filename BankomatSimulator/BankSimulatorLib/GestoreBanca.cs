using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static BankSimulatorLib.ContoCorrente;

namespace BankSimulatorLib
{
    public enum Funzionalita
    {
        Report_Saldo = 1 ,
	    Versamento,
        Prelievo,   
        Registro_Operazioni
    }

    public class GestoreBanca
    {

        public Banca dettaglio(long id)
        {
            Banca banca = null;
            using (SqlConnection cn = ConnectionFactory.getInstance())
            {
                banca = loadBanca(id, cn);
                if (banca != null)
                {
                    banca.elencoFunzionalita = loadFunzionalita(id, cn);
                }
            }
            return banca;
        }

        private ContoCorrente loadContoCorrente(long idUtente, SqlConnection cn)
        {
            ContoCorrente contoCorrente = null;
            SqlCommand cmd = cn.CreateCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText =
              @"SELECT cc.Id , cc.Saldo  from ContiCorrente cc where IdUtente=@idUtente";

            SqlParameter par = cmd.CreateParameter();
            par.DbType = System.Data.DbType.Int64;
            par.ParameterName = "@idUtente";
            par.Value = idUtente;
            cmd.Parameters.Add(par);
            using (SqlDataReader rd = cmd.ExecuteReader())
            {
                if (rd.Read())
                {
                    contoCorrente = new ContoCorrente()
                    {
                        IdContoCorrente = rd.GetInt64(0),
                        Saldo = rd.GetInt32(1)                        
                    };
                }
            }
            return contoCorrente;
        }


        /*  {o}
        public DatiReport ReportSaturazione(OperatoreLogistico operatoreLogistico)
        {
            using (SqlConnection cn = ConnectionFactory.getInstance())
            {
                operatoreLogistico.contoCorrente = loadMagazzino(operatoreLogistico.Id, cn);
            }
            return operatoreLogistico.contoCorrente.datiReport;
        }
        */

        public int SaldoUtente(long idUtente)
        {
            using (SqlConnection cn = ConnectionFactory.getInstance())
            {
                var contoCorrente = loadContoCorrente(idUtente, cn);
                return (int)contoCorrente.Saldo;
            }
        }

            public bool Versamento(int importo, long idUtente)
        {
            bool ret = false;
            using (SqlConnection cn = ConnectionFactory.getInstance())
            {
                var contoCorrente = loadContoCorrente(idUtente, cn);
                contoCorrente.Versamento(importo);

                using (SqlTransaction tx = cn.BeginTransaction())
                {
                    //  Aggiorna il saldo 
                    SqlCommand cmd = cn.CreateCommand();
                    cmd.Transaction = tx;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = @"update ContiCorrente set Saldo=@saldo where IdUtente=@idUtente";
                    SqlParameter par = cmd.CreateParameter();
                    par.DbType = System.Data.DbType.Int64;
                    par.ParameterName = "@idUtente";
                    par.Value = idUtente;
                    cmd.Parameters.Add(par);
                    par = cmd.CreateParameter();
                    par.DbType = System.Data.DbType.Int32;
                    par.ParameterName = "@saldo";
                    par.Value = contoCorrente.Saldo;
                    cmd.Parameters.Add(par);
                    cmd.ExecuteNonQuery();

                    //  Inserire un movimento 

                        //  {f} 

                    tx.Commit();
                    
                    ret = true;
                }
            }
            return ret;
        }

        public bool Prelievo(int importo, long idUtente)
        {
            bool ret = false;
            using (SqlConnection cn = ConnectionFactory.getInstance())
            {
                var contoCorrente = loadContoCorrente(idUtente, cn);
                if( contoCorrente.Prelievo(importo) == false )
                {
                    return false;
                }

                using (SqlTransaction tx = cn.BeginTransaction())
                {
                    //  Aggiorna il saldo 
                    SqlCommand cmd = cn.CreateCommand();
                    cmd.Transaction = tx;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = @"update ContiCorrente set Saldo=@saldo where IdUtente=@idUtente";
                    SqlParameter par = cmd.CreateParameter();
                    par.DbType = System.Data.DbType.Int64;
                    par.ParameterName = "@idUtente";
                    par.Value = idUtente;
                    cmd.Parameters.Add(par);
                    par = cmd.CreateParameter();
                    par.DbType = System.Data.DbType.Int32;
                    par.ParameterName = "@saldo";
                    par.Value = contoCorrente.Saldo;
                    cmd.Parameters.Add(par);
                    cmd.ExecuteNonQuery();

                    //  Inserire un movimento 

                    //  {f} 

                    tx.Commit();

                    ret = true;

                }
            }
            return ret;
        }

        /*    
        private bool VerificaGiacenzaBancali(int numeroBancaliDaPrelevare, OperatoreLogistico operatoreLogistico)
        {
            return operatoreLogistico.contoCorrente.bancali >= numeroBancaliDaPrelevare;
        }

        private List<Bancale> GeneraListaBancaliDaStoccare(int quantita)
        {
            List<Bancale> listaPallet = new List<Bancale>();
            for (int i = 0; i < quantita; i++)
            {
                listaPallet.Add(new Bancale
                {
                    Identificativo = Guid.NewGuid().ToString(),
                    DataCreazione = DateTime.Now
                });
            }
            return listaPallet;
        }
        private static bool VerificaDisponibilitaBancale(int numeroBancaliDaStoccare, OperatoreLogistico operatoreLogistico)
        {
            return operatoreLogistico.contoCorrente.bancali + numeroBancaliDaStoccare <= operatoreLogistico.contoCorrente.capacita;
        }
        */

        private SortedList<long, Funzionalita> loadFunzionalita(long id,SqlConnection cn)
        {
            SortedList<long, Funzionalita> elenco = new SortedList<long, Funzionalita>();
            SqlCommand cmd = cn.CreateCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText =
              @"select opf.IdBanca, opf.IdFunzionalita, f.Nome
            from Banche_Funzionalita opf inner join
                  Funzionalita f on f.Id = opf.IdFunzionalita
            where opf.IdBanca = @idBanca
            order by opf.IdFunzionalita";

            SqlParameter par = cmd.CreateParameter();
            par.DbType = System.Data.DbType.Int64;
            par.ParameterName = "@idBanca";
            par.Value = id;
            cmd.Parameters.Add(par);
            using (SqlDataReader rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    elenco.Add(rd.GetInt64(1), (Funzionalita)rd.GetInt64(1));
                }
            }
            return elenco;
        }

        private static Banca loadBanca(long id,SqlConnection cn)
        {
            Banca ret = null;
            SqlCommand cmd = cn.CreateCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText =
              @"select o.Id, o.Nome
            from Banche o
            where o.Id = @Id";

            SqlParameter par = cmd.CreateParameter();
            par.DbType = System.Data.DbType.Int64;
            par.ParameterName = "@Id";
            par.Value = id;
            cmd.Parameters.Add(par);
            using (SqlDataReader rd = cmd.ExecuteReader())
            {
                if (rd.Read())
                {
                    ret = new Banca()
                    {
                        Id = rd.GetInt64(0),
                        Nome = rd.GetString(1)
                    };
                }
            }
            return ret;
        }
    }
}
