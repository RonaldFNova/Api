using API.Model;
using API.Connection;
using MySql.Data.MySqlClient;
using System.Data;

namespace API.Data
{
    public class DataConfirmarCodigoSms
    {
        private readonly ConnectionBD _connectionBD;

        public DataConfirmarCodigoSms(ConnectionBD connectionBD)
        {
            _connectionBD = connectionBD;
        }

        public async Task EnviarEstadoVerificacionAsync(int id)
        {

            using (var sql = new MySqlConnection(_connectionBD.ConnectionMYSQL()))
            {
                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("CambiarTelefonoVerificado", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("pUserID", id);
                    await cmd.ExecuteNonQueryAsync();

                }

                await sql.CloseAsync();
            }

        }
    }
}