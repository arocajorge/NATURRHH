using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServCorreosRol.Info;
using System.Data;

namespace ServCorreosRol.Database
{
    public class Conexiones
    {
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
        
        public Conexiones()
        {
            builder.DataSource = "192.168.50.5";
            builder.UserID = "sa";
            builder.Password = "";
            builder.InitialCatalog = "rrhh";
            builder.IntegratedSecurity = false;
            
        }

        public List<Mail_RolesProcesados_Info> GetCabecerasSinProcesar()
        {
            try
            {
                #region Querys
                string querySelect =
            "select cro_numero,cro_empre,cro_fecha,cro_usuari,cro_fecini,cro_fecfin,cro_tiprol,cro_observ,cro_tipo,cro_liq_motivo,cro_liq_refere,cro_contab,ID "
             + "from tb_Cabrol A "
             + "where not exists( "
             + "select b.ID from Mail_RolesProcesados B "
             + "where A.ID = B.ID "
             + ")and a.cro_tiprol = 7 and a.cro_tipo = 'NOM'";

                string queryInsert =
            "INSERT INTO [dbo].[Mail_RolesProcesados] "
            + "([ID],[FechaInicio],[TotalEmpleados],[TotalProcesados]) "
            + "VALUES(@ID, @FechaInicio, @TotalEmpleados, @TotalProcesados)";

                string queryInsertDet =
            "INSERT INTO [dbo].[Mail_RolesProcesadosDet] "
            +"([ID],[Secuencia],[pre_empre],[pre_codtra],[trb_email]) "
            +"VALUES(@ID, @Secuencia, @pre_empre, @pre_codtra, @trb_email)";
                #endregion

                #region Variables
                List<Mail_RolesProcesados_Info> Lista = new List<Mail_RolesProcesados_Info>();
                #endregion

                using (SqlConnection connection = new SqlConnection(builder.ToString()))
                {
                    connection.Open();

                    #region Select
                    SqlCommand commandSelect = new SqlCommand(querySelect, connection);                    
                    SqlDataReader reader = commandSelect.ExecuteReader();
                    while (reader.Read())
                    {
                        Lista.Add(new Mail_RolesProcesados_Info
                        {
                            ID = Convert.ToDecimal(reader[12]),
                            FechaInicio = DateTime.Now,
                            TotalEmpleados = 0,
                            TotalProcesados = 0,
                            IdEmpresa = Convert.ToInt32(reader[1]),
                            IdRol = Convert.ToInt32(reader[0])
                        });
                    }
                    reader.Close();
                    #endregion

                    #region Insert
                    foreach (var item in Lista)
                    {
                        #region Procedure
                        string queryProcedure = "EXEC [rrhh].[dbo].[rep_sob_lista] {0}, 7, {1}, 'D', 1";
                        queryProcedure = queryProcedure.Replace("{0}", item.IdEmpresa.ToString());
                        queryProcedure = queryProcedure.Replace("{1}", item.IdRol.ToString());
                        SqlCommand commandProcedure = new SqlCommand(queryProcedure, connection);
                        
                        SqlDataReader readerProcedure = commandProcedure.ExecuteReader();                        
                        item.Lista = new List<Mail_RolesProcesadosDet_Info>();
                        int Secuencia = 1;
                        while (readerProcedure.Read())
                        {
                            Mail_RolesProcesadosDet_Info det = new Mail_RolesProcesadosDet_Info
                            {
                                ID = item.ID,
                                Secuencia = Secuencia++,
                                pre_empre = Convert.ToInt32(readerProcedure[0]),
                                pre_codtra = Convert.ToInt32(readerProcedure[2]),
                                trb_email = Convert.ToString(readerProcedure[15]),
                                infoRpt = new Reporte01_Info
                                {
                                    pre_empresa = Convert.ToInt32(readerProcedure[0]),
                                    pre_codtra = Convert.ToInt32(readerProcedure[2]),
                                    pre_rol = Convert.ToInt32(readerProcedure[3]),
                                    trb_email = Convert.ToString(readerProcedure[15]),
                                    emp_nombre = Convert.ToString(readerProcedure[16]),
                                    cgo_descri = Convert.ToString(readerProcedure[11]),
                                    sub_nombre = Convert.ToString(readerProcedure[10]),
                                    trb_cen = Convert.ToString(readerProcedure[9]),
                                    trb_are = Convert.ToString(readerProcedure[8]),
                                    trb_div = Convert.ToString(readerProcedure[7]),
                                    NOM = Convert.ToString(readerProcedure[6]),
                                    cro_fecini = Convert.ToDateTime(readerProcedure[4]),
                                    cro_fecfin = Convert.ToDateTime(readerProcedure[5]),
                                    ING = Convert.ToDouble(readerProcedure[12]),
                                    EGR = Math.Abs(Convert.ToDouble(readerProcedure[13]))
                                }
                            };
                            det.infoRpt.Saldo = det.infoRpt.ING - det.infoRpt.EGR;
                            item.Lista.Add(det);
                        }
                        item.TotalEmpleados = item.Lista.Count;
                        readerProcedure.Close();
                        #endregion

                        SqlCommand commandInsert = new SqlCommand(queryInsert, connection);
                        commandInsert.Parameters.Add("@ID", SqlDbType.Decimal).Value = item.ID;
                        commandInsert.Parameters.Add("@FechaInicio", SqlDbType.DateTime).Value = item.FechaInicio;
                        commandInsert.Parameters.Add("@TotalEmpleados", SqlDbType.Int).Value = item.TotalEmpleados;
                        commandInsert.Parameters.Add("@TotalProcesados", SqlDbType.Int).Value = item.TotalProcesados;
                        commandInsert.ExecuteNonQuery();


                        #region Insert detalle
                        foreach (var itemDet in item.Lista)
                        {
                            SqlCommand commandInsertDet = new SqlCommand(queryInsertDet, connection);
                            commandInsertDet.Parameters.Add("@ID", SqlDbType.Decimal).Value = itemDet.ID;
                            commandInsertDet.Parameters.Add("@Secuencia", SqlDbType.Int).Value = itemDet.Secuencia;
                            commandInsertDet.Parameters.Add("@pre_empre", SqlDbType.Int).Value = itemDet.pre_empre;
                            commandInsertDet.Parameters.Add("@pre_codtra", SqlDbType.Int).Value = itemDet.pre_codtra;
                            commandInsertDet.Parameters.Add("@trb_email", SqlDbType.Text).Value = itemDet.trb_email;
                            commandInsertDet.ExecuteNonQuery();
                        }
                        #endregion
                    }
                    #endregion

                    connection.Close();
                }

                return Lista;
            }
            catch (Exception ex)
            {
                GuardarExcepcion(new Mail_LogError_Info
                {
                    Metodo = "GetCabecerasSinProcesar",
                    Error = ex == null ? string.Empty : (ex.Message ?? string.Empty),
                    InnerException = ex == null || ex.InnerException == null ? string.Empty : (ex.InnerException.Message.Length > 1000 ? ex.InnerException.Message.Substring(0, 1000) : ex.InnerException.Message)
                });
                return new List<Mail_RolesProcesados_Info>();
            }
        }

