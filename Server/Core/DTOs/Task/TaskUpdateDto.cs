namespace Server.Core.Dtos.Task;

using System.Collections.Generic;

public record TaskUpdateDto(string title, string? body, HashSet<int> categoryIds);
