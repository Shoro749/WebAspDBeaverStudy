using Microsoft.AspNetCore.Mvc;
using WebAspDBeaverStudy.Data;

namespace WebAspDBeaverStudy.Controllers
{
    public class MainController : Controller
    {
        private readonly AppDbContext _dbContext;

        //DI - Depencecy Injection
        public MainController(AppDbContext context)
        {
            _dbContext = context;
        }

        // Метод у контролері називається - action - дія
        public IActionResult Index() //IActionResult - для html сторінки
        {
            var model = _dbContext.Categories.ToList();
            return View(model);
        }
    }
}
