using PickleBallAPI.Models;

namespace PickleBallAPI.Controllers.DTO;

/// <summary>
/// Data transfer object that represents a facility.
/// </summary>
/// <remarks>
/// Provides lightweight facility information returned by the API. Implemented as a positional
/// record (primary constructor) to keep the DTO concise and immutable.
/// </remarks>
/// <param name="FacilityId">Unique identifier for the facility.</param>
/// <param name="Name">Facility display name.</param>
/// <param name="AddressLine1">Primary street address line.</param>
/// <param name="AddressLine2">Optional secondary address line.</param>
/// <param name="City">City where the facility is located.</param>
/// <param name="StateCode">Postal state code (e.g. "CA").</param>
/// <param name="PostalCode">Optional postal / ZIP code.</param>
/// <param name="NumberCourts">Number of courts at the facility.</param>
/// <param name="TypeFacility">Facility type metadata.</param>
/// <param name="Notes">Optional free-form notes about the facility.</param>
/// <param name="ChangedTime">Concurrency token</param>
public sealed record FacilityDto(
    int? FacilityId = null,
    string? Name = null,
    string? AddressLine1 = null,
    string? AddressLine2 = null,
    string? City = null,
    string? StateCode = null,
    string? PostalCode = null,
    int? NumberCourts = null,
    TypeFacilityDto? TypeFacility = null,
    string? Notes = null,
    long ChangedTime = 0
);