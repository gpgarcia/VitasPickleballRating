using System;
using System.Collections.Generic;

namespace PickleBallAPI.Models;

public partial class TypeFacility
{
    public int TypeFacilityId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Facility> Facilities { get; set; } = new List<Facility>();
}
