namespace Server.Core.Models;

using Server.Core;
using System;
using System.Collections.Generic;

public class Task
{
    public int Id { get; set; }

    public required string Title { get; set; }
    public string? Body { get; set; }
    public Status Status { get; set; } = Status.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsEdited { get; set; } = false;

    public User? User { get; set; }
    public int UserId { get; set; }

    public List<Category> Categories { get; set; } = new();
}
