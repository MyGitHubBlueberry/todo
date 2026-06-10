namespace Server.Core.Dtos.Task;

public record TaskCreateDto(
    string title,
    string? body,
    int[] categoryIds
);
