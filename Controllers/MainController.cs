using Microsoft.AspNetCore.Mvc;

namespace WebAspDBeaverStudy.Controllers
{
    public class MainController : Controller
    {
        // Метод у контролері називається - action - дія
        public IActionResult Index() //IActionResult - для html сторінки
        {
            return View();
        }
    }
}
