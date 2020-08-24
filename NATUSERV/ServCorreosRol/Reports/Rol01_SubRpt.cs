using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using ServCorreosRol.Info;
using System.Collections.Generic;

namespace ServCorreosRol.Reports
{
    public partial class Rol01_SubRpt : DevExpress.XtraReports.UI.XtraReport
    {
        public List<Reporte01Det_Info> Lista { get; set; }
        public string Tipo { get; set; }
        public Rol01_SubRpt()
        {
            InitializeComponent();
        }

        private void Rol01_Ing_Rpt_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            try
            {
                lblTipo.Text = Tipo;
                this.DataSource = Lista;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
