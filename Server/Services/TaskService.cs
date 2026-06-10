namespace Server.Services;

using Server.Core.Interfaces;
using Server.Data;

using System.Collections.Generic;
using System.Threading.Tasks;

public class TaskService(AppDbContext db) : ITaskService
{
    public Task<Core.Models.Task> CreateTaskAsync(Core.Models.Task task)
    {
        throw new System.NotImplementedException();
    }

    public Task<bool> DeleteTaskAsync(int taskId, int userId)
    {
        throw new System.NotImplementedException();
    }

    public Task<Core.Models.Task?> GetTaskByIdAsync(int taskId, int userId)
    {
        throw new System.NotImplementedException();
    }

    public Task<(IEnumerable<Core.Models.Task> Tasks, int TotalCount)> GetTasksAsync(int userId, int page, int pageSize, int? categoryId = null, string? searchTerm = null)
    {
        throw new System.NotImplementedException();
    }

    public Task<Core.Models.Task?> UpdateTaskAsync(int taskId, int userId, Core.Models.Task updatedTask)
    {
        throw new System.NotImplementedException();
    }
}
