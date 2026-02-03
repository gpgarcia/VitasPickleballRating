
/*
Post-Deployment Script Template                            
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.        
 Use SQLCMD syntax to include a file in the post-deployment script.            
 Example:      :r .\myfile.sql                                
 Use SQLCMD syntax to reference a variable in the post-deployment script.        
 Example:      :setvar TableName MyTable                            
               SELECT * FROM [$(TableName)]                    
--------------------------------------------------------------------------------------
*/

insert into [dbo].[TypeGame] 
    ([TypeGameId], [Name], [ChangedTime])
VALUES 
    (1, 'Recreational',1768241368000),
    (2, 'Tournament',1768241368000)
    ;
go

insert into [dbo].[TypeFacility] 
    (TypeFacilityId, [Name])
VALUES
    (1, 'Outdoors'),
    (2, 'Covered'),
    (3, 'Indoors'),
    (11,'Dedicated Outdoors'),
    (12,'Dedicated Covered'),
    (13,'Dedicated Indoors'),
    (21,'Multi-Use Outdoors'),
    (22,'Multi-Use Covered'),
    (23,'Multi-Use Indoors')
;
go

SET IDENTITY_INSERT [dbo].[Facility] ON;
go
insert into [dbo].[Facility] 
        ([FacilityId], [Name], [AddressLine1], [AddressLine2], [City], [StateCode], [PostalCode], [NumberCourts], [TypeFacilityId], [Notes],[ChangedTime])
    VALUES 
        (1, 'Isola Condo', '770 Claughton Island Dr', NULL, 'Miami', 'FL', '33131', 2, 21,
            'Private court. Resident must reserve 1 to 7 days in advance',1768241368000),
        (2, 'One Indoor Club', '300 NE 183rd St', NULL, 'Miami Garden', 'FL', '33179', 3, 13,
            'must reserve in advance normally $60/Hr with Groupon $15/Hr', 1768241368000),
        (3, 'Driftwood Park', '3000 N 69th Ave', NULL, 'Hollywood', 'FL', '33024', 6, 11,
            'Hollywood City Park',1768241368000),
        (4, 'Bryan Park', '2301 SW 13th St', NULL, 'Miami', 'FL', '33145', 4, 21,
            'Miami City Park, Crowded in the evenings. Padels up for next game. Lights out 8:00 PM. Pickleball on Wednesday. Friday thru Monday', 1768241368000),
        (5, 'C.B. Smith Park', '900 N Flamingo Rd', null, 'Pembroke Pines', 'FL', '33028', 8, 21,
            'Broward County Park. Free courts.',176824136800)
;
go
SET IDENTITY_INSERT [dbo].[Facility] OFF;
go

SET IDENTITY_INSERT [dbo].[Player] ON;
go

insert into [dbo].[Player] 
    ([PlayerId], [FirstName], [LastName],[NickName], [ChangedTime])
VALUES 
    (1, 'Jean-Michael', 'Querol', NULL, 1768241368000),
    (2, 'Keenan', 'Rodriguez', NULL, 1768241368000),   
    (3, 'Goutham', 'Kanddibanda',NULL, 1768241368000), 
    (4, 'Gerardo', 'Garcia','Gerry', 1768241368000),   
    (5, 'Samuel', 'Horowitz', NULL, 1768241368000),    
    (6, 'Denise', 'Sleem', NULL, 1768241368000),       
    (7, 'Fernando', 'Paz', NULL, 1768241368000) ,      
    (8, 'William', 'Waddell', 'Bill',1768241368000),  
    (9, 'Francisco', 'Vela','Franc',1768241368000),   
    (10, 'Alex', 'Fisher', NULL, 1768241368000),       
    (11, 'Alejandro', 'Diaz', NULL, 1768241368000) ,   
    (12, 'Satish', 'Karlapudi', NULL, 1768241368000),  
    (13, 'Naveen', 'Martha', NULL, 1768241368000),
    (14, 'Jignesh', 'Patel', NULL, 1769880238000)
    ;
go
SET IDENTITY_INSERT [dbo].[Player] OFF;
go

