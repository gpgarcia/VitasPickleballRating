using System;

namespace PickleBallAPI.Controllers.DTO;

public class GamePredictionDto
{
    public int T1p1rating { get; set; }

    public int T1p2rating { get; set; }

    public int T2p1rating { get; set; }

    public int T2p2rating { get; set; }

    public double T1predictedWinProb { get; set; }

    public int? ExpectT1score { get; set; }

    public int? ExpectT2score { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

}
