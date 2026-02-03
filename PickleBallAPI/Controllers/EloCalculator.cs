using System;
using System.Linq;
namespace PickleBallAPI.Controllers;


public static class EloCalculator
{
    const double scaleFactor = 40.0;
    public static int MinimumRating { get; } = 200;
    public static int InitialRating { get; } = 250;
    const double baseKFactor = 80.0;


    public static decimal ExpectedTeamOutcome(int player1Rating, int player2Rating, int oppo3Rating, int oppo4Rating)
    {
        //from https://towardsdatascience.com/developing-an-elo-based-data-driven-ranking-system-for-2v2-multiplayer-games-7689f7d42a53/
        // resulting ratings not predictive enough :-(
        decimal[] eo = [
            ExpectedOutcome(player1Rating, oppo3Rating),
            ExpectedOutcome(player1Rating, oppo4Rating),
            ExpectedOutcome(player2Rating, oppo3Rating),
            ExpectedOutcome(player2Rating, oppo4Rating),
        ];

        return eo.Average();
    }

    public static decimal ExpectedTeamOutcome2(int player1Rating, int player2Rating, int oppo3Rating, int oppo4Rating)
    {
        int[] t1 = [player1Rating, player2Rating];
        int[] t2 = [oppo3Rating, oppo4Rating];

        decimal ep1 = ExpectedOutcome(t1.Average(), t2.Average());

        return ep1;
    }

    public static decimal ExpectedOutcome(double rating, double opponentRating)
    {
        double term1 = 1 + Math.Pow(10, (opponentRating - rating) / scaleFactor);
        return (decimal)(1.0 / term1);
    }


    public static (int, int) CalculateNewRating(int player1Rating, int player2Rating, decimal expectedOutcome, decimal actualOutcome, double kFactor)
    {
        var p1w = (decimal)player1Rating / (decimal)(player1Rating + player2Rating);
        var p2w = (decimal)player2Rating / (decimal)(player1Rating + player2Rating);

        decimal change = (decimal)kFactor * (actualOutcome - expectedOutcome);
        var p1r = Math.Max(Math.Round(player1Rating + change * p1w), MinimumRating);
        var p2r = Math.Max(Math.Round(player2Rating + change * p2w), MinimumRating);
        return ((int)p1r, (int)p2r);
    }

    public static double CalculateKFactor(int currentRating)
    {
        //TODO: need to refine this based on number of games played, rec vs tournament.
        return baseKFactor;
    }

}
