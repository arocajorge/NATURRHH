using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServCorreosRol.Database;
using System.Net.Mail;
using System.Net;
using ServCorreosRol.Info;
using System.IO;
using ServCorreosRol.Reports;

namespace TestServicios
{
    class Program
    {
        static void Main(string[] args)
        {
            Conexiones con = new Conexiones();
            string Asunto = string.Empty;
            string Cuerpo = string.Empty;
            string ListaCorreosEnviados = string.Empty;
            try
            {
                var Lista = con.GetCabecerasSinProcesar();
                if (Lista.Count == 0)
                {
                    Lista = con.GetCabecerasProcesadasAmedias();
                }
                var Credenciales = con.GetInfoCredenciales();

                if (Credenciales.ID == 0)
                    return;

                

                foreach (var Rol in Lista)
                {
                    ListaCorreosEnviados = string.Empty;
                    foreach (var Emp in Rol.Lista)
                    {
                        MemoryStream mem = new MemoryStream();
                        try
                        {
                            Asunto = Credenciales.Asunto;
                            Asunto = Asunto.Replace("[Mes]", Emp.infoRpt.cro_fecini.ToString("MMMM"));
                            Asunto = Asunto.Replace("[Anio]", Emp.infoRpt.cro_fecini.ToString("yyyy"));
                            Asunto = Asunto.Replace("[NombreEmpleado]", Emp.infoRpt.NOM);

                            Cuerpo = Credenciales.Cuerpo;
                            Cuerpo = Cuerpo.Replace("[Mes]", Emp.infoRpt.cro_fecini.ToString("MMMM"));
                            Cuerpo = Cuerpo.Replace("[Anio]", Emp.infoRpt.cro_fecini.ToString("yyyy"));
                            Cuerpo = Cuerpo.Replace("[NombreEmpleado]", Emp.infoRpt.NOM);

                            #region Armar cuerpo del correo correo
                            MailMessage mail = new MailMessage();
                            mail.From = new MailAddress(Credenciales.Usuario);//Correo de envio
                            mail.Subject = Asunto;
                            if (!string.IsNullOrEmpty(Emp.trb_email == null ? "" : Emp.trb_email.Trim()))
                            {
                                string Correo = new string(Emp.trb_email.Trim().Where(c => !char.IsControl(c)).ToArray());
                                mail.To.Add(Correo);
                            }
                            else
                                mail.To.Add(Credenciales.CorreoCopia);

                            string Body = Cuerpo;


                            Rol01_Rpt rpt = new Rol01_Rpt();
                            rpt.info = Emp.infoRpt;
                            rpt.ExportToPdf(mem);

                            // Create a new attachment and put the PDF report into it.
                            mem.Seek(0, System.IO.SeekOrigin.Begin);
                            Attachment att = new Attachment(mem, "ROL" + Emp.infoRpt.cro_fecfin.ToString("yyyyMM") + ".pdf", "application/pdf");
                            mail.Attachments.Add(att);

                            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(Body, null, "text/html");
                            mail.AlternateViews.Add(htmlView);
                            #endregion

                            #region smtp
                            SmtpClient smtp = new SmtpClient();
                            smtp.UseDefaultCredentials = false;
                            smtp.Host = Credenciales.host;
                            smtp.EnableSsl = Credenciales.PermitirSSL;
                            smtp.Port = Credenciales.Puerto;
                            smtp.Credentials = new NetworkCredential(Credenciales.Usuario, Credenciales.Contrasenia);
                            smtp.Send(mail);
                            #endregion

                            ListaCorreosEnviados += "OK " + Emp.infoRpt.NOM.Trim() + " " + (Emp.trb_email ?? "").Trim()+"</br>";


                            Console.WriteLine("OK " + Emp.infoRpt.NOM.Trim() + " " + (Emp.trb_email ?? "").Trim());
                        }
                        catch (Exception ex)
                        {
                            con.GuardarExcepcion(new Mail_LogError_Info
                            {
                                Metodo = "EnviarCorreoSMTP",
                                Error = ex == null ? string.Empty : (ex.Message ?? string.Empty),
                                InnerException = ex == null || ex.InnerException == null ? string.Empty : (ex.InnerException.Message.Length > 1000 ? ex.InnerException.Message.Substring(0, 1000) : ex.InnerException.Message)
                            });
                            Emp.Error = ex.Message;
                            ListaCorreosEnviados += "ERROR " + Emp.infoRpt.NOM.Trim() + " " + (Emp.trb_email ?? "").Trim() + "</br>";
                            Console.WriteLine("ERROR " + Emp.infoRpt.NOM.Trim() + " " + (Emp.trb_email ?? "").Trim());

                            try
                            {

                                MemoryStream memError = new MemoryStream();

                                #region Armar cuerpo del correo correo
                                MailMessage mail = new MailMessage();
                                mail.From = new MailAddress(Credenciales.Usuario);//Correo de envio
                                mail.Subject = Asunto;
                                mail.To.Add(Credenciales.CorreoCopia);

                                string Body = Cuerpo;


                                Rol01_Rpt rpt = new Rol01_Rpt();
                                rpt.info = Emp.infoRpt;
                                rpt.ExportToPdf(memError);

                                // Create a new attachment and put the PDF report into it.
                                memError.Seek(0, System.IO.SeekOrigin.Begin);
                                Attachment att = new Attachment(memError, "ROL" + Emp.infoRpt.cro_fecfin.ToString("yyyyMM") + ".pdf", "application/pdf");
                                mail.Attachments.Add(att);

                                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(Body, null, "text/html");
                                mail.AlternateViews.Add(htmlView);
                                #endregion

                                #region smtp
                                SmtpClient smtp = new SmtpClient();
                                smtp.UseDefaultCredentials = false;
                                smtp.Host = Credenciales.host;
                                smtp.EnableSsl = Credenciales.PermitirSSL;
                                smtp.Port = Credenciales.Puerto;
                                smtp.Credentials = new NetworkCredential(Credenciales.Usuario, Credenciales.Contrasenia);
                                smtp.Send(mail);
                                #endregion
                            }
                            catch (Exception er)
                            {
                                con.GuardarExcepcion(new Mail_LogError_Info
                                {
                                    Metodo = "EnviarCorreoSMTP",
                                    Error = er == null ? string.Empty : (er.Message ?? string.Empty),
                                    InnerException = er == null || er.InnerException == null ? string.Empty : (er.InnerException.Message.Length > 1000 ? er.InnerException.Message.Substring(0, 1000) : er.InnerException.Message)
                                });
                            }
                        }
                        mem.Close();
                        mem.Flush();
                        con.ModificarDet(Emp);
                        con.Modificar(Rol);

                    }

                    try
                    {
                        #region Armar cuerpo del correo correo
                        MailMessage mail = new MailMessage();
                        mail.From = new MailAddress(Credenciales.Usuario);//Correo de envio
                        mail.Subject = "RESUMEN CORREOS ENVIADOS";
                        mail.To.Add(Credenciales.CorreoCopia);

                        string Body = ListaCorreosEnviados;
                        
                        #endregion

                        #region smtp
                        SmtpClient smtp = new SmtpClient();
                        smtp.UseDefaultCredentials = false;
                        smtp.Host = Credenciales.host;
                        smtp.EnableSsl = Credenciales.PermitirSSL;
                        smtp.Port = Credenciales.Puerto;
                        smtp.Credentials = new NetworkCredential(Credenciales.Usuario, Credenciales.Contrasenia);
                        smtp.Send(mail);
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
            }
            catch (Exception ex)
            {
                con.GuardarExcepcion(new Mail_LogError_Info
                {
                    Metodo = "EnviarCorreo",
                    Error = ex == null ? string.Empty : (ex.Message ?? string.Empty),
                    InnerException = ex == null || ex.InnerException == null ? string.Empty : (ex.InnerException.Message.Length > 1000 ? ex.InnerException.Message.Substring(0, 1000) : ex.InnerException.Message)
                });
            }
        }
        
    }
}
