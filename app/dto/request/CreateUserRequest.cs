namespace bank.net.dto.request;

public record CreateUserRequest (string FullName, string Email)
{
    public string FullName {get; set;} = string.Empty;
    public string Email {get; set;} = string.Empty;
};
