using Microsoft.AspNetCore.Identity;

namespace HW_57.Models;

public class User : IdentityUser
{
    public List<ToDoTask> CreatedTasks { get; set; } = new List<ToDoTask>();

    public List<ToDoTask> ExecutedTasks { get; set; } = new List<ToDoTask>();
}