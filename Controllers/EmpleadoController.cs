using Microsoft.AspNetCore.Mvc;

namespace ParkYa.Controllers
{
    public class EmpleadoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}