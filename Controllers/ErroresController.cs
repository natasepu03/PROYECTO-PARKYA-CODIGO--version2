using Microsoft.AspNetCore.Mvc;

namespace ParkYa.Controllers
{
    public class ErroresController : Controller
    {
        [Route("Errores/404")]
        [HttpGet]
        public IActionResult Error404()
        {
            Response.StatusCode = 404;
            return View("404");
        }

        [Route("Errores/500")]
        [HttpGet]
        public IActionResult Error500()
        {
            Response.StatusCode = 500;
            return View("500");
        }
    }
}
