using HW_57.Models;

namespace HW_57.ViewModels;

public class ToDoIndexViewModel
{
    public List<ToDoTask> Tasks { get; set; } = new List<ToDoTask>();
    public PageViewModel PageViewModel { get; set; } = null!;
}