using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServCorreosRol.Info
{
    public class Reporte01_Info
    {
        public int pre_empresa { get; set; }
        public int pre_tiprol { get; set; }
        public int pre_codtra { get; set; }
        public int pre_rol { get; set; }
        public DateTime cro_fecini { get; set; }
        public DateTime cro_fecfin { get; set; }
        public string NOM { get; set; }
        public string trb_div { get; set; }
        public string trb_are { get; set; }
        public string trb_cen { get; set; }
        public string sub_nombre { get; set; }
        public string cgo_descri { get; set; }
        public double ING { get; set; }
        public double EGR { get; set; }
        public string Origen { get; set; }
        public string trb_email { get; set; }
        public string emp_nombre { get; set; }

        #region Campos que no existen en el procedure
        public List<Reporte01Det_Info> ListaDet { get; set; }
        public double Saldo { get; internal set; }
        #endregion
    }
}
