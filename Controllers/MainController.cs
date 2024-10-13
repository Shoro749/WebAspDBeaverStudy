using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        private readonly IMapper _mapper;

        //DI - Depencecy Injection
        public MainController(AppDbContext context,
            IWebHostEnvironment environment, IImageWorker imageWorker,
            IMapper mapper)
        {
            _dbContext = context;
            _environment = environment;
            _imageWorker = imageWorker;
            _mapper = mapper;
        }

        // Метод у контролері називається - action - дія
        public IActionResult Index()
        {
            List<CategoryItemViewModel> model = _dbContext.Categories
                .ProjectTo<CategoryItemViewModel>(_mapper.ConfigurationProvider)
                .ToList(); 
            return View(model);
        }

        [HttpGet] 
        public IActionResult Create() // Відкривається Create.cshtml
        {
            return View();
        }

        [HttpPost] //це означає, що ми отримуємо дані із форми від клієнта
        public IActionResult Create(CategoryCreateViewModel model)
        {
            var entity = _mapper.Map<CategoryEntity>(model);
            //Збережння в Базу даних інформації
            var dirName = "uploading";
            var dirSave = Path.Combine(_environment.WebRootPath, dirName); // Шлях до папки завантаження
            if (!Directory.Exists(dirSave)) // Якщо папкм не існує - створюємо її
            {
                Directory.CreateDirectory(dirSave);
            }
            if (model.Photo != null) // Якщо фото не null викликаємо функцію imageWorker
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
            // Запис данних і збереження
            _dbContext.Categories.Add(entity);
            _dbContext.SaveChanges();
            //Переходимо до списку усіх категорій, тобото визиваємо метод Index нашого контролера
            return Redirect("/");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var category = _dbContext.Categories.Find(id); // Знаходимо категорію в базі даних
            if (category == null) // Якщо категорію не знайдено
            {
                return NotFound(); // Повертаємо помилку
            }

            if (!string.IsNullOrEmpty(category.Image))
            {
                _imageWorker.Delete(category.Image); // Якщо у категорії є зображення - видаляємо зображення через сервіс
            }
            _dbContext.Categories.Remove(category); // Видаляємо категорію з контексту
            _dbContext.SaveChanges(); // Зберігаємо зміни в базі даних

            return Json(new { text = "Ми його видалили" }); // Повертаємо JSON об'єкт з повідомленням
        }

        [HttpGet]
        public IActionResult InCategory(int id)
        {
            var category = _dbContext.Categories.Find(id);
            var model = _dbContext.Products
               .Where(p => p.CategoryId == id).ToList(); // Отримуємо список продуктів для даної категорії

            foreach (var item in model) 
            {
                item.ProductImages = _dbContext.ProductsImages.Where(p => p.ProductId == item.Id).ToList(); // Отримуємо зображення продуктів
            }

            if (category == null)
            {
                return NotFound();
            }

            return View(model); // Переходимо у каталог продуктів і передаємо model
        }
    }
}
