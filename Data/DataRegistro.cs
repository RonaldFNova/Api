using API.Connection;
using API.Model;
using MySql.Data.MySqlClient;
using System.Data;
using API.Security;

namespace API.Data 
{
    public class DataRegistro 
    {
        private readonly ConnectionBD bD = new ConnectionBD();

        private readonly PasswordHasher _passwordHasher;

        public DataRegistro(PasswordHasher passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public async Task<List<ModelRegistro>> MostrarUsuario()
        {
            var lista = new List<ModelRegistro>();

            try
            {
                using (var sql = new MySqlConnection(bD.ConnectionMYSQL()))
                {
                    using (var cmd = new MySqlCommand("sp_get_user", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var registro = new ModelRegistro
                                {
                                    id = (int)reader["user_id"],
                                    name = reader["full_name"] != DBNull.Value ? (string)reader["full_name"] : string.Empty,
                                    email = reader["email"] != DBNull.Value ? (string)reader["email"] : string.Empty,
                                    pass = reader["Pass"] != DBNull.Value ? (string)reader["Pass"] : string.Empty,
                                    tipo = reader["user_type"] != DBNull.Value ? (string)reader["user_type"] : string.Empty
                                };

                                lista.Add(registro);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al mostrar usuarios.", ex);
            }

            return lista;
        }

        public async Task InsertarUsuario(ModelRegistro parametros)
        {
            try
            {
                var passHashed = _passwordHasher.HashPassword(parametros.pass);

                using (var sql = new MySqlConnection(bD.ConnectionMYSQL()))
                {
                    using (var cmd = new MySqlCommand("sp_insert_user", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("p_full_name", parametros.name);
                        cmd.Parameters.AddWithValue("p_email", parametros.email);
                        cmd.Parameters.AddWithValue("p_pass", passHashed);
                        cmd.Parameters.AddWithValue("p_user_type", parametros.tipo);
                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar el usuario.", ex);
            }
        }

        public async Task EditarUsuario(ModelRegistro parametros)
        {
            try
            {
                var passHashed = _passwordHasher.HashPassword(parametros.pass);

                using (var sql = new MySqlConnection(bD.ConnectionMYSQL()))
                {
                    using (var cmd = new MySqlCommand("sp_update_user", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("p_user_id", parametros.id);
                        cmd.Parameters.AddWithValue("p_full_name", parametros.name);
                        cmd.Parameters.AddWithValue("p_email", parametros.email);
                        cmd.Parameters.AddWithValue("p_pass", passHashed);
                        cmd.Parameters.AddWithValue("p_user_type", parametros.tipo);
                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al editar el usuario.", ex);
            }
        }
    }
}
