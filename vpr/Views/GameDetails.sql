CREATE VIEW [dbo].[GameDetails]
AS 
SELECT  g.GameId, g.PlayedDate 
        , game_type_name = tg.Name   
        , team1_player1_PlayerId = t1p1.PlayerId
        , team1_player1_FirstName = t1p1.FirstName
        , team1_player1_LastName = t1p1.LastName
        , team1_player2_PlayerId = t1p2.PlayerId
        , team1_player2_FirstName = t1p2.FirstName
        , team1_player2_LastName = t1p2.LastName
        , team1_Score = g.TeamOneScore
        , team1_Win = CASE WHEN g.TeamOneScore > g.TeamTwoScore THEN 1 ELSE 0 END
        , team2_player1_PlayerId = t2p1.PlayerId
        , team2_player1_FirstName = t2p1.FirstName
        , team2_player1_LastName = t2p1.LastName
        , team2_player2_PlayerId = t2p2.PlayerId
        , team2_player2_FirstName = t2p2.FirstName
        , team2_player2_LastName = t2p2.LastName
        , team2_Score =g.TeamTwoScore
        , team2_Win = CASE WHEN g.TeamTwoScore > g.TeamOneScore THEN 1 ELSE 0 END
FROM        Game g
left join   TypeGame tg on g.TypeGameId = tg.TypeGameId
left join   Player t1p1 on g.TeamOnePlayerOneId = t1p1.PlayerId
left join   Player t1p2 on g.TeamOnePlayerTwoId = t1p2.PlayerId
left join   Player t2p1 on g.TeamTwoPlayerOneId = t2p1.PlayerId
left join   Player t2p2 on g.TeamTwoPlayerTwoId = t2p2.PlayerId
;



