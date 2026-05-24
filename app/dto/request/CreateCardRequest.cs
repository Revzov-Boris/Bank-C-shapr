namespace bank.net.dto.request;

public record CreateCardRequest
{
    public Guid UserId {get; set;}
    public string CardNumber {get; set;} = string.Empty;
    public decimal InitialBalance {get; set;}
}