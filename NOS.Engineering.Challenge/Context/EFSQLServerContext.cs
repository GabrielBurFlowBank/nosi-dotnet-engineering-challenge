using Microsoft.EntityFrameworkCore;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Context;

public class EFSQLServerContext : DbContext
{
    public DbSet<Content> Contents { get; set; }

    public EFSQLServerContext(DbContextOptions<EFSQLServerContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Content>().HasKey(e => e.Id);

        modelBuilder.Entity<Content>().Property(x => x.Title);
        modelBuilder.Entity<Content>().Property(x => x.SubTitle);
        modelBuilder.Entity<Content>().Property(x => x.Description);
        modelBuilder.Entity<Content>().Property(x => x.Duration);
        modelBuilder.Entity<Content>().Property(x => x.ImageUrl);
        modelBuilder.Entity<Content>().Property(x => x.StartTime);
        modelBuilder.Entity<Content>().Property(x => x.EndTime);

        modelBuilder.Entity<Content>()
            .Property(e => e.GenreList)
            .HasConversion(
                g => string.Join(',', g),
                g => g.Split(',', StringSplitOptions.None));
    }
}
