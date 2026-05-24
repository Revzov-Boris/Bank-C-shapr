using rut_shop.net.dto;
using rut_shop.net.dto.request;
using rut_shop.net.dto.response;
using rut_shop.net.interfaces;

namespace rut_shop.net.api;

public static class CustomersEndpoints
{
    /// <summary>
    /// Группа endpoint-ов для работы с клиентами и программой лояльности.
    /// </summary>
    public static RouteGroupBuilder MapCustomersEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/customers").WithTags("Customers");

        group.MapGet("/", async (ICustomerService customers, IMapper mapper) =>
            {
                var result = await customers.GetAllAsync();
                return Results.Ok(result.Select(mapper.Map));
            })
            .WithSummary("Получить список клиентов")
            .WithDescription("Возвращает всех зарегистрированных клиентов и их текущие бонусные баллы.")
            .Produces<IEnumerable<CustomerResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{customerId:guid}/loyalty",
                async (Guid customerId, ICustomerService customers, IMapper mapper) =>
            {
                var customer = await customers.GetByIdAsync(customerId);
                return customer is null
                    ? Results.NotFound(new ErrorResponse { Message = "Клиент не найден." })
                    : Results.Ok(mapper.MapLoyalty(customer));
            })
            .WithSummary("Получить бонусный баланс клиента")
            .WithDescription("Возвращает количество накопленных бонусов по идентификатору клиента.")
            .Produces<CustomerLoyaltyResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapGet("/{customerId:guid}",
                async (Guid customerId, ICustomerService customers, IMapper mapper) =>
            {
                var customer = await customers.GetByIdAsync(customerId);
                return customer is null
                    ? Results.NotFound(new ErrorResponse { Message = "Клиент не найден." })
                    : Results.Ok(mapper.Map(customer));
            })
            .WithSummary("Получить клиента по идентификатору")
            .Produces<CustomerResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/", async (CreateCustomerRequest body, ICustomerService customers, IMapper mapper) =>
            {
                try
                {
                    var created = await customers.AddAsync(body);
                    return Results.Created($"/api/customers/{created.Id}", mapper.Map(created));
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Добавить клиента")
            .Produces<CustomerResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("/{customerId:guid}",
                async (Guid customerId, UpdateCustomerRequest body, ICustomerService customers, IMapper mapper) =>
            {
                try
                {
                    var updated = await customers.UpdateAsync(customerId, body);
                    return updated is null
                        ? Results.NotFound(new ErrorResponse { Message = "Клиент не найден." })
                        : Results.Ok(mapper.Map(updated));
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Изменить клиента")
            .Produces<CustomerResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapDelete("/{customerId:guid}",
                async (Guid customerId, ICustomerService customers) =>
            {
                try
                {
                    var deleted = await customers.DeleteAsync(customerId);
                    return deleted
                        ? Results.NoContent()
                        : Results.NotFound(new ErrorResponse { Message = "Клиент не найден." });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Удалить клиента")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        return api;
    }
}
