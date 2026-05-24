namespace bank.net.model;

public class Card
{
    public Guid Id { get; set; }
    public string CardNumber { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public bool IsBlocked { get; set; }
    public Guid UserId { get; set; }
}