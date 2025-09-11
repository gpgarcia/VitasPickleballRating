using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PickleBallAPI.Models;

public partial class TypeGame
{
    public int TypeGameId { get; set; }

    public string GameType { get; set; } = null!;

    public DateTimeOffset ChangedDate { get; set; }

    public ICollection<Game> Games { get; set; } = new List<Game>();

}
