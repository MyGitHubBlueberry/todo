namespace core.models;

public class Task
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Body { get; set; }
    public int UserId { get; set; }
    public int CategoryId { get; set; }
    public Status Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsEdited { get; set; } = false;
}

public enum Status {
    Done,
    Pending
}
