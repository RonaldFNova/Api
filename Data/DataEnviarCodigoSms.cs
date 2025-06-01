using API.Model;
using API.Connection;
using MySql.Data.MySqlClient;
using System.Data;

namespace API.Data
{
    public class DataEnviarCodigoSms
    {
        private readonly ConnectionBD _connectionBD;

        public DataEnviarCodigoSms(ConnectionBD connectionBD)
        {
            _connectionBD = connectionBD;
        }

        public async Task<string> ObtenerCellAsync(int id)
        {
            string cell = string.Empty;

            using (var sql = new MySqlConnection(_connectionBD.ConnectionMYSQL()))
            {
                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_ObtenerTelefonoPorUsuario", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("p_nUserID", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            cell = reader.GetString("Telefono");
                        }
                    }
                }

                await sql.CloseAsync();
            }


            return cell;
        }
    }
}