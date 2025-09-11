using System;
using System.Collections.Generic;

namespace PickleBallAPI.Controllers;
public partial class GameDto
{
    public int GameId { get; set; }

    public DateTimeOffset? PlayedDate { get; set; }

    public int TypeGameId { get; set; }

    public int TeamOneId { get; set; }

    public int TeamTwoId { get; set; }

    public int? TeamOneScore { get; set; }

    public int? TeamTwoScore { get; set; }

    public virtual TeamDto TeamOne { get; set; } = null!;

    public virtual TeamDto TeamTwo { get; set; } = null!;

    public virtual TypeGameDto TypeGame { get; set; } = null!;
}
