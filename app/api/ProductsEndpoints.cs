using rut_shop.net.dto;
using rut_shop.net.dto.request;
using rut_shop.net.dto.response;
using rut_shop.net.interfaces;

namespace rut_shop.net.api;

public static class ProductsEndpoints
{
    /// <summary>
    /// Группа endpoint-ов для работы с каталогом товаров.
    /// </summary>
    public static RouteGroupBuilder MapProductsEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/products").WithTags("Products");

        group.MapGet("/", async (IProductService products, IMapper mapper) =>
            {
                var result = await products.GetAllAsync();
                return Results.Ok(result.Select(mapper.Map));
            })
            .WithSummary("Получить список товаров")
            .WithDescription("Возвращает весь доступный каталог товаров с текущими остатками.")
            .Produces<IEnumerable<ProductResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{productId:guid}",
                async (Guid productId, IProductService products, IMapper mapper) =>
            {
                var product = await products.GetByIdAsync(productId);
                return product is null
                    ? Results.NotFound(new ErrorResponse { Message = "Товар не найден." })
                    : Results.Ok(mapper.Map(product));
            })
            .WithSummary("Получить товар по идентификатору")
            .Produces<ProductResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/", async (CreateProductRequest body, IProductService products, IMapper mapper) =>
            {
                try
                {
                    var created = await products.AddAsync(body);
                    return Results.Created($"/api/products/{created.Id}", mapper.Map(created));
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Добавить товар")
            .Produces<ProductResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("/{productId:guid}",
                async (Guid productId, UpdateProductRequest body, IProductService products, IMapper mapper) =>
            {
                try
                {
                    var updated = await products.UpdateAsync(productId, body);
                    return updated is null
                        ? Results.NotFound(new ErrorResponse { Message = "Товар не найден." })
                        : Results.Ok(mapper.Map(updated));
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Изменить товар")
            .Produces<ProductResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapDelete("/{productId:guid}",
                async (Guid productId, IProductService products) =>
            {
                try
                {
                    var deleted = await products.DeleteAsync(productId);
                    return deleted
                        ? Results.NoContent()
                        : Results.NotFound(new ErrorResponse { Message = "Товар не найден." });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Удалить товар")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        return api;
    }
}
