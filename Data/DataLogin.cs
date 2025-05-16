using API.Connection;
using API.Model; 
using MySql.Data.MySqlClient;
using System.Data;
namespace API.Data 
{
    public class DataLogin
    {
        private readonly ConnectionBD _baseDatos;

        public DataLogin(ConnectionBD baseDatos)
        {
            _baseDatos = baseDatos;
        }

         public async Task<ModelLogin?> GetUserByEmailAsync(string email)
        {
            using (var sql = new MySqlConnection(_baseDatos.ConnectionMYSQL()))
            {
                using (var cmd = new MySqlCommand("sp_getUserByEmail", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_email", email);

                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var user = new ModelLogin
                            {
                                Id = reader.GetInt32("user_id"),
                                Email = reader.GetString("email"),
                                Pass = reader.GetString("Pass"),
                                Tipo = reader.GetString("user_type")
                            };
                            return user;
                        }
                    }
                }
            }
            return null;
        }

        public async Task InsertLoginAsync(string email)
        {
            using (var sql = new MySqlConnection(_baseDatos.ConnectionMYSQL()))
            {
                using (var cmd = new MySqlCommand("sp_insertLogin", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_email", email);

                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
