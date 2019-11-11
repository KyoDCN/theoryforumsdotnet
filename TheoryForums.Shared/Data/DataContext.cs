using Microsoft.EntityFrameworkCore;
using TheoryForums.Shared.Models;

namespace TheoryForums.Shared.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) {}

        public DbSet<Forum> Forums { get; set; }
        public DbSet<Subforum> Subforums { get; set; }
        public DbSet<Thread> Threads { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Forum>(x =>
            {
                x.HasKey(e => e.Id);
            });

            builder.Entity<Subforum>(x =>
            {
                x.HasKey(e => e.Id);
                x.HasOne(e => e.Forum)
                    .WithMany(e => e.Subforums)
                    .HasForeignKey(e => e.ForumFK)
                    .IsRequired();
            });

            builder.Entity<Thread>(x =>
            {
                x.HasKey(e => e.Id);
                x.HasOne(e => e.Subforum)
                    .WithMany(e => e.Threads)
                    .HasForeignKey(e => e.SubforumFK)
                    .IsRequired();
            });

            builder.Entity<Post>(x =>
            {
                x.HasKey(e => e.Id);
                x.HasOne(e => e.Thread)
                    .WithMany(e => e.Posts)
                    .HasForeignKey(e => e.ThreadFK)
                    .IsRequired();
            });
        }
    }
}
