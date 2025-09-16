using PickleBallAPI.Models;
using System;
using System.Collections.Generic;

namespace PickleBallAPI.Controllers;
public partial class GameDto
{
    public int GameId { get; set; }

    public DateTimeOffset? PlayedDate { get; set; }

    public int TypeGameId { get; set; }

    public int? TeamOneScore { get; set; }

    public int? TeamTwoScore { get; set; }

    public PlayerDto TeamOnePlayerOne { get; set; } = null!;
    public PlayerDto TeamOnePlayerTwo { get; set; } = null!;
    public PlayerDto TeamTwoPlayerOne { get; set; } = null!;
    public PlayerDto TeamTwoPlayerTwo { get; set; } = null!;

    public virtual TypeGameDto TypeGame { get; set; } = null!;
}
