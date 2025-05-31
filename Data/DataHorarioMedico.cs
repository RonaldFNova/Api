using API.Connection;
using API.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace API.Data
{
    public class DataHorarioMedico
    {
        private readonly ConnectionBD _connectionBD;

        public DataHorarioMedico(ConnectionBD connectionBD)
        {
            _connectionBD = connectionBD;
        }

        public async Task<int> ObtenerIdProfesionalAsync(int id)
        {
            int medicoId = 0;

            using (var sql = new MySqlConnection(_connectionBD.ConnectionMYSQL()))
            {
                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_ObtenerProfesionalPorUserID", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_nUserFK", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            medicoId = reader.GetInt32("nProfesionalID");
                        }
                    }

                }

            }

            return medicoId;

        }
        

        public async Task InsertaDiasMedicoAsync(int id)
        {

            using (var sql = new MySqlConnection(_connectionBD.ConnectionMYSQL()))
            {
                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_GenerarDisponibilidad", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("p_nProfesionalFK", id);
                    cmd.Parameters.AddWithValue("p_fechaInicio", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("p_duracionCita", 60);

                    await cmd.ExecuteNonQueryAsync();
                }

                await sql.CloseAsync();

            }
        }

        public async Task InsertHorarioMedicoAsync(int id, ModelHorarioMedico parametros)
        {

            using (var sql = new MySqlConnection(_connectionBD.ConnectionMYSQL()))
            {
                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_InsertarHorarioProfesional", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("p_nProfesionalFK", id);
                    cmd.Parameters.AddWithValue("p_eDiaSemana", parametros.Dia);
                    cmd.Parameters.AddWithValue("p_tHoraInicio", parametros.Inicio);
                    cmd.Parameters.AddWithValue("p_tHoraFin", parametros.Final);

                    await cmd.ExecuteNonQueryAsync();

                }

                await sql.CloseAsync();
            }
        }

        public async Task InsertHorarioPorDefectoAsync(int id)
        {
            var listaDiasDefectos = new List<string> { "Lunes", "Martes", "Miercoles", "Jueves", "Viernes" };

            Console.WriteLine(id);

            var listaHorarioManañaDefecto = new List<TimeSpan>
            {
                new TimeSpan(6,0,0),
                new TimeSpan(11,0,0)
            };

            var listaHorarioTardeDefecto = new List<TimeSpan>
            {
                new TimeSpan(14,0,0),
                new TimeSpan(17,0,0)
            };

            var listaTotalhorario = new List<List<TimeSpan>>
            {
                listaHorarioManañaDefecto,
                listaHorarioTardeDefecto
            };


            using var sql = new MySqlConnection(_connectionBD.ConnectionMYSQL());
      
            foreach (var dias in listaDiasDefectos)
            {

            using var cmd = new MySqlCommand("sp_InsertarHorarioProfesional", sql)
            {
                CommandType = CommandType.StoredProcedure
            };

                await sql.OpenAsync();

                foreach (var horario in listaTotalhorario)
                {
                    cmd.Parameters.Clear();

                    cmd.Parameters.AddWithValue("p_nProfesionalFK", id);
                    cmd.Parameters.AddWithValue("p_eDiaSemana", dias);
                    cmd.Parameters.AddWithValue("p_tHoraInicio", horario[0]);
                    cmd.Parameters.AddWithValue("p_tHoraFin", horario[1]);

                    await cmd.ExecuteNonQueryAsync();
                }

                await sql.CloseAsync();
            }
        }
    }

}