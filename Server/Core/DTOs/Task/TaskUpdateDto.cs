namespace Server.Core.Dtos.Task;

using System.Collections.Generic;

public record TaskUpdateDto(int id, string title, string? body, HashSet<int> categoryIds);
