using bank.net.dto.response;
using bank.net.model;

namespace bank.net.dto;

/// <summary>
/// Реализация маппера вручную для обеспечения максимальной производительности.
/// </summary>
public class Mapper : IMapper
{
    public UserResponse Map(User user)
    {
        if (user is null) throw new ArgumentNullException(nameof(user));

        return new UserResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email
        };
    }

    public CardResponse Map(Card card)
    {
        if (card is null) throw new ArgumentNullException(nameof(card));

        return new CardResponse
        {
            Id = card.Id,
            CardNumber = card.CardNumber,
            Balance = card.Balance,
            IsBlocked = card.IsBlocked,
            UserId = card.UserId
        };
    }

    public TransferResponse Map(Transfer transfer)
    {
        if (transfer is null) throw new ArgumentNullException(nameof(transfer));

        return new TransferResponse
        {
            Id = transfer.Id,
            SourceCardId = transfer.SourceCardId,
            TargetCardId = transfer.TargetCardId,
            Amount = transfer.Amount,
            TimestampUtc = transfer.TimestampUtc
        };
    }
}