using System;
using System.Collections.Generic;

namespace PickleBallAPI.Models;

public partial class TypeGame
{
    public int TypeGameId { get; set; }

    public string GameType { get; set; } = null!;

    public DateTimeOffset ChangedTime { get; set; }
}
