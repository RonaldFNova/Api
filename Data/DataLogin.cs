using API.Connection;
using API.Model; 
using MySql.Data.MySqlClient;
using System.Data;

namespace API.Data 
{
    public class DataLogin
    {
        private readonly ConnectionBD bD = new ConnectionBD();

         public async Task<User?> GetUserByEmailAsync(string email)
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
                            var user = new User
                            {
                                user_id = reader.GetInt32("user_id"),
                                full_name = reader.GetString("full_name"),
                                email = reader.GetString("email"),
                                Pass = reader.GetString("Pass"),
                                user_type = reader.GetString("user_type"),
                                created_at = reader.GetDateTime("created_at")
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

    public class User
    {
        public int user_id { get; set; }
        public string full_name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string Pass { get; set; } = string.Empty;
        public string user_type { get; set; } = string.Empty;
        public DateTime created_at { get; set; }
    }
}
