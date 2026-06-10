using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using todo_backend.Data;
using todo_backend.Models;

namespace todo_backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TodoController : ControllerBase
{
    private readonly AppDbContext _context;

    public TodoController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetTodos()
    {
        int userId =
            int.Parse(
                User.FindFirst("id")!.Value);

        var todos = _context.TodoItems
            .Where(x => x.UserId == userId)
            .ToList();

        return Ok(todos);
    }

    [HttpPost]
    public IActionResult AddTodo(TodoItem todo)
    {
        int userId =
            int.Parse(
                User.FindFirst("id")!.Value);

        todo.UserId = userId;

        _context.TodoItems.Add(todo);
        _context.SaveChanges();

        return Ok(todo);
    }

    [HttpPut("{id}")]
    public IActionResult ToggleTodo(int id)
    {
        var todo =
            _context.TodoItems.Find(id);

        if (todo == null)
        {
            return NotFound();
        }

        todo.IsCompleted =
            !todo.IsCompleted;

        _context.SaveChanges();

        return Ok(todo);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTodo(int id)
    {
        var todo =
            _context.TodoItems.Find(id);

        if (todo == null)
        {
            return NotFound();
        }

        _context.TodoItems.Remove(todo);
        _context.SaveChanges();

        return Ok();
    }
}