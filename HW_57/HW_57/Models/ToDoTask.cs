using System.ComponentModel.DataAnnotations;
using HW_57.Models.Enums;

namespace HW_57.Models;

public class ToDoTask
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be at least 3")]
    public string Title { get; set; } = "";

    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; } = "";

    public string? ResponsableName { get; set; }

    [Required(ErrorMessage = "Choose priority")]
    public TaskPriority Priority { get; set; }

    public TaskState State { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public DateTime? ClosedOn { get; set; }

    public string CreatorId { get; set; } = "";

    public User? Creator { get; set; }

    public string? ExecutorId { get; set; }

    public User? Executor { get; set; }
}