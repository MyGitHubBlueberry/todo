namespace Server.Core.Dtos.Task;

using System;
using Server.Core;

public class TaskResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Body { get; set; }
    public Status Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsEdited { get; set; }
    public List<CategoryResponseDto>? Categories { get; set; }
}
