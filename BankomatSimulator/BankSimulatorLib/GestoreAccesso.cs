using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSimulatorLib
{
    public enum EsitoLogin
    {
        AccessoConsentito,
        UtentePasswordErrati,
        PasswordErrata,
        AccountBloccato
    }

    public class GestoreAccesso
    {

        public List<Banca> Banche()
        {
            List<Banca> ret = new List<Banca>();

            using (SqlConnection cn = ConnectionFactory.getInstance())
            {
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = @"select o.Id,o.Nome
                            from Banche o
                            order by Id";
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        ret.Add(new Banca()
                        {
                            Id = rd.GetInt64(0),
                            Nome = rd.GetString(1)
                        });
                    }
                }
            }
            return ret;
        }
        public EsitoLogin Login(Credenziali     credenziali,
                                Banca           banca,
                                out Utente      utente)
        {
        
            utente = dettaglioUtente(banca.Id, credenziali);
            
            if (utente == null)
            { 
                // se non è nel db .....
                return EsitoLogin.UtentePasswordErrati;
            }

            if (utente.Bloccato)
            { 
                // era già bloccato prima
                return EsitoLogin.AccountBloccato;
            }

            if (credenziali.Password != utente.Password)
            {
                utente.TentativiDiAccessoErrati++;
                if (utente.Bloccato)
                {  // non era bloccato lo diventa adesso
                    bloccaUtente(utente.Id);
                    return EsitoLogin.AccountBloccato;
                }
                return EsitoLogin.PasswordErrata;
            }
            
            // Utente valido
            utente.TentativiDiAccessoErrati = 0;
            return EsitoLogin.AccessoConsentito;
        }

        private Utente dettaglioUtente(long idBanca, Credenziali credenziali)
        {
            Utente ret = null;
            using (SqlConnection cn = ConnectionFactory.getInstance())
            {
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText =
                  @"select u.Id, u.IdBanca, u.NomeUtente, u.Password, u.Bloccato
                    from Utenti u
                    where u.IdBanca = @idBanca and
                  u.NomeUtente = @NomeUtente";

                SqlParameter par = cmd.CreateParameter();
                par.DbType = System.Data.DbType.Int64;
                par.ParameterName = "@idBanca";
                par.Value = idBanca;
                cmd.Parameters.Add(par);

                par = cmd.CreateParameter();
                par.DbType = System.Data.DbType.String;
                par.ParameterName = "@NomeUtente";
                par.Size = 50;
                par.Value = credenziali.NomeUtente;
                cmd.Parameters.Add(par);
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    { // la where è un indice univoco
                        ret = new Utente()
                        {
                            Id = rd.GetInt64(0),
                            NomeUtente = rd.GetString(2),
                            Password = rd.GetString(3),
                            Bloccato = rd.GetBoolean(4)
                        };
                    }
                }
            }
            return ret;
        }
        private void bloccaUtente(long id)
        {
            using (SqlConnection cn = ConnectionFactory.getInstance())
            {
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText =
                  @"update Utenti
                    set Bloccato = 1
                    where Id = @Id";

                SqlParameter par = cmd.CreateParameter();
                par.DbType = System.Data.DbType.Int64;
                par.ParameterName = "@Id";
                par.Value = id;
                cmd.Parameters.Add(par);
                cmd.ExecuteNonQuery();
            }
        }

    }
}
