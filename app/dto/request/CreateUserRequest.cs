namespace bank.net.dto.request;

public record CreateUserRequest
{
    public string FullName {get; set;}
    public string Email {get; set;}
};
