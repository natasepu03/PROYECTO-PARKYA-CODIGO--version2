using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using iText.Html2pdf;
using ParkYa.Data;
using ParkYa.ViewModels;
using ParkYa.Models;
using System.Globalization;
using System.Net;
using System.Text;


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

    var parqueadero = _context.parqueadero
    .FirstOrDefault(p => p.id_Parqueadero == 1);

    if (parqueadero == null)
    {
        parqueadero = new Parqueadero
        {
            nombre = "Mi Parqueadero",
            direccion = "Sin direccion",
            total_cupos = 0,
            cod_parqueadero = 1
        };

        _context.parqueadero.Add(parqueadero);
        _context.SaveChanges();
    }

    var hoy = DateTime.Today;
    var reservasHoy = _context.reserva.Count(r => r.fecha.Date == hoy);
    var pendientesHoy = _context.reserva.Count(r => r.fecha.Date == hoy && r.Estado == Estado.Pendiente);
    var finalizadasHoy = _context.reserva.Count(r => r.fecha.Date == hoy && r.Estado == Estado.Finalizada);
    var ingresosHoy = (
        from venta in _context.venta
        join reserva in _context.reserva on venta.Reserva_id_reserva equals reserva.id_reserva
        where reserva.fecha.Date == hoy
        select (decimal?)venta.monto
    ).Sum() ?? 0;

    var dashboard = new DashboardAdminViewModel
    {
        Administrador = usuario,
        Parqueadero = parqueadero,
        ReservasHoy = reservasHoy,
        IngresosHoy = ingresosHoy,
        PendientesHoy = pendientesHoy,
        FinalizadasHoy = finalizadasHoy
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
[HttpGet]
public IActionResult Usuarios()
{
    var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

    if (usuarioId == null)
        return RedirectToAction("Login", "Autenticacion");

   var admin = _context.usuario
    .FirstOrDefault(u => u.id_usuario == usuarioId);

if (admin == null)
{
    return RedirectToAction("Login", "Autenticacion");
}

if (admin == null)
{
    return RedirectToAction("Login", "Autenticacion");
}

    var usuarios = _context.usuario.ToList();

    var empleados = usuarios.Where(u => u.Rol_id_rol == 2).ToList();
    var clientes = usuarios.Where(u => u.Rol_id_rol == 1).ToList();

    var vm = new AdministradorUsuariosViewModel
    {
         Administrador = admin,
         EmpleadosActivos = empleados
            .Where(e => e.estado == true)
            .Select(e => new UsuarioViewModel
            {
                Id = e.id_usuario,
                Nombre = e.nombre,
                Apellido = e.apellido,
                NombreCompleto = e.nombre + " " + e.apellido,
                Correo = e.correo,
                Telefono = e.telefono,
                Documento = e.documento.ToString(),
                TipoDocumento = e.tipo_doc,
                Rol = "Empleado",
                RolId = e.Rol_id_rol,
                Estado = e.estado == true ? "Activo" : "Inactivo"
            }).ToList(),

        EmpleadosInactivos = empleados
            .Where(e => e.estado != true)
            .Select(e => new UsuarioViewModel
            {
                Id = e.id_usuario,
                Nombre = e.nombre,
                Apellido = e.apellido,
                NombreCompleto = e.nombre + " " + e.apellido,
                Correo = e.correo,
                Telefono = e.telefono,
                Documento = e.documento.ToString(),
                TipoDocumento = e.tipo_doc,
                Rol = "Empleado",
                RolId = e.Rol_id_rol,
                Estado = "Inactivo"
            }).ToList(),

        ClientesActivos = clientes
            .Where(c => c.estado == true)
            .Select(c => new UsuarioViewModel
            {
                Id = c.id_usuario,
                Nombre = c.nombre,
                Apellido = c.apellido,
                NombreCompleto = c.nombre + " " + c.apellido,
                Correo = c.correo,
                Telefono = c.telefono,
                Documento = c.documento.ToString(),
                TipoDocumento = c.tipo_doc,
                Rol = "Cliente",
                RolId = c.Rol_id_rol,
                Estado = "Activo"
            }).ToList(),

        ClientesInactivos = clientes
            .Where(c => c.estado != true)
            .Select(c => new UsuarioViewModel
            {
                Id = c.id_usuario,
                Nombre = c.nombre,
                Apellido = c.apellido,
                NombreCompleto = c.nombre + " " + c.apellido,
                Correo = c.correo,
                Telefono = c.telefono,
                Documento = c.documento.ToString(),
                TipoDocumento = c.tipo_doc,
                Rol = "Cliente",
                RolId = c.Rol_id_rol,
                Estado = "Inactivo"
            }).ToList(),

        TotalEmpleados = empleados.Count,
        TotalClientes = clientes.Count
    };

    return View(vm);
}

[HttpGet]
public IActionResult Tarifas()
{
    var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

    if (usuarioId == null)
        return RedirectToAction("Login", "Autenticacion");

    var admin = _context.usuario.FirstOrDefault(u => u.id_usuario == usuarioId);

    if (admin == null)
        return RedirectToAction("Login", "Autenticacion");

    var tiposVehiculo = _context.tipo_vehiculo.ToList();

    var tarifas = _context.tarifas
        .ToList()
        .Select(t => new TarifaAdminItemViewModel
        {
            Id = t.id_tarifas,
            Codigo = t.cod_tarifa,
            TipoVehiculo = tiposVehiculo
                .FirstOrDefault(tv => tv.TipoVehiculoId == t.Tipo_vehiculo_idTipo_vehiculo)?.nombre_tipo ?? "Sin tipo",
            PrecioDia = t.precio_dia,
            PrecioHora = t.precio_hora,
            Horario = t.horario,
            TipoVehiculoId = t.Tipo_vehiculo_idTipo_vehiculo
        })
        .OrderBy(t => t.TipoVehiculo)
        .ToList();

    var vm = new AdministradorTarifasViewModel
    {
        Administrador = admin,
        Tarifas = tarifas
    };

    return View(vm);
}

[HttpGet]
public async Task<IActionResult> Reservas()
{
    var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

    if (usuarioId == null)
        return RedirectToAction("Login", "Autenticacion");

    var admin = await _context.usuario.FirstOrDefaultAsync(u => u.id_usuario == usuarioId);

    if (admin == null)
        return RedirectToAction("Login", "Autenticacion");

    var reservas = new List<ReservaAdminItemViewModel>();

    await using var connection = _context.Database.GetDbConnection();
    if (connection.State != System.Data.ConnectionState.Open)
        await connection.OpenAsync();

    await using (var command = connection.CreateCommand())
    {
        command.CommandText = @"
            SELECT 
                r.id_reserva,
                u.nombre,
                u.apellido,
                v.placa,
                tv.nombre_tipo,
                r.fecha,
                r.hora_entrada,
                r.hora_salida,
                r.estado,
                t.precio_hora
            FROM reserva r
            INNER JOIN vehiculo v ON r.Vehiculo_id_vehiculo = v.id_vehiculo
            INNER JOIN usuario u ON r.Usuario_id_usuario = u.id_usuario
            INNER JOIN tipo_vehiculo tv ON v.Tipo_vehiculo_idTipo_vehiculo = tv.idTipo_vehiculo
            INNER JOIN tarifas t ON v.Tipo_vehiculo_idTipo_vehiculo = t.Tipo_vehiculo_idTipo_vehiculo
            ORDER BY r.fecha DESC, r.id_reserva DESC";

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var horaEntrada = ParseNullableTime(reader["hora_entrada"]);
            var horaSalida = ParseNullableTime(reader["hora_salida"]);
            var precioHora = ParseDecimal(reader["precio_hora"]);

            reservas.Add(new ReservaAdminItemViewModel
            {
                Id = Convert.ToInt32(reader["id_reserva"]),
                Cliente = $"{reader["nombre"]} {reader["apellido"]}".Trim(),
                Placa = reader["placa"]?.ToString() ?? "Sin placa",
                TipoVehiculo = reader["nombre_tipo"]?.ToString() ?? "Sin tipo",
                Fecha = Convert.ToDateTime(reader["fecha"]),
                HoraEntrada = horaEntrada,
                HoraSalida = horaSalida,
                Estado = reader["estado"]?.ToString() ?? string.Empty,
                Monto = CalcularMontoReserva(horaEntrada, horaSalida, precioHora)
            });
        }
    }

    var vm = new AdministradorReservasViewModel
    {
        Administrador = admin,
        Reservas = reservas,
        TotalReservas = reservas.Count,
        ReservasPendientes = reservas.Count(r => r.Estado == Estado.Pendiente.ToString()),
        ReservasFinalizadas = reservas.Count(r => r.Estado == Estado.Finalizada.ToString()),
        ReservasCanceladas = reservas.Count(r => r.Estado == Estado.Cancelada.ToString())
    };

    return View(vm);
}

