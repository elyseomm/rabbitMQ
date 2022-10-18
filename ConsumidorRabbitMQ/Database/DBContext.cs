using Microsoft.Data.Sqlite;

namespace ConsumidorRabbitMQ.Database
{
    public class DBContext
    {
        public const string TableName = "contador_programas";
        public const string ProgramGroups = "grupo_programas";
        public const string DatabaseName = "./SqliteDB.db";


        public DBContext(SQLiteConfig _config)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder();

            //Use DB in project directory.  If it does not exist, create it:
            connectionStringBuilder.DataSource = DatabaseName;

            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
            //    using (var connection = new SqliteConnection($"Data Source={_config.LocalPath}"))
            //{
                connection.Open();

                //Create a table (drop if already exists first):

                var delTableCmd = connection.CreateCommand();
                delTableCmd.CommandText = $"DROP TABLE IF EXISTS {TableName}";
                delTableCmd.ExecuteNonQuery();

                delTableCmd = connection.CreateCommand();
                delTableCmd.CommandText = $"DROP TABLE IF EXISTS {ProgramGroups}";
                delTableCmd.ExecuteNonQuery();

                var createTableCmd = connection.CreateCommand();
                createTableCmd.CommandText = $"CREATE TABLE {TableName}(nome VARCHAR(100), contador INTEGER)";
                createTableCmd.ExecuteNonQuery();

                createTableCmd = connection.CreateCommand();
                createTableCmd.CommandText = $"CREATE TABLE {ProgramGroups}(grupo_programa VARCHAR(100), nome_programa VARCHAR(100))";
                createTableCmd.ExecuteNonQuery();

                //Seed some data:
                using (var transaction = connection.BeginTransaction())
                {
                    var insertCmd = connection.CreateCommand();

                    insertCmd.CommandText = $"INSERT INTO {TableName} VALUES('PROGRAMA01', 0)";
                    insertCmd.ExecuteNonQuery();

                    // ------------------------------------------------------------------------------------

                    insertCmd.CommandText = $"INSERT INTO {ProgramGroups} VALUES('RECARGAS', 'RecargaIVR')";
                    insertCmd.ExecuteNonQuery();

                    insertCmd.CommandText = $"INSERT INTO {ProgramGroups} VALUES('RECARGAS', 'RecargaSMS')";
                    insertCmd.ExecuteNonQuery();

                    transaction.Commit();
                }

                //Read the newly inserted data:
                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = $"SELECT nome FROM {TableName}";

                using (var reader = selectCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var message = reader.GetString(0);
                    }
                }
            }
        }
    }
}
