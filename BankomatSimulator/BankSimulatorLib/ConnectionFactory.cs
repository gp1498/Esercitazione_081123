using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;


namespace BankSimulatorLib
{
    internal class ConnectionFactory
    {
        internal static SqlConnection getInstance()
        {
            SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
            cn.Open();
            return cn;
        }
    }
}

