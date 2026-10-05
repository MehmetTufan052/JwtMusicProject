using JwtMusicNight.WebApi.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JwtMusicNight.WebApi.Context
{
    public class JwtContext:IdentityDbContext<AppUser>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-MO1FN9D\\SQL2019;initial catalog=JwtMusicNightDb;integrated security=true");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Paket silinince sarkilar silinmesin
            builder.Entity<Song>()
                .HasOne(s => s.Package)
                .WithMany(p => p.Songs)
                .HasForeignKey(s => s.PackageId)
                .OnDelete(DeleteBehavior.Restrict);

            // Paket silinince kullanicilar silinmesin
            builder.Entity<AppUser>()
                .HasOne(u => u.Package)
                .WithMany(p => p.Users)
                .HasForeignKey(u => u.PackageId)
                .OnDelete(DeleteBehavior.Restrict);

            // Bir kullanici bir sanatciyi sadece bir kere takip edebilir
            builder.Entity<ArtistFollow>()
                .HasIndex(f => new { f.AppUserId, f.ArtistId })
                .IsUnique();

            // Bir kullanici bir sarkiyi sadece bir kere begenebilir
            builder.Entity<SongFavorite>()
                .HasIndex(f => new { f.AppUserId, f.SongId })
                .IsUnique();
        }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<ListeningHistory> ListeningHistories { get; set; }
        public DbSet<ArtistFollow> ArtistFollows { get; set; }
        public DbSet<SongFavorite> SongFavorites { get; set; }
    }
} 
