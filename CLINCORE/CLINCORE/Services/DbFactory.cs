using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Text;
using System.Xml.Linq;
using log4net;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CLINCORE
{
    public interface IDbFactory
    {
        IDbConnection Conn();
        void Credentials(dynamic valores, string proveedor);
    }
    public class DbFactory : IDbFactory
    {
        private string _usuario;
        private string _empresa;
        private string _password;

        //private static ILog log = LogManager.GetLogger(typeof(DbFactory));
        public void Credentials(dynamic valores, string proveedor)
        {
            var credencial =  ((JObject)valores);
            _usuario = credencial.SelectToken("credencial.usuario").ToString();
            _password = credencial.SelectToken("credencial.password").ToString();
            _empresa = proveedor;
        }

        public IDbConnection Conn()
        {


            var connectionString = @"server={3};database={0};uid={1};password={2};";

           
            

            ///HttpContext context = HttpContext.Current;
            /*var usuario = "sa";
            var empresa = "CLIN";
            var password = "P@ssw0rd"*/;
            var ip = "";
           

            ip = @"CLINDEV17"; //parametros["IP"];
            var connection = new SqlConnection(
                string.Format(connectionString, _empresa, _usuario, _password, ip)
                );
            return connection;
        }

       /*- public static string MensajeSistema(string codigo)
        {
            string msj = "";
            try
            {
                using (var cn = Conn())
                {
                    cn.Open();
                    msj = cn.ExecuteScalar<string>("SELECT [dbo].[MensajeAlerta](@CODIGO_MENSAJE)",
                        new { CODIGO_MENSAJE = codigo });
                }
                return msj;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }*/

       
    }
}