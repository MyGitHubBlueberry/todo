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
using System.Linq;
using System;

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

        var categories = new CategoryResponseDto[taskDto.categoryIds.Length];
        for (int i = 0; i < categories.Length; i++)
        {
            var id = taskDto.categoryIds[i];
            var category = await db.Categories
                .SingleOrDefaultAsync(c => c.Id == id && c.UserId == userId)
                    ?? throw new InvalidDataException("Category doesn't exist or doesn't belong to the user.");

            task.Categories.Add(category);
            categories[i] = new CategoryResponseDto(category.Id, category.Name);
        }

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

        return ToTaskResponseDto(task);
    }

    public async Task<(IEnumerable<TaskResponseDto> Tasks, int TotalCount)>
        GetTasksAsync(int userId, int page, int pageSize, int? categoryId = null, string? searchTerm = null)
    {
        var tasks = db.Tasks
            .Include(t => t.Categories)
            .Where(t => t.UserId == userId)
            .Where(t => categoryId == null || t.Categories.Any(c => c.Id == categoryId))
            .Where(t => searchTerm == null || (t.Title.Contains(searchTerm)
                     || (t.Body != null && t.Body.Contains(searchTerm))
                     || t.Categories.Any(c => c.Name.Contains(searchTerm))));

        var totalCount = await tasks.CountAsync();

        page = Math.Clamp(page, 0, Math.Max(0, (int)Math.Floor(totalCount / (double)pageSize)));

        var dbTasks = await tasks
            .OrderBy(t => t.Id)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = dbTasks.Select(task => ToTaskResponseDto(task));

        return (dtos, totalCount);
    }

    public async Task<TaskResponseDto?> UpdateTaskAsync(int userId, TaskUpdateDto taskDto)
    {
        var task = await db.Tasks
            .Include(t => t.Categories)
            .FirstOrDefaultAsync(t => t.UserId == userId && t.Id == taskDto.id);

        if (task is null) return null;
        if ((await db.Tasks.Where(t => t.UserId == userId)
                    .Where(t => t.Id != taskDto.id)
                    .AnyAsync(t => t.Title == taskDto.title)))
            throw new InvalidDataException("Failed to update task: This task title is already taken.");

        task.Title = taskDto.title;
        task.Body = taskDto.body;
        task.Status = taskDto.status;
        task.UpdatedAt = System.DateTime.UtcNow;
        task.IsEdited = true;

        task.Categories.RemoveAll(c => !taskDto.categoryIds.Contains(c.Id));

        var categories = await db.Categories
            .Where(c => c.UserId == userId)
            .Where(c => taskDto.categoryIds.Contains(c.Id))
            .Where(c => !task.Categories.Contains(c))
            .ToListAsync();

        categories.ForEach(c => task.Categories.Add(c));

        await db.SaveChangesAsync();
        return ToTaskResponseDto(task);
    }

    private TaskResponseDto ToTaskResponseDto(Server.Core.Models.Task task)
    {
        var categories = new CategoryResponseDto[task.Categories.Count];
        for (int i = 0; i < categories.Length; i++)
        {
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
}
