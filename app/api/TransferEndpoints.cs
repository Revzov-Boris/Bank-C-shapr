using bank.net.dto;
using bank.net.dto.request;
using bank.net.dto.response;
using bank.net.interfaces;

namespace bank.net.api;

public static class TransferEndpoints
{
    public static RouteGroupBuilder MapTransferEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/transfers").WithTags("Transfers");

        group.MapGet("/", async (ITransferService transfers, IMapper mapper) =>
            {
                var result = await transfers.GetAllAsync();
                return Results.Ok(result.Select(mapper.Map));
            })
            .WithSummary("История всех транзакций")
            .Produces<IEnumerable<TransferResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", async (Guid id, ITransferService transfers, IMapper mapper) =>
            {
                var transfer = await transfers.GetByIdAsync(id);
                return transfer is null
                    ? Results.NotFound(new ErrorResponse { Message = "Перевод не найден." })
                    : Results.Ok(mapper.Map(transfer));
            })
            .WithSummary("Информация о переводе")
            .Produces<TransferResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/", async (CreateTransferRequest body, ITransferService transfers, IMapper mapper) =>
            {
                try
                {
                    var result = await transfers.ProcessTransferAsync(body);
                    return Results.Created($"/api/transfers/{result.Id}", mapper.Map(result));
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Выполнить перевод средств")
            .Produces<TransferResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapGet("/{id:guid}/receipt", async (Guid id, ITransferService transfers) =>
            {
                try
                {
                    var receiptText = await transfers.GetReceiptAsync(id);
                    return Results.Text(receiptText, "text/plain");
                }
                catch (InvalidOperationException ex)
                {
                    return Results.NotFound(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Скачать чек по переводу")
            .Produces(StatusCodes.Status200OK, contentType: "text/plain")
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        return api;
    }
}