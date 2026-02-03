using System;
using System.Collections.Generic;

namespace PickleBallAPI.Models;

public partial class GameDetail
{
    public int GameId { get; set; }

    public DateTimeOffset? PlayedDate { get; set; }

    public string? GameTypeName { get; set; }

    public int? Team1Player1PlayerId { get; set; }

    public string? Team1Player1FirstName { get; set; }

    public string? Team1Player1LastName { get; set; }

    public int? Team1Player2PlayerId { get; set; }

    public string? Team1Player2FirstName { get; set; }

    public string? Team1Player2LastName { get; set; }

    public int? Team1Score { get; set; }

    public int Team1Win { get; set; }

    public int? Team2Player1PlayerId { get; set; }

    public string? Team2Player1FirstName { get; set; }

    public string? Team2Player1LastName { get; set; }

    public int? Team2Player2PlayerId { get; set; }

    public string? Team2Player2FirstName { get; set; }

    public string? Team2Player2LastName { get; set; }

    public int? Team2Score { get; set; }

    public int Team2Win { get; set; }
}
