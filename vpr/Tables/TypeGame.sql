CREATE TABLE [dbo].[TypeGame]
(
    [TypeGameId]    INT             NOT NULL IDENTITY(1,1), 
    [GameType]      NVARCHAR(20)    NOT NULL, 
    [ChangedDate]   DATETIMEOFFSET  NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    CONSTRAINT PK_TypeGame_TypeGameId PRIMARY KEY (TypeGameId),
    CONSTRAINT UQ_TypeGame_GameType UNIQUE (GameType)
)
