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

insert into [dbo].[Team] 
    ([PlayerOneId], [PlayerTwoId])
VALUES 
    (1, 2),     -- Jean-Michael Querol, Keenan Rodriguez
    (3, 4),     -- Goutham Kanddibanda, Gerardo Garcia
    (2, 4),     -- Keenan Rodriguez, Gerardo Garcia
    (1, 3),     -- Jean-Michael Querol, Goutham Kanddibanda
    (1, 4),     -- Jean-Michael Querol, Gerardo Garcia
    --6
    (2, 3),     -- Keenan Rodriguez, Goutham Kanddibanda
    (5, 6),     -- Samuel Horowitz, Denise Sleem
    (4, 6),     -- Gerardo Garcia, Denise Sleem
    (3, 5),     -- Goutham Kanddibanda, Samuel Horowitz
    (4, 5),     -- Gerardo Garcia, Samuel Horowitz  
    --11
    (3, 6),      -- Goutham Kanddibanda, Denise Sleem
    (5, 7),      -- Samuel Horowitz, Fernando Paz
    (1, 5),      -- Jean-Michael Querol, Samuel Horowitz
    (2, 5),      -- Keenan Rodriguez, Samuel Horowitz
    (1, 6),      -- Jean-Michael Querol, Denise Sleem
    --16
    (2, 6),      -- Keenan Rodriguez, Denise Sleem
    --------
    (1, 7),      -- Jean-Michael Querol, Fernando Paz
    (2, 7),      -- Keenan Rodriguez, Fernando Paz
    (3, 7),      -- Goutham Kanddibanda, Fernando Paz
    (4, 7),      -- Gerardo Garcia, Fernando Paz
    --21
    (5, 7),      -- Samuel Horowitz, Fernando Paz
    (6, 7),      -- Denise Sleem, Fernando Paz
    (1, 8),      -- Jean-Michael Querol, William Waddell
    (2, 8),      -- Keenan Rodriguez, William Waddell
    (3, 8),      -- Goutham Kanddibanda, William Waddell
    --26
    (4, 8),      -- Gerardo Garcia, William Waddell
    (5, 8),      -- Samuel Horowitz, William Waddell
    (6, 8),      -- Denise Sleem, William Waddell
    (7, 8),      -- Fernando Paz, William Waddell
    (1, 9),      -- Jean-Michael Querol, Francisco Vela
    --31
    (2, 9),      -- Keenan Rodriguez, Francisco Vela
    (3, 9),      -- Goutham Kanddibanda, Francisco Vela
    (4, 9),      -- Gerardo Garcia, Francisco Vela
    (5, 9),      -- Samuel Horowitz, Francisco Vela
    (6, 9),      -- Denise Sleem, Francisco Vela
    --36
    (7, 9),      -- Fernando Paz, Francisco Vela
    (8, 9)       -- William Waddell, Francisco Vela



;

insert into [dbo].[Game] 
    ([PlayedDate], [TypeGameId], [TeamOneId], [TeamOneScore], [TeamTwoId], [TeamTwoScore])
Values
    -- i do not remember the scores for the first set of games isola outdoor
      ('2025-07-25 19:15 -4:00', 1, 9, 11, 8, 4)
    , ('2025-07-25 19:35 -4:00', 1, 9, 11, 8, 6)
    , ('2025-07-25 21:55 -4:00', 1, 10, 11, 11, 9)
    -- second session isola outdoor
    , ('2025-07-31 18:34 -4:00', 1, 1, 11, 2, 3)
    , ('2025-07-31 18:43 -4:00', 1, 3, 11, 4, 0)
    , ('2025-07-31 19:00 -4:00', 1, 6, 11, 5, 8)
    , ('2025-07-31 19:18 -4:00', 1, 6, 11, 5, 5)
    , ('2025-07-31 19:38 -4:00', 1, 1, 11, 2, 9)
    -- third session one indoor
    , ('2025-08-07 19:14 -4:00', 1, 19, 13, 28, 11)
    , ('2025-08-07 19:27 -4:00', 1, 22, 11 ,25, 5)
    , ('2025-08-07 19:41 -4:00', 1, 22, 11, 25, 5)
    , ('2025-08-07 19:51 -4:00', 1, 19, 11, 28, 2)
    -- fourth session one indoor
    , ('2025-08-21 18:37 -4:00', 1, 11, 11, 33, 2)
    , ('2025-08-21 18:50 -4:00', 1, 7, 11, 5, 3)
    , ('2025-08-21 19:00 -4:00', 1, 3, 11, 37, 4)
    , ('2025-08-21 19:14 -4:00', 1, 29, 11, 13, 9)
    , ('2025-08-21 19:29 -4:00', 1, 6, 11, 22, 9)
    , ('2025-08-21 19:39 -4:00', 1, 34, 11, 23, 0)
    , ('2025-08-21 19:46 -4:00', 1, 16, 9, 20, 1)
    , ('2025-08-21 19:57 -4:00', 1, 24, 11, 13, 5)
    , ('2025-08-21 20:20 -4:00', 1, 13, 11, 26, 7)
    , ('2025-08-21 20:21 -4:00', 1, 19, 11, 31, 7)
    -- fifth session one indoor
    , ('2025-08-28 18:56 -4:00', 1, 32, 11, 26, 8)
    , ('2025-08-28 19:08 -4:00', 1, 33, 11, 25, 3)
    , ('2025-08-28 19:17 -4:00', 1, 25, 11, 33, 6)
    , ('2025-08-28 19:30 -4:00', 1, 25, 11, 33, 5)
    , ('2025-08-28 19:58 -4:00', 1, 25, 11, 33, 5)

;

insert into [dbo].[PlayerRating] 
    ([PlayerId], [Rating], [RatingDate])
values
    (1, 250, '2025-07-24 00:00 -4:00'),
    (2, 250, '2025-07-24 00:00 -4:00'),
    (3, 250, '2025-07-24 00:00 -4:00'),
    (4, 250, '2025-07-24 00:00 -4:00'),
    (5, 250, '2025-07-24 00:00 -4:00'),
    (6, 250, '2025-07-24 00:00 -4:00'),
    (7, 250, '2025-07-24 00:00 -4:00'),
    (8, 250, '2025-07-24 00:00 -4:00'),
    (9, 250, '2025-07-24 00:00 -4:00')
;