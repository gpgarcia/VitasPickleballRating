using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PickleBallAPI.Models;

public partial class VprContext : DbContext
{
    public VprContext(DbContextOptions<VprContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<GameDetail> GameDetails { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<PlayerRating> PlayerRatings { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<TypeGame> TypeGames { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.GameId).HasName("PK__Game__2AB897FD1DA4E37D");

            entity.ToTable("Game");

            entity.HasOne(d => d.TeamOne).WithMany(p => p.GameTeamOnes)
                .HasForeignKey(d => d.TeamOneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Game_TeamOne");

            entity.HasOne(d => d.TeamTwo).WithMany(p => p.GameTeamTwos)
                .HasForeignKey(d => d.TeamTwoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Game_TeamTwo");

            entity.HasOne(d => d.TypeGame).WithMany(p => p.Games)
                .HasForeignKey(d => d.TypeGameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Game_TypeGame");
        });

        modelBuilder.Entity<GameDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("GameDetails");

            entity.Property(e => e.GameType).HasMaxLength(20);
            entity.Property(e => e.Team1Player1FirstName)
                .HasMaxLength(50)
                .HasColumnName("team1_player1_FirstName");
            entity.Property(e => e.Team1Player1LastName)
                .HasMaxLength(50)
                .HasColumnName("team1_player1_LastName");
            entity.Property(e => e.Team1Player1PlayerId).HasColumnName("team1_player1_PlayerId");
            entity.Property(e => e.Team1Player2FirstName)
                .HasMaxLength(50)
                .HasColumnName("team1_player2_FirstName");
            entity.Property(e => e.Team1Player2LastName)
                .HasMaxLength(50)
                .HasColumnName("team1_player2_LastName");
            entity.Property(e => e.Team1Player2PlayerId).HasColumnName("team1_player2_PlayerId");
            entity.Property(e => e.Team1Score).HasColumnName("team1_Score");
            entity.Property(e => e.Team1Win).HasColumnName("team1_Win");
            entity.Property(e => e.Team2Player1FirstName)
                .HasMaxLength(50)
                .HasColumnName("team2_player1_FirstName");
            entity.Property(e => e.Team2Player1LastName)
                .HasMaxLength(50)
                .HasColumnName("team2_player1_LastName");
            entity.Property(e => e.Team2Player1PlayerId).HasColumnName("team2_player1_PlayerId");
            entity.Property(e => e.Team2Player2FirstName)
                .HasMaxLength(50)
                .HasColumnName("team2_player2_FirstName");
            entity.Property(e => e.Team2Player2LastName)
                .HasMaxLength(50)
                .HasColumnName("team2_player2_LastName");
            entity.Property(e => e.Team2Player2PlayerId).HasColumnName("team2_player2_PlayerId");
            entity.Property(e => e.Team2Score).HasColumnName("team2_Score");
            entity.Property(e => e.Team2Win).HasColumnName("team2_Win");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.PlayerId).HasName("PK__Player__4A4E74C8252D71F4");

            entity.ToTable("Player");

            entity.HasIndex(e => new { e.FirstName, e.LastName }, "UQ__Player__2457AEF041CFE24D").IsUnique();

            if (Database.IsSqlServer())
            {
                entity.Property(e => e.ChangedDate).HasDefaultValueSql("GetDate()");
            }
            else //if (Database.IsSqlite())
            {
                entity.Property(e => e.ChangedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            }
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
        });

        modelBuilder.Entity<PlayerRating>(entity =>
        {
            entity.HasKey(e => e.PlayerRatingId).HasName("PK__PlayerRa__EC285E8B5D425B94");

            entity.ToTable("PlayerRating");

            if (Database.IsSqlServer())
            {
                entity.Property(e => e.RatingDate).HasDefaultValueSql("GetDate()");
            }
            else //if (Database.IsSqlite())
            {
                entity.Property(e => e.RatingDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            }

            entity.HasOne(d => d.Game).WithMany(p => p.PlayerRatings)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK_PlayerRating_game");

            entity.HasOne(d => d.Player).WithMany(p => p.PlayerRatings)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PlayerRating_Player");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.TeamId).HasName("PK__Team__123AE7995084B5E5");

            entity.ToTable("Team");

            if (Database.IsSqlServer())
            {
                entity.Property(e => e.ChangedDate).HasDefaultValueSql("GetDate()");
            }
            else //if (Database.IsSqlite())
            {
                entity.Property(e => e.ChangedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            }

            entity.HasOne(d => d.PlayerOne).WithMany(p => p.TeamPlayerOnes)
                .HasForeignKey(d => d.PlayerOneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Team_PlayerOne");

            entity.HasOne(d => d.PlayerTwo).WithMany(p => p.TeamPlayerTwos)
                .HasForeignKey(d => d.PlayerTwoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Team_PlayerTwo ");
        });

        modelBuilder.Entity<TypeGame>(entity =>
        {
            entity.HasKey(e => e.TypeGameId).HasName("PK__TypeGame__6EA2929A1F4766B4");

            entity.ToTable("TypeGame");

            if (Database.IsSqlServer())
            {
                entity.Property(e => e.ChangedDate).HasDefaultValueSql("GetDate()");
            }
            else //if (Database.IsSqlite())
            {
                entity.Property(e => e.ChangedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            }
            entity.Property(e => e.GameType).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
