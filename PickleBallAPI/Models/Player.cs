using System;
using System.Collections.Generic;

namespace PickleBallAPI.Models;

public partial class Player
{
    public int PlayerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTimeOffset ChangedDate { get; set; }

    public virtual ICollection<Game> GameTeamOnePlayerOnes { get; set; } = new List<Game>();

    public virtual ICollection<Game> GameTeamOnePlayerTwos { get; set; } = new List<Game>();

    public virtual ICollection<Game> GameTeamTwoPlayerOnes { get; set; } = new List<Game>();

    public virtual ICollection<Game> GameTeamTwoPlayerTwos { get; set; } = new List<Game>();

    public virtual ICollection<PlayerRating> PlayerRatings { get; set; } = new List<PlayerRating>();
}
