using Microsoft.EntityFrameworkCore;
using Warehouse.Notifications.Domain.Models;

namespace Warehouse.Notifications.Infrastructure.Persistence;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
        : base(options)
    {
    }

    public DbSet<NotificationRecord> Notifications => Set<NotificationRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NotificationRecord>(entity =>
        {
            entity.ToTable("Notifications");

            entity.HasKey(notification => notification.Id);

            entity.Property(notification => notification.EventId)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(notification => notification.EventId)
                .IsUnique();

            entity.Property(notification => notification.Type)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(notification => notification.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(notification => notification.Message)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(notification => notification.Severity)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(notification => notification.Status)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(notification => notification.RelatedEntityId)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(notification => notification.RelatedEntityType)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(notification => notification.CreatedAtUtc)
                .IsRequired();
        });
    }
}


