using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;
using Dapper;
using Newtonsoft.Json.Linq;

namespace CLINCORE.Controllers
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    [Route("api/{proveedor}/{metodo}")]
    [ApiController]
    public class CLINController : ControllerBase
    {
        private readonly IHostingEnvironment _env;
        private readonly IDbFactory _dbFactory;
        public CLINController (IHostingEnvironment env, IDbFactory dbFactory)
        {
            _env = env;
            _dbFactory = dbFactory;

        }
     
        [HttpPost]
        public Respuesta procesar(string proveedor, string metodo, [FromBody] dynamic valores)
        {
            _dbFactory.Credentials(valores, proveedor);
            Dapper.SqlMapper.Settings.UseSingleResultOptimization = false;
            Dapper.SqlMapper.Settings.UseSingleRowOptimization = false;
            try
            {
                if (!System.IO.File.Exists(_env.ContentRootPath + "/config/" + proveedor.ToUpper() + ".json"))
                {
                    return new Respuesta { content = null, error = true, message = string.Format("proveedor no valido '{0}'", proveedor) };

                }
                var json = System.IO.File.ReadAllText((_env.ContentRootPath + "/config/" + proveedor.ToUpper() + ".json"));
                Models.Servicio servicio = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Servicio>(json);
                var proceso = servicio.PROCESO.FirstOrDefault((e) => e.METODO == metodo);

                if (proceso == null)
                {
                    return new Respuesta { error = true, message = string.Format("El metodo '{0}' no es valido", metodo) };

                }

                JObject o = (JObject)valores;
                string procedimiento = proceso.PROCEDIMIENTO;

                DynamicParameters parameters = new DynamicParameters();
                ///parameters.Add("PROVEEDOR", proveedor, System.Data.DbType.String);
                foreach (var item in proceso.PARAMETROS)
                {
                    var name = item.NOMBRE;
                    var length = item.LONGITUD;
                    var path = item.RUTA;
                    var tipo = item.TIPO;

                    System.Data.DbType dbType = System.Data.DbType.String;


   
                    object value = null;
                    if (!string.IsNullOrEmpty(path))
                        value = (string)o.SelectToken(path);

                    if (tipo == "$PARAMETROS")
                        value = Newtonsoft.Json.JsonConvert.SerializeObject(valores);


                    if (tipo == "nulo")
                        value = null;

                    if (tipo == "decimal")
                        dbType = System.Data.DbType.Decimal;

                    if (tipo == "int")
                        dbType = System.Data.DbType.Int32;

                    if (tipo == "bool")
                        dbType = System.Data.DbType.Boolean;


                    parameters.Add(name, value, dbType, System.Data.ParameterDirection.Input, length);

                }


                foreach ( var item in proceso.OUTPUT)
                {
                    var name = item.NOMBRE;
                    var length = item.LONGITUD;
                    var path = item.RUTA;
                    var tipo = item.TIPO;

                    System.Data.DbType dbType = System.Data.DbType.String;


                    if (tipo == "decimal")
                        dbType = System.Data.DbType.Decimal;

                    if (tipo == "int")
                        dbType = System.Data.DbType.Int32;

                    if (tipo == "bool")
                        dbType = System.Data.DbType.Boolean;


                    parameters.Add(name, null, dbType, System.Data.ParameterDirection.Output, length);

                }

                Dictionary<string, dynamic> values = new Dictionary<string, dynamic>();
                using (var conn = _dbFactory.Conn())
                {


                    var multi = conn.Query(procedimiento, parameters, commandType: System.Data.CommandType.StoredProcedure);
                    int i = 0;

                    foreach (var item in proceso.OUTPUT)
                    {
                        var result = parameters.Get<object>(item.NOMBRE);
                        values.Add(item.RUTA, result);
                    }

                   

                    foreach (var item in proceso.JSON_MAP)
                    {
                        object data = multi;
                        
                       /* if (!multi.IsConsumed)
                        {
                            data = multi.ReadFirstOrDefault<dynamic>();
                        }*/
                        
                 
                        if (item == "root")
                        {
                            var fields = data as IDictionary<string, object>;
                            foreach (var f in fields)
                            {
                                values.Add(f.Key, f.Value);
                            }

                        }
                        else
                        {
                            values.Add(item, data);
                        }

                        i++;
                    }

                }

                return new Respuesta { content = values, error = false };
            }

            catch (Exception ex)
            {
               
                return new Respuesta { content = null, error = true, message = ex.StackTrace };
            }

        }
    

    }
}