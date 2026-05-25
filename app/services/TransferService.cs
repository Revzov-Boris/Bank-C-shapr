using Microsoft.EntityFrameworkCore;
using bank.net.database;
using bank.net.dto.request;
using bank.net.dto.response;
using bank.net.interfaces;
using bank.net.model;

namespace bank.net.services;

/// <summary>
/// Сервис проведения транзакций и переводов между картами.
/// </summary>
public class TransferService(BankDbContext db) : ITransferService
{
    public async Task<IReadOnlyList<Transfer>> GetAllAsync()
        => await db.Transfers
            .AsNoTracking()
            .OrderByDescending(x => x.TimestampUtc)
            .ToListAsync();

    public async Task<Transfer?> GetByIdAsync(Guid id)
        => await db.Transfers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Transfer> ProcessTransferAsync(CreateTransferRequest request)
    {
        if (request.Amount <= 0)
        {
            throw new ArgumentException("Сумма перевода должна быть больше нуля.");
        }

        if (request.SourceCardId == request.TargetCardId)
        {
            throw new InvalidOperationException("Карта отправителя и получателя не могут совпадать.");
        }

        // Открываем транзакцию
        await using var transaction = await db.Database.BeginTransactionAsync();

        var sourceCard = await db.Cards.FirstOrDefaultAsync(c => c.Id == request.SourceCardId);
        var targetCard = await db.Cards.FirstOrDefaultAsync(c => c.Id == request.TargetCardId);

        if (sourceCard is null) throw new InvalidOperationException("Карта отправителя не найдена.");
        if (targetCard is null) throw new InvalidOperationException("Карта получателя не найдена.");

        if (sourceCard.IsBlocked) throw new InvalidOperationException("Операция заблокирована: карта отправителя заблокирована.");
        if (targetCard.IsBlocked) throw new InvalidOperationException("Операция заблокирована: карта получателя заблокирована.");

        if (sourceCard.Balance < request.Amount)
        {
            throw new InvalidOperationException("Недостаточно средств на карте отправителя.");
        }
        decimal maxBalance = 1000000000;
        if (targetCard.Balance + request.Amount > maxBalance)
        {
            throw new InvalidOperationException("На карте отправителя сумма будет превышать максимальное значение, операция отклонена");
        }
        

        // Изменение связанных сущностей
        sourceCard.Balance -= request.Amount;
        targetCard.Balance += request.Amount;

        var transfer = new Transfer
        {
            Id = Guid.NewGuid(),
            SourceCardId = sourceCard.Id,
            TargetCardId = targetCard.Id,
            Amount = request.Amount,
            TimestampUtc = DateTime.UtcNow
        };

        db.Transfers.Add(transfer);
        await db.SaveChangesAsync();
        await transaction.CommitAsync();

        return transfer;
    }

    public async Task<string> GetReceiptAsync(Guid id)
    {
        var transfer = await db.Transfers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (transfer is null)
        {
            throw new InvalidOperationException("Перевод не найден.");
        }

        string userFullName = "Нет имени";
        var sourceCard = await db.Cards.AsNoTracking().FirstOrDefaultAsync(c => c.Id == transfer.SourceCardId);
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == sourceCard.UserId);
            if (user is not null)
            {
                userFullName = user.FullName;
            }
        return $"""
        RUT BANK.NET
        Чек
        ----------------------------------------
        Транзакция: {transfer.Id}
        Дата: {transfer.TimestampUtc:yyyy-MM-dd HH:mm:ss}
        Отправитель: {userFullName} ({sourceCard?.UserId})
        ----------------------------------------
        ID карты отправителя: {sourceCard?.Id}
        ID карты получателя: {transfer.TargetCardId}
        ----------------------------------------
        СУММА: {transfer.Amount} RUB
        Статус: Проведено успешно
        """;
    }
}