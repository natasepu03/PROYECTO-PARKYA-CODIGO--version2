namespace ParkYa.Models
{
    public class Usuario
    {
        public int id_usuario { get; set; }
        public string tipo_doc { get; set; } = string.Empty;
        public int documento { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string apellido { get; set; } = string.Empty;
        public string correo { get; set; } = string.Empty;
        public string telefono { get; set; } = string.Empty;
        public string contraseña { get; set; } = string.Empty;
        public bool estado { get; set; }

        public int Rol_id_rol { get; set; }
        public Rol? Rol { get; set; }

        public List<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
        public List<Reserva> Reservas { get; set; } = new List<Reserva>();
        public List<Venta> Ventas { get; set; } = new List<Venta>();
    }
}