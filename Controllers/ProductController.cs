using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using WebAspDBeaverStudy.Data;
using WebAspDBeaverStudy.Interfaces;
using WebAspDBeaverStudy.Models.Product;

namespace WebAspDBeaverStudy.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        //DI - Depencecy Injection
        public ProductController(AppDbContext context, IMapper mapper)
        {
            _dbContext = context;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            List<ProductItemViewModel> model = _dbContext.Products
                .ProjectTo<ProductItemViewModel>(_mapper.ConfigurationProvider)
                .ToList();
            return View(model);
        }
    }
}
