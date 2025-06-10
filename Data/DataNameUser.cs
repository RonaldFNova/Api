using API.Connection;
using MySql.Data.MySqlClient;
using System.Data;

namespace API.Data
{
    public class DataNameUser
    {
        private readonly ConnectionBD _connectionBD;

        public DataNameUser(ConnectionBD connectionBD)
        {
            _connectionBD = connectionBD;
        }

        public async Task<string> ObtenerNameUserAsync(int id)
        {
            string Nombre = string.Empty;

            using (var sql = new MySqlConnection(_connectionBD.ConnectionMYSQL()))
            {
                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_NombreCompletoUsuario", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("pNUsuarioID", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            Nombre = reader.GetString("cNombre");
                        }
                    }
                }

                await sql.CloseAsync();
            }

            return Nombre;
        }
    }
}