using Microsoft.EntityFrameworkCore;

namespace PickleBallAPI.Models
{
    public partial class VprContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>(entity =>
            {
                entity.Property(e => e.ChangedTime).IsConcurrencyToken();
            });
            modelBuilder.Entity<Player>(entity =>
            {
                entity.Property(e => e.ChangedTime).IsConcurrencyToken();
            });
            modelBuilder.Entity<PlayerRating>(entity =>
            {
                entity.Property(e => e.ChangedTime).IsConcurrencyToken();
            });
            modelBuilder.Entity<Facility>(entity =>
            {
                entity.Property(e => e.ChangedTime).IsConcurrencyToken();
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
