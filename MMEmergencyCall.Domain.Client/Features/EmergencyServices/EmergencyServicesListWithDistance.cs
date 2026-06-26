namespace MMEmergencyCall.Domain.Client.Features.EmergencyServices;

public class EmergencyServicesListWithDistance
{
    public int PageNo { get; set; }
    public int PageSize { get; set; }
    public int PageCount { get; set; }
    public bool IsEndOfPage => PageNo >= PageCount;
    public List<EmergencyServicesWithDistance> Data { get; set; } = new();
}
