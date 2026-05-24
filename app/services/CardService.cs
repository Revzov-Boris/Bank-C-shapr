using Microsoft.EntityFrameworkCore;
using bank.net.database;
using bank.net.dto.request;
using bank.net.dto.response;
using bank.net.interfaces;
using bank.net.model;

namespace bank.net.services;

/// <summary>
/// Сервис управления банковскими картами через EF Core.
/// </summary>
public class CardService(BankDbContext db) : ICardService
{
    public async Task<IReadOnlyList<Card>> GetAllAsync()
        => await db.Cards
            .AsNoTracking()
            .OrderBy(x => x.CardNumber)
            .ToListAsync();

    public async Task<Card?> GetByIdAsync(Guid id)
        => await db.Cards
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Card> CreateAsync(CreateCardRequest request)
    {
        ValidateCardFields(request.CardNumber, request.InitialBalance);

        var userExists = await db.Users.AnyAsync(u => u.Id == request.UserId);
        if (!userExists)
        {
            throw new InvalidOperationException($"Нельзя создать карту: владелец с ID {request.UserId} не существует.");
        }

        var entity = new Card
        {
            UserId = request.UserId,
            CardNumber = request.CardNumber.Trim(),
            Balance = request.InitialBalance,
            IsBlocked = false
        };

        db.Cards.Add(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<Card> BlockCard(Guid id)
    {
        var entity = await db.Cards.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            throw new InvalidOperationException("Карта не найдена.");
        }

        entity.IsBlocked = true;
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<Card> UnBlockCard(Guid id)
    {
        var entity = await db.Cards.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            throw new InvalidOperationException("Карта не найдена.");
        }

        entity.IsBlocked = false;
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<Card> DeleteAsync(Guid id)
    {
        var entity = await db.Cards.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            throw new InvalidOperationException("Карта не найдена.");
        }

        if (entity.Balance > 0)
        {
            throw new InvalidOperationException("Нельзя удалить карту, на которой остались средства. Сначала выведите баланс.");
        }

        db.Cards.Remove(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    private static void ValidateCardFields(string cardNumber, decimal balance)
    {
        if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length != 16 || !cardNumber.All(char.IsDigit))
        {
            throw new ArgumentException("Номер карты должен состять из 16 цифр");
        }

        if (balance < 0)
        {
            throw new ArgumentException("Начальный баланс не может быть отрицательным.");
        }
    }
}