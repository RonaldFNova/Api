using API.Connection;
using API.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace API.Data
{
    public class DataNotificarCitas
    {
        private readonly ConnectionBD _connection;

        public DataNotificarCitas(ConnectionBD connection)
        {
            _connection = connection;
        }

        public async Task<List<ModelNotificarCitas>> ObtenerCitasDeMananaAsync()
        {
            var lista = new List<ModelNotificarCitas>();

            using (var sql = new MySqlConnection(_connection.ConnectionMYSQL()))
            {
                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_CitasManana", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(new ModelNotificarCitas
                            {
                                NombrePaciente = reader["NombrePaciente"].ToString(),
                                Correo = reader["CorreoPaciente"].ToString(),
                                Fecha = Convert.ToDateTime(reader["FechaCita"]),
                                HoraInicio = TimeSpan.Parse(reader["HoraInicio"].ToString())
                            });
                        }
                    }
                }

                await sql.CloseAsync();
            }

            return lista;
        }
    }
}
