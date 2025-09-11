using System;
using System.Collections.Generic;

namespace PickleBallAPI.Models;

public partial class Game
{
    public int GameId { get; set; }

    public DateTimeOffset? PlayedDate { get; set; }

    public int TypeGameId { get; set; }

    public int TeamOneId { get; set; }

    public int TeamTwoId { get; set; }

    public int? TeamOneScore { get; set; }

    public int? TeamTwoScore { get; set; }

    public virtual ICollection<PlayerRating> PlayerRatings { get; set; } = new List<PlayerRating>();

    public virtual Team TeamOne { get; set; } = null!;

    public virtual Team TeamTwo { get; set; } = null!;

    public virtual TypeGame TypeGame { get; set; } = null!;
}
