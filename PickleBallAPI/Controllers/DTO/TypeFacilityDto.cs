namespace PickleBallAPI.Controllers.DTO;


/// <summary>
/// Data transfer object that represents a facility type.
/// </summary>
/// <remarks>
/// Implemented as a positional record for concise, immutable initialization.
/// </remarks>
/// <param name="TypeFacilityId">Unique identifier for the facility type.</param>
/// <param name="Name">Display name of the facility type.</param>
public sealed record TypeFacilityDto(int TypeFacilityId, string Name);