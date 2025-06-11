using API.Connection;
using API.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace API.Data
{
    public class DataMostrarCitasPaciente
    {
        public ConnectionBD _connection;
        public DataMostrarCitasPaciente(ConnectionBD connection)
        {
            _connection = connection;
        }


        public async Task<int> ObtenerIdPacienteAsync(int id)
        {
            int Idpaciente = 0;

            using (var sql = new MySqlConnection(_connection.ConnectionMYSQL()))
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

        public async Task<List<ModelMostrarCitasPaciente>> MostrarCitasPacienteAsync(int id)
        {
            var lista = new List<ModelMostrarCitasPaciente>();

            using (var sql = new MySqlConnection(_connection.ConnectionMYSQL()))
            {
                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_ObtenerProximasCitasPaciente", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("p_pacienteID", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var valores = new ModelMostrarCitasPaciente
                            {
                                Fecha = Convert.ToDateTime(reader["dFecha"]).ToString("yyyy-MM-dd"),
                                HoraInicio = TimeSpan.Parse(reader["tHoraInicio"].ToString()).ToString(@"hh\:mm"),
                                HoraFinal = TimeSpan.Parse(reader["tHoraFin"].ToString()).ToString(@"hh\:mm"),
                                MotivoConsulta = reader["cMotivoConsulta"].ToString(),
                                Duracion = reader["cDuracion"].ToString(),
                                MedicoTratante = reader["MedicoTratante"].ToString(),
                                EstadoCita = reader["eEstadoCita"].ToString()
                            };

                            lista.Add(valores);
                        }
                    }
                }

                await sql.CloseAsync();
            }


            return lista;
        }
    }
}