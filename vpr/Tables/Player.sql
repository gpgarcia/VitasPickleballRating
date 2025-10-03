CREATE TABLE [dbo].[Player]
(
    [PlayerId] INT NOT NULL  IDENTITY (1, 1) , 
    [FirstName] NVARCHAR(50) NOT NULL, 
    [LastName] NVARCHAR(50) NOT NULL, 
    [ChangedDate] DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    CONSTRAINT PK_Player_PlayerId PRIMARY KEY ([PlayerId]),
    CONSTRAINT UQ_Player_FirstName_LastName UNIQUE NONCLUSTERED ([FirstName], [LastName])
)
