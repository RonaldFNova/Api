using API.Connection;
using API.Model;
using MySql.Data.MySqlClient;
using System.Data;
using API.Security;
using Security;
using Microsoft.VisualBasic;

namespace API.Data 
{
    public class DataRegistro 
    {

        private readonly ConnectionBD bD = new ConnectionBD();

        private readonly CodigoVerificacionService _codigoVerificacionService;

        private readonly PasswordHasher _passwordHasher;

        private readonly EmailService _emailService;

        public int userId;
        private DateTime _Fecha;

        public string codigo;

        public DataRegistro(PasswordHasher passwordHasher, CodigoVerificacionService codigoVerificacionService,EmailService emailService)
        {
            _passwordHasher = passwordHasher;
            _codigoVerificacionService = codigoVerificacionService;
            _emailService = emailService;

        }

        public async Task<List<ModelRegistro>> MostrarUsuario()
        {

            var lista = new List<ModelRegistro>();

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

            return lista;
        }

        public async Task<DateTime> InsertarUsuario(ModelRegistro parametros)
        {

            var codigoVerificacion = _codigoVerificacionService.GenerarCodigo();    

            var passHashed = _passwordHasher.HashPassword(parametros.pass);

            using (var sql = new MySqlConnection(bD.ConnectionMYSQL()))
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
                    cmd.Parameters.AddWithValue("p_codigo_verificacion", codigoVerificacion);
                    await cmd.ExecuteNonQueryAsync();
                }

                    
                using (var cmd = new MySqlCommand("sp_obtener_fecha_creacion", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_user_id", userId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            _Fecha = reader.GetDateTime("fecha_creacion");
                        }
                    }
                }
            }

            await _emailService.SendEmailAsync(parametros.email, "Código de Verificación", 
                $"Tu código de verificación es: {codigoVerificacion}");

            return _Fecha;
        }

        public async Task<DateTime> Reenviar(ModelReenviar parametros)
        {
     
            var nuevoCodigo = _codigoVerificacionService.GenerarCodigo();

            using (var sql = new MySqlConnection(bD.ConnectionMYSQL()))
            {       
                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_update_codigo_verificacion", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_user_id", parametros.id);
                    cmd.Parameters.AddWithValue("p_codigo_verificacion", nuevoCodigo);
                    await cmd.ExecuteNonQueryAsync();
                }

                using (var cmd = new MySqlCommand("sp_obtener_fecha_creacion", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_user_id", parametros.id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            _Fecha = reader.GetDateTime("fecha_creacion");
                        }
                    }
                }
            }

            await _emailService.SendEmailAsync(parametros.email, "Nuevo Código de Verificación",
            $"Tu nuevo código de verificación es: {nuevoCodigo}");

            return _Fecha;

        }

        public async Task EditarUsuario(ModelRegistro parametros)
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

        public async Task ConfirmarVerificacion(ModelConfirmacion parametros)
        {

            using (var sql = new MySqlConnection(bD.ConnectionMYSQL()))
            {
                using (var cmd = new MySqlCommand("sp_obtener_codigo_verificacion", sql))
                {
                    await sql.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_user_id", parametros.id);
                        
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            codigo = reader.GetString("codigo_verificacion");
                        }
                    }
                }

                if (codigo == parametros.codigo)
                {
                    using (var cmd = new MySqlCommand("sp_actualizar_estado_verificacion", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("p_user_id", parametros.id);
                        cmd.Parameters.AddWithValue("p_nuevo_estado", "Verificado");
                        await cmd.ExecuteNonQueryAsync();
                    }

                }
            }
        }
    }
}
