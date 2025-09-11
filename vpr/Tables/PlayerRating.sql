CREATE TABLE [dbo].[PlayerRating]
(
    [PlayerRatingId]    INT         NOT NULL identity(1,1) PRIMARY KEY,
    [PlayerId]          INT         NOT NULL,
    [GameId]            INT         NULL,
    [Rating]            INT         NOT NULL,
    [RatingDate]        DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(), 
    CONSTRAINT [FK_PlayerRating_Player] FOREIGN KEY ([PlayerId]) REFERENCES [Player]([PlayerId]), 
    CONSTRAINT [FK_PlayerRating_game] FOREIGN KEY ([GameId]) REFERENCES [Game]([GameId])

)
