using bank.net.dto.request;
using bank.net.model;

namespace bank.net.interfaces;

public interface ITransferService
{
    Task<Transfer> ProcessTransferAsync(CreateTransferRequest request);
    Task<Transfer?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Transfer>> GetAllAsync();
    Task<string> GetReceiptAsync(Guid id);
}