using System.ComponentModel.DataAnnotations;

namespace WebAspDBeaverStudy.Models.Category
{
    public class CategoryCreateViewModel
    {
        [Display(Name = "Назва категорії")]
        public string Name { get; set; } = String.Empty;
        [Display(Name = "Оберіть фото на ПК")]
        public IFormFile? Photo { get; set; }
        [Display(Name = "Короткий опис")]
        public string Description { get; set; } = string.Empty;
    }
}
