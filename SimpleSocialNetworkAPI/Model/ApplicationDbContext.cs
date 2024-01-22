using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SimpleSocialNetworkAPI.Model
{

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Friendship>()
                .HasOne(x => x.Sender)
                .WithMany(x => x.SentFriendRequests)
                .HasForeignKey(x => x.SenderId);

            modelBuilder.Entity<Friendship>()
                .HasOne(x => x.Reciver)
                .WithMany(x => x.ReceivedFriendRequests)
                .HasForeignKey(x => x.ReciverId);
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
    }
}
