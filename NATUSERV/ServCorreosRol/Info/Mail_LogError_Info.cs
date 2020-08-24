using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServCorreosRol.Info
{
    public class Mail_LogError_Info
    {
        public decimal ID { get; set; }
        public string Metodo { get; set; }
        public DateTime Fecha { get; set; }
        public string Error { get; set; }
        public string InnerException { get; set; }
    }
}
