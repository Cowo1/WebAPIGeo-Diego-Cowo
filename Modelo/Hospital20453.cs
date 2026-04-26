using System.ComponentModel.DataAnnotations;

namespace WebAPIGeo.Modelo
{
    public class Hospital20453
    {
        [Key]
        public string IdPaciente { get; set; } = string.Empty;
        public string NombrePaciente { get; set; } = string.Empty;
        public int Edad { get; set; }
        public int NivelGravedad { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string MedicoResponsable { get; set; } = string.Empty;
        public DateTime FechaIngreso { get; set; }
    }
}
