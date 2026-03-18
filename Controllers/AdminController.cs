using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkYa.Data;

namespace ParkYa.Controllers
{
    public class AdminController : Controller
    {
        private readonly ParkYaDbContext _context;

        public AdminController(ParkYaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Autenticacion");

            ViewBag.TotalUsuarios = await _context.usuario.CountAsync();
            ViewBag.TotalVehiculos = await _context.vehiculo.CountAsync();
            ViewBag.TotalReservas = await _context.reserva.CountAsync();
            ViewBag.TotalVentas = await _context.venta.CountAsync();

            return View();
        }
    }
}