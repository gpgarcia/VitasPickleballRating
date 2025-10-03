using PickleBallAPI.Controllers;
using System;

namespace TestPickleBallApi
{
    [TestClass]
    public class GameLogicTests
    {
        [DataTestMethod]
        [DataRow(0.75, 15, 5)]
        [DataRow(0.550, 11, 9)]
        [DataRow(0.545, 12, 10)]
        [DataRow(0.542, 13, 11)]
        [DataRow(0.538, 14, 12)]
        [DataRow(0.536, 15, 13)]
        [DataRow(0.500, 15, 14)]
        public void CalculateExpectedScore_ValidInputs_ReturnsExpectedScores(double expectedOutcome, int expectedWin, int expectedLoss)
        {
            var (winScore, lossScore) = GameLogic.CalculateExpectedScore(expectedOutcome);

            Assert.AreEqual(expectedWin, winScore);
            Assert.AreEqual(expectedLoss, lossScore);
        }

        [DataTestMethod]
        [DataRow(-0.1)]
        [DataRow(1.1)]
        public void CalculateExpectedScore_InvalidInputs_ThrowsArgumentOutOfRange(double expectedOutcome)
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                GameLogic.CalculateExpectedScore(expectedOutcome));
        }

    }
}
