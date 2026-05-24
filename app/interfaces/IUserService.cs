using bank.net.dto.request;
using bank.net.dto.response;
using bank.net.model;

namespace bank.net.interfaces;

public interface IUserService
{
    Task<User?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<User>> GetAllAsync();
    Task<User> CreateAsync(CreateUserRequest request);
    Task<User> UpdateAsync(Guid id, CreateUserRequest request);
    Task<User> DeleteAsync(Guid id);
}