namespace Server.Core.Dtos.Task;

using Server.Core.Dtos.Category;

using System;
using Server.Core;

public record TaskResponseDto(
    int Id,
    string Title,
    string? Body,
    Status Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool IsEdited,
    CategoryResponseDto[] Categories
);
