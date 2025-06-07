using API.Connection;
using MySql.Data.MySqlClient;
using System.Data;

namespace API.Data
{
    public class DataListaEspecialidades
    {
        private readonly ConnectionBD _connectionBD;

        public DataListaEspecialidades(ConnectionBD connectionBD)
        {
            _connectionBD = connectionBD;
        }


        public async Task<List<string>> ObtenerListaEspecialidades()
        {
            var lista = new List<string>();


            using (var sql = new MySqlConnection(_connectionBD.ConnectionMYSQL()))
            {
                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_ListarEspecialidades", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(reader.GetString("Especialidad"));
                        }
                    }
                }

                await sql.CloseAsync();
            }

            return lista;
        }
    }
}