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
        GetTasksAsync(int userId, TaskGetQueryDto dto)
    {
        var query = db.Tasks
            .Include(t => t.Categories)
            .Where(t => t.UserId == userId)
            .AsQueryable();

        if (dto.selectedStatus.HasValue) {
            query = query.Where(t => t.Status == dto.selectedStatus);
        }

        if (dto.categoryIds != null && dto.categoryIds.Length != 0)
        {
            var categorySet = dto.categoryIds.ToHashSet();
            query = query.Where(t => t.Categories.Any(c => categorySet.Contains(c.Id)));
        }

        if (!string.IsNullOrWhiteSpace(dto.searchTerm))
        {
            query = query.Where(t => t.Title.Contains(dto.searchTerm)
                                  || (t.Body != null && t.Body.Contains(dto.searchTerm))
                                  || t.Categories.Any(c => c.Name.Contains(dto.searchTerm)));
        }

        query = dto.sortBy switch
        {
            SortBy.AlphAsc => query.OrderBy(t => t.Title),
            SortBy.AlphDsc => query.OrderByDescending(t => t.Title),
            SortBy.CrtAsc => query.OrderBy(t => t.CreatedAt),
            SortBy.CrtDsc => query.OrderByDescending(t => t.CreatedAt),
            SortBy.UpdAsc => query.OrderByDescending(t => t.UpdatedAt.HasValue)
                .ThenBy(t => t.UpdatedAt),
            SortBy.UpdDsc => query.OrderByDescending(t => t.UpdatedAt),

            _ => query.OrderByDescending(t => t.Id)
        };

        Console.WriteLine(dto.sortBy);

        var totalCount = await query.CountAsync();

        int maxPage = totalCount == 0 ? 0 : (totalCount - 1) / dto.pageSize;
        int page = Math.Clamp(dto.page, 0, maxPage);

        var dbTasks = await query
            .Skip(page * dto.pageSize)
            .Take(dto.pageSize)
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
