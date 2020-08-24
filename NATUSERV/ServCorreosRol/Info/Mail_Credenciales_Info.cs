using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServCorreosRol.Info
{
    public class Mail_Credenciales_Info
    {
        public int ID { get; set; }
        public string Usuario { get; set; }
        public string Contrasenia { get; set; }
        public string Asunto { get; set; }
        public string Cuerpo { get; set; }
        public int Puerto { get; set; }
        public string host { get; set; }
        public bool PermitirSSL { get; set; }
        public string CorreoCopia { get; set; }
    }
}
