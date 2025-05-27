using System.Data;
using API.Connection;
using API.Model;
using API.Security;
using MySql.Data.MySqlClient;

namespace API.Data
{
    public class DataPatientPersonalInform
    {
        private readonly ConnectionBD _baseDatos;
        public DataPatientPersonalInform(ConnectionBD baseDatos)
        {
            _baseDatos = baseDatos;
        }


        public async Task InsertInformacionPersonal(ModelPatientPersonalInform parametros)
        {

            int pacienteId = 0;

            using (var sql = new MySqlConnection(_baseDatos.ConnectionMYSQL()))
            {
                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_insertarPaciente", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_nUserFK", parametros.Id);
                    cmd.Parameters.AddWithValue("p_eTipoIdentificacion", parametros.TipoId);
                    cmd.Parameters.AddWithValue("p_cNroIdentificacion", parametros.PersonalId);
                    cmd.Parameters.AddWithValue("p_cNroContacto", parametros.Cell);
                    cmd.Parameters.AddWithValue("p_eGrupoSanguineo", parametros.BloodGroup);

                    await cmd.ExecuteNonQueryAsync();
                }

                using (var cmd = new MySqlCommand("sp_getPacienteIdByUserId", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_nUserID", parametros.Id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            pacienteId = reader.GetInt32("nPacienteID");
                        }
                    }
                }

                if (parametros.AlergiasGeneral.Count != 0 && parametros.AlergiasGeneral != null)
                {
                    foreach (var alergia in parametros.AlergiasGeneral)
                    {
                        using (var cmd = new MySqlCommand("sp_insertAlergiaGeneral", sql))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("p_nPacienteFK", pacienteId);
                            cmd.Parameters.AddWithValue("p_cNombreAlergia", alergia);

                            await cmd.ExecuteNonQueryAsync();

                        }
                    }
                }

                if (parametros.AlergiasMedications.Count != 0 && parametros.AlergiasMedications != null)
                {
                    foreach (var alergia in parametros.AlergiasMedications)
                    {
                        using (var cmd = new MySqlCommand("sp_insertAlergiaMedicamento", sql))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("p_nPacienteFK", pacienteId);
                            cmd.Parameters.AddWithValue("p_cNombreMedicamento", alergia);

                            await cmd.ExecuteNonQueryAsync();

                        }
                    }
                }
            }
        }


        public async Task<List<ModelPatientPersonalInform>> MostrarInformacionPersonal()
        {

            var lista = new List<ModelPatientPersonalInform>();

            using (var sql = new MySqlConnection(_baseDatos.ConnectionMYSQL()))
            {
                using (var cmd = new MySqlCommand("sp_MostrarPacientes", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var paciente = new ModelPatientPersonalInform
                            {
                                Id = (int)reader["nPacienteID"],
                                Cell = reader["cNroContacto"] != DBNull.Value ? (string)reader["cNroContacto"] : string.Empty,
                                TipoId = reader["eTipoIdentificacion"] != DBNull.Value ? (string)reader["eTipoIdentificacion"] : string.Empty,
                                PersonalId = reader["cNroIdentificacion"] != DBNull.Value ? (string)reader["cNroIdentificacion"] : string.Empty,
                                BloodGroup = reader["eGrupoSanguineo"] != DBNull.Value ? (string)reader["eGrupoSanguineo"] : string.Empty

                            };

                            lista.Add(paciente);
                        }
                    }
                }
            }

            return lista;
        }


        public async Task EditarInformacionPersonal(ModelPatientPersonalInform parametros)
        {

            using (var sql = new MySqlConnection(_baseDatos.ConnectionMYSQL()))
            {
                using (var cmd = new MySqlCommand("sp_ActualizarPaciente", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_nPacienteID", parametros.Id);
                    cmd.Parameters.AddWithValue("p_eTipoIdentificacion", parametros.TipoId);
                    cmd.Parameters.AddWithValue("p_cNroIdentificacion", parametros.PersonalId);
                    cmd.Parameters.AddWithValue("p_cNroContacto", parametros.Cell);
                    cmd.Parameters.AddWithValue("p_eGrupoSanguineo", parametros.BloodGroup);
                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    
        public async Task EliminarInformacionPersonal(int userId)
        {
            using (var sql = new MySqlConnection(_baseDatos.ConnectionMYSQL()))
            {

                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_EliminarPaciente", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_nPacienteID", userId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}