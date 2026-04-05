using Microsoft.AspNetCore.Mvc;
using ParkYa.Data;
using ParkYa.ViewModels;
using ParkYa.Models;


namespace ParkYa.Controllers
{

     public class AdministradorController : Controller
   {
    private readonly ParkYaDbContext _context;

    public AdministradorController(ParkYaDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index()
   {
    var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
    if (usuarioId == null)
        return RedirectToAction("Login", "Autenticacion");

    var usuario = _context.usuario
        .FirstOrDefault(u => u.id_usuario == usuarioId);

    if (usuario == null)
        return RedirectToAction("Login", "Autenticacion");

    // 🔥 NUEVO: traer parqueadero
    var parqueadero = _context.parqueadero
    .FirstOrDefault(p => p.id_Parqueadero == 1);

    // 🔥 Si no existe, lo creamos
    if (parqueadero == null)
    {
        parqueadero = new Parqueadero
        {
            nombre = "Mi Parqueadero",
            direccion = "Sin dirección",
            total_cupos = 0,
            cod_parqueadero = 1
        };

        _context.parqueadero.Add(parqueadero);
        _context.SaveChanges();
    }

    var dashboard = new DashboardAdminViewModel
    {
        Administrador = usuario,
        Parqueadero = parqueadero // 👈 IMPORTANTE
    };

    return View(dashboard);
}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ActualizarPerfilAdmin(Usuario modelo)
    {
        var usuario = _context.usuario
            .FirstOrDefault(u => u.id_usuario == modelo.id_usuario);

        if (usuario == null)
            return RedirectToAction("Index");

        usuario.nombre = modelo.nombre;
        usuario.apellido = modelo.apellido;
        usuario.correo = modelo.correo;

        _context.SaveChanges();

        TempData["PerfilActualizado"] = "Perfil actualizado correctamente";

        return RedirectToAction("Index");
    }
   [HttpPost]
[ValidateAntiForgeryToken]
public IActionResult ActualizarParqueadero([FromForm] Parqueadero modelo)
{
    var parqueadero = _context.parqueadero.Find(modelo.id_Parqueadero);

    if (parqueadero == null)
    {
        TempData["Error"] = "No se encontró el parqueadero";
        return RedirectToAction("Index");
    }

    parqueadero.nombre = modelo.nombre;
    parqueadero.direccion = modelo.direccion;
    parqueadero.total_cupos = modelo.total_cupos;

    _context.SaveChanges();

    TempData["ParqueaderoActualizado"] = "Parqueadero actualizado correctamente";

    return RedirectToAction("Index");
}

    
   }
   
}
