namespace PickleBallAPI.Controllers.DTO;

/// <summary>
/// Data transfer object that represents a game type.
/// </summary>
/// <remarks>
/// Implemented as a positional record to keep the DTO concise and immutable.
/// </remarks>
/// <param name="TypeGameId">Unique identifier for the game type.</param>
/// <param name="Name">Display name of the game type.</param>
/// <param name="ChangedTime">Concurrency token.</param>
public sealed record TypeGameDto
    (
        int? TypeGameId= null
    , string? Name = null
    );
