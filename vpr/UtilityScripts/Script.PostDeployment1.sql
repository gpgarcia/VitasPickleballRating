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
insert into [dbo].[Player] 
    ([FirstName], [LastName])
VALUES 
    ('Jean-Michael', 'Querol'),
    ('Keenan', 'Rodriguez'),
    ('Goutham', 'Kanddibanda'),
    ('Gerardo', 'Garcia'),
    ('Samuel', 'Horowitz'),
    ('Denise', 'Sleem'),
    ('Fernando', 'Paz') ,
    ('William', 'Waddell'),
    ('Francisco', 'Vela') 
    ;

insert into [dbo].[TypeGame] 
    ([GameType])
VALUES 
    ('Recreational'),
    ('Tournament')
    ;

insert into [dbo].[Game] 
    ([PlayedDate], [TypeGameId], [TeamOnePlayerOneId], [TeamOnePlayerTwoId], [TeamOneScore], [TeamTwoPlayerOneId], [TeamTwoPlayerTwoId],  [TeamTwoScore])
Values
    -- i do not remember the scores for the first set of games isola outdoor
      ('2025-07-25 19:15 -4:00', 1, 3,5, 11, 4,6, 4)
    , ('2025-07-25 19:35 -4:00', 1, 3,5, 11, 4,6, 6)
    , ('2025-07-25 21:55 -4:00', 1, 4,5, 11, 3,6, 9)
    -- second session isola outdoor
    , ('2025-07-31 18:34 -4:00', 1, 1,2, 11, 3,4, 3)
    , ('2025-07-31 18:43 -4:00', 1, 2,4, 11, 1,3, 0)
    , ('2025-07-31 19:00 -4:00', 1, 2,3, 11, 1,4, 8)
    , ('2025-07-31 19:18 -4:00', 1, 2,3, 11, 1,4, 5)
    , ('2025-07-31 19:38 -4:00', 1, 1,2, 11, 3,4, 9)
    -- third session one indoor
    , ('2025-08-07 19:14 -4:00', 1, 3,7, 13, 6,8, 11)
    , ('2025-08-07 19:27 -4:00', 1, 6,7, 11 ,3,8, 5)
    , ('2025-08-07 19:41 -4:00', 1, 6,7, 11, 3,8, 5)
    , ('2025-08-07 19:51 -4:00', 1, 3,7, 11, 6,8, 2)
    -- fourth session one indoor
    , ('2025-08-21 18:37 -4:00', 1, 3,6, 11, 4,9, 2)
    , ('2025-08-21 18:50 -4:00', 1, 5,6, 11, 1,4, 3)
    , ('2025-08-21 19:00 -4:00', 1, 2,4, 11, 8,9, 4)
    , ('2025-08-21 19:14 -4:00', 1, 7,8, 11, 1,5, 9)
    , ('2025-08-21 19:29 -4:00', 1, 2,3, 11, 6,7, 9)
    , ('2025-08-21 19:39 -4:00', 1, 5,9, 11, 1,8, 0)
    , ('2025-08-21 19:46 -4:00', 1, 2,6, 9, 4,7, 1)
    , ('2025-08-21 19:57 -4:00', 1, 2,8, 11, 1,5, 5)
    , ('2025-08-21 20:20 -4:00', 1, 1,5, 11, 4,8, 7)
    , ('2025-08-21 20:21 -4:00', 1, 3,7, 11, 2,9, 7)
    -- fifth session one indoor
    , ('2025-08-28 18:56 -4:00', 1, 3,9, 11, 4,8, 8)
    , ('2025-08-28 19:08 -4:00', 1, 4,9, 11, 3,8, 3)
    , ('2025-08-28 19:17 -4:00', 1, 3,8, 11, 4,9, 6)
    , ('2025-08-28 19:30 -4:00', 1, 3,8, 11, 4,9, 5)
    , ('2025-08-28 19:58 -4:00', 1, 3,8, 11, 4,9, 5)

;
