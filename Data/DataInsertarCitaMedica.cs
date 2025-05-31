using API.Model;
using API.Connection;
using MySql.Data.MySqlClient;
using System.Data;

namespace API.Data
{
    public class DataInsertarCitaMedica
    {
        private readonly ConnectionBD _connectionBD;

        public DataInsertarCitaMedica(ConnectionBD connectionBD)
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

        public async Task<int> InsertarNombreIdAsync(ModelInsertarCitaMedica parametros)
        {
            int id = 0;

            using (var sql = new MySqlConnection(_connectionBD.ConnectionMYSQL()))
            {

                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_ObtenerIdMedicoPorNombre", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;


                    cmd.Parameters.AddWithValue("p_nombreMedico", parametros.Nombre);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            id = reader.GetInt32("nProfesionalID");
                        }
                    }

                }
                await sql.CloseAsync();
            }


            return id;
        }

        public async Task InsertarCitaMedicaAsync(ModelInsertarCitaMedica parametros, int idMedico, int Idpaciente)
        {
            using (var sql = new MySqlConnection(_connectionBD.ConnectionMYSQL()))
            {
                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_InsertarCitaMedica", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("p_nPacienteFK", Idpaciente);
                    cmd.Parameters.AddWithValue("p_nMedicoFK", idMedico);
                    cmd.Parameters.AddWithValue("p_dFecha", parametros.DiaFecha);
                    cmd.Parameters.AddWithValue("p_tHoraInicio", parametros.FechaInicio);
                    cmd.Parameters.AddWithValue("p_tHoraFin", parametros.FechaFinal);
                    cmd.Parameters.AddWithValue("p_cMotivoConsulta", parametros.MotivoConsulta);

                    await cmd.ExecuteNonQueryAsync();

                }

                await sql.CloseAsync();
            }
        }
    }
}