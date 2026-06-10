using System.Collections.Generic;

namespace Server.Core.Models;

public class Category
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public int UserId { get; set; }
    public required User User { get; set; }

    public ICollection<Task>? Tasks { get; set; }
}
