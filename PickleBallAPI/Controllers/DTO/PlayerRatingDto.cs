using System;

namespace PickleBallAPI.Controllers.DTO;

/// <summary>
/// Data transfer object that represents a player's rating for a specific game (or an as-of rating).
/// </summary>
/// <remarks>
/// Implemented as a positional record (primary constructor) for concise immutable-style DTOs.
/// The <see cref="Player"/> property contains lightweight player metadata.
/// </remarks>
/// <param name="PlayerRatingId">Unique identifier for the player rating record.</param>
/// <param name="PlayerId">Identifier of the player to whom this rating belongs.</param>
/// <param name="GameId">Optional identifier of the game this rating is associated with.</param>
/// <param name="Rating">Numeric rating value.</param>
/// <param name="RatingDate">Timestamp for when the rating was recorded.</param>
/// <param name="Player">Lightweight player DTO associated with the rating.</param>
/// <param name="ChangedTime">Concurrency token</param>
public sealed record PlayerRatingDto(
    int? PlayerRatingId=null,
    int? PlayerId = null,
    int? GameId = null,
    int? Rating = null,
    DateTimeOffset? RatingDate = null,
    PlayerDto? Player = null,
    long ChangedTime = 0
);