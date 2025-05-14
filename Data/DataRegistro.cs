using API.Connection;
using API.Model;
using MySql.Data.MySqlClient;
using System.Data;
using API.Security;
using API.Error;

namespace API.Data 
{
    public class DataRegistro 
    {

        private readonly ConnectionBD _baseDatos;

        private readonly CodigoVerificacionService _codigoVerificacionService;

        private readonly PasswordHasher _passwordHasher;

        private readonly TokenHelper _tokenHelper;

        private readonly EmailService _emailService;

        public int userId;
        private string? _Email;
        private string nombre;
        private string email_prueba;
        public string codigo;

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
            }

            return userId;
        }

        public async Task Reenviar(ModelReenviar parametros)
        {
            string nuevoCodigo = _codigoVerificacionService.GenerarCodigo();

            string? idString =_tokenHelper.ObtenerUserIdDesdeTokenValidado(parametros.token);
            
            if (string.IsNullOrWhiteSpace(idString))
            {
                throw new TokenInvalidoException();
            }

            int id = Convert.ToInt32(idString);
            
            using (var sql = new MySqlConnection(_baseDatos.ConnectionMYSQL()))
            {       
                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_update_codigo_verificacion", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_user_id", id);
                    cmd.Parameters.AddWithValue("p_codigo_verificacion", nuevoCodigo);
                    await cmd.ExecuteNonQueryAsync();
                }

                using (var cmd = new MySqlCommand("sp_checkUserVerificationStatus", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_user_id", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            email_prueba = reader.GetString("user_not_verified");
                        }
                    }
                }

                using (var cmd = new MySqlCommand("sp_get_email_by_user_id", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_user_id", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            _Email = reader.GetString("email");
                        }
                    }
                }

                if (email_prueba =="False") throw new EstadoEmailVerificadoException();
            }
            await _emailService.SendEmailAsync(_Email,nombre,nuevoCodigo);
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
            string? idString =_tokenHelper.ObtenerUserIdDesdeTokenValidado(parametros.token);
            
            if (string.IsNullOrWhiteSpace(idString))
            {
                throw new TokenInvalidoException();
            }

            int id = Convert.ToInt32(idString);

            using (var sql = new MySqlConnection(_baseDatos.ConnectionMYSQL()))
            {
                using (var cmd = new MySqlCommand("sp_obtener_codigo_verificacion", sql))
                {
                    await sql.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_user_id", id);
                        
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            codigo = reader.GetString("codigo_verificacion");
                        }

                        else
                        {
                            throw new UsuarioNoEncontradoException();
                        }

                    }
                }

                if (codigo == parametros.codigo)
                {
                    using (var cmd = new MySqlCommand("sp_actualizar_estado_verificacion", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("p_user_id", id);
                        cmd.Parameters.AddWithValue("p_nuevo_estado", "Verificado");
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
