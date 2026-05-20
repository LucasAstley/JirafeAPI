namespace JirafeAPI.Entities;

public class Label
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ColorHex { get; set; } = string.Empty;
    public int BoardId { get; set; }

    public virtual Board Board { get; set; } = null!;
    public virtual ICollection<CardLabel> CardLabels { get; set; } = new List<CardLabel>();
}