[HttpGet]
public async Task<IActionResult> Reportes(DateTime? fechaInicio, DateTime? fechaFin)
{
    var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

    if (usuarioId == null)
        return RedirectToAction("Login", "Autenticacion");

    var admin = await _context.usuario.FirstOrDefaultAsync(u => u.id_usuario == usuarioId);
    if (admin == null)
        return RedirectToAction("Login", "Autenticacion");

    var hoy = DateTime.Today;
    var inicioSemana = hoy.AddDays(-(int)hoy.DayOfWeek + (int)DayOfWeek.Monday);
    if (hoy.DayOfWeek == DayOfWeek.Sunday)
        inicioSemana = hoy.AddDays(-6);
    var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);

    var reporteBase = from r in _context.reserva
                      join v in _context.vehiculo on r.Vehiculo_id_vehiculo equals v.id_vehiculo
                      join u in _context.usuario on r.Usuario_id_usuario equals u.id_usuario
                      join tv in _context.tipo_vehiculo on v.Tipo_vehiculo_idTipo_vehiculo equals tv.TipoVehiculoId
                      join venta in _context.venta on r.id_reserva equals venta.Reserva_id_reserva into ventas
                      select new
                      {
                          r.id_reserva,
                          Cliente = u.nombre + " " + u.apellido,
                          Placa = v.placa,
                          TipoVehiculo = tv.nombre_tipo,
                          Fecha = r.fecha,
                          HoraEntrada = r.hora_entrada,
                          Estado = r.Estado,
                          Monto = ventas.Sum(x => (decimal?)x.monto) ?? 0
                      };

    var totalReservasDia = await _context.reserva.CountAsync(r => r.fecha.Date == hoy);
    var totalReservasSemana = await _context.reserva.CountAsync(r => r.fecha.Date >= inicioSemana && r.fecha.Date <= hoy);
    var totalReservasMes = await _context.reserva.CountAsync(r => r.fecha.Date >= inicioMes && r.fecha.Date <= hoy);

    if (fechaInicio.HasValue)
        reporteBase = reporteBase.Where(r => r.Fecha.Date >= fechaInicio.Value.Date);

    if (fechaFin.HasValue)
        reporteBase = reporteBase.Where(r => r.Fecha.Date <= fechaFin.Value.Date);

    var reservasFiltradas = await reporteBase
        .OrderByDescending(r => r.Fecha)
        .Select(r => new ReporteReservaAdminItemViewModel
        {
            IdReserva = r.id_reserva,
            Cliente = r.Cliente,
            Placa = r.Placa,
            TipoVehiculo = r.TipoVehiculo,
            Fecha = r.Fecha,
            HoraEntrada = r.HoraEntrada,
            Estado = r.Estado.ToString(),
            Monto = r.Monto
        })
        .ToListAsync();

    var horaPico = reservasFiltradas
        .Where(r => r.HoraEntrada.HasValue)
        .GroupBy(r => r.HoraEntrada!.Value.Hours)
        .Select(g => new { Hora = g.Key, Total = g.Count() })
        .OrderByDescending(g => g.Total)
        .FirstOrDefault();

    var vm = new AdministradorReportesViewModel
    {
        Administrador = admin,
        FechaInicio = fechaInicio,
        FechaFin = fechaFin,
        TotalReservasDia = totalReservasDia,
        TotalReservasSemana = totalReservasSemana,
        TotalReservasMes = totalReservasMes,
        ReservasPendientes = reservasFiltradas.Count(r => r.Estado == Estado.Pendiente.ToString()),
        ReservasFinalizadas = reservasFiltradas.Count(r => r.Estado == Estado.Finalizada.ToString()),
        ReservasCanceladas = reservasFiltradas.Count(r => r.Estado == Estado.Cancelada.ToString()),
        IngresosReales = reservasFiltradas.Sum(r => r.Monto),
        MontoPromedio = reservasFiltradas.Count > 0 ? Math.Round(reservasFiltradas.Average(r => r.Monto), 2) : 0,
        HoraPico = horaPico != null ? $"{horaPico.Hora:00}:00 - {horaPico.Hora:00}:59" : "Sin datos",
        TotalReservasHoraPico = horaPico?.Total ?? 0,
        ReservasFiltradas = reservasFiltradas
    };

    return View(vm);
}

