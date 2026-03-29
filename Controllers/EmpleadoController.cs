using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkYa.Data;
using ParkYa.ViewModels;

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

            var usuario = await _context.usuario
                .FirstOrDefaultAsync(u => u.id_usuario == usuarioId.Value);

            if (usuario == null)
                return RedirectToAction("Login", "Autenticacion");

            var vehiculosActivos = await (
                from r in _context.reserva
                join v in _context.vehiculo on r.Vehiculo_id_vehiculo equals v.id_vehiculo
                join u in _context.usuario on v.Usuario_id_usuario equals u.id_usuario
                where r.Estado == ParkYa.Models.Estado.Activo
                select new VehiculoActivoItemViewModel
                {
                    Placa = v.placa,
                    Marca = v.marca,
                    Color = v.color,
                    Propietario = u.nombre + " " + u.apellido,
                    Fecha = r.fecha,
                    HoraEntrada = r.hora_entrada,
                    HoraSalida = r.hora_salida,
                    Estado = r.Estado.ToString()
                }
            ).ToListAsync();

            var vm = new DashboardEmpleadoViewModel
            {
                Empleado = usuario,
                TotalVehiculos = await _context.vehiculo.CountAsync(),
                TotalReservas = await _context.reserva.CountAsync(),
                ReservasActivas = await _context.reserva.CountAsync(r => r.Estado == ParkYa.Models.Estado.Activo),
                VehiculosActivos = vehiculosActivos
            };

            return View(vm);
        }
        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ActualizarPerfilEmpleado(int id_usuario, string nombre, string apellido, string correo)
{
    var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
    if (usuarioId == null)
        return RedirectToAction("Login", "Autenticacion");

    // Seguridad: el empleado solo puede editar su propio perfil
    if (usuarioId.Value != id_usuario)
        return RedirectToAction("Index");

    var usuario = await _context.usuario.FirstOrDefaultAsync(u => u.id_usuario == id_usuario);
    if (usuario == null)
        return RedirectToAction("Index");

    usuario.nombre = nombre?.Trim() ?? "";
    usuario.apellido = apellido?.Trim() ?? "";
    usuario.correo = correo?.Trim() ?? "";

    await _context.SaveChangesAsync();

    TempData["PerfilActualizado"] = "Perfil actualizado correctamente.";
    return RedirectToAction("Index");
}

        [HttpGet]
        public async Task<IActionResult> Vehiculos()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Autenticacion");

            var usuario = await _context.usuario
                .FirstOrDefaultAsync(u => u.id_usuario == usuarioId.Value);

            if (usuario == null)
                return RedirectToAction("Login", "Autenticacion");

            var hoy = DateTime.Today;

            var vehiculosDentro = await (
                from r in _context.reserva
                join v in _context.vehiculo on r.Vehiculo_id_vehiculo equals v.id_vehiculo
                join u in _context.usuario on v.Usuario_id_usuario equals u.id_usuario
                where r.Estado == ParkYa.Models.Estado.Activo
                select new MovimientoVehiculoItemViewModel
                {
                    Placa = v.placa,
                    Marca = v.marca,
                    Color = v.color,
                    Propietario = u.nombre + " " + u.apellido,
                    Fecha = r.fecha,
                    HoraEntrada = r.hora_entrada,
                    HoraSalida = r.hora_salida,
                    Estado = r.Estado.ToString()
                }
            ).ToListAsync();

            var historialSalidasHoy = await (
                from r in _context.reserva
                join v in _context.vehiculo on r.Vehiculo_id_vehiculo equals v.id_vehiculo
                join u in _context.usuario on v.Usuario_id_usuario equals u.id_usuario
                where r.fecha.Date == hoy && r.Estado != ParkYa.Models.Estado.Activo
                select new MovimientoVehiculoItemViewModel
                {
                    Placa = v.placa,
                    Marca = v.marca,
                    Color = v.color,
                    Propietario = u.nombre + " " + u.apellido,
                    Fecha = r.fecha,
                    HoraEntrada = r.hora_entrada,
                    HoraSalida = r.hora_salida,
                    Estado = r.Estado.ToString()
                }
            ).OrderByDescending(x => x.HoraSalida).ToListAsync();

            var vm = new EmpleadoVehiculosViewModel
            {
                Empleado = usuario,
                TotalDentro = vehiculosDentro.Count,
                TotalSalidosHoy = historialSalidasHoy.Count,
                VehiculosDentro = vehiculosDentro,
                HistorialSalidasHoy = historialSalidasHoy
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Reservas()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Autenticacion");

            var usuario = await _context.usuario
                .FirstOrDefaultAsync(u => u.id_usuario == usuarioId.Value);

            if (usuario == null)
                return RedirectToAction("Login", "Autenticacion");

            var hoy = DateTime.Today;
            var inicioSemana = hoy.AddDays(-(int)hoy.DayOfWeek + (int)DayOfWeek.Monday);
            if (hoy.DayOfWeek == DayOfWeek.Sunday)
                inicioSemana = hoy.AddDays(-6);

            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);

            var reservasActuales = await (
                from r in _context.reserva
                join v in _context.vehiculo on r.Vehiculo_id_vehiculo equals v.id_vehiculo
                join u in _context.usuario on r.Usuario_id_usuario equals u.id_usuario
                where r.Estado == ParkYa.Models.Estado.Activo || r.Estado == ParkYa.Models.Estado.Pendiente
                select new ReservaEmpleadoItemViewModel
                {
                    IdReserva = r.id_reserva,
                    CodigoReserva = r.cod_reserva,
                    Placa = v.placa,
                    Cliente = u.nombre + " " + u.apellido,
                    Fecha = r.fecha,
                    HoraEntrada = r.hora_entrada,
                    HoraSalida = r.hora_salida,
                    Estado = r.Estado.ToString()
                }
            ).OrderByDescending(x => x.Fecha).ToListAsync();

            var historialHoy = await (
                from r in _context.reserva
                join v in _context.vehiculo on r.Vehiculo_id_vehiculo equals v.id_vehiculo
                join u in _context.usuario on r.Usuario_id_usuario equals u.id_usuario
                where r.fecha.Date == hoy
                select new ReservaEmpleadoItemViewModel
                {
                    IdReserva = r.id_reserva,
                    CodigoReserva = r.cod_reserva,
                    Placa = v.placa,
                    Cliente = u.nombre + " " + u.apellido,
                    Fecha = r.fecha,
                    HoraEntrada = r.hora_entrada,
                    HoraSalida = r.hora_salida,
                    Estado = r.Estado.ToString()
                }
            ).OrderByDescending(x => x.Fecha).ToListAsync();

            var historialSemana = await (
                from r in _context.reserva
                join v in _context.vehiculo on r.Vehiculo_id_vehiculo equals v.id_vehiculo
                join u in _context.usuario on r.Usuario_id_usuario equals u.id_usuario
                where r.fecha.Date >= inicioSemana && r.fecha.Date <= hoy
                select new ReservaEmpleadoItemViewModel
                {
                    IdReserva = r.id_reserva,
                    CodigoReserva = r.cod_reserva,
                    Placa = v.placa,
                    Cliente = u.nombre + " " + u.apellido,
                    Fecha = r.fecha,
                    HoraEntrada = r.hora_entrada,
                    HoraSalida = r.hora_salida,
                    Estado = r.Estado.ToString()
                }
            ).OrderByDescending(x => x.Fecha).ToListAsync();

            var historialMes = await (
                from r in _context.reserva
                join v in _context.vehiculo on r.Vehiculo_id_vehiculo equals v.id_vehiculo
                join u in _context.usuario on r.Usuario_id_usuario equals u.id_usuario
                where r.fecha.Date >= inicioMes && r.fecha.Date <= hoy
                select new ReservaEmpleadoItemViewModel
                {
                    IdReserva = r.id_reserva,
                    CodigoReserva = r.cod_reserva,
                    Placa = v.placa,
                    Cliente = u.nombre + " " + u.apellido,
                    Fecha = r.fecha,
                    HoraEntrada = r.hora_entrada,
                    HoraSalida = r.hora_salida,
                    Estado = r.Estado.ToString()
                }
            ).OrderByDescending(x => x.Fecha).ToListAsync();

            var vm = new EmpleadoReservasViewModel
            {
                Empleado = usuario,
                TotalActivasOPendientes = reservasActuales.Count,
                TotalHoy = historialHoy.Count,
                TotalSemana = historialSemana.Count,
                TotalMes = historialMes.Count,
                ReservasActuales = reservasActuales,
                HistorialHoy = historialHoy,
                HistorialSemana = historialSemana,
                HistorialMes = historialMes
            };

            return View(vm);
        }
    }
}