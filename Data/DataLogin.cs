using API.Connection;
using API.Model;
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
using System.Data;
using System.Security.Cryptography.X509Certificates;

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
                                Id = reader.GetInt32("nUserID"),
                                Email = reader.GetString("cEmail"),
                                Pass = reader.GetString("cPassword"),
                                Tipo = reader.GetString("eRolUsuario")
                            };
                            return user;
                        }
                    }
                }
            }
            return null;
        }

        public async Task InsertLoginAsync(int Id)
        {
            using (var sql = new MySqlConnection(_baseDatos.ConnectionMYSQL()))
            {
                using (var cmd = new MySqlCommand("sp_insertLogin", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_user_id", Id);

                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<ModelLogin>> MostrarLoginAsync(ModelLogin parametros)
        {
            var list = new List<ModelLogin>();

            using (var sql = new MySqlConnection(_baseDatos.ConnectionMYSQL()))
            {
                using (var cmd = new MySqlCommand("sp_MostrarLogins", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await sql.OpenAsync();
                    
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var registro = new ModelLogin
                            {
                                Id = (int)reader["nLoginID"],
                                Name = reader["cNombre"] != DBNull.Value ? (string)reader["cNombre"] : string.Empty,
                                Tipo = reader["eRolUsuario"] != DBNull.Value ? (string)reader["eRolUsuario"] : string.Empty,
                                Tiempo = (DateTime)reader["dLogin"]
                            };

                            list.Add(registro);
                        }

                    }
                }
            }

            return list;
        }
    }
}
