using API.Connection;
using API.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace API.Data
{
    public class DataInsertarCitaMedicaId
    {
        private readonly ConnectionBD _connectionBD;

        public DataInsertarCitaMedicaId(ConnectionBD connectionBD)
        {
            _connectionBD = connectionBD;
        }

        public async Task<int> ObtenerIdPacienteAsync(int id)
        {
            int Idpaciente = 0;

            using (var sql = new MySqlConnection(_connectionBD.ConnectionMYSQL()))
            {
                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_ObtenerIdPaciente", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("p_nUserFK",id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            Idpaciente = reader.GetInt32("nPacienteID");
                        }
                    }
                }

                await sql.CloseAsync();
            }

            return Idpaciente;
        }

        public async Task InsertarCitaMedicaIdAsync(ModelInsertarCitaMedicaId parametros, int id)
        {
            using (var sql = new MySqlConnection(_connectionBD.ConnectionMYSQL()))
            {
                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_CrearCitaPorDisponibilidad", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("p_nDisponibilidadID", parametros.Id);
                    cmd.Parameters.AddWithValue("p_cMotivoConsulta", parametros.MotivoConsulta);
                    cmd.Parameters.AddWithValue("p_nPacienteID", id);

                    await cmd.ExecuteNonQueryAsync();

                }

                await sql.CloseAsync();
            }

        }
    }
}