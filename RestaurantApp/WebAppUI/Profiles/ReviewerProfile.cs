using WebAppUI.Areas.Admin.Models.DTOs;
using WebAppUI.Areas.Admin.Models.ViewModels;
using WebAppUI.Areas.Critic.Models.ViewModels;
using WebAppUI.Models.ViewModels;
namespace WebAppUI.Profiles;
public class ReviewerProfile : Profile
{
    public ReviewerProfile()
    {
        CreateMap<NewReviewerDto, Reviewer>();
        CreateMap<Reviewer, ExistentReviewerDto>();
        CreateMap<Reviewer, CardReviewerVm>()
            .ForMember(d => d.ReviewerId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => src.Name));

        CreateMap<Reviewer, ShortReviewerVm>()
            .ForMember(d => d.ReviewerId, s => s.MapFrom(src => src.Id));
    }
}