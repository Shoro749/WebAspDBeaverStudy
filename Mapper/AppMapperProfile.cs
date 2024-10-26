using AutoMapper;
using System.Globalization;
using WebAspDBeaverStudy.Data.Entities;
using WebAspDBeaverStudy.Models.Category;
using WebAspDBeaverStudy.Models.Product;

namespace WebAspDBeaverStudy.Mapper
{
    public class AppMapperProfile : Profile
    {
        public AppMapperProfile()
        {
            CreateMap<CategoryEntity, CategoryItemViewModel>();
            CreateMap<CategoryCreateViewModel, CategoryEntity>();
            CreateMap<CategoryEditViewModel, CategoryEntity>();
            CreateMap<ProductCreateViewModel, ProductEntity>();

            CreateMap<ProductEntity, ProductItemViewModel>()
                .ForMember(x => x.Images, opt => opt.MapFrom(p => p.ProductImages.Select(x => x.Image).ToList()))
                .ForMember(x => x.CategoryId, opt => opt.MapFrom(c => c.Category.Id));

            //CreateMap<ProductCreateViewModel, ProductEntity>()
            //    .ForMember(x => x.Price, opt => opt.MapFrom(p => Decimal.Parse(p.Price.Replace('.', ','), new CultureInfo("uk-UA"))));
        }
    }
}
