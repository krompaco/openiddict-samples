using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Velusia.Server.ViewModels.Shared;

namespace Velusia.Server.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml", new ErrorViewModel());
        }
    }
}
