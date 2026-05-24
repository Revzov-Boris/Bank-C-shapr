using bank.net.dto;
using bank.net.dto.request;
using bank.net.dto.response;
using bank.net.interfaces;
using Microsoft.EntityFrameworkCore;

namespace bank.net.api;

public static class UsersEndpoints
{
    public static RouteGroupBuilder MapUsersEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/users").WithTags("Users");

        group.MapGet("/", async (IUserService users, IMapper mapper) =>
            {
                var result = await users.GetAllAsync();
                return Results.Ok(result.Select(mapper.Map));
            })
            .WithSummary("Получить список пользователей")
            .Produces<IEnumerable<UserResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", async (Guid id, IUserService users, IMapper mapper) =>
            {
                var user = await users.GetByIdAsync(id);
                return user is null
                    ? Results.NotFound(new ErrorResponse { Message = "Клиент не найден." })
                    : Results.Ok(mapper.Map(user));
            })
            .WithSummary("Получить пользователя по ID")
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/", async (CreateUserRequest body, IUserService users, IMapper mapper) =>
            {
                try
                {
                    var created = await users.CreateAsync(body);
                    return Results.Created($"/api/users/{created.Id}", mapper.Map(created));
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
                catch (DbUpdateException ex)
                {
                    return Results.BadRequest(new ErrorResponse { Message = "Некорректыне данные (такой email уже существует) " + ex.Message });
                }
            })
            .WithSummary("Создать пользователя")
            .Produces<UserResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:guid}", async (Guid id, CreateUserRequest body, IUserService users, IMapper mapper) =>
            {
                try
                {
                    var updated = await users.UpdateAsync(id, body);
                    return Results.Ok(mapper.Map(updated));
                }
                catch (InvalidOperationException ex)
                {
                    return Results.NotFound(new ErrorResponse { Message = ex.Message });
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Обновить профиль пользователя")
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", async (Guid id, IUserService users, IMapper mapper) =>
            {
                try
                {
                    var deleted = await users.DeleteAsync(id);
                    return Results.Ok(mapper.Map(deleted));
                }
                catch (InvalidOperationException ex)
                {
                    // Различаем "Не найден" и "Бизнес-ошибка удаления" по тексту ошибки
                    return ex.Message.Contains("не найден")
                        ? Results.NotFound(new ErrorResponse { Message = ex.Message })
                        : Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Удалить пользователя")
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        return api;
    }
}