insert into [dbo].[Game] 
    (FacilityId, [PlayedDate], [TypeGameId], [TeamOnePlayerOneId], [TeamOnePlayerTwoId], [TeamOneScore], [TeamTwoPlayerOneId], [TeamTwoPlayerTwoId],  [TeamTwoScore], ChangedTime)
Values
    -- i do not remember the scores for the first set of games isola outdoor
      (1, '2025-07-25 19:15 -4:00', 1, 3,5, 11, 4,6, 4,1768241368000)
    , (1, '2025-07-25 19:35 -4:00', 1, 3,5, 11, 4,6, 6,1768241368000)
    , (1, '2025-07-25 21:55 -4:00', 1, 4,5, 11, 3,6, 9,1768241368000)
    -- second session isola outdoor
    , (1, '2025-07-31 18:34 -4:00', 1, 1,2, 11, 3,4, 3,1768241368000)
    , (1, '2025-07-31 18:43 -4:00', 1, 2,4, 11, 1,3, 0,1768241368000)
    , (1, '2025-07-31 19:00 -4:00', 1, 2,3, 11, 1,4, 8,1768241368000)
    , (1, '2025-07-31 19:18 -4:00', 1, 2,3, 11, 1,4, 5,1768241368000)
    , (1, '2025-07-31 19:38 -4:00', 1, 1,2, 11, 3,4, 9,1768241368000)
    -- third session one indoor
    , (2, '2025-08-07 19:14 -4:00', 1, 3,7, 13, 6,8, 11,1768241368000)
    , (2, '2025-08-07 19:27 -4:00', 1, 6,7, 11 ,3,8, 5,1768241368000)
    , (2, '2025-08-07 19:41 -4:00', 1, 6,7, 11, 3,8, 5,1768241368000)
    , (2, '2025-08-07 19:51 -4:00', 1, 3,7, 11, 6,8, 2,1768241368000)
    -- fourth session one indoor
    , (2, '2025-08-21 18:37 -4:00', 1, 3,6, 11, 4,9, 2,1768241368000)
    , (2, '2025-08-21 18:50 -4:00', 1, 5,6, 11, 1,4, 3,1768241368000)
    , (2, '2025-08-21 19:00 -4:00', 1, 2,4, 11, 8,9, 4,1768241368000)
    , (2, '2025-08-21 19:14 -4:00', 1, 7,8, 11, 1,5, 9,1768241368000)
    , (2, '2025-08-21 19:29 -4:00', 1, 2,3, 11, 6,7, 9,1768241368000)
    , (2, '2025-08-21 19:39 -4:00', 1, 5,9, 11, 1,8, 0,1768241368000)
    , (2, '2025-08-21 19:46 -4:00', 1, 2,6, 9, 4,7, 1, 1768241368000)
    , (2, '2025-08-21 19:57 -4:00', 1, 2,8, 11, 1,5, 5,1768241368000)
    , (2, '2025-08-21 20:20 -4:00', 1, 1,5, 11, 4,8, 7,1768241368000)
    , (2, '2025-08-21 20:21 -4:00', 1, 3,7, 11, 2,9, 7,1768241368000)
    -- fifth session one indoor
    , (2, '2025-08-28 18:56 -4:00', 1, 3,9, 11, 4,8, 8,1768241368000)
    , (2, '2025-08-28 19:08 -4:00', 1, 4,9, 11, 3,8, 3,1768241368000)
    , (2, '2025-08-28 19:17 -4:00', 1, 3,8, 11, 4,9, 6,1768241368000)
    , (2, '2025-08-28 19:30 -4:00', 1, 3,8, 11, 4,9, 5,1768241368000)
    , (2, '2025-08-28 19:58 -4:00', 1, 3,8, 11, 4,9, 5,1768241368000)
    -- sixth session one indoor
    , (2, '2025-10-16 18:45 -4:00', 1, 3,8, 11, 7,10, 5,1768241368000)
    , (2, '2025-10-16 18:57 -4:00', 1, 3,7, 11, 8,10, 5,1768241368000)
    , (2, '2025-10-16 19:04 -4:00', 1, 7,8, 11, 3,10, 0,1768241368000)
    ;
go

insert into [dbo].[Game] 
    (FacilityId,[PlayedDate], [TypeGameId], [TeamOnePlayerOneId], [TeamOnePlayerTwoId], [TeamOneScore], [TeamTwoPlayerOneId], [TeamTwoPlayerTwoId],  [TeamTwoScore], ChangedTime)
