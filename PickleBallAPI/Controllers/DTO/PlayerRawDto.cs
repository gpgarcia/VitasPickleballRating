namespace PickleBallAPI.Controllers.DTO;


/// <summary>
/// Data transfer object that represents a player's raw data, wihout any Navigation properties.
/// </summary>
/// <remarks>
/// This DTO is used for API surface interactions where only basic player data is required:
/// - identifier,
/// - first/last name and optional nickname.
/// Implemented as a positional record (primary constructor) to keep the DTO immutable and concise.
/// </remarks>
/// <param name="PlayerId">Unique identifier for the player.</param>
/// <param name="FirstName">Player's first name.</param>
/// <param name="NickName">Player's nickname or preferred short name.</param>
/// <param name="LastName">Player's last/family name.</param>
/// <param name="ChangedTime">App level oportunistic locking token</param>
public sealed record PlayerRawDto(
    int? PlayerId = null,
    string? FirstName = null,
    string? NickName = null,
    string? LastName = null,
    long ? ChangedTime = null
);
