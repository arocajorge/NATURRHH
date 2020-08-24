using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServCorreosRol.Info
{
    public class Mail_RolesProcesadosDet_Info
    {
        public decimal ID { get; set; }
        public int Secuencia { get; set; }
        public int pre_empre { get; set; }
        public int pre_codtra { get; set; }
        public string trb_email { get; set; }
        public DateTime FechaEnvio { get; set; }
        public string Error { get; set; }

        #region Campos que no existen en la tabla
        public Reporte01_Info infoRpt { get; set; }
        #endregion
    }
}
