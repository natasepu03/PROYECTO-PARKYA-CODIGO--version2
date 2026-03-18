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
                .FirstOrDefaultAsync(u => u.id_usuario == usuarioId.Value);

            if (usuario == null)
                return RedirectToAction("Login", "Autenticacion");

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarPerfil(EditarPerfilViewModel model)
        {
            var usuarioId = ObtenerUsuarioId();
            if (usuarioId == null)
                return RedirectToAction("Login", "Autenticacion");

            if (!ModelState.IsValid)
            {
                var usuarioError = await _context.usuario.FirstOrDefaultAsync(u => u.id_usuario == usuarioId.Value);
                return View("Index", usuarioError);
            }

            var usuario = await _context.usuario.FirstOrDefaultAsync(u => u.id_usuario == usuarioId.Value);
            if (usuario == null)
                return RedirectToAction("Login", "Autenticacion");

            usuario.nombre = model.Nombre;
            usuario.apellido = model.Apellido;
            usuario.correo = model.Correo;
            usuario.telefono = model.Telefono;

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Perfil actualizado correctamente.";
            return RedirectToAction("Index");
        }
    }
}