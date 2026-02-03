namespace PickleBallAPI.Controllers.DTO;

// DTO used for CSV export with explicit types so method can return a strongly-typed list
public sealed record GameRawDto(
    int GameId,
    int? FacilityId,
    string PlayedDate,
    int TypeGameId,
    int TeamOnePlayerOneId,
    int TeamOnePlayerTwoId,
    int? TeamOneScore,
    int TeamTwoPlayerOneId,
    int TeamTwoPlayerTwoId,
    int? TeamTwoScore,
    long ChangedTime
);
