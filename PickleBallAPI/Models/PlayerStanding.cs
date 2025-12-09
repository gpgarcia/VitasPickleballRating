using System;
using System.Collections.Generic;

namespace PickleBallAPI.Models;

public partial class PlayerStanding
{
    public string FirstName { get; set; } = null!;

    public int? GamesPlayed { get; set; }

    public int? Wins { get; set; }

    public int? Losses { get; set; }

    public decimal? WinPct { get; set; }
}
