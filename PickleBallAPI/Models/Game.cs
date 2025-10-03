using System;
using System.Collections.Generic;

namespace PickleBallAPI.Models;

public partial class Game
{
    public int GameId { get; set; }

    public DateTimeOffset? PlayedDate { get; set; }

    public int TypeGameId { get; set; }

    public int TeamOnePlayerOneId { get; set; }

    public int TeamOnePlayerTwoId { get; set; }

    public int TeamTwoPlayerOneId { get; set; }

    public int TeamTwoPlayerTwoId { get; set; }

    public int? TeamOneScore { get; set; }

    public int? TeamTwoScore { get; set; }

    public virtual GamePrediction? GamePrediction { get; set; }

    public virtual ICollection<PlayerRating> PlayerRatings { get; set; } = new List<PlayerRating>();

    public virtual Player TeamOnePlayerOne { get; set; } = null!;

    public virtual Player TeamOnePlayerTwo { get; set; } = null!;

    public virtual Player TeamTwoPlayerOne { get; set; } = null!;

    public virtual Player TeamTwoPlayerTwo { get; set; } = null!;

    public virtual TypeGame TypeGame { get; set; } = null!;
}
