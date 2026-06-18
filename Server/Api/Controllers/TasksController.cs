namespace Server.Api.Controllers;

using Server.Core.Dtos.Task;
using Server.Core.Extensions;
using Server.Core.Interfaces;

using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System;

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
            return Unauthorized(new { message = "Invalid token payload." });

        try
        {
            var task = await service.CreateTaskAsync(userId, dto);
            logger.LogInformation("User with id {userId} created task '{Title}' successfuly.",
                    userId, dto.title);
            return Ok(task);
        }
        catch (InvalidDataException ex)
        {
            logger.LogWarning(ex, "Failed to create a task with title '{Title}': {Message}.",
                    dto.title, ex.Message);
            return Conflict(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Error creating task with title '{Title}': {Message}.",
                    dto.title, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        if (!User.TryGetUserId(out int userId))
            return Unauthorized(new { message = "Invalid token payload." });


        var result = await service.DeleteTaskAsync(id, userId);

        if (!result)
        {
            return NotFound(new
            {
                message = "Failed to delete task: task doesn't exist."
            });
        }

        logger.LogInformation("User with id {userId} deleted task with id {taskId} successfuly.",
                userId, id);

        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskResponseDto>> GetById(int id)
    {
        if (!User.TryGetUserId(out int userId))
            return Unauthorized(new { message = "Invalid token payload." });

        var task = await service.GetTaskByIdAsync(id, userId);
        return task is null
            ? NotFound(new { message = "Task not found." })
            : Ok(task);
    }

    [HttpGet]
    public async Task<ActionResult> GetTasks([FromQuery] TaskGetQueryDto dto)
    {
        if (!User.TryGetUserId(out int userId))
            return Unauthorized(new { message = "Invalid token payload." });

        var (tasks, totalCount) = await service
            .GetTasksAsync(userId, dto);

        return Ok(new
        {
            tasks = tasks,
            totalCount = totalCount
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TaskResponseDto>> Update(int id, [FromBody] TaskUpdateDto dto)
    {
        if (!User.TryGetUserId(out int userId))
            return Unauthorized(new { message = "Invalid token payload." });

        try
        {
            var task = await service.UpdateTaskAsync(id, userId, dto);

            if (task is null)
                return NotFound(new { message = "Task not found." });

            logger.LogInformation("User with id {userId} updated task with id {taskId} successfuly.",
                userId, id);

            return Ok(task);
        }
        catch (InvalidDataException ex)
        {
            logger.LogWarning("Failed to update task: {Message}", ex.Message);
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult<TaskResponseDto>> UpdateStatus(int id, [FromBody] TaskStatusUpdateDto dto)
    {
        if (!User.TryGetUserId(out int userId))
            return Unauthorized(new { message = "Invalid token payload." });

        var task = await service.UpdateTaskStatusAsync(id, userId, dto.status);

        if (task is null)
            return NotFound(new { message = "Task not found." });

        logger.LogInformation("User with id {userId} update task status with id {taskId} successfuly.",
                userId, id);

        return Ok(task);
    }
}
