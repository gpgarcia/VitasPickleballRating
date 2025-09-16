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
    CONSTRAINT [FK_Game_Player_t1p1]   FOREIGN KEY ([TeamOnePlayerOneId]) REFERENCES [Player]([PlayerId]),
    CONSTRAINT [FK_Game_Player_t1p2]   FOREIGN KEY ([TeamOnePlayerTwoId]) REFERENCES [Player]([PlayerId]),
    CONSTRAINT [FK_Game_Player_t2p1]   FOREIGN KEY ([TeamTwoPlayerOneId]) REFERENCES [Player]([PlayerId]),
    CONSTRAINT [FK_Game_Player_t2p2]   FOREIGN KEY ([TeamTwoPlayerTwoId]) REFERENCES [Player]([PlayerId]),
    CONSTRAINT [CK_Game_DifferentPlayers]   CHECK (
                                                         [TeamOnePlayerOneId] <> [TeamTwoPlayerOneId] AND [TeamOnePlayerOneId]<>[TeamTwoPlayerTwoId]
                                                     AND [TeamOnePlayerTwoId] <> [TeamTwoPlayerOneId] AND [TeamOnePlayerTwoId]<>[TeamTwoPlayerTwoId]
                                                  ),
    CONSTRAINT [CK_Team1_Player_Order]      CHECK ([TeamOnePlayerOneId] < [TeamOnePlayerTwoId]),
    CONSTRAINT [CK_Team2_Player_Order]      CHECK ([TeamTwoPlayerOneId] < [TeamTwoPlayerTwoId]),
    CONSTRAINT [CK_Team1_PlayersDifferent]  CHECK ([TeamOnePlayerOneId] <> [TeamOnePlayerTwoId]),
    CONSTRAINT [CK_Team2_PlayersDifferent]  CHECK ([TeamTwoPlayerOneId] <> [TeamTwoPlayerTwoId]),

)
