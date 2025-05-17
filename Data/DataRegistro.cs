using API.Connection;
using API.Model;
using MySql.Data.MySqlClient;
using System.Data;
using API.Security;
using API.Error;
using Org.BouncyCastle.Asn1;

namespace API.Data 
{
    public class DataRegistro 
    {
        private readonly ConnectionBD _baseDatos;

        private readonly CodigoVerificacionService _codigoVerificacionService;

        private readonly PasswordHasher _passwordHasher;

        private readonly TokenHelper _tokenHelper;

        private readonly EmailService _emailService;


        public DataRegistro(PasswordHasher passwordHasher,CodigoVerificacionService codigoVerificacionService,
        EmailService emailService,TokenHelper tokenHelper, ConnectionBD baseDatos)
        {
            _tokenHelper = tokenHelper;
            _passwordHasher = passwordHasher;
            _codigoVerificacionService = codigoVerificacionService;
            _emailService = emailService;
            _baseDatos = baseDatos;

        }

        public async Task<List<ModelRegistro>> MostrarUsuario()
        {

            var lista = new List<ModelRegistro>();

            using (var sql = new MySqlConnection(_baseDatos.ConnectionMYSQL()))
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
                                Id = (int)reader["p_user_id"],
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
                            userId = reader.GetInt32("p_user_id");
                        }
                    }
                }
            }

            return userId;
        }

        public async Task<(string idString, string codigo)> EnviarCodigo(ModelEnviarCodigo parametros)
        {
            string email = string.Empty;
            
            string emailConfirmacionVerificacion = string.Empty;

            string nombreCompleto = string.Empty;

            string codigo = _codigoVerificacionService.GenerarCodigo();

            string? idString =_tokenHelper.ObtenerUserIdDesdeTokenValidado(parametros.Token);

            if (string.IsNullOrWhiteSpace(idString))
            {
                throw new TokenInvalidoException();
            }

            int userId = Convert.ToInt32(idString);
            
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

                if (emailConfirmacionVerificacion =="False") throw new EstadoEmailVerificadoException();
            }
            await _emailService.SendEmailAsync(email,nombreCompleto,codigo);
            return (idString, codigo);
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
                    cmd.Parameters.AddWithValue("p_full_name", parametros.Name);
                    cmd.Parameters.AddWithValue("p_email", parametros.Email);
                    cmd.Parameters.AddWithValue("p_pass", passHashed);
                    cmd.Parameters.AddWithValue("p_user_type", parametros.Tipo);
                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task  ConfirmarVerificacion(ModelConfirmacion parametros)
        {

            (string? idString, string? codigoString) =_tokenHelper.ObtenerUserIdCodigoDesdeTokenValidado(parametros.TokenCodigo);

            if (string.IsNullOrWhiteSpace(idString) ||string.IsNullOrWhiteSpace(codigoString))
            {
                throw new TokenInvalidoException();
            }

            int userId = Convert.ToInt32(idString);

            int codigo = Convert.ToInt32(codigoString);

            using (var sql = new MySqlConnection(_baseDatos.ConnectionMYSQL()))
            {
                if (codigo == parametros.Codigo)
                {
                    using (var cmd = new MySqlCommand("sp_actualizar_estado_verificacion", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("p_user_id", userId);
                        cmd.Parameters.AddWithValue("p_nuevo_estado", "Verificado");
                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                else
                {
                    throw new CodigoIncorrectoException();

                }
            }
        }
    }
}
