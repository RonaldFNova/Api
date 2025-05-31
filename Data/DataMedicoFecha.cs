using API.Model;
using API.Connection;
using MySql.Data.MySqlClient;
using System.Data;


namespace API.Data
{
    public class DataMedicoFecha
    {
        private readonly ConnectionBD _connectionBD;

        public DataMedicoFecha(ConnectionBD connectionBD)
        {
            _connectionBD = connectionBD;
        }

        public async Task<int> MostraFechaMedicaAsync(ModelMedicoFecha parametros)
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
        public async Task<List<string>> ObternerFechasDisponiblesAsync(int id)
        {
            var disponibilidad = new List<string>();

            using (var sql = new MySqlConnection(_connectionBD.ConnectionMYSQL()))
            {
                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_ObtenerHorariosDisponibles", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_nProfesionalID", id);
                    cmd.Parameters.AddWithValue("p_fechaDesde", DateTime.Now);
                    cmd.Parameters.AddWithValue("p_fechaHasta", DateTime.Now.AddDays(14));

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string fecha = reader.GetDateTime("dFecha").ToString("yyyy-MM-dd");
                            string horaInicio = ((TimeSpan)reader["tHoraInicio"]).ToString(@"hh\:mm");
                            string horaFin = ((TimeSpan)reader["tHoraFin"]).ToString(@"hh\:mm");

                            disponibilidad.Add($"{fecha} ({horaInicio} - {horaFin})");
                        }
                    }
                }
            }

            return disponibilidad;
        }



    }
}