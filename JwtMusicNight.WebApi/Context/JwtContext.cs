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

            // Bir sarki bir playliste sadece bir kere eklenebilir
            builder.Entity<PlaylistSong>()
                .HasIndex(ps => new { ps.PlaylistId, ps.SongId })
                .IsUnique();

            // Playlist silinince icindeki sarki kayitlari da silinsin
            builder.Entity<PlaylistSong>()
                .HasOne(ps => ps.Playlist)
                .WithMany(p => p.PlaylistSongs)
                .HasForeignKey(ps => ps.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);

            // Coklu cascade path hatasina girmemek icin (Package silinince sarkilar silinmiyordu, ayni mantik)
            builder.Entity<PlaylistSong>()
                .HasOne(ps => ps.Song)
                .WithMany()
                .HasForeignKey(ps => ps.SongId)
                .OnDelete(DeleteBehavior.Restrict);
        }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<ListeningHistory> ListeningHistories { get; set; }
        public DbSet<ArtistFollow> ArtistFollows { get; set; }
        public DbSet<SongFavorite> SongFavorites { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistSong> PlaylistSongs { get; set; }
    }
} 
