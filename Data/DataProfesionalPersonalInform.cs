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
        public DataProfesionalPersonalInform(ConnectionBD connectionBD)
        {
            _connectionBD = connectionBD;
        }

        public async Task InsertarProfesionalAsync(ModelProfesionalPersonalInform parametros)
        {

            using (var sql = new MySqlConnection(_connectionBD.ConnectionMYSQL()))
            {
                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_InsertarProfesional", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_nUserFK", parametros.Id);
                    cmd.Parameters.AddWithValue("p_eTipoIdentificacion", parametros.TipoId);
                    cmd.Parameters.AddWithValue("p_cNroIdentificacion", parametros.PersonalId);
                    cmd.Parameters.AddWithValue("p_cNroContacto", parametros.Cell);
                    cmd.Parameters.AddWithValue("p_datos", JsonSerializer.Serialize(parametros.Especialidad));
                    cmd.Parameters.AddWithValue("p_cDireccion", parametros.Direccion);


                    await cmd.ExecuteNonQueryAsync();

                }

                await sql.CloseAsync();
            }
        }

    }
}