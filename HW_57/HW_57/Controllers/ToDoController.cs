using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using HW_57.Models;
using HW_57.Models.Enums;
using HW_57.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HW_57.Controllers;

public class TodoController : Controller
{
    private readonly ToDoContext _context;
    private readonly UserManager<User> _userManager;
    public TodoController(ToDoContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index(string? title,
        DateTime? createdFrom,
        DateTime? createdTo,
        string? descriptionWords,
        TaskPriority? priority,
        TaskState? state,
        TodoSortState sortOrder = TodoSortState.CreatedOnDescending,
        int page = 1)
    {
        int pageSize = 10;

        IQueryable<ToDoTask> tasks = _context.Tasks
            .Include(task => task.Creator)
            .Include(task => task.Executor);

        if (!string.IsNullOrWhiteSpace(title))
        {
            title = title.Trim();
            tasks = tasks.Where(t => t.Title.ToLower().Contains(title.ToLower()));
        }

        if (createdFrom != null)
        {
            DateTime toDate = DateTime.SpecifyKind(createdTo.Value.Date.AddDays(1), DateTimeKind.Utc);
            tasks = tasks.Where(t => t.CreatedOn >= toDate);
        }
        if (priority != null)
        {
            tasks = tasks.Where(t => t.Priority == priority);
        }

        if (state != null)
        {
            tasks = tasks.Where(t => t.State == state);
        }

        ViewBag.TitleSort = sortOrder == TodoSortState.TitleAscending
            ? TodoSortState.TitleDescending
            : TodoSortState.TitleAscending;
        ViewBag.PrioritySort = sortOrder == TodoSortState.PriorityAscending
            ? TodoSortState.PriorityDescending
            : TodoSortState.PriorityAscending;
        ViewBag.StateSort = sortOrder == TodoSortState.StateAscending
            ? TodoSortState.StateDescending
            : TodoSortState.StateAscending;
        ViewBag.CreatedOnSort = sortOrder == TodoSortState.CreatedOnAscending
            ? TodoSortState.CreatedOnDescending
            : TodoSortState.CreatedOnAscending;

        tasks = sortOrder switch
        {
            TodoSortState.TitleAscending => tasks.OrderBy(t => t.Title),
            TodoSortState.TitleDescending => tasks.OrderByDescending(t => t.Title),

            TodoSortState.PriorityAscending => tasks.OrderBy(t =>
                t.Priority == TaskPriority.High ? 1 :
                t.Priority == TaskPriority.Normal ? 2 : 3),

            TodoSortState.PriorityDescending => tasks.OrderByDescending(t =>
                t.Priority == TaskPriority.High ? 1 :
                t.Priority == TaskPriority.Normal ? 2 : 3),

            TodoSortState.StateAscending => tasks.OrderBy(t => t.State),
            TodoSortState.StateDescending => tasks.OrderByDescending(t => t.State),

            TodoSortState.CreatedOnAscending => tasks.OrderBy(t => t.CreatedOn),
            TodoSortState.CreatedOnDescending => tasks.OrderByDescending(t => t.CreatedOn),

            _ => tasks.OrderByDescending(t => t.CreatedOn)
        };
        
        int count = tasks.Count();

        if (page < 1)
        {
            page = 1;
        }

        int totalPages = (int)Math.Ceiling(count / (double)pageSize);

        if (totalPages > 0 && page > totalPages)
        {
            page = totalPages;
        }

        List<ToDoTask> items = tasks
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ToDoIndexViewModel viewModel = new ToDoIndexViewModel()
        {
            Tasks = items,
            PageViewModel = new PageViewModel(count, page, pageSize)
        };
        ViewBag.FilterTitle = title;
        ViewBag.CreatedFrom = createdFrom?.ToString("dd-MMM-yyyy");
        ViewBag.CreatedTo = createdTo?.ToString("dd-MMM-yyyy");
        ViewBag.DesciptionWords = descriptionWords;
        ViewBag.Priority = priority;
        ViewBag.State = state;
        ViewBag.SortOrder = sortOrder;
        
        ViewBag.Priorities = new SelectList(Enum.GetValues<TaskPriority>(), priority);
        ViewBag.States = new SelectList(Enum.GetValues<TaskState>(), state);
        
        return View(viewModel);
    }

    [HttpGet]
    [Authorize]
    public IActionResult Create()
    {
        ViewBag.Priorities = new SelectList(Enum.GetValues<TaskPriority>());
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public IActionResult Create(ToDoTask task)
    {
        
        string? userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        task.CreatorId = userId;
        task.ExecutorId = null;
        task.ResponsableName = null;
        task.State = TaskState.New;
        task.CreatedOn = DateTime.UtcNow;
        task.ClosedOn = null;
        
        task.Title = task.Title?.Trim() ?? "";
        task.Description = task.Description?.Trim() ?? "";

        if (!ModelState.IsValid)
        {
            ViewBag.Priorities = new SelectList(Enum.GetValues<TaskPriority>(), task.Priority);
            return View(task);
        }

        _context.Tasks.Add(task);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    public IActionResult Open(int id)
    {
        ToDoTask? task = _context.Tasks.FirstOrDefault(t => t.Id == id);

        if (task == null)
        {
            return NotFound();
        }

        if (task.State == TaskState.Closed)
        {
            TempData["Message"] = "Closed task cannot be opened.";
            return RedirectToAction("Index");
        }

        task.State = TaskState.Open;
        _context.SaveChanges();

        return RedirectToAction("Index");
    }
    
    public IActionResult Details(int id)
    {
        ToDoTask? task = _context.Tasks.FirstOrDefault(t => t.Id == id);

        if (task == null)
        {
            return NotFound();
        }

        return View(task);
    }

    public IActionResult Close(int id)
    {
        ToDoTask? task = _context.Tasks.FirstOrDefault(t => t.Id == id);

        if (task == null)
        {
            return NotFound();
        }

        if (task.State != TaskState.Closed)
        {
            task.State = TaskState.Closed;
            task.ClosedOn = DateTime.UtcNow;
            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id)
    {
        ToDoTask? task = _context.Tasks.FirstOrDefault(t => t.Id == id);

        if (task == null)
        {
            return NotFound();
        }

        if (task.State == TaskState.Open)
        {
            TempData["Message"] = "Open task cannot be deleted.";
            return RedirectToAction("Index");
        }

        _context.Tasks.Remove(task);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }
    
}