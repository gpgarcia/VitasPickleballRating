using Microsoft.EntityFrameworkCore;

namespace PickleBallAPI.Models
{
    public partial class VprContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>(entity =>
            {
                if (Database.IsSqlServer())
                {
                    entity.Property(e => e.ChangedDate).HasDefaultValueSql("SYSDATETIME()");
                }
                else //if (Database.IsSqlite())
                {
                    entity.Property(e => e.ChangedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
                }
            });
            modelBuilder.Entity<PlayerRating>(entity =>
            {
                if (Database.IsSqlServer())
                {
                    entity.Property(e => e.RatingDate).HasDefaultValueSql("SYSDATETIME()");
                }
                else //if (Database.IsSqlite())
                {
                    entity.Property(e => e.RatingDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
                }
            });
            modelBuilder.Entity<TypeGame>(entity =>
            {
                if (Database.IsSqlServer())
                {
                    entity.Property(e => e.ChangedDate).HasDefaultValueSql("SYSDATETIME()");
                }
                else //if (Database.IsSqlite())
                {
                    entity.Property(e => e.ChangedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
                }
            });

            modelBuilder.Entity<GamePrediction>(entity =>
            {
                if (Database.IsSqlServer())
                {
                    entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
                }
                else //if (Database.IsSqlite())
                {
                    entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                }
            });

            //modelBuilder.Entity<Game>(entity =>
            //{
            //    entity
            //        .HasOne(d => d.TypeGame)
            //        .WithMany()
            //        .HasForeignKey(d => d.TypeGameId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("FK_Game_TypeGame");
            //});

        }

    }
}
