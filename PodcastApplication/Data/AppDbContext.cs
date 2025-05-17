using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PodcastApplication.Models;

namespace PodcastApplication.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base (options)
        {
            
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Podcast> Podcasts { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<EpisodeLike> EpisodeLikes { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistItem> PlaylistItems { get; set; }
        public DbSet<EpisodeListener> EpisodeListeners { get; set; }
        public DbSet<SavedPlaylist> SavedPlaylists {  get; set; }
        public DbSet<UserEpisodeProgress> UserEpisodeProgresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Episode>()
                .HasOne(e => e.Podcast)
                .WithMany(p => p.Episodes)
                .HasForeignKey(e => e.PodcastId)
                .OnDelete(DeleteBehavior.Restrict);

            
        }




    }
}
