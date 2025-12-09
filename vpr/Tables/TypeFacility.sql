CREATE TABLE [dbo].[TypeFacility]
(
    [TypeFacilityId] INT NOT NULL
    , [FacilityType] VARCHAR(50) NOT NULL
    , CONSTRAINT PK_TypeFacility_TypeFacilityId PRIMARY KEY (TypeFacilityId)
    , CONSTRAINT UQ_TypeFacility_FacilityType UNIQUE (FacilityType)

)
