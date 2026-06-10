namespace Server.Core.Dtos.Task;

using System.Collections.Generic;
using Server.Core;

public record TaskUpdateDto(int id, string title, string? body, Status status, HashSet<int> categoryIds);
