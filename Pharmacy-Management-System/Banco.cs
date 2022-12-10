using Microsoft.SqlServer.Management.Common;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Pharmacy_Management_System
{
    public class Banco
    {
        readonly SqlConnection con = new SqlConnection();
        readonly SqlCommand cmd = new SqlCommand();

        //public static string caminho = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
        //public static string caminho = System.Environment.CurrentDirectory;
        //public static string caminhoBanco = caminho + @"\banco\";

        //Construtor
        public Banco()
        {
            RestaurarDBPadraoCasoNaoExista();
            con.ConnectionString = @"Server=(localdb)\MSSQLLocalDB;Integrated Security=true";
            //con.ConnectionString = @"Data Source = RENATO-PC\SQLEXPRESS;TrustServerCertificate = True ;Integrated Security = True";
        }

        private void RestaurarDBPadraoCasoNaoExista()
        {
            try
            {
                var bancoExiste = VerificaSeBancoJaExiste();

                if (!bancoExiste)
                {
                    RestaurarDBPadrao();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Erro", (MessageBoxButton)MessageBoxButtons.OK, (MessageBoxImage)MessageBoxIcon.Error);
            }
        }

        private bool VerificaSeBancoJaExiste()
        {
            bool retorno = false;

            try
            {
                using (var conn = new System.Data.SqlClient.SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Database=master; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = True; ApplicationIntent = ReadWrite; MultiSubnetFailover = False"))
                {
                    conn.Open();
                    using (var comm = conn.CreateCommand())
                    {
                        comm.CommandText = "SELECT 1 FROM SYS.DATABASES WHERE NAME LIKE 'PharmacyQR'";
                        var valor = comm.ExecuteScalar();

                        if (valor != null && valor != DBNull.Value && Convert.ToInt32(valor).Equals(1))
                        {
                            retorno = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Erro", (MessageBoxButton)MessageBoxButtons.OK, (MessageBoxImage)MessageBoxIcon.Error);
            }

            return retorno;
        }

        private void DescobrirDiretoriosPadrao(out string diretorioDados, out string diretorioLog, out string diretorioBackup)
        {
            using (var connection = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB; Database=master; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = True; ApplicationIntent = ReadWrite; MultiSubnetFailover = False"))
            {
                var serverConnection = new ServerConnection(connection);
                var server = new Microsoft.SqlServer.Management.Smo.Server(serverConnection);
                diretorioDados = !string.IsNullOrWhiteSpace(server.Settings.DefaultFile) ? server.Settings.DefaultFile : (!string.IsNullOrWhiteSpace(server.DefaultFile) ? server.DefaultFile : server.MasterDBPath);
                diretorioLog = !string.IsNullOrWhiteSpace(server.Settings.DefaultLog) ? server.Settings.DefaultLog : (!string.IsNullOrWhiteSpace(server.DefaultLog) ? server.DefaultLog : server.MasterDBLogPath);
                diretorioBackup = !string.IsNullOrWhiteSpace(server.Settings.BackupDirectory) ? server.Settings.BackupDirectory : server.BackupDirectory;
            }
        }
        private void RestaurarDBPadrao()
        {
            try
            {
                string diretorioDados, diretorioLog, diretorioBackup;
                DescobrirDiretoriosPadrao(out diretorioDados, out diretorioLog, out diretorioBackup);

                using (var conn = new System.Data.SqlClient.SqlConnection(@"Server=(localdb)\MSSQLLocalDB;Database=master;Trusted_Connection=True;"))
                {
                    conn.Open();
                    using (var comm = conn.CreateCommand())
                    {
                        var caminhoCompletoBackup = System.IO.Path.Combine(diretorioBackup, "PharmacyQR.bak");
                        var caminhoCompletoDados = System.IO.Path.Combine(diretorioDados, "PharmacyQR.mdf");
                        var caminhoCompletoLog = System.IO.Path.Combine(diretorioLog, "PharmacyQR_log.ldf");
                        System.IO.File.Copy("PharmacyQR.bak", caminhoCompletoBackup, true);
                        comm.CommandText =
                            @"RESTORE DATABASE PharmacyQR " +
                            @"FROM DISK = N'" + caminhoCompletoBackup + "' " +
                            @"WITH FILE = 1, " +
                            @"MOVE N'PharmacyQR' TO N'" + caminhoCompletoDados + "', " +
                            @"MOVE N'PharmacyQR_log' TO N'" + caminhoCompletoLog + "', " +
                            @"NOUNLOAD, REPLACE, STATS = 10";
                        comm.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Erro", (MessageBoxButton)MessageBoxButtons.OK, (MessageBoxImage)MessageBoxIcon.Error);
            }
        }

        //Método Conectar
        private SqlConnection Conectar()
        {
            if (con.State == System.Data.ConnectionState.Closed)
            {
                con.Open();
            }
            return con;
        }

        private void Desconectar()
        {
            if(con.State == System.Data.ConnectionState.Open)
            {
                con.Dispose();
                con.Close();
            }
        }
       
        //Método NonQuery
        public void NonQuery(String sql)
        {
            this.cmd.CommandText = sql;
            this.cmd.ExecuteNonQuery();
        }

        //Método Query
        public SqlDataReader Query(String sql)
        {
            cmd.Connection = Conectar();
            cmd.CommandText = sql;
            return this.cmd.ExecuteReader();
        }
    }
}