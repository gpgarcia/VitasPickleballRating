CREATE TABLE [dbo].[Facility]
(
    [FacilityId] INT NOT NULL identity(1,1)
    ,[Name] VARCHAR(50) NOT NULL
    ,[AddressLine1] VARCHAR(50) NOT NULL
    ,[AddressLine2] VARCHAR(50) NULL
    ,[City] VARCHAR(50) NOT NULL
    ,[StateCode] CHAR(2) NOT NULL
    ,[PostalCode] VARCHAR(11) NULL
    ,[NumberCourts] INT NOT NULL
    ,[TypeFacilityId] INT NOT NULL
    ,[Notes] VARCHAR(MAX) NULL
    ,[ChangedTime] DATETIMEOFFSET NOT NULL
    ,CONSTRAINT PK_Facility_FacilityId PRIMARY KEY (FacilityId)
    ,Constraint FK_Facility_TypeFacilityID FOREIGN KEY (TypeFacilityId) REFERENCES TypeFacility(TypeFacilityId)

);
