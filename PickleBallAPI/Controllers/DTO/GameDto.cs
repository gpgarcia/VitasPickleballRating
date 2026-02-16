using System;

namespace PickleBallAPI.Controllers.DTO;
/// <summary>
/// Data transfer object that represents a game and its lightweight related metadata.
/// </summary>
/// <remarks>
/// This DTO is used for API surface interactions where game details are required.
/// Implemented as a positional record to provide concise, immutable initialization.
/// Use the mapped domain models (e.g. <c>Game</c>) for persistence concerns.
/// </remarks>
/// <param name="GameId">Unique identifier for the game.</param>
/// <param name="PlayedDate">Optional date/time when the game was played.</param>
/// <param name="Facility">Optional facility information where the game was played.</param>
/// <param name="TypeGameId">Identifier of the game type/category.</param>
/// <param name="TeamOneScore">Optional score for team one.</param>
/// <param name="TeamTwoScore">Optional score for team two.</param>
/// <param name="TeamOnePlayerOne">Player DTO for team one, player one.</param>
/// <param name="TeamOnePlayerTwo">Player DTO for team one, player two.</param>
/// <param name="TeamTwoPlayerOne">Player DTO for team two, player one.</param>
/// <param name="TeamTwoPlayerTwo">Player DTO for team two, player two.</param>
/// <param name="TypeGame">Lightweight type-game metadata.</param>
/// <param name="Prediction">Optional prediction metadata for the game.</param>
/// <param name="ChangedTime">Concurrency token</param>
public sealed record GameDto(
    int? GameId = null,
    DateTimeOffset? PlayedDate = null,
    FacilityDto? Facility = null,
    int? TypeGameId = null,
    int? TeamOneScore = null,
    int? TeamTwoScore = null,
    PlayerDto? TeamOnePlayerOne = null,
    PlayerDto? TeamOnePlayerTwo = null,
    PlayerDto? TeamTwoPlayerOne = null,
    PlayerDto? TeamTwoPlayerTwo = null,
    TypeGameDto? TypeGame = null,
    GamePredictionDto? Prediction = null,
    long ChangedTime =0
);