[HttpGet]
public async Task<IActionResult> ExportarReportes(DateTime? fechaInicio, DateTime? fechaFin)
{
    var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

    if (usuarioId == null)
        return RedirectToAction("Login", "Autenticacion");

    var admin = await _context.usuario.FirstOrDefaultAsync(u => u.id_usuario == usuarioId);
    if (admin == null)
        return RedirectToAction("Login", "Autenticacion");

    var hoy = DateTime.Today;
    var inicioSemana = hoy.AddDays(-(int)hoy.DayOfWeek + (int)DayOfWeek.Monday);
    if (hoy.DayOfWeek == DayOfWeek.Sunday)
        inicioSemana = hoy.AddDays(-6);
    var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);

    var reporteBase = from r in _context.reserva
                      join v in _context.vehiculo on r.Vehiculo_id_vehiculo equals v.id_vehiculo
                      join u in _context.usuario on r.Usuario_id_usuario equals u.id_usuario
                      join tv in _context.tipo_vehiculo on v.Tipo_vehiculo_idTipo_vehiculo equals tv.TipoVehiculoId
                      join venta in _context.venta on r.id_reserva equals venta.Reserva_id_reserva into ventas
                      select new
                      {
                          Cliente = u.nombre + " " + u.apellido,
                          Placa = v.placa,
                          TipoVehiculo = tv.nombre_tipo,
                          Fecha = r.fecha,
                          HoraEntrada = r.hora_entrada,
                          Estado = r.Estado,
                          Monto = ventas.Sum(x => (decimal?)x.monto) ?? 0
                      };

    if (fechaInicio.HasValue)
        reporteBase = reporteBase.Where(r => r.Fecha.Date >= fechaInicio.Value.Date);

    if (fechaFin.HasValue)
        reporteBase = reporteBase.Where(r => r.Fecha.Date <= fechaFin.Value.Date);

    var datos = await reporteBase
        .OrderByDescending(r => r.Fecha)
        .ToListAsync();

    var horaPico = datos
        .Where(r => r.HoraEntrada.HasValue)
        .GroupBy(r => r.HoraEntrada!.Value.Hours)
        .Select(g => new { Hora = g.Key, Total = g.Count() })
        .OrderByDescending(g => g.Total)
        .FirstOrDefault();

    var cultura = CultureInfo.GetCultureInfo("es-CO");
    var html = new StringBuilder();

    html.Append($@"
<!DOCTYPE html>
<html lang=""es"">
<head>
    <meta charset=""utf-8"" />
    <style>
        body {{
            font-family: Arial, sans-serif;
            color: #18366f;
            margin: 28px;
            font-size: 11px;
        }}
        .header {{
            background: linear-gradient(135deg, #173c7b 0%, #2f65b9 100%);
            color: #fff;
            padding: 22px 24px;
            border-radius: 18px;
            margin-bottom: 20px;
        }}
        .header h1 {{
            margin: 0 0 6px 0;
            font-size: 24px;
        }}
        .header p {{
            margin: 0;
            opacity: .9;
        }}
        .meta {{
            margin-top: 10px;
            font-size: 10px;
        }}
        .section-title {{
            font-size: 15px;
            margin: 22px 0 12px 0;
            color: #173c7b;
        }}
        .grid {{
            width: 100%;
        }}
        .card {{
            display: inline-block;
            vertical-align: top;
            width: 47%;
            margin: 0 2% 12px 0;
            background: #f7faff;
            border: 1px solid #d9e7ff;
            border-radius: 16px;
            padding: 14px 16px;
            box-sizing: border-box;
        }}
        .card h3 {{
            margin: 0 0 10px 0;
            font-size: 13px;
            color: #173c7b;
        }}
        .metric {{
            margin: 4px 0;
            color: #35527f;
        }}
        .metric strong {{
            color: #102b5c;
        }}
        table {{
            width: 100%;
            border-collapse: collapse;
            margin-top: 8px;
        }}
        thead th {{
            background: #edf4ff;
            color: #173c7b;
            text-align: left;
            padding: 10px;
            font-size: 10px;
            border-bottom: 1px solid #d4e2fb;
        }}
        tbody td {{
            padding: 10px;
            border-bottom: 1px solid #ebf1fb;
            color: #28456f;
        }}
        .badge {{
            display: inline-block;
            padding: 4px 10px;
            border-radius: 999px;
            font-size: 10px;
            font-weight: bold;
        }}
        .pendiente {{
            background: #edf1f6;
            color: #5c6778;
        }}
        .finalizada {{
            background: #e6f8ee;
            color: #1f8f57;
        }}
        .cancelada {{
            background: #ffe9e9;
            color: #c74444;
        }}
        .empty {{
            padding: 14px;
            background: #f7faff;
            border: 1px solid #d9e7ff;
            border-radius: 14px;
            color: #4b6288;
        }}
    </style>
</head>
<body>
    <div class=""header"">
        <h1>Reporte Administrativo de Reservas</h1>
        <p>ParkYa</p>
        <div class=""meta"">
            Generado por: {WebUtility.HtmlEncode($"{admin.nombre} {admin.apellido}".Trim())}<br />
            Fecha de generacion: {DateTime.Now.ToString("dd/MM/yyyy HH:mm", cultura)}<br />
            Rango: {(fechaInicio.HasValue ? fechaInicio.Value.ToString("dd/MM/yyyy", cultura) : "Inicio del historial")} - {(fechaFin.HasValue ? fechaFin.Value.ToString("dd/MM/yyyy", cultura) : "Hoy")}
        </div>
    </div>

    <div class=""section-title"">Resumen General</div>
    <div class=""grid"">
        <div class=""card"">
            <h3>Reservas por periodo</h3>
            <div class=""metric"">Dia: <strong>{_context.reserva.Count(r => r.fecha.Date == hoy)}</strong></div>
            <div class=""metric"">Semana: <strong>{_context.reserva.Count(r => r.fecha.Date >= inicioSemana && r.fecha.Date <= hoy)}</strong></div>
            <div class=""metric"">Mes: <strong>{_context.reserva.Count(r => r.fecha.Date >= inicioMes && r.fecha.Date <= hoy)}</strong></div>
        </div>
        <div class=""card"">
            <h3>Estados del rango</h3>
            <div class=""metric"">Pendientes: <strong>{datos.Count(r => r.Estado == Estado.Pendiente)}</strong></div>
            <div class=""metric"">Finalizadas: <strong>{datos.Count(r => r.Estado == Estado.Finalizada)}</strong></div>
            <div class=""metric"">Canceladas: <strong>{datos.Count(r => r.Estado == Estado.Cancelada)}</strong></div>
        </div>
        <div class=""card"">
            <h3>Ingresos</h3>
            <div class=""metric"">Total real: <strong>{datos.Sum(r => r.Monto).ToString("C0", cultura)}</strong></div>
            <div class=""metric"">Monto promedio: <strong>{(datos.Count > 0 ? datos.Average(r => r.Monto) : 0).ToString("C0", cultura)}</strong></div>
        </div>
        <div class=""card"">
            <h3>Hora pico</h3>
            <div class=""metric"">Franja: <strong>{(horaPico != null ? $"{horaPico.Hora:00}:00 - {horaPico.Hora:00}:59" : "Sin datos")}</strong></div>
            <div class=""metric"">Total reservas: <strong>{(horaPico?.Total ?? 0)}</strong></div>
        </div>
    </div>

    <div class=""section-title"">Detalle de Reservas</div>");

    if (datos.Count > 0)
    {
        html.Append(@"
    <table>
        <thead>
            <tr>
                <th>Cliente</th>
                <th>Placa</th>
                <th>Tipo vehiculo</th>
                <th>Fecha</th>
                <th>Monto</th>
                <th>Estado</th>
            </tr>
        </thead>
        <tbody>");

        foreach (var item in datos)
        {
            var estadoClase = item.Estado == Estado.Pendiente ? "pendiente"
                : item.Estado == Estado.Cancelada ? "cancelada"
                : "finalizada";

            html.Append($@"
            <tr>
                <td>{WebUtility.HtmlEncode(item.Cliente)}</td>
                <td>{WebUtility.HtmlEncode(item.Placa)}</td>
                <td>{WebUtility.HtmlEncode(item.TipoVehiculo)}</td>
                <td>{item.Fecha.ToString("dd/MM/yyyy", cultura)}</td>
                <td>{item.Monto.ToString("C0", cultura)}</td>
                <td><span class=""badge {estadoClase}"">{WebUtility.HtmlEncode(item.Estado.ToString())}</span></td>
            </tr>");
        }

        html.Append(@"
        </tbody>
    </table>");
    }
    else
    {
        html.Append(@"<div class=""empty"">No hay reservas para el rango seleccionado.</div>");
    }

    html.Append(@"
</body>
</html>");

    using var stream = new MemoryStream();
    HtmlConverter.ConvertToPdf(html.ToString(), stream);
    var nombre = $"reportes-admin-{DateTime.Now:yyyyMMdd-HHmmss}.pdf";

    return File(stream.ToArray(), "application/pdf", nombre);
}

private static TimeSpan? ParseNullableTime(object? value)
{
    if (value == null || value == DBNull.Value)
        return null;

    if (value is TimeSpan timeSpan)
        return timeSpan;

    if (value is DateTime dateTime)
        return dateTime.TimeOfDay;

    var raw = value.ToString();
    if (string.IsNullOrWhiteSpace(raw))
        return null;

    if (TimeSpan.TryParse(raw, out var parsedTime))
        return parsedTime;

    if (int.TryParse(raw, out var numeric))
    {
        var hours = numeric / 100;
        var minutes = numeric % 100;

        if (hours is >= 0 and < 24 && minutes is >= 0 and < 60)
            return new TimeSpan(hours, minutes, 0);
    }

    return null;
}

private static decimal ParseDecimal(object? value)
{
    if (value == null || value == DBNull.Value)
        return 0;

    if (value is decimal decimalValue)
        return decimalValue;

    return Convert.ToDecimal(value);
}

private static decimal CalcularMontoReserva(TimeSpan? horaEntrada, TimeSpan? horaSalida, decimal precioHora)
{
    if (!horaEntrada.HasValue || !horaSalida.HasValue)
        return 0;

    var horas = (decimal)(horaSalida.Value - horaEntrada.Value).TotalHours;
    if (horas < 0)
        horas = 0;

    return horas * precioHora;
}
[HttpPost]
public IActionResult DesactivarUsuario([FromBody] CambioEstadoUsuarioRequest request)
{
    if (request == null || request.Id <= 0)
        return BadRequest();

    var usuario = _context.usuario.Find(request.Id);

    if (usuario == null)
        return NotFound();

    usuario.estado = false; // 🔥 0 = Inactivo
    _context.SaveChanges();

    return Ok();
}

[HttpPost]
public IActionResult ActivarUsuario([FromBody] CambioEstadoUsuarioRequest request)
{
    if (request == null || request.Id <= 0)
        return BadRequest();

    var usuario = _context.usuario.Find(request.Id);

    if (usuario == null)
        return NotFound();

    usuario.estado = true; // 🔥 1 = Activo
    _context.SaveChanges();

    return Ok();
}

[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult ActualizarUsuario(ActualizarUsuarioAdminViewModel model)
{
    var adminId = HttpContext.Session.GetInt32("UsuarioId");

    if (adminId == null)
        return RedirectToAction("Login", "Autenticacion");

    var usuario = _context.usuario.FirstOrDefault(u => u.id_usuario == model.Id);

    if (usuario == null)
    {
        TempData["Error"] = "Usuario no encontrado.";
        return RedirectToAction("Usuarios");
    }

    if (model.RolId < 1 || model.RolId > 3)
    {
        TempData["Error"] = "Rol no valido.";
        return RedirectToAction("Usuarios");
    }

    if (usuario.id_usuario == adminId.Value && model.RolId != 3)
    {
        TempData["Error"] = "No puedes quitarte el rol de administrador.";
        return RedirectToAction("Usuarios");
    }

    usuario.nombre = model.Nombre;
    usuario.apellido = model.Apellido;
    usuario.correo = model.Correo;
    usuario.telefono = model.Telefono;
    usuario.tipo_doc = model.TipoDocumento;
    usuario.Rol_id_rol = model.RolId;

    _context.SaveChanges();

    TempData["MensajeUsuarios"] = "Usuario actualizado correctamente.";

    return RedirectToAction("Usuarios");
}

[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult ActualizarTarifa(ActualizarTarifaAdminViewModel model)
{
    var adminId = HttpContext.Session.GetInt32("UsuarioId");

    if (adminId == null)
        return RedirectToAction("Login", "Autenticacion");

    var tarifa = _context.tarifas.FirstOrDefault(t => t.id_tarifas == model.Id);

    if (tarifa == null)
    {
        TempData["ErrorTarifas"] = "Tarifa no encontrada.";
        return RedirectToAction("Tarifas");
    }

    tarifa.precio_dia = model.PrecioDia;
    tarifa.precio_hora = model.PrecioHora;

    _context.SaveChanges();

    TempData["MensajeTarifas"] = "Tarifa actualizada correctamente.";

    return RedirectToAction("Tarifas");
}

public class CambioEstadoUsuarioRequest
{
    public int Id { get; set; }
}

public class ActualizarUsuarioAdminViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string TipoDocumento { get; set; } = string.Empty;
    public int RolId { get; set; }
}

public class ActualizarTarifaAdminViewModel
{
    public int Id { get; set; }
    public decimal PrecioDia { get; set; }
    public decimal PrecioHora { get; set; }
}

    
   }
   
}


