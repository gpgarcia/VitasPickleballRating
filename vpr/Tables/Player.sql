CREATE TABLE [dbo].[Player]
( 
    [PlayerId] INT NOT NULL  IDENTITY (1, 1) , 
    [FirstName] NVARCHAR(50) NOT NULL, 
    [NickName] NVARCHAR(50) NULL,
    [LastName] NVARCHAR(50) NOT NULL,
    [ChangedTime] DATETIMEOFFSET NOT NULL, -- Do not Default. This is the App level concurrency token
    CONSTRAINT PK_Player_PlayerId PRIMARY KEY ([PlayerId]),
    CONSTRAINT UQ_Player_FirstName_LastName UNIQUE NONCLUSTERED ([FirstName], [LastName])
);