using API.Connection;
using API.Model;
using API.Security;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Pqc.Crypto.Cmce;
using Sprache;
using System.Data;

namespace API.Data
{
    public class DataTipoUser
    {
        public readonly ConnectionBD _connection;

        public DataTipoUser(ConnectionBD connection)
        {
            _connection = connection;
        }

        public async Task tipoUserAsyns(ModelTipoUser parametros)
        {
        
            using (var sql = new MySqlConnection(_connection.ConnectionMYSQL()))
            {
                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_ObtenerRolUsuario", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("pUserID", parametros.Id);

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