using WebAppUI.Areas.Admin.Models.DTOs;
using WebAppUI.Areas.Admin.Models.ViewModels;
using WebAppUI.Models.DTOs;

namespace WebAppUI.Profiles;

public class EmployeeScheduleProfile : Profile
{
    public EmployeeScheduleProfile()
    {
        CreateMap<EmployeeSchedule, EmployeeScheduleVm>()
            .ForMember(d => d.Employee, s => s.MapFrom(src => src.Employee!.UserName))
            .ForMember(d => d.Day, s => s.MapFrom(src => src.Day!.Name));

        CreateMap<NewEmployeeScheduleDto, EmployeeSchedule>()
            .ForMember(d => d.Employee, s => s.Ignore())
            .ForMember(d => d.Day, s => s.Ignore());

        CreateMap<EmployeeSchedule, ExistentEmployeeScheduleDto>()
            .ForMember(d => d.Employee, s => s.MapFrom(src => src.Employee!.UserName))
            .ForMember(d => d.Day, s => s.MapFrom(src => src.Day!.Name))
            .ForMember(d => d.EmployeeId, s => s.MapFrom(src => src.EmployeeId))
            .ForMember(d => d.DayId, s => s.MapFrom(src => src.DayId));

        
         CreateMap<ExistentEmployeeScheduleDto, EmployeeSchedule>()
            .ForMember(d => d.Employee, s => s.Ignore())
            .ForMember(d => d.Day, s => s.Ignore());
    }
}