using System;
using System.Collections.Generic;

namespace PickleBallAPI.Models;

public partial class Facility
{
    public int FacilityId { get; set; }

    public string Name { get; set; } = null!;

    public string AddressLine1 { get; set; } = null!;

    public string? AddressLine2 { get; set; }

    public string City { get; set; } = null!;

    public string StateCode { get; set; } = null!;

    public string? PostalCode { get; set; }

    public int NumberCourts { get; set; }

    public int TypeFacilityId { get; set; }

    public string? Notes { get; set; }

    public long ChangedTime { get; set; }

    public virtual TypeFacility TypeFacility { get; set; } = null!;
}
