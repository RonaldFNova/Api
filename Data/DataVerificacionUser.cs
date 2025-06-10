using API.Connection;
using MySql.Data.MySqlClient;
using System.Data;

namespace API.Data
{
    public class DataVerificacionUser
    {
        private readonly ConnectionBD _connectionBD;

        public DataVerificacionUser(ConnectionBD connectionBD)
        {
            _connectionBD = connectionBD;
        }

        public async Task<string> ObtenerVerificacionUserAsync(int id)
        {
            string EstadoUser = string.Empty;

            using (var sql = new MySqlConnection(_connectionBD.ConnectionMYSQL()))
            {

                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_VerificarEstadoUsuario", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("pNUsuarioID", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {

                        if (await reader.ReadAsync())
                        {
                            EstadoUser = reader.GetString("eVerificacion");
                        }
                    }
                }

                await sql.CloseAsync();

            }

            return EstadoUser;
        }
    }
}