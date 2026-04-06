using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkYa.Data;
using ParkYa.Models;
using ParkYa.Models.ViewModels;


namespace ParkYa.Controllers
{
    public class ClientesController : Controller
    {
        private readonly ParkYaDbContext _context;

        public ClientesController(ParkYaDbContext context)
        {
            _context = context;
        }

        private int? ObtenerUsuarioId()
        {
            return HttpContext.Session.GetInt32("UsuarioId");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var usuarioId = ObtenerUsuarioId();
            if (usuarioId == null)
                return RedirectToAction("Login", "Autenticacion");

            var usuario = await _context.usuario
                .Include(u => u.Vehiculos!)
                    .ThenInclude(v => v.TipoVehiculo)
                .FirstOrDefaultAsync(u => u.id_usuario == usuarioId.Value);

            if (usuario == null)
                return RedirectToAction("Login", "Autenticacion");

            var reservas = await _context.reserva
                .Where(r => r.Usuario_id_usuario == usuarioId.Value)
                .Include(r => r.Vehiculo)
                .OrderByDescending(r => r.id_reserva)
                .ToListAsync();

            ViewBag.Reservas = reservas;

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarPerfilCompleto(EditarPerfilCompletoViewModel model)
        {
            var usuarioId = ObtenerUsuarioId();

            if (usuarioId == null)
                return RedirectToAction("Login", "Autenticacion");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Verifica los datos del formulario.";
                return RedirectToAction("Index");
            }

            var usuario = await _context.usuario
                .FirstOrDefaultAsync(u => u.id_usuario == usuarioId.Value);

            if (usuario == null)
                return RedirectToAction("Login", "Autenticacion");


            usuario.nombre = model.Nombre;
            usuario.apellido = model.Apellido;
            usuario.correo = model.Correo;
            usuario.telefono = model.Telefono;
            usuario.documento = model.Documento;
            usuario.tipo_doc = model.TipoDocumento;

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Perfil actualizado correctamente.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarVehiculo(string placa, string marca, string color, int tipoVehiculoId)
        {
            var usuarioId = ObtenerUsuarioId();

            if (usuarioId == null)
                return RedirectToAction("Login", "Autenticacion");

            var parqueadero = await _context.parqueadero
                .FirstOrDefaultAsync(p => p.id_Parqueadero == 1);

            if (parqueadero == null)
            {
                TempData["Error"] = "No existe el parqueadero con id 1.";
                return RedirectToAction("Index");
            }

            var tipos = await _context.tipo_vehiculo.ToListAsync();
            var tipoVehiculo = tipos.FirstOrDefault(t => t.TipoVehiculoId == tipoVehiculoId);

            if (tipoVehiculo == null)
            {
                TempData["Error"] = "No hay tipos de vehículo registrados.";
                return RedirectToAction("Index");
            }

            var vehiculo = new Vehiculo
            {
                placa = placa,
                marca = marca,
                color = color,
                Usuario_id_usuario = usuarioId.Value,
                Parqueadero_id_Parqueadero = parqueadero.id_Parqueadero,
                Tipo_vehiculo_idTipo_vehiculo = tipoVehiculo.TipoVehiculoId
            };

            _context.vehiculo.Add(vehiculo);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Vehículo guardado correctamente";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearReserva(ReservaPagoViewModel model)
        {
            var usuarioId = ObtenerUsuarioId();

            if (usuarioId == null)
                return Content("DEBUG: usuarioId es null");

            if (!ModelState.IsValid)
            {
                var errores = ModelState
                    .SelectMany(x => x.Value!.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Content("DEBUG: ModelState inválido -> " + string.Join(" | ", errores));
            }

            var ahora = DateTime.Now;
            var hoy = ahora.Date;
            var horaActual = ahora.TimeOfDay;

            var horaInicio = new TimeSpan(8, 0, 0);
            var horaFin = new TimeSpan(22, 0, 0);

            // 1. No permitir fechas pasadas
            if (model.Fecha.Date < hoy)
            {
                TempData["Error"] = "No puedes hacer una reserva en una fecha pasada.";
                return RedirectToAction("Index");
            }

            // 2. No permitir reservas con más de 7 días de anticipación
            if (model.Fecha.Date > hoy.AddDays(7))
            {
                TempData["Error"] = "Solo puedes reservar con máximo 7 días de anticipación.";
                return RedirectToAction("Index");
            }

            // 3. Validar horario permitido: entre 8:00 a. m. y 10:00 p. m.
            if (model.HoraReservada < horaInicio || model.HoraReservada > horaFin)
            {
                TempData["Error"] = "Solo puedes reservar entre las 8:00 a. m. y las 10:00 p. m.";
                return RedirectToAction("Index");
            }

            // 4. Si la reserva es para hoy, la hora debe ser posterior a la actual
            if (model.Fecha.Date == hoy && model.HoraReservada <= horaActual)
            {
                TempData["Error"] = "Debes seleccionar una hora posterior a la actual.";
                return RedirectToAction("Index");
            }

            var vehiculo = await _context.vehiculo
                .FirstOrDefaultAsync(v => v.id_vehiculo == model.VehiculoId && v.Usuario_id_usuario == usuarioId.Value);

            if (vehiculo == null)
                return Content("DEBUG: vehiculo es null");

            // 5. Evitar reserva duplicada para el mismo vehículo, fecha y hora
            var reservaExistente = await _context.reserva.AnyAsync(r =>
                r.Vehiculo_id_vehiculo == model.VehiculoId &&
                r.fecha == model.Fecha &&
                r.hora_reservada == model.HoraReservada
            );

            if (reservaExistente)
            {
                TempData["Error"] = "Ya existe una reserva para ese vehículo en esa fecha y hora.";
                return RedirectToAction("Index");
            }

            var tarifaId = await _context.tarifas
                .Where(t => t.Tipo_vehiculo_idTipo_vehiculo == vehiculo.Tipo_vehiculo_idTipo_vehiculo)
                .Select(t => (int?)t.id_tarifas)
                .FirstOrDefaultAsync();

            if (tarifaId == null)
                return Content("DEBUG: tarifa es null");

            var codReserva = new Random().Next(100000, 999999);

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            INSERT INTO reserva
            (cod_reserva, fecha, hora_reservada, hora_entrada, hora_salida, estado, Usuario_id_usuario, Tarifas_id_tarifas, Vehiculo_id_vehiculo)
            VALUES
            ({codReserva}, {model.Fecha}, {model.HoraReservada}, {null}, {null}, {"Pendiente"}, {usuarioId.Value}, {tarifaId.Value}, {model.VehiculoId})
            ");

            TempData["Mensaje"] = "Reserva guardada correctamente.";
            return RedirectToAction("Index");
        }
        /*
                [HttpPost]
                [ValidateAntiForgeryToken]
                public async Task<IActionResult> ConfirmarPago(string metodoPago, decimal monto)
                {
                    var usuarioId = ObtenerUsuarioId();

                    if (usuarioId == null)
                        return RedirectToAction("Login", "Autenticacion");

                    var ultimaReservaId = await _context.reserva
                        .Where(r => r.Usuario_id_usuario == usuarioId.Value)
                        .OrderByDescending(r => r.id_reserva)
                        .Select(r => (int?)r.id_reserva)
                        .FirstOrDefaultAsync();

                    if (ultimaReservaId == null)
                    {
                        TempData["Error"] = "No se encontró una reserva para asociar al pago.";
                        return RedirectToAction("Index");
                    }

                    var codPago = "PAGO-" + new Random().Next(100000, 999999).ToString();

                    await _context.Database.ExecuteSqlInterpolatedAsync($@"
                        INSERT INTO venta
                        (cod_pago, monto, fecha_pago, metodo_pago, Usuario_id_usuario, Reserva_id_reserva)
                        VALUES
                        ({codPago}, {monto}, {DateTime.Now}, {metodoPago}, {usuarioId.Value}, {ultimaReservaId.Value})
                    ");

                    TempData["Mensaje"] = "Pago guardado correctamente.";
                    return RedirectToAction("Index");
                }
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarVehiculo(int id)
        {
            var usuarioId = ObtenerUsuarioId();

            if (usuarioId == null)
                return RedirectToAction("Login", "Autenticacion");

            var vehiculoSeleccionado = HttpContext.Session.GetInt32("VehiculoSeleccionado");

            if (vehiculoSeleccionado == id)
            {
                TempData["Error"] = "No puedes eliminar el vehículo actual.";
                return RedirectToAction("Index");
            }

            var vehiculo = await _context.vehiculo
                .FirstOrDefaultAsync(v => v.id_vehiculo == id && v.Usuario_id_usuario == usuarioId.Value);

            if (vehiculo == null)
            {
                TempData["Error"] = "Vehículo no encontrado.";
                return RedirectToAction("Index");
            }

            var tieneReservas = await _context.reserva
                .AnyAsync(r => r.Vehiculo_id_vehiculo == id);

            if (tieneReservas)
            {
                TempData["Error"] = "No puedes eliminar este vehículo porque tiene reservas asociadas.";
                return RedirectToAction("Index");
            }

            _context.vehiculo.Remove(vehiculo);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Vehículo eliminado correctamente.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarVehiculo(int id, string placa, string marca, string color, int tipoVehiculoId)
        {
            var usuarioId = ObtenerUsuarioId();

            if (usuarioId == null)
                return RedirectToAction("Login", "Autenticacion");

            var vehiculo = await _context.vehiculo
                .FirstOrDefaultAsync(v => v.id_vehiculo == id && v.Usuario_id_usuario == usuarioId.Value);

            if (vehiculo == null)
            {
                TempData["Error"] = "Vehículo no encontrado.";
                return RedirectToAction("Index");
            }

            vehiculo.placa = placa;
            vehiculo.marca = marca;
            vehiculo.color = color;
            vehiculo.Tipo_vehiculo_idTipo_vehiculo = tipoVehiculoId;

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Vehículo actualizado correctamente.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SeleccionarVehiculoActual(int id)
        {
            var usuarioId = ObtenerUsuarioId();

            if (usuarioId == null)
                return RedirectToAction("Login", "Autenticacion");

            HttpContext.Session.SetInt32("VehiculoSeleccionado", id);

            TempData["Mensaje"] = "Vehículo actual cambiado correctamente.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarReserva(int id)
        {
            var usuarioId = ObtenerUsuarioId();

            if (usuarioId == null)
                return RedirectToAction("Login", "Autenticacion");

            var reserva = await _context.reserva
                .FirstOrDefaultAsync(r => r.id_reserva == id && r.Usuario_id_usuario == usuarioId.Value);

            if (reserva == null)
            {
                TempData["Error"] = "Reserva no encontrada.";
                return RedirectToAction("Index");
            }

            if (reserva.Estado != Estado.Pendiente)
            {
                TempData["Error"] = "Solo puedes cancelar reservas pendientes.";
                return RedirectToAction("Index");
            }

            reserva.Estado = Estado.Cancelada;

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Reserva cancelada correctamente.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarReserva(int id, DateTime fecha, TimeSpan horaReservada)
        {
            var usuarioId = ObtenerUsuarioId();

            if (usuarioId == null)
                return RedirectToAction("Login", "Autenticacion");

            var reserva = await _context.reserva
                .FirstOrDefaultAsync(r => r.id_reserva == id && r.Usuario_id_usuario == usuarioId.Value);

            if (reserva == null)
            {
                TempData["Error"] = "Reserva no encontrada.";
                return RedirectToAction("Index");
            }

            if (reserva.Estado != Estado.Pendiente)
            {
                TempData["Error"] = "Solo puedes editar reservas pendientes.";
                return RedirectToAction("Index");
            }

            reserva.fecha = fecha;
            reserva.hora_reservada = horaReservada;

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Reserva actualizada correctamente.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> SimularPago(int idReserva)
        {
            var reserva = await _context.reserva
                .FirstOrDefaultAsync(r => r.id_reserva == idReserva);

            if (reserva == null)
                return NotFound();

            return View(reserva);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmarPago(int idReserva)
        {
            var reserva = await _context.reserva
                .FirstOrDefaultAsync(r => r.id_reserva == idReserva);

            if (reserva == null)
                return NotFound();

            reserva.pagado = true;

            await _context.SaveChangesAsync();

            TempData["MensajePago"] = "Pago simulado realizado correctamente.";
            return RedirectToAction("Index");
        }

        public IActionResult Comprobante(int idReserva)
        {
            var reserva = _context.reserva
                .Include(r => r.Usuario)
                .Include(r => r.Vehiculo)
                .FirstOrDefault(r => r.id_reserva == idReserva);

            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

    }
}