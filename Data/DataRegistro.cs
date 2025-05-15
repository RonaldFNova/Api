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

            return lista;
        }

        public async Task<int> InsertarUsuario(ModelRegistro parametros)
        {
            int userId = 0;

            var codigoVerificacion = _codigoVerificacionService.GenerarCodigo();    

            var passHashed = _passwordHasher.HashPassword(parametros.pass);

            using (var sql = new MySqlConnection(_baseDatos.ConnectionMYSQL()))
            {
                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_insert_user", sql))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_full_name", parametros.name);
                    cmd.Parameters.AddWithValue("p_email", parametros.email);
                    cmd.Parameters.AddWithValue("p_pass", passHashed);
                    cmd.Parameters.AddWithValue("p_user_type", parametros.tipo);
                    await cmd.ExecuteNonQueryAsync();
                }

                using (var cmd = new MySqlCommand("sp_get_user_id_by_email", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_email", parametros.email);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            userId = reader.GetInt32("user_id");
                        }
                    }
                }

                using (var cmd = new MySqlCommand("sp_insert_verificacion", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_user_id", userId);
                    await cmd.ExecuteNonQueryAsync();
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

            string? idString =_tokenHelper.ObtenerUserIdDesdeTokenValidado(parametros.token);

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
                            email = reader.GetString("email");
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
                            nombreCompleto = reader.GetString("full_name");
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
  
            var passHashed = _passwordHasher.HashPassword(parametros.pass);

            using (var sql = new MySqlConnection(_baseDatos.ConnectionMYSQL()))
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

        public async Task  ConfirmarVerificacion(ModelConfirmacion parametros)
        {

            (string? idString, string? codigoString) =_tokenHelper.ObtenerUserIdCodigoDesdeTokenValidado(parametros.tokenCodigo);

            if (string.IsNullOrWhiteSpace(idString) ||string.IsNullOrWhiteSpace(codigoString))
            {
                throw new TokenInvalidoException();
            }

            int userId = Convert.ToInt32(idString);

            int codigo = Convert.ToInt32(codigoString);

            using (var sql = new MySqlConnection(_baseDatos.ConnectionMYSQL()))
            {
                if (codigo == parametros.codigo)
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
