CREATE TABLE [dbo].[Game]
(
    [GameId]        INT     NOT NULL    IDENTITY(1,1) PRIMARY KEY,
    [PlayedDate]    DATETIMEOFFSET NULL,
    [TypeGameId]    INT     NOT NULL,
    [TeamOnePlayerOneId]  INT     NOT NULL,
    [TeamOnePlayerTwoId]  INT     NOT NULL,
    [TeamTwoPlayerOneId]  INT     NOT NULL,
    [TeamTwoPlayerTwoId]  INT     NOT NULL,
    [TeamOneScore]  INT     NULL,
    [TeamTwoScore]  INT     NULL, 
    CONSTRAINT [FK_Game_TypeGame]   FOREIGN KEY ([TypeGameId]) REFERENCES [TypeGame]([TypeGameId]),
    CONSTRAINT [CK_Game_DifferentPlayers]   CHECK (
                                                         [TeamOnePlayerOneId] <> [TeamTwoPlayerOneId] AND [TeamOnePlayerOneId]<>[TeamTwoPlayerTwoId]
                                                     AND [TeamOnePlayerTwoId] <> [TeamTwoPlayerOneId] AND [TeamOnePlayerTwoId]<>[TeamTwoPlayerTwoId]
                                                  ),
    CONSTRAINT [CK_Team1_Player_Order]      CHECK ([TeamOnePlayerOneId] < [TeamOnePlayerTwoId]),
    CONSTRAINT [CK_Team2_Player_Order]      CHECK ([TeamTwoPlayerOneId] < [TeamTwoPlayerTwoId]),
    CONSTRAINT [CK_Team1_PlayersDifferent]  CHECK ([TeamOnePlayerOneId] <> [TeamOnePlayerTwoId]),
    CONSTRAINT [CK_Team2_PlayersDifferent]  CHECK ([TeamTwoPlayerOneId] <> [TeamTwoPlayerTwoId]),

)
