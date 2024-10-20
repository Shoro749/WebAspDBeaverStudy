using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAspDBeaverStudy.Data;
using WebAspDBeaverStudy.Interfaces;
using WebAspDBeaverStudy.Models.Product;

namespace WebAspDBeaverStudy.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IImageWorker _imageWorker;
        //DI - Depencecy Injection
        public ProductController(AppDbContext context, IMapper mapper, IImageWorker imageWorker)
        {
            _dbContext = context;
            _mapper = mapper;
            _imageWorker = imageWorker;
        }

        public IActionResult Index(int id)
        {
            List<ProductItemViewModel> model = _dbContext.Products
                .Where(p => p.CategoryId == id)
                .ProjectTo<ProductItemViewModel>(_mapper.ConfigurationProvider)
                .ToList();
            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var product = _dbContext.Products.Include(x => x.ProductImages).SingleOrDefault(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            if (product.ProductImages != null)
            {
                foreach (var productImage in product.ProductImages)
                {
                    _imageWorker.Delete(productImage.Image);
                    _dbContext.ProductsImages.Remove(productImage);

                }
            }
            _dbContext.Products.Remove(product);
            _dbContext.SaveChanges();

            return Json(new { text = "Ми його видалили" }); // Вертаю об'єкт у відповідь
        }
    }
}
