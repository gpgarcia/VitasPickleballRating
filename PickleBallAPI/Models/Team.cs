using System;
using System.Collections.Generic;

namespace PickleBallAPI.Models;

public partial class Team
{
    public int TeamId { get; set; }

    public int PlayerOneId { get; set; }

    public int PlayerTwoId { get; set; }

    public DateTimeOffset ChangedDate { get; set; }

    public virtual ICollection<Game> GameTeamOnes { get; set; } = new List<Game>();

    public virtual ICollection<Game> GameTeamTwos { get; set; } = new List<Game>();

    public virtual Player PlayerOne { get; set; } = null!;

    public virtual Player PlayerTwo { get; set; } = null!;
}
