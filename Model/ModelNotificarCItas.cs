namespace API.Model
{
    public class ModelNotificarCitas
    {
        public string NombrePaciente { get; set; }
        public string Correo { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan HoraInicio { get; set; }
    }
}
