using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using ServCorreosRol.Info;
using System.Collections.Generic;
using System.Linq;
using ServCorreosRol.Database;

namespace ServCorreosRol.Reports
{
    public partial class Rol01_Rpt : DevExpress.XtraReports.UI.XtraReport
    {
        public Reporte01_Info info { get; set; }
        public List<Reporte01Det_Info> Lista { get; set; }
        Conexiones db = new Conexiones();
        public Rol01_Rpt()
        {
            InitializeComponent();
        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            try
            {
                XtraReport subreport = ((XRSubreport)sender).ReportSource;
                ((Rol01_SubRpt)subreport).Tipo = "INGRESOS";
                ((Rol01_SubRpt)subreport).Lista = Lista.Where(q => q.Signo == "+").ToList();
            }
            catch (Exception ex)
            {
                
            }
        }

        private void xrSubreport2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            try
            {
                XtraReport subreport = ((XRSubreport)sender).ReportSource;
                ((Rol01_SubRpt)subreport).Tipo = "EGRESOS";
                ((Rol01_SubRpt)subreport).Lista = Lista.Where(q => q.Signo == "-").ToList();
            }
            catch (Exception ex)
            {
                
            }
        }

        private void Rol01_Rpt_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            try
            {
                lblPeriodo.Text = "PERIODO: " + info.cro_fecini.ToString("dd/MMM/yyyy").ToUpper() + " - " + info.cro_fecfin.ToString("dd/MMM/yyyy").ToUpper();
                info.ListaDet = db.GetList(info.pre_empresa, info.pre_rol, info.pre_codtra);
                Lista = info.ListaDet;
                List<Reporte01_Info> ListaRpt = new List<Reporte01_Info>();
                ListaRpt.Add(info);
                this.DataSource = ListaRpt;
            }
            catch (Exception ex)
            {
                
            }
            
        }
    }
}
