using API.Model;
using API.Connection;
using MySql.Data.MySqlClient;
using System.Data;

namespace API.Data
{
    public class DataMostrarCitaFecha
    {
        private readonly ConnectionBD _connectionBD;

        public DataMostrarCitaFecha(ConnectionBD connectionBD)
        {
            _connectionBD = connectionBD;
        }

        public async Task<List<ModelListaMostrarCitaFecha>> MostrarCitaAsync(ModelMostrarCitaFecha parametros)
        {
            var lista = new List<ModelListaMostrarCitaFecha>();

            using (var sql = new MySqlConnection(_connectionBD.ConnectionMYSQL()))
            {
                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_ProfesionalesDisponiblesPorFechaYEspecialidad", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("fechaConsulta", parametros.Fecha);
                    cmd.Parameters.AddWithValue("especialidadNombre", parametros.Especialidad);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var valore = new ModelListaMostrarCitaFecha
                            {
                                Id = reader.GetInt32("nDisponibilidadID"),
                                Nombre = reader.GetString("Nombre"),
                                Fecha = Convert.ToDateTime(reader["Fecha"]).ToString("yyyy-MM-dd"),
                                FechaInicio = TimeSpan.Parse(reader["tHoraInicio"].ToString()).ToString(@"hh\:mm"),
                                FechaFinal = TimeSpan.Parse(reader["tHoraFin"].ToString()).ToString(@"hh\:mm")

                            };

                            lista.Add(valore);
                        }
                    }
                }

                await sql.CloseAsync();
            }


            return lista;
        }
    }
}