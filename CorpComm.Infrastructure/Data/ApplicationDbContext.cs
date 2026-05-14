using CorpComm.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CorpComm.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Meeting> Meetings => Set<Meeting>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Fluent API конфігурація (замість атрибутів у Domain моделях)
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(e => e.Email).IsUnique(); // Email має бути унікальним
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Meeting>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
        });

        base.OnModelCreating(modelBuilder);
    }
}