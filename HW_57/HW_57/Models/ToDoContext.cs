using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HW_57.Models;

public class ToDoContext : IdentityDbContext<User>
{
    public ToDoContext(DbContextOptions<ToDoContext> options) : base(options)
    {
    }

    public DbSet<ToDoTask> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ToDoTask>()
            .HasOne(task => task.Creator)
            .WithMany(user => user.CreatedTasks)
            .HasForeignKey(task => task.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ToDoTask>()
            .HasOne(task => task.Executor)
            .WithMany(user => user.ExecutedTasks)
            .HasForeignKey(task => task.ExecutorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}