CREATE TABLE [dbo].[Game]
(
    [GameId]        INT     NOT NULL    IDENTITY(1,1),
    [FacilityId]    INT     NULL,
    [PlayedDate]    DATETIMEOFFSET NULL,
    [TypeGameId]    INT     NOT NULL,
    [TeamOnePlayerOneId]  INT     NOT NULL,
    [TeamOnePlayerTwoId]  INT     NOT NULL,
    [TeamTwoPlayerOneId]  INT     NOT NULL,
    [TeamTwoPlayerTwoId]  INT     NOT NULL,
    [TeamOneScore]  INT     NULL,
    [TeamTwoScore]  INT     NULL, 
    -- Use unix epoch milliseconds for UTC timestamps
    [ChangedTime] BIGINT NOT NULL, -- Do not Default. This is the App level concurrency token
    CONSTRAINT [PK_Game_GameId]     PRIMARY KEY CLUSTERED ([GameId]),
    Constraint [FK_Game_FacilityId] Foreign key ([FacilityId]) References [Facility]([FacilityId]),
    CONSTRAINT [FK_Game_TypeGameId] FOREIGN KEY ([TypeGameId]) REFERENCES [TypeGame]([TypeGameId]),
    CONSTRAINT [FK_Game_Player_t1p1] FOREIGN KEY ([TeamOnePlayerOneId]) REFERENCES [Player]([PlayerId]),
    CONSTRAINT [FK_Game_Player_t1p2] FOREIGN KEY ([TeamOnePlayerTwoId]) REFERENCES [Player]([PlayerId]),
    CONSTRAINT [FK_Game_Player_t2p1] FOREIGN KEY ([TeamTwoPlayerOneId]) REFERENCES [Player]([PlayerId]),
    CONSTRAINT [FK_Game_Player_t2p2] FOREIGN KEY ([TeamTwoPlayerTwoId]) REFERENCES [Player]([PlayerId]),
    CONSTRAINT [CK_Game_DifferentPlayers]   CHECK (
                                                         [TeamOnePlayerOneId] <> [TeamTwoPlayerOneId] AND [TeamOnePlayerOneId]<>[TeamTwoPlayerTwoId]
                                                     AND [TeamOnePlayerTwoId] <> [TeamTwoPlayerOneId] AND [TeamOnePlayerTwoId]<>[TeamTwoPlayerTwoId]
                                                  ),
    CONSTRAINT [CK_Team1_Player_Order]      CHECK ([TeamOnePlayerOneId] < [TeamOnePlayerTwoId]),
    CONSTRAINT [CK_Team2_Player_Order]      CHECK ([TeamTwoPlayerOneId] < [TeamTwoPlayerTwoId]),
    CONSTRAINT [CK_Team1_PlayersDifferent]  CHECK ([TeamOnePlayerOneId] <> [TeamOnePlayerTwoId]),
    CONSTRAINT [CK_Team2_PlayersDifferent]  CHECK ([TeamTwoPlayerOneId] <> [TeamTwoPlayerTwoId]),
    CONSTRAINT [CK_TeamScores_NonNegative]    CHECK ([TeamOneScore] >= 0 AND [TeamTwoScore] >= 0)


);