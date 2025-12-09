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
            entity.HasOne(d => d.TypeFacility).WithMany()
                .HasForeignKey(d => d.TypeFacilityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Facility_TypeFacilityId");

        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.GameId).HasName("PK_Game_GameId");

            entity.ToTable("Game");

            entity.HasOne(d => d.Facility).WithMany()
                .HasForeignKey(d => d.FacilityId)
                .HasConstraintName("FK_Game_FacilityId");

            entity.HasOne(d => d.TeamOnePlayerOne).WithMany()
                .HasForeignKey(d => d.TeamOnePlayerOneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Game_Player_t1p1");

            entity.HasOne(d => d.TeamOnePlayerTwo).WithMany()
                .HasForeignKey(d => d.TeamOnePlayerTwoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Game_Player_t1p2");

            entity.HasOne(d => d.TeamTwoPlayerOne).WithMany()
                .HasForeignKey(d => d.TeamTwoPlayerOneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Game_Player_t2p1");

            entity.HasOne(d => d.TeamTwoPlayerTwo).WithMany()
                .HasForeignKey(d => d.TeamTwoPlayerTwoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Game_Player_t2p2");

            entity.HasOne(d => d.TypeGame).WithMany()
                .HasForeignKey(d => d.TypeGameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Game_TypeGameId");
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

            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
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

            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.WinPct).HasColumnType("numeric(24, 12)");
        });

        modelBuilder.Entity<TypeFacility>(entity =>
        {
            entity.HasKey(e => e.TypeFacilityId).HasName("PK_TypeFacility_TypeFacilityId");

            entity.ToTable("TypeFacility");

            entity.HasIndex(e => e.FacilityType, "UQ_TypeFacility_FacilityType").IsUnique();

            entity.Property(e => e.TypeFacilityId).ValueGeneratedNever();
            entity.Property(e => e.FacilityType)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TypeGame>(entity =>
        {
            entity.HasKey(e => e.TypeGameId).HasName("PK_TypeGame_TypeGameId");

            entity.ToTable("TypeGame");

            entity.HasIndex(e => e.GameType, "UQ_TypeGame_GameType").IsUnique();

            entity.Property(e => e.TypeGameId).ValueGeneratedNever();
            entity.Property(e => e.GameType).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
