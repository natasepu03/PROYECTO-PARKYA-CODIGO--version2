using ParkYa.Models;
namespace ParkYa.ViewModels
{
public class AdministradorUsuariosViewModel
{
    public Usuario Administrador { get; set; } = new Usuario();
    public List<UsuarioViewModel> EmpleadosActivos { get; set; }= new();
    public List<UsuarioViewModel> EmpleadosInactivos { get; set; }= new();

    public List<UsuarioViewModel> ClientesActivos { get; set; }= new();
    public List<UsuarioViewModel> ClientesInactivos { get; set; }= new();

    // Totales (como en reservas)
    public int TotalEmpleados { get; set; }
    public int TotalClientes { get; set; }
}

public class UsuarioViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string NombreCompleto { get; set; }= string.Empty;
    public string Correo { get; set; }= string.Empty;
    public string Telefono { get; set; }= string.Empty;
    public string Documento { get; set; }= string.Empty;
    public string TipoDocumento { get; set; }= string.Empty;
    public string Rol { get; set; } = string.Empty;
    public int RolId { get; set; }
    public string Estado { get; set; } = string.Empty; // Activo / Inactivo
}
}
