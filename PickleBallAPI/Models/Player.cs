using System;
using System.Collections.Generic;

namespace PickleBallAPI.Models;

public partial class Player
{
    public int PlayerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string? NickName { get; set; }

    public string LastName { get; set; } = null!;

    public long ChangedTime { get; set; }

    public virtual ICollection<PlayerRating> PlayerRatings { get; set; } = new List<PlayerRating>();
}
