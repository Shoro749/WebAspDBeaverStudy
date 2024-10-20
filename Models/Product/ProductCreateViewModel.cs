using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebAspDBeaverStudy.Models.Product
{
    public class ProductCreateViewModel
    {
        [Display(Name = "Назва продукту")]
        public string Name { get; set; } = String.Empty;
        [Display(Name = "Ціна продукту")]
        public decimal Price { get; set; }
        [Display(Name = "Оберіть фото на ПК")]
        public List<IFormFile>? Photos { get; set; }
        [Display(Name = "Оберіть категорію")]
        public int CategoryId { get; set; }
        public IEnumerable<SelectListItem>? Categories { get; set; }
    }
}
