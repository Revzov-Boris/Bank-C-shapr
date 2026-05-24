using bank.net.dto.response;
using bank.net.model;

namespace bank.net.dto;

public interface IMapper
{
    UserResponse Map(User user);
    CardResponse Map(Card card);
    TransferResponse Map(Transfer transfer);
}