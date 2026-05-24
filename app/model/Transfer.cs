namespace bank.net.model;

public class Transfer
{
    public Guid Id { get; set; }
    public Guid SourceCardId { get; set; }
    public Guid TargetCardId { get; set; }
    public decimal Amount { get; set; }
    public DateTime TimestampUtc { get; set; }
}