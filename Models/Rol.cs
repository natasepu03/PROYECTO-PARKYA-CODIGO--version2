namespace ParkYa.Models
{
    public class Rol
    {
        public int id_rol { get; set; }

        public int cod_rol { get; set; }

        public required string nombre_rol { get; set; }

        public required string descripcion { get; set; }

        public List<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}