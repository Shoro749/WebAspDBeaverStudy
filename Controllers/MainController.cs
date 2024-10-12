using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            _imageWorker = imageWorker;
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

        [HttpPost] //це означає, що ми отримуємо дані із форми від клієнта
        public IActionResult Create(CategoryCreateViewModel model)
        {
            var entity = new CategoryEntity();
            //Збережння в Базу даних інформації
            var dirName = "uploading";
            var dirSave = Path.Combine(_environment.WebRootPath, dirName);
            if (!Directory.Exists(dirSave))
            {
                Directory.CreateDirectory(dirSave);
            }
            if (model.Photo != null)
            {
                //унікальне значенн, яке ніколи не повториться
                //string fileName = Guid.NewGuid().ToString();
                //var ext = Path.GetExtension(model.Photo.FileName);
                //fileName += ext;
                //var saveFile = Path.Combine(dirSave, fileName);
                //using (var stream = new FileStream(saveFile, FileMode.Create)) 
                //    model.Photo.CopyTo(stream);
                entity.Image = _imageWorker.Save(model.Photo);
            }
            entity.Name = model.Name;
            entity.Description = model.Description;
            _dbContext.Categories.Add(entity);
            _dbContext.SaveChanges();
            //Переходимо до списку усіх категорій, тобото визиваємо метод Index нашого контролера
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

            if (!string.IsNullOrEmpty(category.Image))
            {
                _imageWorker.Delete(category.Image);
            }
            _dbContext.Categories.Remove(category);
            _dbContext.SaveChanges();

            return Json(new { text = "Ми його видалили" }); // Вертаю об'єкт у відповідь
        }

        [HttpGet]
        public IActionResult InCategory(int id)
        {
            var category = _dbContext.Categories.Find(id);
            var model = _dbContext.Products
               .Where(p => p.CategoryId == id).ToList();

            foreach(var item in model)
            {
                item.ProductImages = _dbContext.ProductsImages.Where(p => p.ProductId == item.Id).ToList();
            }
            
            if (category == null)
            {
                return NotFound();
            }

            return View(model);
        }
    }
}
