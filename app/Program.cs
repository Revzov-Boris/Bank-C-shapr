using Microsoft.EntityFrameworkCore;
using rut_shop.net.api;
using rut_shop.net.database;
using rut_shop.net.dto;
using rut_shop.net.interfaces;
using rut_shop.net.services;

/*
 * =============================================================================
 * RUT SHOP.NET — ЧИСТЫЙ ПРИМЕР MINIMAL API ПРОЕКТА
 * =============================================================================
 *
 * Архитектура проекта:
 * - model     : сущности домена + DTO для API.
 * - database  : EF Core DbContext + PostgreSQL.
 * - services  : бизнес-логика магазина, покупок и программы лояльности.
 * - api       : endpoint-модули (каждый модуль отвечает за свою группу ручек).
 *
 * Реализованы сценарии:
 * - просмотр товаров;
 * - просмотр клиентов;
 * - создание покупок;
 * - расчёт и накопление бонусов лояльности;
 * - выгрузка текстового чека по покупке.
 */
var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// SWAGGER / OPENAPI
// -----------------------------------------------------------------------------
// Включаем генерацию OpenAPI и Swagger UI.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "RUT Shop API",
        Version = "v1",
        Description = "Демонстрационный API магазина: товары, клиенты, покупки, лояльность, чек."
    });
});

// -----------------------------------------------------------------------------
// DATABASE (EF Core + PostgreSQL)
// -----------------------------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException("Не найдена строка подключения ConnectionStrings:Postgres.");

builder.Services.AddDbContext<ShopDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// Сервисы с бизнес-логикой.
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ILoyaltyService, LoyaltyService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<IReceiptService, ReceiptService>();
builder.Services.AddScoped<IMapper, Mapper>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    // В dev-режиме отображаем красивую интерактивную документацию.
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "RUT Shop API v1");
        options.RoutePrefix = "swagger";
    });
}

var api = app.MapGroup("/api");
api.MapProductsEndpoints();
api.MapCustomersEndpoints();
api.MapPurchasesEndpoints();

app.MapGet("/", () => Results.Ok(new
{
    message = "RUT Shop API работает. Откройте /api/products, /api/customers, /api/purchases."
}));

await app.RunAsync();