        public List<Mail_RolesProcesados_Info> GetCabecerasProcesadasAmedias()
        {
            try
            {
                List<Mail_RolesProcesados_Info> Lista = new List<Mail_RolesProcesados_Info>();


                string querySelect =
            "select cro_numero,cro_empre,a.ID,FechaInicio,FechaFin,TotalEmpleados,TotalProcesados from mail_rolesprocesados a inner join "
            + "tb_cabrol as b on a.ID = b.ID "
            + "where totalEmpleados != TotalProcesados and datediff(hour, FechaInicio, getdate()) > 24";


                using (SqlConnection connection = new SqlConnection(builder.ToString()))
                {
                    connection.Open();

                    #region Select
                    SqlCommand commandSelect = new SqlCommand(querySelect, connection);
                    SqlDataReader reader = commandSelect.ExecuteReader();
                    while (reader.Read())
                    {
                        Lista.Add(new Mail_RolesProcesados_Info
                        {
                            ID = Convert.ToDecimal(reader[2]),
                            FechaInicio = DateTime.Now,
                            TotalEmpleados = Convert.ToInt32(reader[5]),
                            TotalProcesados = Convert.ToInt32(reader[6]),
                            IdEmpresa = Convert.ToInt32(reader[1]),
                            IdRol = Convert.ToInt32(reader[0])
                        });
                    }
                    reader.Close();
                    #endregion
                    foreach (var item in Lista)
                    {
                        string querySelectDet =
                    "select pre_empre,pre_codtra,trb_email,FechaEnvio,Error from mail_RolesProcesadosDet "
                    + "where ID = " + item.ID.ToString() + " and FechaEnvio is not null";

                        #region SelectDet
                        List<Mail_RolesProcesadosDet_Info> ListaDet = new List<Mail_RolesProcesadosDet_Info>();
                        SqlCommand commandSelectDet = new SqlCommand(querySelectDet, connection);
                        SqlDataReader readerDet = commandSelectDet.ExecuteReader();
                        while (readerDet.Read())
                        {
                            var det = new Mail_RolesProcesadosDet_Info
                            {
                                pre_codtra = Convert.ToInt32(readerDet[1]),
                                pre_empre = Convert.ToInt32(readerDet[0]),
                            };
                            ListaDet.Add(det);
                        }
                        readerDet.Close();
                        #endregion

                        #region Procedure
                        string queryProcedure = "EXEC [rrhh].[dbo].[rep_sob_lista] {0}, 7, {1}, 'D', 1";
                        queryProcedure = queryProcedure.Replace("{0}", item.IdEmpresa.ToString());
                        queryProcedure = queryProcedure.Replace("{1}", item.IdRol.ToString());
                        SqlCommand commandProcedure = new SqlCommand(queryProcedure, connection);

                        SqlDataReader readerProcedure = commandProcedure.ExecuteReader();
                        item.Lista = new List<Mail_RolesProcesadosDet_Info>();
                        int Secuencia = 1;
                        while (readerProcedure.Read())
                        {
                            Mail_RolesProcesadosDet_Info det = new Mail_RolesProcesadosDet_Info
                            {
                                ID = item.ID,
                                Secuencia = Secuencia++,
                                pre_empre = Convert.ToInt32(readerProcedure[0]),
                                pre_codtra = Convert.ToInt32(readerProcedure[2]),
                                trb_email = Convert.ToString(readerProcedure[15]),
                                infoRpt = new Reporte01_Info
                                {
                                    pre_empresa = Convert.ToInt32(readerProcedure[0]),
                                    pre_codtra = Convert.ToInt32(readerProcedure[2]),
                                    pre_rol = Convert.ToInt32(readerProcedure[3]),
                                    trb_email = Convert.ToString(readerProcedure[15]),
                                    emp_nombre = Convert.ToString(readerProcedure[16]),
                                    cgo_descri = Convert.ToString(readerProcedure[11]),
                                    sub_nombre = Convert.ToString(readerProcedure[10]),
                                    trb_cen = Convert.ToString(readerProcedure[9]),
                                    trb_are = Convert.ToString(readerProcedure[8]),
                                    trb_div = Convert.ToString(readerProcedure[7]),
                                    NOM = Convert.ToString(readerProcedure[6]),
                                    cro_fecini = Convert.ToDateTime(readerProcedure[4]),
                                    cro_fecfin = Convert.ToDateTime(readerProcedure[5]),
                                    ING = Convert.ToDouble(readerProcedure[12]),
                                    EGR = Math.Abs(Convert.ToDouble(readerProcedure[13]))
                                }
                            };
                            det.infoRpt.Saldo = det.infoRpt.ING - det.infoRpt.EGR;
                            if (ListaDet.Where(q => q.pre_codtra == det.pre_codtra && q.pre_empre == det.pre_empre).FirstOrDefault() == null)
                                item.Lista.Add(det);

                        }
                        item.TotalEmpleados = item.Lista.Count;
                        readerProcedure.Close();
                        #endregion

                    }
                    connection.Close();
                }

                return Lista;
            }
            catch (Exception ex)
            {
                GuardarExcepcion(new Mail_LogError_Info
                {
                    Metodo = "GetCabecerasProcesadasAmedias",
                    Error = ex == null ? string.Empty : (ex.Message ?? string.Empty),
                    InnerException = ex == null || ex.InnerException == null ? string.Empty : (ex.InnerException.Message.Length > 1000 ? ex.InnerException.Message.Substring(0, 1000) : ex.InnerException.Message)
                });
                return new List<Mail_RolesProcesados_Info>();
            }
        }

