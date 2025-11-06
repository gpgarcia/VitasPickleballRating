using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PickleBallAPI.Controllers;

public partial class PlayerDto
{
    public int PlayerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public int? LastRating { get; set; } = null!;


}
