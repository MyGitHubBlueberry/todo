namespace Server.Core.Dtos.Task;

using Server.Core.Dtos.Category;

using System;
using Server.Core;

public record TaskResponseDto(
    int id,
    string title,
    string? body,
    Status status,
    DateTime createdAt,
    DateTime? updatedAt,
    bool isEdited,
    CategoryResponseDto[] categories
);
