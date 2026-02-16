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

    public virtual DbSet<Facility> Facilities { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<GameDetail> GameDetails { get; set; }

    public virtual DbSet<GamePrediction> GamePredictions { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<PlayerRating> PlayerRatings { get; set; }

    public virtual DbSet<PlayerStanding> PlayerStandings { get; set; }

    public virtual DbSet<TypeFacility> TypeFacilities { get; set; }

    public virtual DbSet<TypeGame> TypeGames { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Facility>(entity =>
        {
            entity.HasKey(e => e.FacilityId).HasName("PK_Facility_FacilityId");

            entity.ToTable("Facility");

            entity.Property(e => e.AddressLine1)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.AddressLine2)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Notes).IsUnicode(false);
            entity.Property(e => e.PostalCode)
                .HasMaxLength(11)
                .IsUnicode(false);
            entity.Property(e => e.StateCode)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.TypeFacility).WithMany(p => p.Facilities)
                .HasForeignKey(d => d.TypeFacilityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Facility_TypeFacilityID");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.GameId).HasName("PK_Game_GameId");

            entity.ToTable("Game");

            entity.HasOne(d => d.Facility).WithMany(p => p.Games)
                .HasForeignKey(d => d.FacilityId)
                .HasConstraintName("FK_Game_FacilityId");

            entity.HasOne(d => d.TeamOnePlayerOne).WithMany(p => p.GameTeamOnePlayerOnes)
                .HasForeignKey(d => d.TeamOnePlayerOneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Game_Player_t1p1");

            entity.HasOne(d => d.TeamOnePlayerTwo).WithMany(p => p.GameTeamOnePlayerTwos)
                .HasForeignKey(d => d.TeamOnePlayerTwoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Game_Player_t1p2");

            entity.HasOne(d => d.TeamTwoPlayerOne).WithMany(p => p.GameTeamTwoPlayerOnes)
                .HasForeignKey(d => d.TeamTwoPlayerOneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Game_Player_t2p1");

            entity.HasOne(d => d.TeamTwoPlayerTwo).WithMany(p => p.GameTeamTwoPlayerTwos)
                .HasForeignKey(d => d.TeamTwoPlayerTwoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Game_Player_t2p2");

            entity.HasOne(d => d.TypeGame).WithMany(p => p.Games)
                .HasForeignKey(d => d.TypeGameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Game_TypeGameId");
        });

        modelBuilder.Entity<GameDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("GameDetails");

            entity.Property(e => e.GameTypeName)
                .HasMaxLength(20)
                .HasColumnName("game_type_name");
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
            entity.Property(e => e.ExpectT1score).HasColumnName("ExpectT1Score");
            entity.Property(e => e.ExpectT2score).HasColumnName("ExpectT2Score");
            entity.Property(e => e.T1p1rating).HasColumnName("T1P1Rating");
            entity.Property(e => e.T1p2rating).HasColumnName("T1P2Rating");
            entity.Property(e => e.T1predictedWinProb)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("T1PredictedWinProb");
            entity.Property(e => e.T2p1rating).HasColumnName("T2P1Rating");
            entity.Property(e => e.T2p2rating).HasColumnName("T2P2Rating");

            entity.HasOne(d => d.Game).WithOne(p => p.Prediction)
                .HasForeignKey<GamePrediction>(d => d.GameId)
                .HasConstraintName("FK_GamePrediction_Game");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.PlayerId).HasName("PK_Player_PlayerId");

            entity.ToTable("Player");

            entity.HasIndex(e => new { e.FirstName, e.LastName }, "UQ_Player_FirstName_LastName").IsUnique();

            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.NickName).HasMaxLength(50);
        });

        modelBuilder.Entity<PlayerRating>(entity =>
        {
            entity.HasKey(e => e.PlayerRatingId)
                .HasName("PK_PlayerRating_PlayerRatingId")
                .IsClustered(false);

            entity.ToTable("PlayerRating");

            entity.HasIndex(e => new { e.PlayerId, e.GameId }, "UQ_PlayerRating_PlayerId_GameId")
                .IsUnique()
                .IsClustered();

            entity.HasOne(d => d.Game).WithMany(p => p.PlayerRatings)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PlayerRating_game");

            entity.HasOne(d => d.Player).WithMany(p => p.PlayerRatings)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PlayerRating_Player");
        });

        modelBuilder.Entity<PlayerStanding>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("PlayerStanding");

            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.WinPct).HasColumnType("numeric(24, 12)");
        });

        modelBuilder.Entity<TypeFacility>(entity =>
        {
            entity.HasKey(e => e.TypeFacilityId).HasName("PK_TypeFacility_TypeFacilityId");

            entity.ToTable("TypeFacility");

            entity.HasIndex(e => e.Name, "UQ_TypeFacility_FacilityType").IsUnique();

            entity.Property(e => e.TypeFacilityId).ValueGeneratedNever();
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TypeGame>(entity =>
        {
            entity.HasKey(e => e.TypeGameId).HasName("PK_TypeGame_TypeGameId");

            entity.ToTable("TypeGame");

            entity.HasIndex(e => e.Name, "UQ_TypeGame_GameType").IsUnique();

            entity.Property(e => e.TypeGameId).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
