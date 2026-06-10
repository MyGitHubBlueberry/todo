namespace Server.Core.Interfaces;

using System.Collections.Generic;
using System.Threading.Tasks;
using TaskModel = Server.Core.Models.Task;

public interface ITaskService
{
    Task<(IEnumerable<TaskModel> Tasks, int TotalCount)> GetTasksAsync(
        int userId,
        int page,
        int pageSize,
        int? categoryId = null,
        string? searchTerm = null);

    Task<TaskModel> CreateTaskAsync(TaskModel task);
    Task<TaskModel?> GetTaskByIdAsync(int taskId, int userId);
    Task<TaskModel?> UpdateTaskAsync(int taskId, int userId, TaskModel updatedTask);
    Task<bool> DeleteTaskAsync(int taskId, int userId);
}
