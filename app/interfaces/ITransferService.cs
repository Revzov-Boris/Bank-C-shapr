using bank.net.dto.request;
using bank.net.dto.response;

namespace bank.net.interfaces;

public interface ITransferService
{
    Task<TransferResponse> ProcessTransferAsync(CreateTransferRequest request);
    Task<TransferResponse?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<TransferResponse>> GetAllAsync();
    Task<string> GetReceiptAsync(Guid id);
}