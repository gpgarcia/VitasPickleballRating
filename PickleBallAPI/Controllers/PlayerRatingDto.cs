using System;

namespace PickleBallAPI.Controllers;

public partial class PlayerRatingDto
{
    public int PlayerRatingId { get; set; }

    public int PlayerId { get; set; }

    public int? GameId { get; set; }

    public int Rating { get; set; }

    public DateTimeOffset RatingDate { get; set; }

    public PlayerDto Player { get; set; } = null!;

}
