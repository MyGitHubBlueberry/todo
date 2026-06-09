namespace core.models;

public class Category
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string Name { get; set; }
}
