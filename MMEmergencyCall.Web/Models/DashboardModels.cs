namespace MMEmergencyCall.Web.Models;

public sealed class DashboardModel
{
    public RequestSummaryModel RequestSummary { get; set; } = new();
    public List<TopTenServicePerTownshipModel> TopTenServicePerTownship { get; set; } = new();
    public List<ServiceProviderActivityModel> ServiceProviderActivity { get; set; } = new();
    public List<TopTenRequestPerUserModel> TopTenRequestPerUser { get; set; } = new();
}

public sealed class RequestSummaryModel
{
    public int DailyRequest { get; set; }
    public int WeeklyRequest { get; set; }
    public int MonthlyRequest { get; set; }
}

public sealed class ServiceProviderActivityModel
{
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string ServiceType { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Count { get; set; }
}

public sealed class TopTenRequestPerUserModel
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string UserPhoneNumber { get; set; } = string.Empty;
    public string UserAddress { get; set; } = string.Empty;
    public int Count { get; set; }
}

public sealed class TopTenServicePerTownshipModel
{
    public string TownshipCode { get; set; } = string.Empty;
    public string TownshipNameEn { get; set; } = string.Empty;
    public string TownshipNameMM { get; set; } = string.Empty;
    public int Count { get; set; }
}
