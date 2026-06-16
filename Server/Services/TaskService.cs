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
using Server.Core;

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

        var categories = await db.Categories
                .Where(c => c.UserId == userId && taskDto.categoryIds.Contains(c.Id))
                .ToListAsync();

        if (categories.Count != taskDto.categoryIds.Length)
            throw new InvalidOperationException("One or more categories don't exist or don't belong to the user.");

        task.Categories = categories;

        await db.Tasks.AddAsync(task);
        await db.SaveChangesAsync();

        return ToTaskResponseDto(task);
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
        GetTasksAsync(
            int userId,
            int page,
            int pageSize,
            int[]? categoryIds,
            Core.Status? selectedStatus = null,
            string? searchTerm = null)
    {
        var query = db.Tasks
            .Include(t => t.Categories)
            .Where(t => t.UserId == userId)
            .AsQueryable();

        if (selectedStatus.HasValue) {
            query = query.Where(t => t.Status == selectedStatus);
        }

        if (categoryIds != null && categoryIds.Length != 0)
        {
            var categorySet = categoryIds.ToHashSet();
            query = query.Where(t => t.Categories.Any(c => categorySet.Contains(c.Id)));
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(t => t.Title.Contains(searchTerm)
                                  || (t.Body != null && t.Body.Contains(searchTerm))
                                  || t.Categories.Any(c => c.Name.Contains(searchTerm)));
        }

        var totalCount = await query.CountAsync();

        int maxPage = totalCount == 0 ? 0 : (totalCount - 1) / pageSize;
        page = Math.Clamp(page, 0, maxPage);

        var dbTasks = await query
            .OrderBy(t => t.Id)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (dbTasks.Select(ToTaskResponseDto), totalCount);
    }

    public async Task<TaskResponseDto?> UpdateTaskAsync(int taskId, int userId, TaskUpdateDto taskDto)
    {
        var task = await db.Tasks
            .Include(t => t.Categories)
            .FirstOrDefaultAsync(t => t.UserId == userId && t.Id == taskId);

        if (task is null) return null;
        if ((await db.Tasks.Where(t => t.UserId == userId)
                    .Where(t => t.Id != taskId)
                    .AnyAsync(t => t.Title == taskDto.title)))
            throw new InvalidDataException("Failed to update task: This task title is already taken.");

        task.Title = taskDto.title;
        task.Body = taskDto.body;
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

    public async Task<TaskResponseDto?> UpdateTaskStatusAsync(int taskId, int userId, Status status)
    {
        var task = await db.Tasks
            .Include(t => t.Categories)
            .FirstOrDefaultAsync(t => t.UserId == userId && t.Id == taskId);
        if (task is null) return null;
        task.Status = status;
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
