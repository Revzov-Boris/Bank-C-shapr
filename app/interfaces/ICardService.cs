using bank.net.dto.request;
using bank.net.model;

namespace bank.net.interfaces;

public interface ICardService
{
    Task<Card?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Card>> GetAllAsync();
    Task<Card> CreateAsync(CreateCardRequest request);
    Task<Card> BlockCard(Guid id);
    Task<Card> UnBlockCard(Guid id);
    Task<Card> DeleteAsync(Guid id);
}