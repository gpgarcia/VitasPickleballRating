using System;
namespace PickleBallAPI.Controllers;


public static class EloCalculator
{
    const double scaleFactor =59.0;
    public static int MinimumRating { get; } = 200;
    public static int InitialRating { get; } = 250;


    public static double ExpectedTeamOutcome(int player1Rating, int player2Rating, int oppo3Rating, int oppo4Rating)
    {
        double ep1 = ExpectedPlayerOutcome(player1Rating, oppo3Rating, oppo4Rating);
        double ep2 = ExpectedPlayerOutcome(player2Rating, oppo3Rating, oppo4Rating);

        return (ep1 + ep2) / 2.0;
    }
    private static double ExpectedPlayerOutcome(int rating, int opponentRating1, int opponentRating2)
    {
        double term1 = 1 + Math.Pow(10, (opponentRating1 - rating) / scaleFactor);
        double term2 = 1 + Math.Pow(10, (opponentRating2 - rating) / scaleFactor);

        return 0.5 * (1.0/term1 +  1.0/term2);
    }

    public static (int, int) CalculateNewRating(int player1Rating, int player2Rating, double expectedOutcome, double actualOutcome, double kFactor)
    {
        var p1w = (double)player1Rating / (double)(player1Rating + player2Rating);
        var p2w = (double)player2Rating / (double)(player1Rating + player2Rating);

        var change = kFactor * (actualOutcome - expectedOutcome);
        var p1r = Math.Max(Math.Round(player1Rating + change * p1w), MinimumRating);
        var p2r = Math.Max(Math.Round(player2Rating + change * p2w), MinimumRating);
        return ((int)p1r, (int)p2r);
    }

    public static double CalculateKFactor(int currentRating)
    {
        //TODO: need to refine this based on number of games played, rec vs tournament.
        return 40.0;
    }

}
