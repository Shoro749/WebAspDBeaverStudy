using Microsoft.AspNetCore.Mvc;
using WebAspDBeaverStudy.Data;
using WebAspDBeaverStudy.Data.Entities;
using WebAspDBeaverStudy.Interfaces;
using WebAspDBeaverStudy.Models.Category;

namespace WebAspDBeaverStudy.Controllers
{
    public class MainController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IImageWorker _imageWorker;
        private readonly IWebHostEnvironment _environment;

        //DI - Depencecy Injection
        public MainController(AppDbContext context,
            IWebHostEnvironment environment, IImageWorker imageWorker)
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
            var entity = new CategoryEntity();
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
                entity.Image = fileName;
            }
            entity.Name = model.Name;
            entity.Description = model.Description;
            _dbContext.Categories.Add(entity);
            _dbContext.SaveChanges();
            // Переходимо до списку усіх категорій, тобто визиваємо метод Index нашого контролера
            return Redirect("/");
        }

		[HttpPost]
		public IActionResult Delete(int id)
		{
			var category = _dbContext.Categories.Find(id);
			if (category == null)
			{
				return NotFound();
			}

            if (!string.IsNullOrEmpty(category.Name))
            {
                _imageWorker.Delete(category.Image);

            }

			_dbContext.Categories.Remove(category);
			_dbContext.SaveChanges();

			return Json(new { text="Deleted" });
		}
	}
}
