using bank.net.dto;
using bank.net.dto.request;
using bank.net.dto.response;
using bank.net.interfaces;
using Microsoft.EntityFrameworkCore;

namespace bank.net.api;

public static class CardEndpoints
{
    public static RouteGroupBuilder MapCardEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/cards").WithTags("Cards");

        group.MapGet("/", async (ICardService cards, IMapper mapper) =>
            {
                var result = await cards.GetAllAsync();
                return Results.Ok(result.Select(mapper.Map));
            })
            .WithSummary("Список всех карт")
            .Produces<IEnumerable<CardResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", async (Guid id, ICardService cards, IMapper mapper) =>
            {
                var card = await cards.GetByIdAsync(id);
                return card is null
                    ? Results.NotFound(new ErrorResponse { Message = "Карта не найдена." })
                    : Results.Ok(mapper.Map(card));
            })
            .WithSummary("Получить карту по ID")
            .Produces<CardResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/", async (CreateCardRequest body, ICardService cards, IUserService userService, IMapper mapper) =>
            {
                try
                {
                    var owner = await userService.GetByIdAsync(body.UserId);
                    System.Console.WriteLine("Владелец:");

                    System.Console.WriteLine(owner is null);
                    System.Console.WriteLine(owner);

                    if (owner is null)
                    {
                        return Results.NotFound(new ErrorResponse { Message = "Клиент не найден." });
                    }
                    var created = await cards.CreateAsync(body);
                    return Results.Created($"/api/cards/{created.Id}", mapper.Map(created));
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                } catch (DbUpdateException ex)
                {
                    return Results.BadRequest(new ErrorResponse {Message = "Карту невозможн создать (такое номер уже есть): " + ex.Message});
                }
            })
            .WithSummary("Выпустить новую карту")
            .Produces<CardResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapPost("/{id:guid}/block", async (Guid id, ICardService cards, IMapper mapper) =>
            {
                try
                {
                    var card = await cards.BlockCard(id);
                    return Results.Ok(mapper.Map(card));
                }
                catch (InvalidOperationException ex)
                {
                    return Results.NotFound(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Заблокировать карту")
            .Produces<CardResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/{id:guid}/unblock", async (Guid id, ICardService cards, IMapper mapper) =>
            {
                try
                {
                    var card = await cards.UnBlockCard(id);
                    return Results.Ok(mapper.Map(card));
                }
                catch (InvalidOperationException ex)
                {
                    return Results.NotFound(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Разблокировать карту")
            .Produces<CardResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", async (Guid id, ICardService cards, IMapper mapper) =>
            {
                try
                {
                    var deleted = await cards.DeleteAsync(id);
                    return Results.Ok(mapper.Map(deleted));
                }
                catch (InvalidOperationException ex)
                {
                    return ex.Message.Contains("не найдена")
                        ? Results.NotFound(new ErrorResponse { Message = ex.Message })
                        : Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Закрыть/удалить карту")
            .Produces<CardResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        return api;
    }
}