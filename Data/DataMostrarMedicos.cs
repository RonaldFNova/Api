using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Schema;
using API.Connection;
using API.Model;
using MySql.Data.MySqlClient;

namespace API.Data
{
    public class DataMostrarMedicos
    {
        public ConnectionBD _connection;
        public DataMostrarMedicos(ConnectionBD connection)
        {
            _connection = connection;
        }

        public async Task<List<ModelMostrarMedicos>> MostrarMedicosAsync()
        {

            var Lista = new List<ModelMostrarMedicos>();

            using (var sql = new MySqlConnection(_connection.ConnectionMYSQL()))
            {
                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("spListarMedicos", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var medico = new ModelMostrarMedicos

                            {
                                Name = reader.GetString(reader.GetOrdinal("NombreMedico"))
                            };

                            Lista.Add(medico);

                        }
                    }
                }
            }

            return Lista;
        }
    }
}