using API.Connection;
using API.Model;
using API.Security;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Pqc.Crypto.Cmce;
using Sprache;
using System.Data;

namespace API.Data
{
    public class dataTipoUser
    {
        public readonly TokenHelper _tokenHelper;
        public readonly ConnectionBD _connection;

        public dataTipoUser(ConnectionBD connection, TokenHelper tokenHelper)
        {
            _connection = connection;
            _tokenHelper = tokenHelper;
        }

        public async Task tipoUserAsyns(ModelTipoUser parametros)
        {
            string? idString = _tokenHelper.ObtenerUserIdDesdeTokenValidado(parametros.Token);
            int id = Convert.ToInt32(idString);


            using (var sql = new MySqlConnection(_connection.ConnectionMYSQL()))
            {
                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_ObtenerRolUsuario", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("pUserID", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            parametros.Tipo = reader.GetString("eRolUsuario");   
                        }
                    }
                }
                
                await sql.CloseAsync();
            }
        }
    }
}