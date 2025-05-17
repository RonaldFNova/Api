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
        private readonly TokenHelper _tokenHelper;
        public DataPatientPersonalInform(ConnectionBD baseDatos, TokenHelper tokenHelper)
        {
            _baseDatos = baseDatos;
            _tokenHelper = tokenHelper;
        }


        public async Task InsertInformacionPersonal(ModelPatientPersonalInform modelo)
        {

            string? idString = _tokenHelper.ObtenerUserIdDesdeTokenValidado(modelo.Token);

            int userId = Convert.ToInt32(idString);

            int pacienteId = 0;

            using (var sql = new MySqlConnection(_baseDatos.ConnectionMYSQL()))
            {
                await sql.OpenAsync();

                using (var cmd = new MySqlCommand("sp_insertarPaciente", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_nUserFK", userId);
                    cmd.Parameters.AddWithValue("p_eTipoIdentificacion", modelo.TipoId);
                    cmd.Parameters.AddWithValue("p_cNroIdentificacion", modelo.PersonalId);
                    cmd.Parameters.AddWithValue("p_cNroContacto", modelo.Cell);
                    cmd.Parameters.AddWithValue("p_eGrupoSanguineo", modelo.BloodGroup);

                    await cmd.ExecuteNonQueryAsync();
                }

                using (var cmd = new MySqlCommand("sp_getPacienteIdByUserId", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_nUserID", userId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            pacienteId = reader.GetInt32("nPacienteID");
                        }
                    }
                }

                if (modelo.AlergiasGeneral.Count != 0 && modelo.AlergiasGeneral != null)
                {
                    foreach (var alergia in modelo.AlergiasGeneral)
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

                if (modelo.AlergiasMedications.Count != 0 && modelo.AlergiasMedications != null)
                {
                    foreach (var alergia in modelo.AlergiasMedications)
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
    }
}