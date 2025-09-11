CREATE TABLE [dbo].[TypeGame]
(
    [TypeGameId]    INT             NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [GameType]      NVARCHAR(20)       NOT NULL, 
    [ChangedDate]   DATETIMEOFFSET  NOT NULL DEFAULT SYSDATETIMEOFFSET()
)
