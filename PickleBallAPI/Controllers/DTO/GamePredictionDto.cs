using System;

namespace PickleBallAPI.Controllers.DTO;

/// <summary>
/// Data transfer object that represents a game's prediction and the ratings used to compute it.
/// </summary>
/// <remarks>
/// This DTO conveys the per-player ratings used for the prediction, the computed win probability
/// for Team 1, expected team scores and the timestamp when the prediction was created.
/// Implemented as a positional record for immutability and concise initialization.
/// </remarks>
/// <param name="T1p1rating">Rating for Team 1, player 1.</param>
/// <param name="T1p2rating">Rating for Team 1, player 2.</param>
/// <param name="T2p1rating">Rating for Team 2, player 1.</param>
/// <param name="T2p2rating">Rating for Team 2, player 2.</param>
/// <param name="T1predictedWinProb">Predicted win probability for Team 1 (0.0 - 1.0).</param>
/// <param name="ExpectT1score">Expected score for Team 1 (nullable).</param>
/// <param name="ExpectT2score">Expected score for Team 2 (nullable).</param>
/// <param name="CreatedAt">When rating was created</param>
/// <param name="ChangedTime">Concurrency token</param>
public sealed record GamePredictionDto(
    int T1p1rating,
    int T1p2rating,
    int T2p1rating,
    int T2p2rating,
    decimal T1predictedWinProb,
    int? ExpectT1score,
    int? ExpectT2score,
    DateTimeOffset CreatedAt,
    long ChangedTime
);
