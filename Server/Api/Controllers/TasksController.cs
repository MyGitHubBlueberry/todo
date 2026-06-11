namespace Server.Api.Controllers;

using Server.Core.Dtos.Task;
using Server.Core.Interfaces;

using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Server.Core.Extensions;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TasksController(ILogger<TasksController> logger, ITaskService service)
    : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<TaskResponseDto>> Create([FromBody] TaskCreateDto dto)
    {
        if (!User.TryGetUserId(out int userId))
        {
            return Unauthorized(new { message = "Invalid token payload." });
        }

        try
        {
            return Ok(await service.CreateTaskAsync(userId, dto));
        }
        catch (InvalidDataException ex)
        {
            logger.LogError(ex, "Error creating task with title '{Title}': {Message}.",
                    dto.title, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        if (!User.TryGetUserId(out int userId))
        {
            return Unauthorized(new { message = "Invalid token payload." });
        }

        return await service.DeleteTaskAsync(id, userId)
            ? NoContent()
            : BadRequest(new
            {
                message = "Failed to delete task: task doesn't exist."
            });

    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskResponseDto>> GetById(int id)
    {
        if (!User.TryGetUserId(out int userId))
        {
            return Unauthorized(new { message = "Invalid token payload." });
        }
        var task = await service.GetTaskByIdAsync(id, userId);
        return task is null
            ? NotFound(new { message = "Task not found." })
            : Ok(task);
    }

    [HttpGet]
    public async Task<ActionResult> GetTasks(
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? categoryId = null,
            [FromQuery] string? searchTerm = null)
    {
        if (!User.TryGetUserId(out int userId))
        {
            return Unauthorized(new { message = "Invalid token payload." });
        }
        var (tasks, totalCount) = await service
            .GetTasksAsync(userId, page, pageSize, categoryId, searchTerm);

        return Ok(new
        {
            tasks = tasks,
            totalCount = totalCount
        });
    }

    [HttpPut]
    public async Task<ActionResult<TaskResponseDto>> Update([FromBody] TaskUpdateDto dto)
    {
        if (!User.TryGetUserId(out int userId))
        {
            return Unauthorized(new { message = "Invalid token payload." });
        }

        try
        {
            var task = await service.UpdateTaskAsync(userId, dto);

            return task is null
                ? NotFound(new { message = "Task not found." })
                : Ok(task);
        }
        catch (InvalidDataException ex)
        {
            logger.LogWarning("Failed to update task: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
    }
}
