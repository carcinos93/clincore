using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Data;
using log4net;

namespace CLINCORE
{


    #region Conversiones de los XElement
    /// <summary>
    /// Clase estatica para las conversiones de los elemento xml
    /// </summary>
    public static class XmlParse
    {
         private static ILog log = LogManager.GetLogger(typeof(XmlParse));
        public static DateTime? ToDateF(this string valor)
        {
            Regex regex = new Regex(@"^([0-9]{4})[\/-]?([0-9]{2})[\/-]?([0-9]{2})$");
            if (!regex.IsMatch(valor))
                return new DateTime(1900,1,1);
            return DateTime.Parse(regex.Replace(valor, "$1-$2-$3"));
        }

        /// <summary>
        /// Funcion que convierte a entero
        /// </summary>
        /// <param name="element"></param>
        /// <param name="default"></param>
        /// <returns></returns>
        public static int ToInt(this XElement element, Int32? @default = null)
        {
            int i;
            if (Int32.TryParse(element.Value, out i))
            {
                return i;
            }
            if (@default.HasValue)
                return @default.Value;
            return 0;
        }
        /// <summary>
        /// Funcion que convierte el valor a fecha
        /// </summary>
        /// <param name="element">>elemento del xml</param>
        /// <returns></returns>
        public static DateTime ToDate(this XElement element)
        {
            DateTime d = (DateTime.Parse(element.Value));
            return d;
        }
        /// <summary>
        ///  Funcion que convierte el valor a doble
        /// </summary>
        /// <param name="element">>elemento del xml</param>
        /// <returns></returns>
        public static Double ToDouble(this XElement element)
        {
            return (Double.Parse(element.Value));
        }
        /// <summary>
        /// Funcion que retorna el valor del elemento
        /// </summary>
        /// <param name="element">elemento del xml</param>
        /// <returns>String del valor</returns>
        public static String ToStr(this XElement element)
        {
            return (element == null ? "" : element.Value.Trim());
        }

        public static String TryValue(this XElement element, string name)
        {
            try
            {
                return element.Element(element.Name.Namespace + name).Value;
            }
            catch (Exception ex)
            {
                
                throw new Exception(string.Format("Error en el elemento con nombre {0}, excepcion : {1}",name, ex.Message));
            }
        }
        public static object XmlToDictionary(this XElement el)
        {

            if (el.Descendants().Count() > 0)
            {
                //Console.WriteLine(el.Name.LocalName);
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                XmlDocument doc = new XmlDocument();
                string element = el.ToString();
                doc.LoadXml(element);

                string json = JsonConvert.SerializeXmlNode(doc);

                return JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            }
            else
            {
                return el.Value;
            }
        }

        public static string JsonToXml(this string valor)
        {
            try
            {
                XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(valor);
                return doc.OuterXml;
            }
            catch (Exception ex)
            {

                throw new Exception("Error en conversion de json a xml ", ex);
            }
        }

        public static bool IsConnectionValid(this IDbConnection connection)
        {
            try
            {
                var connestring = connection.ConnectionString;
                using (var conn = new SqlConnection(connestring))
                {
                    conn.Open();
                    conn.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Error("Verificar datos de conexion ", ex);
                return false;
            }
        }
    }
    #endregion
}