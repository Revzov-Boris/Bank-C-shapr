namespace bank.net.dto.response;

public record UserResponse 
{
    public Guid Id{get; set;}
    public string FullName {get; set;} = string.Empty;
    public string Email {get; set;}= string.Empty;
}