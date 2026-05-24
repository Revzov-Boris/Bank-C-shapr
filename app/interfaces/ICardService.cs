using bank.net.dto.request;
using bank.net.dto.response;

namespace bank.net.interfaces;

public interface ICardService
{
    Task<CardResponse?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<CardResponse>> GetAllAsync();
    Task<CardResponse> CreateAsync(CreateCardRequest request);
    Task<CardResponse> BlockCard(Guid id);
    Task<CardResponse> UnBlockCard(Guid id);
    Task<CardResponse> DeleteAsync(Guid id);
}