namespace MMEmergencyCall.Web.Models;

public sealed class PagedList<T>
{
    public int PageNo { get; set; }
    public int PageSize { get; set; }
    public int PageCount { get; set; }
    public bool IsEndOfPage { get; set; }
    public List<T> Data { get; set; } = new();
}

public sealed class EmergencyRequestListItem
{
    public int RequestId { get; set; }
    public int UserId { get; set; }
    public int ServiceId { get; set; }
    public int? ProviderId { get; set; }
    public DateTime RequestTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? ResponseTime { get; set; }
    public string? Notes { get; set; }
    public string? TownshipCode { get; set; }
}

public sealed class EmergencyServiceListItem
{
    public int ServiceId { get; set; }
    public int UserId { get; set; }
    public string ServiceGroup { get; set; } = string.Empty;
    public string ServiceType { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? Availability { get; set; }
    public string? TownshipCode { get; set; }
    public string? ServiceStatus { get; set; }
}

public sealed class UserListItem
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? EmergencyType { get; set; }
    public string? EmergencyDetails { get; set; }
    public string TownshipCode { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string UserStatus { get; set; } = string.Empty;
}

public sealed class TownshipListItem
{
    public int TownshipId { get; set; }
    public string TownshipCode { get; set; } = string.Empty;
    public string TownshipNameEn { get; set; } = string.Empty;
    public string TownshipNameMm { get; set; } = string.Empty;
    public string StateRegionCode { get; set; } = string.Empty;
}

public sealed class StateRegionListItem
{
    public int StateRegionId { get; set; }
    public string StateRegionCode { get; set; } = string.Empty;
    public string StateRegionNameEn { get; set; } = string.Empty;
    public string? StateRegionNameMm { get; set; }
}
