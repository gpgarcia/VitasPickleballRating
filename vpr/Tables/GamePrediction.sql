CREATE TABLE [dbo].[GamePrediction]
(
    [GameId]      INT   NOT NULL, 
    T1P1Rating    INT   NOT NULL,
    T1P2Rating    INT   NOT NULL,
    T2P1Rating    INT   NOT NULL,
    T2P2Rating    INT   NOT NULL,
    T1PredictedWinProb  FLOAT NOT NULL,
    ExpectT1Score INT   NULL,
    ExpectT2Score INT   NULL,
    CreatedAt     DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    CONSTRAINT [PK_GamePrediction_GameId] PRIMARY KEY CLUSTERED ([GameId]),
    CONSTRAINT [FK_GamePrediction_Game] FOREIGN KEY ([GameId]) REFERENCES [Game]([GameId]) ON DELETE CASCADE
);
