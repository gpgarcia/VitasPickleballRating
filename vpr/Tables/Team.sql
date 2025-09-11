CREATE TABLE [dbo].[Team]
(
    [TeamId] INT NOT NULL Identity(1,1) PRIMARY KEY, 
    [PlayerOneId] INT NOT NULL, 
    [PlayerTwoId] INT NOT NULL, 
    [ChangedDate] DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(), 
    CONSTRAINT [FK_Team_PlayerOne] FOREIGN KEY ([PlayerOneId]) REFERENCES [Player]([PlayerId]), 
    CONSTRAINT [FK_Team_PlayerTwo ] FOREIGN KEY ([PlayerTwoId]) REFERENCES [Player]([PlayerId]), 
    CONSTRAINT [CK_Team_Player] CHECK ([PlayerOneId] <> [PlayerTwoId]),
    CONSTRAINT [CK_Team_Player_order] CHECK ([PlayerOneId] < [PlayerTwoId])
)
