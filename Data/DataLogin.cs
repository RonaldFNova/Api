using API.Connection;
using API.Model; 
using MySql.Data.MySqlClient;
using System.Data;
namespace API.Data 
{
    public class DataLogin
    {
        private readonly ConnectionBD bD = new ConnectionBD();

         public async Task<ModelRegistro?> GetUserByEmailAsync(string email)
        {
            using (var sql = new MySqlConnection(bD.ConnectionMYSQL()))
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
                            var user = new ModelRegistro
                            {
                                id = reader.GetInt32("user_id"),
                                name = reader.GetString("full_name"),
                                email = reader.GetString("email"),
                                pass = reader.GetString("Pass"),
                                tipo = reader.GetString("user_type"),
                            };
                            return user;
                        }
                    }
                }
            }
            return null;
        }

        public async Task InsertLoginAsync(int userId)
        {
            using (var sql = new MySqlConnection(bD.ConnectionMYSQL()))
            {
                using (var cmd = new MySqlCommand("sp_insertLogin", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_user_id", userId);

                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
