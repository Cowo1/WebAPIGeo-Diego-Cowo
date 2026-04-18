namespace WebAPIGeo.Modelo
{
    public class Alumno
    {
        public int Id { get; set; }
        public string Carnet { get; set; }
        public string NombreCompleto { get; set; }
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
    }
}
