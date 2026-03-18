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
                return RedirectToAction("Index", "Admin");

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

            var correoExiste = await _context.usuario.AnyAsync(u => u.correo == model.correo);
            if (correoExiste)
            {
                ModelState.AddModelError("correo", "El correo ya está registrado.");
                return View(model);
            }

            var docExiste = await _context.usuario.AnyAsync(u => u.documento == model.documento);
            if (docExiste)
            {
                ModelState.AddModelError("documento", "El documento ya está registrado.");
                return View(model);
            }

            var rolCliente = await _context.rol.FirstOrDefaultAsync(r => r.nombre_rol.ToLower() == "cliente");
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
                documento = model.documento,
                correo = model.correo,
                telefono = model.telefono,
                contraseña = BCrypt.Net.BCrypt.HashPassword(model.contraseña),
                estado = true,
                Rol_id_rol = rolCliente.id_rol
            };

            _context.usuario.Add(usuario);
            await _context.SaveChangesAsync();

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
    }
}