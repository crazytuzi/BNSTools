using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace notify_API.Models
{
    public static class DBHelper
    {
       static string connStr = string.Empty;

       static DBHelper()
       {
           connStr = ConfigurationManager.ConnectionStrings["SQLDBConn"].ConnectionString;
       }

       public static int IDU(string sql,params SqlParameter[] paras)
       {
            SqlConnection conn = new SqlConnection(connStr);
               conn.Open();
               SqlCommand cmd = new SqlCommand(sql,conn);
               if (paras!=null && paras.Length>0)
               {
                   cmd.Parameters.AddRange(paras);
               }
               int x = cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
            return x;
           
       }

       public static SqlDataReader SelectReader(string sql, params SqlParameter[] paras)
       {
           SqlConnection conn = new SqlConnection(connStr);
           conn.Open();
           SqlCommand cmd = new SqlCommand(sql,conn);
           if (paras != null && paras.Length > 0)
           {
               cmd.Parameters.AddRange(paras);
           }
           return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
       }
        public static SqlDataAdapter SelectAdapter(string sql, params SqlParameter[] paras)
        {
            SqlConnection conn = new SqlConnection(connStr);
            conn.Open();
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            DataSet ds = new DataSet();
            da.Fill(ds);
            conn.Close();
            return da;
        }

        public static object SelectScalar(string sql, params SqlParameter[] paras)
        {
            SqlConnection conn = new SqlConnection(connStr);
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            if (paras != null && paras.Length > 0)
            {
                cmd.Parameters.AddRange(paras);
            }
            object x = cmd.ExecuteScalar();
            conn.Close();
            conn.Dispose();
            return x;
        }
    }
}

