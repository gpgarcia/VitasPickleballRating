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

    public virtual DbSet<TypeGame> TypeGames { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.GameId).HasName("PK_Game_GameId");

            entity.ToTable("Game");


            entity.HasOne(d => d.TypeGame).WithMany()
                .HasForeignKey(d => d.TypeGameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Game_TypeGameId");
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

        modelBuilder.Entity<GamePrediction>(entity =>
        {
            entity.HasKey(e => e.GameId).HasName("PK_GamePrediction_GameId");

            entity.ToTable("GamePrediction");

            entity.Property(e => e.GameId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.ExpectT1score).HasColumnName("ExpectT1Score");
            entity.Property(e => e.ExpectT2score).HasColumnName("ExpectT2Score");
            entity.Property(e => e.T1p1rating).HasColumnName("T1P1Rating");
            entity.Property(e => e.T1p2rating).HasColumnName("T1P2Rating");
            entity.Property(e => e.T1predictedWinProb).HasColumnName("T1PredictedWinProb");
            entity.Property(e => e.T2p1rating).HasColumnName("T2P1Rating");
            entity.Property(e => e.T2p2rating).HasColumnName("T2P2Rating");

            entity.HasOne(d => d.Game).WithOne(p => p.GamePrediction)
                .HasForeignKey<GamePrediction>(d => d.GameId)
                .HasConstraintName("FK_GamePrediction_Game");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.PlayerId).HasName("PK_Player_PlayerId");

            entity.ToTable("Player");

            entity.HasIndex(e => new { e.FirstName, e.LastName }, "UQ_Player_FirstName_LastName").IsUnique();

            entity.Property(e => e.ChangedDate).HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
        });

        modelBuilder.Entity<PlayerRating>(entity =>
        {
            entity.HasKey(e => e.PlayerRatingId).HasName("PK_PlayerRating_PlayerRatingId");

            entity.ToTable("PlayerRating");

            entity.HasIndex(e => new { e.PlayerId, e.GameId }, "UQ_PlayerRating_PlayerId_GameId").IsUnique();

            entity.Property(e => e.RatingDate).HasDefaultValueSql("(sysdatetimeoffset())");

            entity.HasOne(d => d.Game).WithMany(p => p.PlayerRatings)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK_PlayerRating_game");

            entity.HasOne(d => d.Player).WithMany(p => p.PlayerRatings)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PlayerRating_Player");
        });

        modelBuilder.Entity<TypeGame>(entity =>
        {
            entity.HasKey(e => e.TypeGameId).HasName("PK_TypeGame_TypeGameId");

            entity.ToTable("TypeGame");

            entity.HasIndex(e => e.GameType, "UQ_TypeGame_GameType").IsUnique();

            entity.Property(e => e.ChangedDate).HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.GameType).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
