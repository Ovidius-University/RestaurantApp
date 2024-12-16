namespace WebAppUI.Areas.Admin.Models.ViewModels
{
    public class IndexEmployeeSchedulesVm
    {
        public int DayId { get; set; }
        public string Day { get; set; } = string.Empty;
        public List<EmployeeScheduleVm>? ListEmployeeSchedules { get; set; }
    }
}
