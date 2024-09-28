using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAspDBeaverStudy.Data;
using WebAspDBeaverStudy.Data.Entities;
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
            var category = _dbContext.Categories.FirstOrDefault(c => c.Id == id);
            if (category != null)
            {
                var imagePath = Path.Combine(_environment.WebRootPath, "uploading", category.Image);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                _dbContext.Categories.Remove(category);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return Redirect("/");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var item = _dbContext.Categories.Find(id);
            var model = new CategoryEditViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(CategoryEditViewModel model)
        {
            var id = model.Id;
            var entity = _dbContext.Categories.Find(model.Id);
            var dirName = "uploading";
            var dirSave = Path.Combine(_environment.WebRootPath, dirName);
            if (!Directory.Exists(dirSave))
            {
                Directory.CreateDirectory(dirSave);
            }
            if (model.Photo != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var ext = Path.GetExtension(model.Photo.FileName);
                fileName += ext;
                var saveFile = Path.Combine(dirSave, fileName);
                using (var stream = new FileStream(saveFile, FileMode.Create))
                    model.Photo.CopyTo(stream);

                var oldFile = Path.Combine(_environment.WebRootPath, "uploading", entity.Image);
                if (System.IO.File.Exists(oldFile))
                {
                    System.IO.File.Delete(oldFile);
                }
                entity.Image = fileName;
            }
            entity.Name = model.Name;
            entity.Description = model.Description;

            _dbContext.SaveChanges();

            return Redirect("/");
        }
    }
}
