using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PickleBallAPI.Controllers;

public partial class TeamDto
{
    public int TeamId { get; set; }

    public int PlayerOneId { get; set; }

    public int PlayerTwoId { get; set; }

    public virtual PlayerDto PlayerOne { get; set; } = null!;

    public virtual PlayerDto PlayerTwo { get; set; } = null!;
}
