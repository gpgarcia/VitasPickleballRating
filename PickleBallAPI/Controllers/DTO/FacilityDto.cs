using PickleBallAPI.Models;

namespace PickleBallAPI.Controllers.DTO;

public class FacilityDto
{
    public int FacilityId { get; set; }

    public string Name { get; set; } = null!;

    public string AddressLine1 { get; set; } = null!;

    public string? AddressLine2 { get; set; }

    public string City { get; set; } = null!;

    public string StateCode { get; set; } = null!;

    public string? PostalCode { get; set; }

    public int NumberCourts { get; set; }

    public TypeFacilityDto TypeFacility { get; set; } = null!;

    public string? Notes { get; set; }

}
