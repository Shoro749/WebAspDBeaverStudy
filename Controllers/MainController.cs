using Microsoft.AspNetCore.Mvc;
using WebAspDBeaverStudy.Data;
using WebAspDBeaverStudy.Models.Category;

namespace WebAspDBeaverStudy.Controllers
{
    public class MainController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;

        //DI - Depencecy Injection
        public MainController(AppDbContext context,
            IWebHostEnvironment environment)
        {
            _dbContext = context;
            _environment = environment;
        }

        // Метод у контролері називається - action - дія
        public IActionResult Index() //IActionResult - для html сторінки
        {
            var model = _dbContext.Categories.ToList();
            return View(model);
        }

        [HttpGet] 
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost] // Отримуємо дані із форми 
        public IActionResult Create(CategoryCreateViewModel model)
        {
            // Збережемо в базу даних інформації
            var dirName = "uploading";
            var dirSave = Path.Combine(_environment.WebRootPath, dirName);  
            if (!Directory.Exists(dirSave))
            {
                Directory.CreateDirectory(dirSave);
            }
            if (model.Photo != null)
            {
                // Унікальне значення (неповторне)
                string fileName = Guid.NewGuid().ToString();
                var ext = Path.GetExtension(model.Photo.FileName);
                fileName += ext;
                var saveFile = Path.Combine(dirSave, fileName);
                using (var stream = new FileStream(saveFile, FileMode.Create))
                    model.Photo.CopyTo(stream);
            }
            // Що отримали те й повертаємо назад
            return View(model);
        }
    }
}
