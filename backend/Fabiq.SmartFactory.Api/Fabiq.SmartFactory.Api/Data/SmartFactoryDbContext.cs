using Fabiq.SmartFactory.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fabiq.SmartFactory.Api.Data;

public class SmartFactoryDbContext : DbContext
{
    public SmartFactoryDbContext(DbContextOptions<SmartFactoryDbContext> options)
        : base(options)
    {
    }

    public DbSet<Machine> Machines => Set<Machine>();
    public DbSet<MachineStatusEvent> MachineStatusEvents => Set<MachineStatusEvent>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<ProductionEvent> ProductionEvents => Set<ProductionEvent>();
    public DbSet<DowntimeEvent> DowntimeEvents => Set<DowntimeEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Machine>(entity =>
        {
            entity.ToTable("machines");

            entity.HasKey(m => m.Id);

            entity.HasIndex(m => m.MachineId)
                .IsUnique();

            entity.Property(m => m.MachineId)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(m => m.LineId)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(m => m.Status)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(m => m.CurrentWorkOrderId)
                .HasMaxLength(50);
        });

        modelBuilder.Entity<MachineStatusEvent>(entity =>
        {
            entity.ToTable("machine_status_events");

            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.EventId)
                .IsUnique();

            entity.HasIndex(e => e.CreatedAt);

            entity.HasIndex(e => e.MachineId);

            entity.Property(e => e.EventId)
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(e => e.MachineId)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.LineId)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(e => e.CurrentWorkOrderId)
                .HasMaxLength(50);

            entity.Property(e => e.Notes)
                .HasMaxLength(500);
        });

        modelBuilder.Entity<WorkOrder>(entity =>
        {
            entity.ToTable("work_orders");

            entity.HasKey(w => w.Id);

            entity.HasIndex(w => w.WorkOrderId)
                .IsUnique();

            entity.Property(w => w.WorkOrderId)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(w => w.ProductCode)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(w => w.LineId)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(w => w.Status)
                .HasMaxLength(30)
                .IsRequired();
        });

        modelBuilder.Entity<ProductionEvent>(entity =>
        {
            entity.ToTable("production_events");

            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.EventId)
                .IsUnique();

            entity.HasIndex(e => e.CreatedAt);

            entity.Property(e => e.EventId)
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(e => e.MachineId)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.WorkOrderId)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.LineId)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.EventType)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(e => e.PartId)
                .HasMaxLength(80);

            entity.Property(e => e.DefectType)
                .HasMaxLength(80);

            entity.Property(e => e.Notes)
                .HasMaxLength(500);
        });

        modelBuilder.Entity<DowntimeEvent>(entity =>
        {
            entity.ToTable("downtime_events");

            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.EventId)
                .IsUnique();

            entity.HasIndex(e => e.MachineId);
            entity.HasIndex(e => e.LineId);
            entity.HasIndex(e => e.ReasonCode);
            entity.HasIndex(e => e.StartedAt);

            entity.Property(e => e.EventId)
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(e => e.MachineId)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.LineId)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.WorkOrderId)
                .HasMaxLength(50);

            entity.Property(e => e.ReasonCode)
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(e => e.Notes)
                .HasMaxLength(500);
        });
    }
}