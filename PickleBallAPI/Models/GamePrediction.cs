using System;
using System.Collections.Generic;

namespace PickleBallAPI.Models;

public partial class GamePrediction
{
    public int GameId { get; set; }

    public int T1p1rating { get; set; }

    public int T1p2rating { get; set; }

    public int T2p1rating { get; set; }

    public int T2p2rating { get; set; }

    public decimal T1predictedWinProb { get; set; }

    public int ExpectT1score { get; set; }

    public int ExpectT2score { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public long ChangedTime { get; set; }

    public virtual Game Game { get; set; } = null!;
}
