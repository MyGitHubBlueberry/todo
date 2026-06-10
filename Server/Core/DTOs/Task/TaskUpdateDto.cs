namespace Server.Core.Dtos.Task;

using Server.Core;

public record TaskUpdateDto(int id, string title, string? body, Status status, int[] categoryIds);
