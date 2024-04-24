namespace Farola.Database.Models;

public partial class Review
{
    public int Id { get; set; }

    public int StatementId { get; set; }

    public float Grade { get; set; }

    public string? Text { get; set; }
}
