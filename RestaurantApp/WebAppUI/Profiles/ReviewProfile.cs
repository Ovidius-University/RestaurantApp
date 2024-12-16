using WebAppUI.Areas.Critic.Models.DTOs;
using WebAppUI.Areas.Manager.Models.ViewModels;
using WebAppUI.Areas.Critic.Models.ViewModels;
using WebAppUI.Areas.Admin.Models.DTOs;
using WebAppUI.Areas.Admin.Models.ViewModels;
using WebAppUI.Models.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace WebAppUI.Profiles;

public class ReviewProfile : Profile
{
    public ReviewProfile()
    {
        CreateMap<Review, Areas.Critic.Models.ViewModels.ReviewVm>();

        CreateMap<NewReviewDto, Review>()
            .ForMember(d => d.IsFinal, s => s.MapFrom(src => false));

        CreateMap<Review, Areas.Admin.Models.ViewModels.ReviewVm>()
            .ForMember(d => d.FoodId, s => s.MapFrom(src => src.Food!.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => src.Food!.Title));

        CreateMap<Review, ExistentReviewDto>()
            .ForMember(d => d.Food, s => s.MapFrom(src => src.Food!.Title))
            .ForMember(d => d.Reviewer, s => s.MapFrom(src => src.Reviewer!.Name))
            .ForMember(d => d.FoodId, s => s.MapFrom(src => src.FoodId))
            .ForMember(d => d.ReviewerId, s => s.MapFrom(src => src.ReviewerId));

        CreateMap<Review, ShortReviewerVm>()
            .ForMember(d => d.ReviewerId, s => s.MapFrom(src => src.ReviewerId))
            .ForMember(d => d.Name, s => s.MapFrom(src => src.Reviewer!.Name));

        CreateMap<Review, ReviewDto>()
            .ForMember(d => d.FoodId, s => s.MapFrom(src => src.FoodId))
            .ForMember(d => d.ReviewerId, s => s.MapFrom(src => src.ReviewerId))
            .ForMember(d => d.Content, s => s.MapFrom(src => $"{src.Content}"));

       CreateMap<Review, Areas.Critic.Models.ViewModels.ReviewDetailsVm>()
            .ForMember(d => d.Food, s => s.MapFrom(src => src.Food!.Title))
            .ForMember(d => d.Reviewer, s => s.MapFrom(src => src.Reviewer!.Name));

        CreateMap<Review, CardReviewVm>()
            .ForMember(d => d.Food, s => s.MapFrom(src => src.Food!.Title))
            .ForMember(d => d.Reviewer, s => s.MapFrom(src => src.Reviewer!.Name))
            .ForMember(d => d.FoodId, s => s.MapFrom(src => src.FoodId))
            .ForMember(d => d.ReviewerId, s => s.MapFrom(src => src.ReviewerId));

        CreateMap<Review, Models.ViewModels.ReviewDetailsVm>()
            .ForMember(d => d.Food, s => s.MapFrom(src => src.Food!.Title))
            .ForMember(d => d.Reviewer, s => s.MapFrom(src => src.Reviewer!.Name))
            .ForMember(d => d.FoodId, s => s.MapFrom(src => src.FoodId))
            .ForMember(d => d.ReviewerId, s => s.MapFrom(src => src.ReviewerId));
    }
}