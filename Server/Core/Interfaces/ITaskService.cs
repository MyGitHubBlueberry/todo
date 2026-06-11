namespace Server.Core.Interfaces;

using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Core.Dtos.Task;

public interface ITaskService
{
    Task<(IEnumerable<TaskResponseDto> Tasks, int TotalCount)> GetTasksAsync(
        int userId,
        int page,
        int pageSize,
        int? categoryId = null,
        string? searchTerm = null);

    Task<TaskResponseDto> CreateTaskAsync(int userId, TaskCreateDto taskDto);
    Task<TaskResponseDto?> GetTaskByIdAsync(int taskId, int userId);
    Task<TaskResponseDto?> UpdateTaskAsync(int userId, TaskUpdateDto taskDto);
    Task<bool> DeleteTaskAsync(int taskId, int userId);
}