Values
    -- seventh session one indoor
      (2, '2025-10-30 18:19 -4:00', 1, 7,9, 11, 3,10, 4,1768241368000)
    , (2, '2025-10-30 18:36 -4:00', 1, 3,9, 11, 7,10, 6,1768241368000)
    , (2, '2025-10-30 20:23 -4:00', 1, 7,10, 11, 3,9, 8,1768241368000)
    -- eighth session Driftwood Park outdoor
    , (3, '2025-11-12 19:10 -4:00', 1, 8,9, 11, 6,7, 4,1768241368000)
    , (3, '2025-11-12 19:25 -4:00', 1, 6,8, 11, 7,9, 5,1768241368000)
    -- ninth session Driftwood Park outdoor
    , (3, '2025-11-20 18:40 -4:00', 1, 8,9, 11, 4,12, 5,1768241368000)
    , (3, '2025-11-20 18:46 -4:00', 1, 3,9, 11, 8,10, 8,1768241368000)
    , (3, '2025-11-20 19:03 -4:00', 1, 4,12, 11, 3,9, 8,1768241368000)
    , (3, '2025-11-20 19:14 -4:00', 1, 10,12, 11, 3,8, 9,1768241368000)
    , (3, '2025-11-20 19:25 -4:00', 1, 3,8, 11, 10,12, 6,1768241368000)
    , (3, '2025-11-20 19:36 -4:00', 1, 8,9, 11, 3,12, 9,1768241368000)
    , (3, '2025-11-20 19:47 -4:00', 1, 3,4, 11, 9,10, 9,1768241368000)
    --Bryan Park outdoor -- too crowded at 6:00 PM, better at 7:00. lights out at 8:20
    , (4, '2025-11-25 19:45 -4:00', 1, 4,7, 11, 3,9, 9,1768241368000)
;
go

insert into [dbo].[Game] 
    (FacilityId,[PlayedDate], [TypeGameId], [TeamOnePlayerOneId], [TeamOnePlayerTwoId], [TeamOneScore], [TeamTwoPlayerOneId], [TeamTwoPlayerTwoId],  [TeamTwoScore], ChangedTime)
Values    -- Driftwood Park 
    (3, '2025-12-04 18:19 -4:00', 1, 6,8,  11, 7,10,  2,1768241368000),
    (3, '2025-12-04 18:41 -4:00', 1, 7,9,  11, 8,11,  0,1768241368000),
    (3, '2025-12-04 18:46 -4:00', 1, 9,12, 11, 6,10,  10,1768241368000),
    (3, '2025-12-04 18:53 -4:00', 1, 8,10, 11, 7,11,  2,1768241368000),
    (3, '2025-12-04 19:09 -4:00', 1, 7,8,  11, 6,9,   7,1768241368000),
    (3, '2025-12-04 19:32 -4:00', 1, 8,10, 11, 9,11,  8,1768241368000),
    (3, '2025-12-04 19:44 -4:00', 1, 6,9,  11, 10,11, 5,1768241368000),
    -- C.B. Smith Park outdoor IT christmas event
    (5, '2025-12-11 15:09 -4:00', 1, 4,7, 11, 8,10, 2,1768241368000),
    (5, '2025-12-11 15:26 -4:00', 1, 4,8, 11, 7,10, 7,1768241368000),
    (5, '2025-12-11 15:58 -4:00', 1, 4,8, 11, 7,10, 7,1768241368000),
    (5, '2025-12-11 16:07 -4:00', 1, 7,8, 11, 3,13, 2,1768241368000),
    (5, '2025-12-11 16:25 -4:00', 1, 3,10, 11, 4,13, 9,1768241368000),
    (5, '2025-12-11 16:45 -4:00', 1, 7,8, 11, 3,13, 3,1768241368000),
    (5, '2025-12-11 15:08 -4:00', 1, 10,13, 11, 3,4, 4,1768241368000)
;

insert into [dbo].[Game] 
    (FacilityId,[PlayedDate], [TypeGameId], [TeamOnePlayerOneId], [TeamOnePlayerTwoId], [TeamOneScore], [TeamTwoPlayerOneId], [TeamTwoPlayerTwoId],  [TeamTwoScore], ChangedTime)
