using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkYa.Data;
using ParkYa.Models;
using ParkYa.Models.ViewModels;

namespace ParkYa.Controllers
{
    public class AutenticacionController : Controller
    {
        private readonly ParkYaDbContext _context;

        public AutenticacionController(ParkYaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = await _context.usuario
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.correo == model.Correo);

            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Correo o contraseña inválidos.");
                return View(model);
            }

            var passwordValido = BCrypt.Net.BCrypt.Verify(model.Password, usuario.contraseña);

            if (!passwordValido)
            {
                ModelState.AddModelError(string.Empty, "Correo o contraseña inválidos.");
                return View(model);
            }

            HttpContext.Session.SetInt32("UsuarioId", usuario.id_usuario);
            HttpContext.Session.SetString("UsuarioNombre", $"{usuario.nombre} {usuario.apellido}");
            HttpContext.Session.SetInt32("RolId", usuario.Rol_id_rol);
            HttpContext.Session.SetString("RolNombre", usuario.Rol?.nombre_rol ?? string.Empty);

            var rol = (usuario.Rol?.nombre_rol ?? string.Empty).ToLower();

            if (rol.Contains("admin"))
                return RedirectToAction("Index", "Administrador");

            if (rol.Contains("empleado"))
                return RedirectToAction("Index", "Empleado");

            return RedirectToAction("Index", "Clientes");
        }

        [HttpGet]
        public IActionResult Registro()
        {
            return View(new RegistroViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(RegistroViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var correoNormalizado = model.correo.Trim().ToLower();
            var documentoNormalizado = model.documento.Trim();
            var preguntaSeguridad = model.PreguntaSeguridad?.Trim() ?? string.Empty;
            var respuestaSeguridad = model.RespuestaSeguridad?.Trim() ?? string.Empty;

            var correoExiste = await _context.usuario.AnyAsync(u => u.correo.ToLower() == correoNormalizado);
            if (correoExiste)
            {
                ModelState.AddModelError("correo", "El correo ya está registrado.");
                return View(model);
            }

            var docExiste = await _context.usuario.AnyAsync(u => u.documento == documentoNormalizado);
            if (docExiste)
            {
                ModelState.AddModelError("documento", "El documento ya está registrado.");
                return View(model);
            }

            var rolCliente = await _context.rol.FirstOrDefaultAsync(r =>
                r.nombre_rol != null && r.nombre_rol.Trim().ToLower().Contains("cliente"));
            if (rolCliente == null)
            {
                ModelState.AddModelError(string.Empty, "No existe un rol 'cliente' en la base de datos.");
                return View(model);
            }

            var usuario = new Usuario
            {
                nombre = model.nombre,
                apellido = model.apellido,
                tipo_doc = model.tipo_doc,
                documento = documentoNormalizado,
                correo = correoNormalizado,
                telefono = model.telefono.Trim(),
                contraseña = BCrypt.Net.BCrypt.HashPassword(model.contraseña),
                estado = true,
                Rol_id_rol = rolCliente.id_rol,

                PreguntaSeguridad = preguntaSeguridad,
                RespuestaSeguridad = respuestaSeguridad

            };

            try
            {
                _context.usuario.Add(usuario);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var detalle = ex.InnerException?.Message ?? ex.Message;
                ModelState.AddModelError(string.Empty, $"No se pudo completar el registro. {detalle}");
                return View(model);
            }

            HttpContext.Session.SetInt32("UsuarioId", usuario.id_usuario);
            HttpContext.Session.SetString("UsuarioNombre", $"{usuario.nombre} {usuario.apellido}");
            HttpContext.Session.SetInt32("RolId", usuario.Rol_id_rol);
            HttpContext.Session.SetString("RolNombre", "Cliente");

            return RedirectToAction("Index", "Clientes");
        }

        [HttpGet]
        public IActionResult RecuperacionC()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RecuperacionC(string correo)
        {
            ViewBag.Mensaje = $"Si el correo {correo} existe, se enviará un enlace de recuperación.";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Recuperacion()
        {
            return View(new ParkYa.Models.ViewModels.RecuperarPasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Recuperacion(ParkYa.Models.ViewModels.RecuperarPasswordViewModel model)
        {
            if (!model.MostrarPaso2)
            {
                if (string.IsNullOrWhiteSpace(model.Correo))
                {
                    ModelState.AddModelError("Correo", "El correo es obligatorio");
                    return View(model);
                }

                var usuario = await _context.usuario
                    .FirstOrDefaultAsync(u => u.correo == model.Correo);

                if (usuario == null)
                {
                    TempData["Error"] = "No existe una cuenta con ese correo.";
                    return View(model);
                }

                model.PreguntaSeguridad = usuario.PreguntaSeguridad ?? "";
                model.MostrarPaso2 = true;

                ModelState.Clear();
                return View(model);
            }

            var usuarioPaso2 = await _context.usuario
                .FirstOrDefaultAsync(u => u.correo == model.Correo);

            if (usuarioPaso2 == null)
            {
                TempData["Error"] = "No existe una cuenta con ese correo.";
                return View(model);
            }

            if ((usuarioPaso2.RespuestaSeguridad ?? "").Trim().ToLower() !=
                (model.RespuestaSeguridad ?? "").Trim().ToLower())
            {
                TempData["Error"] = "La respuesta de seguridad no es correcta.";
                model.PreguntaSeguridad = usuarioPaso2.PreguntaSeguridad ?? "";
                model.MostrarPaso2 = true;
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.NuevaPassword) || string.IsNullOrWhiteSpace(model.ConfirmarPassword))
            {
                TempData["Error"] = "Completa la nueva contraseña y su confirmación.";
                model.PreguntaSeguridad = usuarioPaso2.PreguntaSeguridad ?? "";
                model.MostrarPaso2 = true;
                return View(model);
            }

            if (model.NuevaPassword != model.ConfirmarPassword)
            {
                TempData["Error"] = "Las contraseñas no coinciden.";
                model.PreguntaSeguridad = usuarioPaso2.PreguntaSeguridad ??"";
                model.MostrarPaso2 = true;
                return View(model);
            }

            usuarioPaso2.contraseña = BCrypt.Net.BCrypt.HashPassword(model.NuevaPassword);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Contraseña actualizada correctamente.";
            return RedirectToAction("Login");
        }
    }
}
