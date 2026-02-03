CREATE TABLE [dbo].[TypeGame]
(
    [TypeGameId]    INT             NOT NULL, 
    [Name]      NVARCHAR(20)    NOT NULL, 
    -- Use unix epoch milliseconds for UTC timestamps
    [ChangedTime] BIGINT NOT NULL, -- Do not Default. This is the App level concurrency token
    CONSTRAINT PK_TypeGame_TypeGameId PRIMARY KEY (TypeGameId),
    CONSTRAINT UQ_TypeGame_GameType UNIQUE (Name)
);
GO