using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkYa.Data;

namespace ParkYa.Controllers
{
    public class EmpleadoController : Controller
    {
        private readonly ParkYaDbContext _context;

        public EmpleadoController(ParkYaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Autenticacion");

            ViewBag.TotalVehiculos = await _context.vehiculo.CountAsync();
            ViewBag.TotalReservas = await _context.reserva.CountAsync();
            ViewBag.ReservasActivas = await _context.reserva.CountAsync(r => r.Estado == Models.Estado.Activo);

            return View();
        }
    }
}