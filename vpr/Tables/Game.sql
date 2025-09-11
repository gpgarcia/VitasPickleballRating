CREATE TABLE [dbo].[Game]
(
    [GameId]        INT     NOT NULL    IDENTITY(1,1) PRIMARY KEY,
    [PlayedDate]    DATETIMEOFFSET NULL,
    [TypeGameId]    INT     NOT NULL,
    [TeamOneId]     INT     NOT NULL,
    [TeamTwoId]     INT     NOT NULL,
    [TeamOneScore]  INT     NULL,
    [TeamTwoScore]  INT     NULL, 
    CONSTRAINT [FK_Game_TeamOne]    FOREIGN KEY ([TeamOneId]) REFERENCES [Team]([TeamId]), 
    CONSTRAINT [FK_Game_TeamTwo]    FOREIGN KEY ([TeamTwoId]) REFERENCES [Team]([TeamId]),
    CONSTRAINT [FK_Game_TypeGame]   FOREIGN KEY ([TypeGameId]) REFERENCES [TypeGame]([TypeGameId]),
    CONSTRAINT [CK_Game_Team]       CHECK       ([TeamOneId] <> [TeamTwoId]),

)
