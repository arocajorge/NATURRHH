using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServCorreosRol.Info
{
    public class Mail_RolesProcesados_Info
    {
        public decimal ID { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int TotalEmpleados { get; set; }
        public int TotalProcesados { get; set; }

        #region Campos que no existen en la tabla
        public int IdEmpresa { get; set; }
        public int IdRol { get; set; }
        public List<Mail_RolesProcesadosDet_Info> Lista { get; set; }
        #endregion

    }
}
