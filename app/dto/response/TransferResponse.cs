namespace bank.net.dto.response;

public record TransferResponse 
{
    public Guid Id { get; init; }
    public Guid SourceCardId { get; init; }
    public Guid TargetCardId { get; init; }
    public decimal Amount { get; init; }
    public DateTime TimestampUtc { get; init; }
}
