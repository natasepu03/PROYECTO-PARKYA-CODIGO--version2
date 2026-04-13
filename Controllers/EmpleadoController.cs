using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkYa.Data;
using ParkYa.ViewModels;
using ParkYa.Models;

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
                where r.Estado == ParkYa.Models.Estado.Finalizada
                select new VehiculoActivoItemViewModel
                {
                    Placa = v.placa,
                    Marca = v.marca,
                    Color = v.color,
                    Propietario = u.nombre + " " + u.apellido,
                    Fecha = r.fecha,
                    HoraEntrada = r.hora_entrada,
                    HoraReservada = r.hora_reservada,
                    HoraSalida = r.hora_salida,
                    Estado = r.Estado.ToString()
                }
            ).ToListAsync();

            var vm = new DashboardEmpleadoViewModel
            {
                Empleado = usuario,
                TotalVehiculos = await _context.vehiculo.CountAsync(),
                TotalReservas = await _context.reserva.CountAsync(),
                ReservasActivas = await _context.reserva.CountAsync(r => r.Estado == ParkYa.Models.Estado.Finalizada),
                VehiculosActivos = vehiculosActivos
            };

            return View(vm);
        }
        [HttpGet]
        public async Task<IActionResult> Reportes()
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

            var historialHoy = await (
                from r in _context.reserva
                join u in _context.usuario on r.Usuario_id_usuario equals u.id_usuario
                join v in _context.vehiculo on r.Vehiculo_id_vehiculo equals v.id_vehiculo
                where r.fecha.Date == hoy && r.Estado == Estado.Finalizada
                select new ReservaItemViewModel
                {
                    IdReserva = r.id_reserva,
                    CodigoReserva = r.cod_reserva,
                    Cliente = u.nombre + " " + u.apellido,
                    Placa = v.placa,
                    Fecha = r.fecha,
                    HoraEntrada = r.hora_entrada,
                    HoraSalida = r.hora_salida,
                    HoraReservada = r.hora_reservada,
                    Estado = r.Estado.ToString(),
                    Monto = r.monto
                }
            ).OrderByDescending(x => x.Fecha).ToListAsync();

            var historialSemana = await (
                from r in _context.reserva
                join u in _context.usuario on r.Usuario_id_usuario equals u.id_usuario
                join v in _context.vehiculo on r.Vehiculo_id_vehiculo equals v.id_vehiculo
                where r.fecha.Date >= inicioSemana && r.fecha.Date <= hoy
                      && r.Estado == Estado.Finalizada
                select new ReservaItemViewModel
                {
                    IdReserva = r.id_reserva,
                    CodigoReserva = r.cod_reserva,
                    Cliente = u.nombre + " " + u.apellido,
                    Placa = v.placa,
                    Fecha = r.fecha,
                    HoraEntrada = r.hora_entrada,
                    HoraSalida = r.hora_salida,
                    HoraReservada = r.hora_reservada,
                    Estado = r.Estado.ToString(),
                    Monto = r.monto
                }
            ).OrderByDescending(x => x.Fecha).ToListAsync();

            var historialMes = await (
                from r in _context.reserva
                join u in _context.usuario on r.Usuario_id_usuario equals u.id_usuario
                join v in _context.vehiculo on r.Vehiculo_id_vehiculo equals v.id_vehiculo
                where r.fecha.Date >= inicioMes && r.fecha.Date <= hoy
                      && r.Estado == Estado.Finalizada
                select new ReservaItemViewModel
                {
                    IdReserva = r.id_reserva,
                    CodigoReserva = r.cod_reserva,
                    Cliente = u.nombre + " " + u.apellido,
                    Placa = v.placa,
                    Fecha = r.fecha,
                    HoraEntrada = r.hora_entrada,
                    HoraSalida = r.hora_salida,
                    HoraReservada = r.hora_reservada,
                    Estado = r.Estado.ToString(),
                    Monto = r.monto
                }
            ).OrderByDescending(x => x.Fecha).ToListAsync();

            var vm = new EmpleadoReportesViewModel
            {
                Empleado = usuario,
                TotalReservasHoy = historialHoy.Count,
                TotalReservasSemana = historialSemana.Count,
                TotalReservasMes = historialMes.Count,
                HistorialHoy = historialHoy,
                HistorialSemana = historialSemana,
                HistorialMes = historialMes
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ReportePeriodo(string periodo)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Autenticacion");

            var usuario = await _context.usuario
                .FirstOrDefaultAsync(u => u.id_usuario == usuarioId.Value);

            if (usuario == null)
                return RedirectToAction("Login", "Autenticacion");

            var hoy = DateTime.Today;
            var periodoNormalizado = (periodo ?? string.Empty).Trim().ToLower();

            var inicioSemana = hoy.AddDays(-(int)hoy.DayOfWeek + (int)DayOfWeek.Monday);
            if (hoy.DayOfWeek == DayOfWeek.Sunday)
                inicioSemana = hoy.AddDays(-6);

            var finSemana = inicioSemana.AddDays(6);
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
            var finMes = new DateTime(hoy.Year, hoy.Month, DateTime.DaysInMonth(hoy.Year, hoy.Month));

            var consultaBase =
                from r in _context.reserva
                join u in _context.usuario on r.Usuario_id_usuario equals u.id_usuario
                join v in _context.vehiculo on r.Vehiculo_id_vehiculo equals v.id_vehiculo
                where r.Estado == Estado.Finalizada || r.Estado == Estado.Cancelada
                select new ReservaItemViewModel
                {
                    IdReserva = r.id_reserva,
                    CodigoReserva = r.cod_reserva,
                    Cliente = u.nombre + " " + u.apellido,
                    Placa = v.placa,
                    Fecha = r.fecha,
                    HoraEntrada = r.hora_entrada,
                    HoraSalida = r.hora_salida,
                    HoraReservada = r.hora_reservada,
                    Estado = r.Estado.ToString(),
                    Monto = r.monto
                };

            IQueryable<ReservaItemViewModel> consultaFiltrada = consultaBase;
            string titulo = "Reporte de reservas";
            string textoPeriodo = "General";

            switch (periodoNormalizado)
            {
                case "hoy":
                    consultaFiltrada = consultaBase.Where(r => r.Fecha.Date == hoy);
                    titulo = "Reporte del día";
                    textoPeriodo = hoy.ToString("dd/MM/yyyy");
                    break;
                case "semana":
                    consultaFiltrada = consultaBase.Where(r => r.Fecha.Date >= inicioSemana && r.Fecha.Date <= finSemana);
                    titulo = "Reporte de la semana";
                    textoPeriodo = $"{inicioSemana:dd/MM/yyyy} - {finSemana:dd/MM/yyyy}";
                    break;
                case "mes":
                    consultaFiltrada = consultaBase.Where(r => r.Fecha.Date >= inicioMes && r.Fecha.Date <= finMes);
                    titulo = "Reporte del mes";
                    textoPeriodo = hoy.ToString("MMMM yyyy");
                    break;
                default:
                    return RedirectToAction("Reportes");
            }

            var reservas = await consultaFiltrada
                .OrderByDescending(r => r.Fecha)
                .ThenByDescending(r => r.HoraEntrada)
                .ToListAsync();

            var vm = new ReportePeriodoEmpleadoViewModel
            {
                Titulo = titulo,
                Periodo = textoPeriodo,
                FechaGeneracion = DateTime.Now,
                TotalReservas = reservas.Count,
                TotalMonto = reservas.Sum(r => r.Monto ?? 0),
                Reservas = reservas
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ReporteReserva(int idReserva)
        {
            var reserva = await (
                from r in _context.reserva
                join u in _context.usuario on r.Usuario_id_usuario equals u.id_usuario
                join v in _context.vehiculo on r.Vehiculo_id_vehiculo equals v.id_vehiculo
                join t in _context.tarifas on v.Tipo_vehiculo_idTipo_vehiculo equals t.Tipo_vehiculo_idTipo_vehiculo
                where r.id_reserva == idReserva && r.Estado == Estado.Finalizada
                select new ReservaReporteViewModel
                {
                    IdReserva = r.id_reserva,
                    CodigoReserva = r.cod_reserva,
                    Cliente = u.nombre + " " + u.apellido,
                    Placa = v.placa,
                    Fecha = r.fecha,
                    HoraReservada = r.hora_reservada,
                    HoraEntrada = r.hora_entrada,
                    HoraSalida = r.hora_salida,
                    TarifaHora = t.precio_hora,
                    Monto = r.monto,
                    Estado = r.Estado.ToString()
                }
            ).FirstOrDefaultAsync();

            if (reserva == null)
                return NotFound();

            return View(reserva);
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

            var reservasHoy = await (
    from r in _context.reserva
    join u in _context.usuario on r.Usuario_id_usuario equals u.id_usuario
    join v in _context.vehiculo on r.Vehiculo_id_vehiculo equals v.id_vehiculo
    where r.fecha.Date == DateTime.Today
    select new ReservaItemViewModel
    {
        IdReserva = r.id_reserva,
        Cliente = u.nombre + " " + u.apellido,
        Placa = v.placa,
        Fecha = r.fecha,
        HoraEntrada = r.hora_entrada,
        HoraSalida = r.hora_salida,
        HoraReservada = r.hora_reservada,
        Estado = r.Estado.ToString()
    }
).ToListAsync();

            var vm = new EmpleadoReportesViewModel
            {
                Empleado = usuario,
                TotalReservasHoy = reservasHoy.Count,
                HistorialHoy = reservasHoy
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

            // Inicio de semana
            var inicioSemana = hoy.AddDays(-(int)hoy.DayOfWeek + (int)DayOfWeek.Monday);
            if (hoy.DayOfWeek == DayOfWeek.Sunday)
                inicioSemana = hoy.AddDays(-6);

            // Fin de semana
            var finSemana = inicioSemana.AddDays(6);

            // Inicio y fin de mes
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
            var finMes = new DateTime(hoy.Year, hoy.Month, DateTime.DaysInMonth(hoy.Year, hoy.Month));

            var reservasActuales = await (
                from r in _context.reserva
                join v in _context.vehiculo on r.Vehiculo_id_vehiculo equals v.id_vehiculo
                join u in _context.usuario on r.Usuario_id_usuario equals u.id_usuario
                join t in _context.tarifas on v.Tipo_vehiculo_idTipo_vehiculo equals t.Tipo_vehiculo_idTipo_vehiculo
                where r.Estado == ParkYa.Models.Estado.Pendiente
                   || r.Estado == ParkYa.Models.Estado.Activa
                select new ReservaEmpleadoItemViewModel
                {
                    IdReserva = r.id_reserva,
                    CodigoReserva = r.cod_reserva,
                    Placa = v.placa,
                    Cliente = u.nombre + " " + u.apellido,
                    Fecha = r.fecha,
                    HoraEntrada = r.hora_entrada,
                    HoraReservada = r.hora_reservada,
                    HoraSalida = r.hora_salida,
                    Estado = r.Estado.ToString(),
                    TarifaHora = t.precio_hora,
                    Monto = r.monto
                }
            ).OrderByDescending(x => x.Fecha).ToListAsync();

            var historialHoy = await (
                from r in _context.reserva
                join v in _context.vehiculo on r.Vehiculo_id_vehiculo equals v.id_vehiculo
                join u in _context.usuario on r.Usuario_id_usuario equals u.id_usuario
                join t in _context.tarifas on v.Tipo_vehiculo_idTipo_vehiculo equals t.Tipo_vehiculo_idTipo_vehiculo
                where r.fecha.Date == hoy
                   && (r.Estado == ParkYa.Models.Estado.Finalizada
                    || r.Estado == ParkYa.Models.Estado.Cancelada)
                select new ReservaEmpleadoItemViewModel
                {
                    IdReserva = r.id_reserva,
                    CodigoReserva = r.cod_reserva,
                    Placa = v.placa,
                    Cliente = u.nombre + " " + u.apellido,
                    Fecha = r.fecha,
                    HoraEntrada = r.hora_entrada,
                    HoraReservada = r.hora_reservada,
                    HoraSalida = r.hora_salida,
                    Estado = r.Estado.ToString(),
                    TarifaHora = t.precio_hora,
                    Monto = r.monto
                }
            ).OrderByDescending(x => x.Fecha).ToListAsync();

            var historialSemana = await (
                from r in _context.reserva
                join v in _context.vehiculo on r.Vehiculo_id_vehiculo equals v.id_vehiculo
                join u in _context.usuario on r.Usuario_id_usuario equals u.id_usuario
                join t in _context.tarifas on v.Tipo_vehiculo_idTipo_vehiculo equals t.Tipo_vehiculo_idTipo_vehiculo
                where r.fecha.Date >= inicioSemana
                   && r.fecha.Date <= finSemana
                   && (r.Estado == ParkYa.Models.Estado.Finalizada
                    || r.Estado == ParkYa.Models.Estado.Cancelada)
                select new ReservaEmpleadoItemViewModel
                {
                    IdReserva = r.id_reserva,
                    CodigoReserva = r.cod_reserva,
                    Placa = v.placa,
                    Cliente = u.nombre + " " + u.apellido,
                    Fecha = r.fecha,
                    HoraEntrada = r.hora_entrada,
                    HoraReservada = r.hora_reservada,
                    HoraSalida = r.hora_salida,
                    Estado = r.Estado.ToString(),
                    TarifaHora = t.precio_hora,
                    Monto = r.monto
                }
            ).OrderByDescending(x => x.Fecha).ToListAsync();

            var historialMes = await (
                from r in _context.reserva
                join v in _context.vehiculo on r.Vehiculo_id_vehiculo equals v.id_vehiculo
                join u in _context.usuario on r.Usuario_id_usuario equals u.id_usuario
                join t in _context.tarifas on v.Tipo_vehiculo_idTipo_vehiculo equals t.Tipo_vehiculo_idTipo_vehiculo
                where r.fecha.Date >= inicioMes
                   && r.fecha.Date <= finMes
                   && (r.Estado == ParkYa.Models.Estado.Finalizada
                    || r.Estado == ParkYa.Models.Estado.Cancelada)
                select new ReservaEmpleadoItemViewModel
                {
                    IdReserva = r.id_reserva,
                    CodigoReserva = r.cod_reserva,
                    Placa = v.placa,
                    Cliente = u.nombre + " " + u.apellido,
                    Fecha = r.fecha,
                    HoraEntrada = r.hora_entrada,
                    HoraReservada = r.hora_reservada,
                    HoraSalida = r.hora_salida,
                    Estado = r.Estado.ToString(),
                    TarifaHora = t.precio_hora,
                    Monto = r.monto
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
        [HttpPost]
        public async Task<IActionResult> FinalizarReserva(int id, string horaEntrada, string horaSalida)
        {
            var reserva = await _context.reserva.FirstOrDefaultAsync(r => r.id_reserva == id);
            if (reserva == null)
                return RedirectToAction("Reservas");

            if (reserva.hora_entrada == null)
            {
                TempData["Mensaje"] = "Primero debes registrar la hora de entrada.";
                return RedirectToAction("Reservas");
            }

            if (reserva.hora_salida == null)
            {
                TempData["Mensaje"] = "Primero debes registrar la hora de salida.";
                return RedirectToAction("Reservas");
            }

            reserva.Estado = Estado.Finalizada;

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = $"Reserva finalizada. Total: {(reserva.monto.HasValue ? reserva.monto.Value.ToString("C") : "$0")}";
            return RedirectToAction("Reservas");
        }

        public class ReservaEntradaDto
        {
            public int id { get; set; }
            public string? horaEntrada { get; set; }
        }

        public class ReservaSalidaDto
        {
            public int id { get; set; }
            public string? horaSalida { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> GuardarEntrada([FromBody] ReservaEntradaDto data)
        {
            var reserva = await _context.reserva
                .FirstOrDefaultAsync(r => r.id_reserva == data.id);

            if (reserva == null)
                return Json(new { ok = false });

            if (TimeSpan.TryParse(data.horaEntrada, out TimeSpan horaEntradaTs))
            {
                reserva.hora_entrada = horaEntradaTs;
                reserva.Estado = Estado.Activa;

                await _context.SaveChangesAsync();
            }

            return Json(new { ok = true });
        }

        [HttpPost]
        public async Task<IActionResult> GuardarSalida([FromBody] ReservaSalidaDto data)
        {
            var reserva = await _context.reserva
                .FirstOrDefaultAsync(r => r.id_reserva == data.id);

            if (reserva == null)
                return Json(new { ok = false, mensaje = "Reserva no encontrada." });

            if (string.IsNullOrWhiteSpace(data.horaSalida))
                return Json(new { ok = false, mensaje = "Hora de salida vacía." });

            if (!TimeSpan.TryParse(data.horaSalida, out TimeSpan horaSalidaTs))
                return Json(new { ok = false, mensaje = "Hora de salida inválida." });

            reserva.hora_salida = horaSalidaTs;

            if (reserva.hora_entrada == null)
                return Json(new { ok = false, mensaje = "Primero debes guardar la hora de entrada." });

            var vehiculo = await _context.vehiculo
                .FirstOrDefaultAsync(v => v.id_vehiculo == reserva.Vehiculo_id_vehiculo);

            if (vehiculo == null)
                return Json(new { ok = false, mensaje = "Vehículo no encontrado." });

            var tarifa = await _context.tarifas
            .FirstOrDefaultAsync(t => t.Tipo_vehiculo_idTipo_vehiculo == vehiculo.Tipo_vehiculo_idTipo_vehiculo);

            if (tarifa == null)
                return Json(new { ok = false, mensaje = "Tarifa no encontrada." });

            var horas = (reserva.hora_salida.Value - reserva.hora_entrada.Value).TotalHours;

            if (horas < 0)
                horas = 0;

            var monto = (decimal)horas * tarifa.precio_hora;

            reserva.monto = monto;
            reserva.Estado = Estado.Finalizada;

            await _context.SaveChangesAsync();

            return Json(new
            {
                ok = true,
                monto = monto
            });
        }
    }


}
