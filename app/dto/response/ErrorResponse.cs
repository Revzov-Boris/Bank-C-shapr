namespace bank.net.dto.response;

/// <summary>
/// DTO стандартной ошибки API.
/// </summary>
public record ErrorResponse
{
    public string Message { get; init; } = string.Empty;
}
