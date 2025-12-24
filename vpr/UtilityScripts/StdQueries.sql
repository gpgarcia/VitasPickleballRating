select top 100 *
from [vpr].[dbo].GameDetails gd
where gd.PlayedDate > '2025-10-16'
order by gd.PlayedDate
;
go

-- game summary
select  gd.GameId
        , gd.PlayedDate
        , t1p1_FirstName = gd.team1_player1_FirstName
        , t1p2_FirstName = gd.team1_player2_FirstName
        , t1_Score = gd.team1_Score
        , t2p1_FirstName = gd.team2_player1_FirstName
        , t2p2_FirstName = gd.team2_player2_FirstName
        , t2_Score = gd.team2_Score
from [vpr].[dbo].GameDetails gd
--where gd.PlayedDate > '2025-11-28'
order by gd.PlayedDate
;
go

-- games played by each player
select p.FirstName
    , GameCount = COUNT(g.GameId)
from [vpr].[dbo].[Player] p 
--left join Team  t on t.PlayerOneId = p.PlayerId or t.PlayerTwoId = p.PlayerId
left join Game  g on p.PlayerId in (
                     g.TeamOnePlayerOneId
                    ,g.TeamOnePlayerTwoId
                    ,g.TeamTwoPlayerOneId
                    ,g.TeamTwoPlayerTwoId
                    )
group by p.FirstName
order by COUNT(g.GameId) desc, p.FirstName asc
;
-- name=ISNULL(p.NickName, p.FirstName)
go

-- player Ratings all
select  rn = ROW_NUMBER() OVER (PARTITION BY p.PlayerId ORDER BY pr.RatingDate DESC),
        p.PlayerId,
        p.FirstName,
        NickName = ISNULL(p.NickName, p.FirstName),
        p.LastName,
        pr.Rating,
        pr.RatingDate
from    [vpr].[dbo].[PlayerRating] pr
join    [vpr].[dbo].[Player] p on pr.PlayerId = p.PlayerId
where   pr.RatingDate <= GETDATE()
order by p.PlayerId, pr.RatingDate desc
;
go

-- Game Prediction Accuracy Date
Select  g.GameId
        , g.PlayedDate
        , gp.T1P1Rating
        , gp.T1P2Rating
        , gp.T2P1Rating
        , gp.T2P2Rating
        , gp.ExpectT1Score
        , gp.ExpectT2Score
        , ExpectedRatioScore = gp.ExpectT1Score / cast(gp.ExpectT1Score + gp.ExpectT2Score as float )
        , g.TeamOneScore
        , g.TeamTwoScore
        , ActualRatioScore = g.TeamOneScore / Cast(g.TeamOneScore + g.TeamTwoScore as float )
        , Team1WinsExpected = CASE WHEN gp.ExpectT1Score > gp.ExpectT2Score THEN 1 ELSE 0 END
        , Team1Wins = CASE WHEN g.TeamOneScore > g.TeamTwoScore THEN 1 ELSE 0 END
FROM    [vpr].[dbo].[Game] g
JOIN    [vpr].[dbo].[GamePrediction] gp on gp.GameId = g.GameId
WHERE   g.PlayedDate IS NOT NULL
;
go


-- Player rating change base data
Select top 100 g.GameId
        , pr.PlayerRatingId,	pr.PlayerId,	pr.Rating
        , gp.GameId,	gp.T1P1Rating, gp.T1P2Rating,	gp.T2P1Rating,	gp.T2P2Rating, gp.T1PredictedWinProb
        , actualT1RatioScore = g2.TeamOneScore / Cast(g2.TeamOneScore + g2.TeamTwoScore as float )
FROM    [vpr].[dbo].game g
left join [vpr].[dbo].PlayerRating pr on pr.PlayerId in (g.TeamOnePlayerOneId, g.TeamOnePlayerTwoId, g.TeamTwoPlayerOneId, g.TeamTwoPlayerTwoId)
left join [vpr].[dbo].GamePrediction gp on gp.GameId = pr.GameId
left join [vpr].[dbo].Game g2 on g2.GameId = gp.GameId
WHERE   g.PlayedDate >= pr.RatingDate
AND     g.GameId = 3
order by g.gameId, pr.PlayerId
;
go

