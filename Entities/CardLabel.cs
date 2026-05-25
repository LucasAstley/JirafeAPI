namespace JirafeAPI.Entities;

public class CardLabel
{
    public int Id { get; set; }
    public int CardId { get; set; }
    public int LabelId { get; set; }

    public virtual Card Card { get; set; } = null!;
    public virtual Label Label { get; set; } = null!;
}

