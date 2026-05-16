using Microsoft.EntityFrameworkCore;
using Zyntra.Domain.Entities;

namespace Zyntra.Data.Context;

public class ZyntraContext(DbContextOptions<ZyntraContext> option) : DbContext(option)
{
    public DbSet<User> User => Set<User>();
    public DbSet<Student> Student => Set<Student>();
    public DbSet<Instructor> Instructor => Set<Instructor>();
    public DbSet<Administrator> Administrator => Set<Administrator>();
    public DbSet<Class> Class => Set<Class>();
    public DbSet<Schedule> Schedule => Set<Schedule>();
    public DbSet<WaitList> WaitList => Set<WaitList>();
    public DbSet<CheckIn> CheckIn => Set<CheckIn>();
    public DbSet<PartnerIntegration> PartnerIntegration => Set<PartnerIntegration>();
    public DbSet<WorkoutSheet> WorkoutSheet => Set<WorkoutSheet>();
    public DbSet<Exercise> Exercise => Set<Exercise>();
    public DbSet<PhysicalAssessment> PhysicalAssessment => Set<PhysicalAssessment>();
    public DbSet<Payment> Payment => Set<Payment>();
    public DbSet<ChatMessage> ChatMessage => Set<ChatMessage>();
    public DbSet<Notification> Notification => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ZyntraContext).Assembly);
    }
}