Values    -- Driftwood Park 
    (3, '2026-01-08 18:36 -4:00', 1, 6,13, 11, 7,11,  7, 1768241368),
    (3, '2026-01-08 18:42 -4:00', 1, 6,7,  11, 11,13,  2, 1768241368),
    (3, '2026-01-08 18:50 -4:00', 1, 7,8,  11, 3,13,  3, 1768241368),
    (3, '2026-01-08 19:08 -4:00', 1, 8,11, 11, 3,10,  8, 1768241368),
    (3, '2026-01-08 19:26 -4:00', 1, 8,13, 11, 3,7,   6, 1768241368),
    (3, '2026-01-08 19:31 -4:00', 1, 7,13, 11, 10,11, 0, 1768241368),
    (3, '2026-01-08 19:44 -4:00', 1, 10,13,11, 7,11,  6, 1768241368),
    (3, '2026-01-08 19:59 -4:00', 1, 7,13, 11, 3,8,   8, 1768241368),
    (3, '2026-01-08 20:11 -4:00', 1, 11,13,11, 3,10,  8, 1768241368),
    (3, '2026-01-08 20:20 -4:00', 1, 11,13,11, 3,10,  5, 1768241368)
;

insert into [dbo].[Game] 
    (FacilityId,[PlayedDate], [TypeGameId], [TeamOnePlayerOneId], [TeamOnePlayerTwoId], [TeamOneScore], [TeamTwoPlayerOneId], [TeamTwoPlayerTwoId],  [TeamTwoScore], ChangedTime)
Values    -- Driftwood Park 
    (3, '2026-01-14 18:29 -4:00', 1, 7,10, 11, 4,11,  9, 1768510800011),
    (3, '2026-01-14 18:40 -4:00', 1, 8,10, 11, 7,11,  4, 1768510800011),
    (3, '2026-01-14 18:56 -4:00', 1, 4,08, 11, 7,10,  6, 1768510800011),
    (3, '2026-01-14 19:14 -4:00', 1, 8,10, 11, 11,13,  3, 1768510800011),
    (3, '2026-01-14 19:22 -4:00', 1, 8,13, 11, 04,07,  3, 1768510800011),
    (3, '2026-01-14 19:36 -4:00', 1, 11,13, 11, 07,10,  8, 1768510800011),
    (3, '2026-01-14 19:53 -4:00', 1, 10,13, 11, 04,07,  9, 1768510800011),
    (3, '2026-01-14 20:03 -4:00', 1, 07,08, 11, 04,13,  2, 1768510800011),
    (3, '2026-01-14 20:24 -4:00', 1, 10,13, 13, 08,11,  11, 1768510800011)
;

insert into [dbo].[Game] 
    (FacilityId,[PlayedDate], [TypeGameId], [TeamOnePlayerOneId], [TeamOnePlayerTwoId], [TeamOneScore], [TeamTwoPlayerOneId], [TeamTwoPlayerTwoId],  [TeamTwoScore], ChangedTime)
Values    -- One indoor club
    (2, '2026-01-21 18:33 -4:00', 1, 3,10, 11, 13,14,  8, 1769880238000),
    (2, '2026-01-21 18:54 -4:00', 1, 8,9, 13, 3,7,  11, 1769880238000),
    (2, '2026-01-21 19:09 -4:00', 1, 10,13, 11, 9,14,  2, 1769880238000),
    (2, '2026-01-21 19:23 -4:00', 1, 3,8, 11, 7,13,  9, 1769880238000),
    (2, '2026-01-21 19:35 -4:00', 1, 9,10, 11, 7,14,  2, 1769880238000),
    (2, '2026-01-21 19:43 -4:00', 1, 8,9, 11, 3,13,  2, 1769880238000),
    (2, '2026-01-21 20:00 -4:00', 1, 9,10, 11, 3,14,  5, 1769880238000),
    (2, '2026-01-21 20:09 -4:00', 1, 3,8, 11, 9,13,  2, 1769880238000),
    (2, '2026-01-21 20:15 -4:00', 1, 10,13, 11, 3,14,  2, 1769880238000)
    ;



