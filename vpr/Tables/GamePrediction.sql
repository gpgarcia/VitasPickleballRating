CREATE TABLE [dbo].[GamePrediction]
(
    [GameId]        INT     NOT NULL PRIMARY KEY, 
    CONSTRAINT [FK_GamePrediction_Game] FOREIGN KEY ([GameId]) REFERENCES [Game]([GameId]) ON DELETE CASCADE,
    T1P1Rating    int   NOT NULL,
    T1P2Rating    int   NOT NULL,
    T2P1Rating    int   NOT NULL,
    T2P2Rating    int   NOT NULL,
    T1PredictedWinProb  float NOT NULL,
    ExpectT1Score int   NULL,
    ExpectT2Score int   NULL,
    CreatedAt     DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIME()
)