        public bool ModificarDet(Mail_RolesProcesadosDet_Info info)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(builder.ToString()))
                {
                    connection.Open();

                    string QueryUpdate = 
                        "update Mail_RolesProcesadosDet set FechaEnvio = getdate(), Error = '"+info.Error+"' "
                        +"where ID = @Id and Secuencia = @Secuencia";
                    SqlCommand commandUpdate = new SqlCommand(QueryUpdate, connection);
                    commandUpdate.Parameters.Add("@Id", SqlDbType.Decimal).Value = info.ID;
                    commandUpdate.Parameters.Add("@Secuencia", SqlDbType.Int).Value = info.Secuencia;
                    commandUpdate.ExecuteNonQuery();

                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                GuardarExcepcion(new Mail_LogError_Info
                {
                    Metodo = "ModificarDet",
                    Error = ex == null ? string.Empty : (ex.Message ?? string.Empty),
                    InnerException = ex == null || ex.InnerException == null ? string.Empty : (ex.InnerException.Message.Length > 1000 ? ex.InnerException.Message.Substring(0, 1000) : ex.InnerException.Message)
                });
                return true;
            }
        }

        public bool Modificar(Mail_RolesProcesados_Info info)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(builder.ToString()))
                {
                    connection.Open();

                    string QueryUpdate =
                    "update Mail_RolesProcesados Set TotalProcesados = ContEnviado, FechaFin = case when ContEnviado = ContTotal then getdate() else null end "
                    + "FROM( "
                    + "select ID, sum(ContEnviado) ContEnviado, sum(ContTotal) ContTotal from( "
                    + "select ID, case when FechaEnvio is not null then 1 else 0 end as ContEnviado, 1 as ContTotal "
                    + "from Mail_RolesProcesadosDet "
                    + "where ID = @Id "
                    + ") A GROUP BY ID) A "
                    + "WHERE A.ID = Mail_RolesProcesados.ID ";
                    SqlCommand commandUpdate = new SqlCommand(QueryUpdate, connection);
                    commandUpdate.Parameters.Add("@Id", SqlDbType.Decimal).Value = info.ID;
                    commandUpdate.ExecuteNonQuery();

                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                GuardarExcepcion(new Mail_LogError_Info
                {
                    Metodo = "Modificar",
                    Error = ex == null ? string.Empty : (ex.Message ?? string.Empty),
                    InnerException = ex == null || ex.InnerException == null ? string.Empty : (ex.InnerException.Message.Length > 1000 ? ex.InnerException.Message.Substring(0, 1000) : ex.InnerException.Message)
                });
                return true;
            }
        }

        public bool GuardarExcepcion(Mail_LogError_Info info)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(builder.ToString()))
                {
                    connection.Open();
                    string QueryInsert =
                        "INSERT INTO [dbo].[Mail_LogError]([Metodo],[Fecha],[Error],[InnerException]) "
                        +"VALUES(@Metodo, getdate(), @Error, @InnerException)";

                    SqlCommand commandInsert = new SqlCommand(QueryInsert,connection);
                    commandInsert.Parameters.Add("@Metodo", SqlDbType.VarChar,500).Value = info.Metodo;
                    commandInsert.Parameters.Add("@Error", SqlDbType.Text).Value = info.Error;
                    commandInsert.Parameters.Add("@InnerException", SqlDbType.Text).Value = info.InnerException;
                    commandInsert.ExecuteNonQuery();
                    connection.Close();
                }

                return true;
            }
            catch (Exception)
            {
                return true;
            }
        }

        public Mail_Credenciales_Info GetInfoCredenciales()
        {
            try
            {
                string QuerySelect = "select ID,Usuario,Contrasenia,Asunto,Cuerpo,Puerto,host,PermitirSSL,CorreoCopia from Mail_Credenciales";
                Mail_Credenciales_Info info = new Mail_Credenciales_Info();

                using (SqlConnection connection = new SqlConnection(builder.ToString()))
                {
                    connection.Open();
                    SqlCommand commandSelect = new SqlCommand(QuerySelect,connection);
                    SqlDataReader reader = commandSelect.ExecuteReader();
                    while (reader.Read())
                    {
                        info = new Mail_Credenciales_Info
                        {
                         ID = Convert.ToInt32(reader[0]),
                            Usuario = Convert.ToString(reader[1]),
                            Contrasenia = Convert.ToString(reader[2]),
                            Asunto = Convert.ToString(reader[3]),
                            Cuerpo = Convert.ToString(reader[4]),
                            Puerto = Convert.ToInt32(reader[5]),
                            host = Convert.ToString(reader[6]),
                            PermitirSSL = Convert.ToBoolean(reader[7]),
                            CorreoCopia = Convert.ToString(reader[8])
                        };
                    }
                    reader.Close();

                    connection.Close();
                }

                return info;
            }
            catch (Exception ex)
            {
                GuardarExcepcion(new Mail_LogError_Info
                {
                    Metodo = "GetInfoCredenciales",
                    Error = ex == null ? string.Empty : (ex.Message ?? string.Empty),
                    InnerException = ex == null || ex.InnerException == null ? string.Empty : (ex.InnerException.Message.Length > 1000 ? ex.InnerException.Message.Substring(0, 1000) : ex.InnerException.Message)
                });
                return new Mail_Credenciales_Info();
            }
        }

        #region Reporte 01
        public List<Reporte01Det_Info> GetList(int pre_empre, int pre_rol, int pre_codtra)
        {
            try
            {
                List<Reporte01Det_Info> Lista = new List<Reporte01Det_Info>();

                using (SqlConnection connection = new SqlConnection(builder.ToString()))
                {
                    connection.Open();

                    
                    string queryProcedure = "EXEC [rrhh].[dbo].[rep_sob_egresos] "+pre_empre.ToString()+", 7, "+pre_rol.ToString()+", "+pre_codtra.ToString()+", 'D'";
                    SqlCommand commandProcedure = new SqlCommand(queryProcedure, connection);

                    SqlDataReader readerProcedure = commandProcedure.ExecuteReader();
                    
                    while (readerProcedure.Read())
                    {
                        Reporte01Det_Info det = new Reporte01Det_Info
                        {
                            IdEmpresa = Convert.ToInt32(readerProcedure[0]),
                            pre_tiprol = Convert.ToInt32(readerProcedure[1]),
                            pre_rol = Convert.ToInt32(readerProcedure[2]),
                            pre_codtra = Convert.ToInt32(readerProcedure[3]),
                            rub_codigo = Convert.ToInt32(readerProcedure[4]),
                            rub_descri = Convert.ToString(readerProcedure[5]),
                            pre_valor = Convert.ToDouble(readerProcedure[6]),
                            pre_refere = Convert.ToString(readerProcedure[7]),
                            pre_saldo = Convert.ToDouble(readerProcedure[8]),
                            Signo = "-"
                        };
                        if (!string.IsNullOrEmpty(det.pre_refere))
                            det.rub_descri = det.pre_refere;

                        Lista.Add(det);
                    }
                    readerProcedure.Close();

                    queryProcedure = "EXEC [rrhh].[dbo].[rep_sob_ingresos] " + pre_empre.ToString() + ", 7, " + pre_rol.ToString() + ", " + pre_codtra.ToString() + ", 'D'";
                    SqlCommand commandProcedureIng = new SqlCommand(queryProcedure, connection);

                    SqlDataReader readerProcedureIng = commandProcedureIng.ExecuteReader();

                    while (readerProcedureIng.Read())
                    {
                        Reporte01Det_Info det = new Reporte01Det_Info
                        {
                            IdEmpresa = Convert.ToInt32(readerProcedureIng[0]),
                            pre_tiprol = Convert.ToInt32(readerProcedureIng[1]),
                            pre_rol = Convert.ToInt32(readerProcedureIng[2]),
                            pre_codtra = Convert.ToInt32(readerProcedureIng[3]),
                            rub_codigo = Convert.ToInt32(readerProcedureIng[4]),
                            rub_descri = Convert.ToString(readerProcedureIng[5]),
                            pre_valor = Convert.ToDouble(readerProcedureIng[6]),
                            pre_refere = Convert.ToString(readerProcedureIng[7]),
                            Signo = "+"
                        };
                        Lista.Add(det);
                    }

                    connection.Close();
                }

                return Lista;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}
