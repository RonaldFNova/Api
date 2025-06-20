using API.Connection;
using API.Model;
using MySql.Data.MySqlClient;
using System.Data;
using API.Security;
using API.Services;
using API.Error;

namespace API.Data 
{
    public class DataRegistro 
    {
        private readonly ConnectionBD _baseDatos;

        private readonly CodigoVerificacionService _codigoVerificacionService;

        private readonly PasswordHasher _passwordHasher;

        private readonly EmailService _emailService;


        public DataRegistro(PasswordHasher passwordHasher,CodigoVerificacionService codigoVerificacionService,
        EmailService emailService, ConnectionBD baseDatos)
        {
            _passwordHasher = passwordHasher;
            _codigoVerificacionService = codigoVerificacionService;
            _emailService = emailService;
            _baseDatos = baseDatos;

        }

        public async Task<int> InsertarUsuario(ModelRegistro parametros)
        {
            int userId = 0;

            string passHashed = _passwordHasher.HashPassword(parametros.Pass);

            using (var sql = new MySqlConnection(_baseDatos.ConnectionMYSQL()))
            {
                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_insert_user", sql))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_nombre", parametros.Name);
                    cmd.Parameters.AddWithValue("p_email", parametros.Email);
                    cmd.Parameters.AddWithValue("p_pass", passHashed);
                    cmd.Parameters.AddWithValue("p_rol", parametros.Tipo);
                    await cmd.ExecuteNonQueryAsync();
                }

                using (var cmd = new MySqlCommand("sp_get_user_id_by_email", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_email", parametros.Email);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            userId = reader.GetInt32("nUserID");
                        }
                    }
                }
            }

            return userId;
        }


        public async Task EditarUsuario(ModelRegistro parametros)
        {
  
            string passHashed = _passwordHasher.HashPassword(parametros.Pass);

            using (var sql = new MySqlConnection(_baseDatos.ConnectionMYSQL()))
            {
                using (var cmd = new MySqlCommand("sp_update_user", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_user_id", parametros.Id);
                    cmd.Parameters.AddWithValue("p_nombre", parametros.Name);
                    cmd.Parameters.AddWithValue("p_email", parametros.Email);
                    cmd.Parameters.AddWithValue("p_pass", passHashed);
                    cmd.Parameters.AddWithValue("p_rol", parametros.Tipo);
                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<ModelRegistro>> MostrarUsuario()
        {

            var lista = new List<ModelRegistro>();

            using (var sql = new MySqlConnection(_baseDatos.ConnectionMYSQL()))
            {
                using (var cmd = new MySqlCommand("sp_get_users", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var registro = new ModelRegistro
                            {
                                Id = (int)reader["nUserID"],
                                Name = reader["cNombre"] != DBNull.Value ? (string)reader["cNombre"] : string.Empty,
                                Email = reader["cEmail"] != DBNull.Value ? (string)reader["cEmail"] : string.Empty,
                                Pass = reader["cPassword"] != DBNull.Value ? (string)reader["cPassword"] : string.Empty,
                                Tipo = reader["eRolUsuario"] != DBNull.Value ? (string)reader["eRolUsuario"] : string.Empty
                            };

                            lista.Add(registro);
                        }
                    }
                }
            }

            return lista;
        }


        public async Task EliminarUsuario(int userId)
        {
            using (var sql = new MySqlConnection(_baseDatos.ConnectionMYSQL()))
            {

                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_delete_user", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_user_id", userId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        
        public async Task<string> EnviarCodigo(int Id)
        {
            string email = string.Empty;

            string emailConfirmacionVerificacion = string.Empty;

            string nombreCompleto = string.Empty;

            string codigo = _codigoVerificacionService.GenerarCodigo();

            int userId = Convert.ToInt32(Id);

            using (var sql = new MySqlConnection(_baseDatos.ConnectionMYSQL()))
            {
                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_checkUserVerificationStatus", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_user_id", userId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            emailConfirmacionVerificacion = reader.GetString("user_not_verified");
                        }
                    }
                }

                using (var cmd = new MySqlCommand("sp_get_email_by_user_id", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_user_id", userId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            email = reader.GetString("cEmail");
                        }
                    }
                }

                using (var cmd = new MySqlCommand("sp_get_full_name_by_user_id", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_user_id", userId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            nombreCompleto = reader.GetString("cNombre");
                        }
                    }
                }

                if (emailConfirmacionVerificacion == "False") throw new EstadoEmailVerificadoException();
            }
            await _emailService.SendEmailAsync(email, nombreCompleto, codigo);
            return codigo;
        }


        public async Task  ConfirmarVerificacion(int Id)
        {

            using (var sql = new MySqlConnection(_baseDatos.ConnectionMYSQL()))
            {
              
                using (var cmd = new MySqlCommand("sp_actualizar_estado_verificacion", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_user_id", Id);
                    cmd.Parameters.AddWithValue("p_nuevo_estado", "Verificado");
                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
