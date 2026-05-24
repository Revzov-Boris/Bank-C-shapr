using Microsoft.EntityFrameworkCore;
using bank.net.database;
using bank.net.dto.request;
using bank.net.dto.response;
using bank.net.interfaces;
using bank.net.model;

namespace bank.net.services;

/// <summary>
/// Сервис доступа к пользователям через EF Core.
/// </summary>
public class UserService(BankDbContext db) : IUserService
{
    public async Task<IReadOnlyList<User>> GetAllAsync()
        => await db.Users
            .AsNoTracking()
            .OrderBy(x => x.FullName)
            .ToListAsync();

    public async Task<User?> GetByIdAsync(Guid id)
        => await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<User> CreateAsync(CreateUserRequest request)
    {
        ValidateUserFields(request.FullName, request.Email);
        var entity = new User
        {
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim()
        };

        db.Users.Add(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<User> UpdateAsync(Guid id, CreateUserRequest request)
    {
        
        ValidateUserFields(request.FullName, request.Email);

        var entity = await db.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            throw new InvalidOperationException("Клиент не найден.");
        }

        entity.FullName = request.FullName.Trim();
        entity.Email = request.Email.Trim();
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<User> DeleteAsync(Guid id)
    {
        var entity = await db.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            throw new InvalidOperationException("Клиент не найден.");
        }

        var hasCards = await db.Cards.AnyAsync(c => c.UserId == id);
        if (hasCards)
        {
            throw new InvalidOperationException("Нельзя удалить клиента: к нему привязаны активные карты.");
        }

        db.Users.Remove(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    private static void ValidateUserFields(string fullName, string email)
    {
        System.Console.WriteLine("Валидация " + fullName + " " + email);
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("ФИО пользователя не должно быть пустым.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email не должен быть пустым.");
        }
    }
}