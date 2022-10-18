using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsumidorRabbitMQ.Database
{
    public class GrupoProgramasAdapter
    {
        public static string GrupoPrograma { get; set; }
        public static bool CheckIsGrupo(string programName)
        {
            var result = false;
            try
            {
                var connectionStringBuilder = new SqliteConnectionStringBuilder();
                connectionStringBuilder.DataSource = DBContext.DatabaseName;

                using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
                {
                    connection.Open();

                    // * Verificar se já existe registro para o programa
                    var selectCmd = connection.CreateCommand();
                    selectCmd.CommandText = $"SELECT grupo_programa FROM {DBContext.ProgramGroups} WHERE nome_programa = '{programName}'";

                    using (var reader = selectCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            GrupoPrograma = reader.GetString(0); 
                            
                            result = !string.IsNullOrWhiteSpace(GrupoPrograma);
                        }                      
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return result;
        }
        

        private static void DoUpdate(SqliteConnection conn, string programName, int value)
        {
            using (var transaction = conn.BeginTransaction())
            {
                var insertCmd = conn.CreateCommand();

                insertCmd.CommandText = $"UPDATE {DBContext.TableName} SET contador = {value} " +
                    $" WHERE nome = '{programName}'";

                insertCmd.ExecuteNonQuery();

                transaction.Commit();
            }
        }

        private static void DoInsert(SqliteConnection conn, string programName, int value)
        {
            using (var transaction = conn.BeginTransaction())
            {
                var insertCmd = conn.CreateCommand();

                insertCmd.CommandText = $"INSERT INTO {DBContext.TableName} VALUES('{programName}', {value})";

                insertCmd.ExecuteNonQuery();

                transaction.Commit();
            }
        }

    }
}
