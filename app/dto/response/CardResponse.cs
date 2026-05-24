namespace bank.net.dto.response;

public record CardResponse
{
    public Guid Id { get; init; }
    public decimal Balance { get; init; }
    public bool IsBlocked { get; init; }
    public Guid UserId { get; init; }
    public string CardNumber { get; init; } = string.Empty;
}