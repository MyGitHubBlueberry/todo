using System.Collections.Generic;

namespace Server.Core.Models;

public class Category
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public List<Task> Tasks { get; set; } = new();
}
