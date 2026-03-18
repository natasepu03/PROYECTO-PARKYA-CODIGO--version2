using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkYa.Data;
using ParkYa.Models;
using ParkYa.Models.ViewModels;

namespace ParkYa.Controllers
{
    public class ReservaController : Controller
    {
        private readonly ParkYaDbContext _context;

        public ReservaController(ParkYaDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(ReservaViewModel model)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Autenticacion");

            if (!ModelState.IsValid)
                return RedirectToAction("Index", "Clientes");

            var tarifa = await _context.tarifas.FirstOrDefaultAsync();
            if (tarifa == null)
            {
                TempData["Error"] = "No hay tarifas configuradas.";
                return RedirectToAction("Index", "Clientes");
            }

            var reserva = new Reserva
            {
                cod_reserva = new Random().Next(100000, 999999),
                fecha = model.Fecha.Date,
                hora_entrada = model.HoraEntrada,
                hora_salida = model.HoraSalida,
                Estado = Estado.Pendiente,
                Usuario_id_usuario = usuarioId.Value,
                Tarifas_id_tarifas = tarifa.id_tarifas,
                Vehiculo_id_vehiculo = model.VehiculoId
            };

            _context.reserva.Add(reserva);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Reserva creada correctamente.";
            return RedirectToAction("Index", "Clientes");
        }
    }
}