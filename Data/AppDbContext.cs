using Microsoft.EntityFrameworkCore;
using WordMemoryApi.Entities;

namespace WordMemoryApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Word> Words { get; set; }
        public DbSet<UserWord> UserWords { get; set; }
        public DbSet<QuizResult> QuizResults { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserWord>()
        .HasOne(uw => uw.Word)
        .WithMany(u => u.UserWords)
        .HasForeignKey(uw => uw.WordId)
        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserWord>()
                .HasOne(uw => uw.User)
                .WithMany(u => u.UserWords)
                .HasForeignKey(uw => uw.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QuizResult>()
                .HasOne(qr => qr.User)
                .WithMany(u => u.QuizResults)
                .HasForeignKey(qr => qr.UserId);

            modelBuilder.Entity<UserSettings>()
    .HasOne(us => us.User)
    .WithOne(u => u.Settings)
    .HasForeignKey<UserSettings>(us => us.UserId);
        }

        
    }
}
