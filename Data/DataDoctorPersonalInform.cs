using API.Connection;
using API.Model;
using API.Security;
using MySql.Data.MySqlClient;
using System.Data;
namespace API.Data
{
    public class DataDoctorPersonalInform
    {
        public readonly ConnectionBD _connectionBD;
        public readonly TokenHelper _tokenHelper;

        public DataDoctorPersonalInform(ConnectionBD connectionBD, TokenHelper tokenHelper)
        {
            _connectionBD = connectionBD;
            _tokenHelper = tokenHelper;
        }

        public async Task InsertarDoctorAsync(ModelDoctorPersonalInform parametros)
        {

            string? idString = _tokenHelper.ObtenerUserIdDesdeTokenValidado(parametros.Token);

            parametros.Id = Convert.ToInt32(idString);

            using (var sql = new MySqlConnection(_connectionBD.ConnectionMYSQL()))
            {
                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_InsertarDoctor", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_nUserFK", parametros.Id);
                    cmd.Parameters.AddWithValue("p_eTipoIdentificacion", parametros.TipoId);
                    cmd.Parameters.AddWithValue("p_cNroIdentificacion", parametros.PersonalId);
                    cmd.Parameters.AddWithValue("p_cNroContacto", parametros.Cell);
                    cmd.Parameters.AddWithValue("p_cEspecialidad", parametros.Especialidad);

                    await cmd.ExecuteNonQueryAsync();

                }

                await sql.CloseAsync();
            }
        }

    }
}