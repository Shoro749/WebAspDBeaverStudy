using AutoMapper;
using WebAspDBeaverStudy.Data.Entities;
using WebAspDBeaverStudy.Models.Category;

namespace WebAspDBeaverStudy.Mapper
{
    public class AppMapperProfile : Profile
    {
        public AppMapperProfile()
        {
            CreateMap<CategoryEntity, CategoryItemViewModel>();
            CreateMap<CategoryCreateViewModel, CategoryEntity>();
        }
    }
}
