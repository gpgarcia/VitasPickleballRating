using System;
using System.Collections.Generic;

namespace PickleBallAPI.Models;

public partial class PlayerRating
{
    public int PlayerRatingId { get; set; }

    public int PlayerId { get; set; }

    public int GameId { get; set; }

    public int Rating { get; set; }

    public DateTimeOffset RatingDate { get; set; }

    public long ChangedTime { get; set; }

    public virtual Player Player { get; set; } = null!;
}
