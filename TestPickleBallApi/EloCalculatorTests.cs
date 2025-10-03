using Microsoft.VisualStudio.TestTools.UnitTesting;
using PickleBallAPI.Controllers;
using System;

namespace TestPickleBallApi;

[TestClass]
public class EloCalculatorTests
{

    [TestMethod]
    public void ExpectedTeamOutcome_ShouldReturnAProbability()
    {
        // Arrange
        int player1Rating = 250;
        int player2Rating = 251;
        int opponent3Rating = 250;
        int opponent4Rating = 250;

        // Act
        double result1 = EloCalculator.ExpectedTeamOutcome(player1Rating, player2Rating, opponent3Rating, opponent4Rating);
        double result2 = EloCalculator.ExpectedTeamOutcome(opponent3Rating, opponent4Rating, player1Rating, player2Rating);

        // Assert
        Assert.IsTrue(result1 > 0.0, "ExpectedTeamOutcome should return a positive value.");
        Assert.IsTrue(result1 <= 1.0, "ExpectedTeamOutcome should return probability.");
        Assert.AreEqual(result2, 1.0-result1, 0.001 , "ET2 = 1 - ET1.");
        Assert.AreEqual(0.505, result1, 0.001);
        Assert.AreEqual(15.0, Math.Round(result1*29)); 

    }

    [TestMethod]
    public void ExpectedTeamOutcome_ShouldBeSymmetric()
    {
        // Arrange
        int player1Rating = 250;
        int player2Rating = 300;
        int opponent3Rating = 325;
        int opponent4Rating = 350;

        // Act
        double result1 = EloCalculator.ExpectedTeamOutcome(player1Rating, player2Rating, opponent3Rating, opponent4Rating);
        double result2 = EloCalculator.ExpectedTeamOutcome(player2Rating, player1Rating, opponent3Rating, opponent4Rating);

        // Assert
        Assert.AreEqual(result1, result2, 0.001, "ExpectedTeamOutcome should be symmetric for player order.");
    }

    [TestMethod]
    public void ExpectedTeamOutcome_LargeDifference()
    {
        // Arrange
        int player1Rating = 200;
        int player2Rating = 200;
        int opponent3Rating = 600;
        int opponent4Rating = 700;

        // Act
        double result1 = EloCalculator.ExpectedTeamOutcome(player1Rating, player2Rating, opponent3Rating, opponent4Rating);

        // Assert
        Assert.AreEqual(0.0, result1, 0.001, "ExpectedTeamOutcome should be 0.0.");
    }

    [TestMethod]
    public void ExpectedTeamOutcome_50Diff()
    {
        // Arrange
        int player1Rating = 200;
        int player2Rating = 200;
        int opponent3Rating = 250;
        int opponent4Rating = 250;

        // Act
        double result1 = EloCalculator.ExpectedTeamOutcome(player1Rating, player2Rating, opponent3Rating, opponent4Rating);

        // Assert
        Assert.AreEqual(0.124, result1, 0.001, "ExpectedTeamOutcome should be 14%.");
    }

    [TestClass]
    public class ScoreCalculatorTests
    {

        private const double KFactor = 40.0;

        [DataTestMethod]
        [DataRow(200, 200, 0.5, 1.0, 210, 210)]
        [DataRow(300, 300, 0.5, 0.0, 290, 290)]
        [DataRow(400, 400, 0.6, 1.0, 408, 408)]
        [DataRow(500, 500, 0.4, 0.0, 492, 492)]
        [DataRow(600, 600, 0.7, 1.0, 606, 606)]
        [DataRow(200, 600, 0.3, 1.0, 207, 621)]
        [DataRow(600, 200, 0.7, 0.0, 579, 200)]
        [DataRow(300, 500, 0.45, 0.55, 302, 502)]
        [DataRow(500, 300, 0.55, 0.45, 498, 298)]
        public void CalculateNewRating_ValidInputs_ReturnsExpectedRatings(
            int player1Rating, int player2Rating, double expectedOutcome, double actualOutcome,
            int expectedP1Rating, int expectedP2Rating)
        {
            var (newP1Rating, newP2Rating) = EloCalculator.CalculateNewRating(player1Rating, player2Rating, expectedOutcome, actualOutcome, KFactor);

            Assert.IsTrue(newP1Rating >= 200, "Player 1 rating should not be below minimum.");
            Assert.IsTrue(newP2Rating >= 200, "Player 2 rating should not be below minimum.");
            var change = (newP1Rating + newP2Rating) - (player1Rating + player2Rating);
            var isHigher = actualOutcome > expectedOutcome;
            if (isHigher)
            {
                Assert.IsTrue(change > 0, "Team rating should increase when actual outcome is better than expected.");
                Assert.IsTrue(change <= KFactor, "Team rating change should not exceed K-Factor.");
            }
            else if (Math.Abs(actualOutcome - expectedOutcome) < 0.001)
            {
                Assert.AreEqual(0, change, "Team rating should remain the same when actual outcome equals expected.");
            }
            else
            {
                Assert.IsTrue(change < 0, "Team rating should decrease when actual outcome is worse than expected.");
                Assert.IsTrue(change <= KFactor, "Team rating change should not exceed K-Factor.");
            }
            Assert.AreEqual(expectedP1Rating, newP1Rating);
            Assert.AreEqual(expectedP2Rating, newP2Rating);

        }
    }
}

