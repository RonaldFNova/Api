using API.Connection;
using MySql.Data.MySqlClient;
using System.Data;

namespace API.Data
{
    public class DataClasificarMedico
    {
        private readonly ConnectionBD _connectionBD;

        public DataClasificarMedico(ConnectionBD connectionBD)
        {
            _connectionBD = connectionBD;
        }


        public async Task<List<string>> ObtenerListaMedicoClasificarAsync(ModelClasificarMedico parametros)
        {
            var listaNombre = new List<string>();

            string nombre = string.Empty;

            using (var sql = new MySqlConnection(_connectionBD.ConnectionMYSQL()))
            {
                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_ObtenerMedicosPorEspecialidad", sql))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_especialidad", parametros.Tipo);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {

                        while (await reader.ReadAsync())
                        {
                            nombre = reader.GetString("NombreMedico");
                            listaNombre.Add(nombre);

                        }
                    }
                }

                return listaNombre;
            }
        }
    }
}