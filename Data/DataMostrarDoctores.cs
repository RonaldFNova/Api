using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Schema;
using API.Connection;
using API.Model;
using MySql.Data.MySqlClient;

namespace API.Data
{
    public class DataMostrarDoctores
    {
        public ConnectionBD _connection;
        public DataMostrarDoctores(ConnectionBD connection)
        {
            _connection = connection;
        }

        public async Task<List<ModelMostrarDoctores>> MostrarDoctoresAsync()
        {

            var Lista = new List<ModelMostrarDoctores>();

            using (var sql = new MySqlConnection(_connection.ConnectionMYSQL()))
            {
                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("spListarDoctores", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var doctor = new ModelMostrarDoctores

                            {
                                Name = reader.GetString(reader.GetOrdinal("NombreDoctor"))
                            };

                            Lista.Add(doctor);

                        }
                    }
                }
            }

            return Lista;
        }
    }
}