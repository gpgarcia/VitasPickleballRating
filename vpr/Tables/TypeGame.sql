CREATE TABLE [dbo].[TypeGame]
(
    [TypeGameId]    INT             NOT NULL, 
    [GameType]      NVARCHAR(20)    NOT NULL, 
    [ChangedTime]   DATETIMEOFFSET  NOT NULL,
    CONSTRAINT PK_TypeGame_TypeGameId PRIMARY KEY (TypeGameId),
    CONSTRAINT UQ_TypeGame_GameType UNIQUE (GameType)
);
