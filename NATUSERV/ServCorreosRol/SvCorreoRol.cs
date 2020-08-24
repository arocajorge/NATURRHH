using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ServCorreosRol
{
    partial class SvCorreoRol : ServiceBase
    {
        protected override void OnStart(string[] args)
        {
            Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = TimeSpan.FromHours(12).TotalMilliseconds;
            aTimer.Enabled = true;
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            Serv_EnvioCorreosRol();
        }

        protected override void OnStop()
        {
        }

        public void Serv_EnvioCorreosRol()
        {
            try
            {

            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
