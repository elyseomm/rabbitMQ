using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsumidorRabbitMQ.Database
{
    public class ProgramaAdapter
    {
        public static int Contador { get; set; }
        public static void UpdateCounter(string programName)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = DBContext.DatabaseName;

            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();

                // * Verificar se já existe registro para o programa
                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = $"SELECT nome FROM {DBContext.TableName} WHERE nome = '{programName}'";

                using (var reader = selectCmd.ExecuteReader())
                {
                    try
                    {
                        if (reader.HasRows)
                        {
                            // * UPDATE 
                            while (reader.Read())
                            {
                                var message = reader.GetString(0);
                                if (!string.IsNullOrWhiteSpace(message))
                                    DoUpdate(connection, programName);
                            }
                        }
                        // * INSERT
                        else DoInsert(connection, programName);
                    }
                    catch (Exception ex)
                    {
                        
                    }

                    // * Retorna o valor do contador atualizado
                    Contador = GetCounter(connection, programName);
                }
            }
        }
        

        public static int GetCounter(SqliteConnection conn, string programName)
        {
            int result = 0;
            try
            {
                var selectCmd = conn.CreateCommand();
                selectCmd.CommandText = $"SELECT contador FROM {DBContext.TableName} WHERE nome = '{programName}'";

                using (var reader = selectCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result = reader.GetInt32(0);
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return result;
        }


        private static void DoUpdate(SqliteConnection conn, string programName)
        {
            using (var transaction = conn.BeginTransaction())
            {
                var insertCmd = conn.CreateCommand();

                insertCmd.CommandText = $"UPDATE {DBContext.TableName} SET contador = contador+1 " +
                    $" WHERE nome = '{programName}'";

                insertCmd.ExecuteNonQuery();

                transaction.Commit();
            }
        }

        private static void DoInsert(SqliteConnection conn, string programName)
        {
            using (var transaction = conn.BeginTransaction())
            {
                var insertCmd = conn.CreateCommand();

                insertCmd.CommandText = $"INSERT INTO {DBContext.TableName} VALUES('{programName}', 1)";

                insertCmd.ExecuteNonQuery();

                transaction.Commit();
            }
        }

    }
}
