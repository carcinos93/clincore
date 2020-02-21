using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLINCORE.Models
{
    public class Datos
    {
        public Credencial credencial { get; set; }
    }

    public class Credencial
    {
        public string usuario { get; set; }
        public string password { get; set; }
    }
    public class Respuesta
    {
        public string message { get; set; }
        public bool error { get; set; }
        public object content { get; set; }
    }

    public class validacion
    {
        public int CORRELATIVO { get; set; }
        public string MENSAJE { get; set; }
    }
    public class Servicio
    {
        public IList<Proceso> PROCESO { get; set; }
        public class Proceso
        {
            public string METODO { get; set; }
            public string PROCEDIMIENTO { get; set; }
            public IList<string> JSON_MAP { get; set; }
            public IList<PARAMETRO> PARAMETROS { get; set; }
            public IList<PARAMETRO> OUTPUT { get; set; }
            public class PARAMETRO
            {
                public string NOMBRE { get; set; }
                public string TIPO { get; set; }
                public int LONGITUD { get; set; }
                public string RUTA { get; set; }
            }


        }
    }

  



}