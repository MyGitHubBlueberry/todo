namespace Server.Services;

using TaskModel = Server.Core.Models.Task;
using Server.Core.Dtos.Task;
using Server.Core.Interfaces;
using Server.Data;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Server.Core.Dtos.Category;
using Server.Core.Models;
using System.Linq;

public class TaskService(AppDbContext db) : ITaskService
{
    public async Task<TaskResponseDto> CreateTaskAsync(int userId, TaskCreateDto taskDto)
    {
        if (await db.Tasks.AnyAsync(t => t.UserId == userId && t.Title == taskDto.title))
            throw new InvalidDataException("Task with this title already exists");
        var task = new TaskModel
        {
            Title = taskDto.title,
            Body = taskDto.body,
            UserId = userId,
        };

        //transaction
        var categories = new CategoryResponseDto[taskDto.categoryIds.Length];
        await Parallel.ForAsync(0, taskDto.categoryIds.Length, async (i, ct) =>
        {
            var id = taskDto.categoryIds[i];
            Category? category = await db.Categories.FindAsync(new { id, userId });
            if (category is null)
                throw new InvalidDataException("One of the task categories doesn't exist.");
            category.Tasks.Add(task);
            categories[i] = new CategoryResponseDto(category.Id, category.Name);
        });
        // end transaction

        await db.Tasks.AddAsync(task);
        await db.SaveChangesAsync();
        return new TaskResponseDto(
            task.Id,
            task.Title,
            task.Body,
            task.Status,
            task.CreatedAt,
            task.UpdatedAt,
            task.IsEdited,
            categories
        );
    }

    public async Task<bool> DeleteTaskAsync(int taskId, int userId) =>
        (await db.Tasks
            .Where(t => t.UserId == userId && t.Id == taskId)
            .ExecuteDeleteAsync()) != 0;

    public async Task<TaskResponseDto?> GetTaskByIdAsync(int taskId, int userId)
    {
        var task = await db.Tasks
            .Include(t => t.Categories)
            .FirstOrDefaultAsync(t => t.UserId == userId && t.Id == taskId);

        if (task is null) return null;

        var categories = new CategoryResponseDto[task.Categories.Count];
        for (int i = 0; i < categories.Length; i++) {
            categories[i] = new CategoryResponseDto
                (task.Categories[i].Id, task.Categories[i].Name);
        }

        return new TaskResponseDto(
            task.Id,
            task.Title,
            task.Body,
            task.Status,
            task.CreatedAt,
            task.UpdatedAt,
            task.IsEdited,
            categories
        );
    }

    public Task<(IEnumerable<TaskResponseDto> Tasks, int TotalCount)> GetTasksAsync(int userId, int page, int pageSize, int? categoryId = null, string? searchTerm = null)
    {
        throw new System.NotImplementedException();
    }

    public Task<TaskResponseDto?> UpdateTaskAsync(int userId, TaskUpdateDto taskDto)
    {
        throw new System.NotImplementedException();
    }
}
