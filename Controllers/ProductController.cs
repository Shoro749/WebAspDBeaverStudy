using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAspDBeaverStudy.Data;
using WebAspDBeaverStudy.Data.Entities;
using WebAspDBeaverStudy.Interfaces;
using WebAspDBeaverStudy.Models.Category;
using WebAspDBeaverStudy.Models.Product;

namespace WebAspDBeaverStudy.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IImageWorker _imageWorker;
        private readonly IWebHostEnvironment _environment;
        //DI - Depencecy Injection
        public ProductController(AppDbContext context, IMapper mapper, IImageWorker imageWorker, IWebHostEnvironment environment)
        {
            _dbContext = context;
            _mapper = mapper;
            _imageWorker = imageWorker;
            _environment = environment;
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

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _dbContext.Products.Find(id);
            var model = new ProductEditViewModel
            {
                Id = id,
                Name = product.Name,
                Price = product.Price
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(ProductEditViewModel model)
        {
            var entity = _mapper.Map<ProductEntity>(model);
            var product = _dbContext.Products.Find(model.Id);

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
            product.Price = entity.Price;
            product.Name = entity.Name;

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
    }
}
