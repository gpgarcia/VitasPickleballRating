CREATE VIEW [dbo].[PlayerStanding]
    AS 
    -- standings by player
    WITH PlayerGameStats AS
    (
        select  p.PlayerId
                , g.GameId
                , Result = CASE WHEN (  g.TeamOneScore > g.TeamTwoScore 
                                    and
                                        (p.PlayerId = g.TeamOnePlayerOneId 
                                        OR  p.PlayerId = g.TeamOnePlayerTwoId)
                                    )
                                    OR ( g.TeamTwoScore > g.TeamOneScore
                                        and
                                        (p.PlayerId = g.TeamTwoPlayerOneId 
                                        OR  p.PlayerId = g.TeamTwoPlayerTwoId)
                                    )
                               THEN  1
                               ELSE 0 END
        from [dbo].[Player] p 
        join [dbo].[Game]  g on    p.PlayerId in (
                                g.TeamOnePlayerOneId,
                                g.TeamOnePlayerTwoId,
                                g.TeamTwoPlayerOneId,
                                g.TeamTwoPlayerTwoId
                                )
    )
    select Name=ISNULL(p.NickName, p.FirstName)
        , GamesPlayed = COUNT(pg.GameId)
        , Wins = COUNT(CASE WHEN Result = 1 THEN 1 END)
        , Losses = COUNT(CASE WHEN Result = 0 THEN 1 END)
        , WinPct = COUNT(CASE WHEN pg.Result = 1 THEN 1 END) * 1.0 / COUNT(pg.GameId)

        from PlayerGameStats pg
        join [dbo].[Player] p on pg.PlayerId = p.PlayerId
        group by ISNULL(p.NickName, p.FirstName)
    ;


