namespace bank.net.dto.request;

public record CreateTransferRequest 
{
    public Guid SourceCardId {get; set;}
    public Guid TargetCardId {get; set;}
    public decimal Amount {get; set;}
}