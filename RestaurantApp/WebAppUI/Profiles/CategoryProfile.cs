using WebAppUI.Areas.Manager.Models.DTOs;
using WebAppUI.Areas.Manager.Models.ViewModels;
using WebAppUI.Areas.Chef.Models.DTOs;
using WebAppUI.Areas.Chef.Models.ViewModels;
using WebAppUI.Models.ViewModels;
namespace WebAppUI.Profiles;
public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Areas.Chef.Models.DTOs.NewCategoryDto, Category>();

        CreateMap<Category, Areas.Chef.Models.DTOs.ExistentCategoryDto>();

        CreateMap<Category, Areas.Chef.Models.DTOs.CategoryDto>()
            .ForMember(d => d.CategoryId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => $"{src.Name}"));

        CreateMap<Areas.Manager.Models.DTOs.NewCategoryDto, Category>();

        CreateMap<Category, Areas.Manager.Models.DTOs.ExistentCategoryDto>();

        CreateMap<Category, Areas.Manager.Models.DTOs.CategoryDto>()
            .ForMember(d => d.CategoryId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => $"{src.Name}"));

        CreateMap<Category, CardCategoryVm>()
            .ForMember(d => d.CategoryId, s => s.MapFrom(src => src.Id));
    }
}