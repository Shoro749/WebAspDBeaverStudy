using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAspDBeaverStudy.Data;
using WebAspDBeaverStudy.Data.Entities;
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

        [HttpGet]
        public IActionResult Create(/*int categoryId*/)
        {
            var model = new ProductCreateViewModel
            {
                Categories = _dbContext.Categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(ProductCreateViewModel model)
        {
            var product = _mapper.Map<ProductEntity>(model);
            product.Category = _dbContext.Categories.Find(model.CategoryId);
            _dbContext.Add(product);
            _dbContext.SaveChanges();
            int imageCount = model.Photos.Count();
            for (int i = 0; i < imageCount; i++)
            {
                var imageProduct = new ProductImageEntity
                {
                    Product = product,
                    Image = _imageWorker.Save(model.Photos.ElementAt(i)),
                    Priority = i
                };
                _dbContext.Add(imageProduct);
                _dbContext.SaveChanges();
            }
            return Redirect("/");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var product = _dbContext.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }
            var productImg = _dbContext.ProductsImages.Where(p => p.ProductId == product.Id).ToList();

            foreach (var img in productImg)
            {
                if (!string.IsNullOrEmpty(img.Image))
                {
                    _imageWorker.Delete(img.Image);
                    _dbContext.ProductsImages.Remove(img);
                }
            }
            _dbContext.Products.Remove(product);
            _dbContext.SaveChanges();

            return Json(new { text = "Ми його видалили" });
        }
    }
}
