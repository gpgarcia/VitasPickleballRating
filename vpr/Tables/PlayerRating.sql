CREATE TABLE [dbo].[PlayerRating]
(
    [PlayerRatingId]    INT         NOT NULL identity(1,1),
    [PlayerId]          INT         NOT NULL,
    [GameId]            INT         NOT NULL,
    [Rating]            INT         NOT NULL,
    [RatingDate]        DATETIMEOFFSET NOT NULL ,
    [ChangedTime]       DATETIMEOFFSET(7) NOT NULL-- do not default, this is the app level concurrency token
    CONSTRAINT [PK_PlayerRating_PlayerRatingId] PRIMARY KEY NONCLUSTERED (PlayerRatingId),
    CONSTRAINT [UQ_PlayerRating_PlayerId_GameId] UNIQUE     CLUSTERED (PlayerId, GameId),
    CONSTRAINT [FK_PlayerRating_Player]         FOREIGN KEY ([PlayerId]) REFERENCES [Player]([PlayerId]), 
    CONSTRAINT [FK_PlayerRating_game]           FOREIGN KEY ([GameId]) REFERENCES [Game]([GameId])

)
