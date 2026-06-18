namespace Server.Core.Dtos.Task;

public record TaskGetQueryDto(
        int page,
        int pageSize,
        int[]? categoryIds = null,
        Core.Status? selectedStatus = null,
        string? searchTerm = null);
