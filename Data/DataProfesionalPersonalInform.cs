using API.Connection;
using API.Model;
using API.Security;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text.Json;

namespace API.Data
{
    public class DataProfesionalPersonalInform
    {
        public readonly ConnectionBD _connectionBD;
        public readonly TokenHelper _tokenHelper;

        public DataProfesionalPersonalInform(ConnectionBD connectionBD, TokenHelper tokenHelper)
        {
            _connectionBD = connectionBD;
            _tokenHelper = tokenHelper;
        }

        public async Task InsertarProfesionalAsync(ModelProfesionalPersonalInform parametros)
        {

            string? idString = _tokenHelper.ObtenerUserIdDesdeTokenValidado(parametros.Token);

            parametros.Id = Convert.ToInt32(idString);

            using (var sql = new MySqlConnection(_connectionBD.ConnectionMYSQL()))
            {
                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_InsertarMedico", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_nUserFK", parametros.Id);
                    cmd.Parameters.AddWithValue("p_eTipoIdentificacion", parametros.TipoId);
                    cmd.Parameters.AddWithValue("p_cNroIdentificacion", parametros.PersonalId);
                    cmd.Parameters.AddWithValue("p_cNroContacto", parametros.Cell);
                    cmd.Parameters.AddWithValue("p_especialidades", JsonSerializer.Serialize(parametros.Especialidad));

                    await cmd.ExecuteNonQueryAsync();

                }

                await sql.CloseAsync();
            }
        }

    }
}