using Microsoft.EntityFrameworkCore;
using bank.net.api; // Изменили регистр на Api, чтобы избежать конфликтов компилятора
using bank.net.database;
using bank.net.dto;
using bank.net.interfaces;
using bank.net.services;

/*
 * =============================================================================
 * RUT BANK.NET — ЧИСТЫЙ ПРИМЕР MINIMAL API ПРОЕКТА
 * =============================================================================
 *
 * Архитектура проекта:
 * - model     : сущности домена (User, Card, Transfer).
 * - database  : EF Core DbContext + PostgreSQL (BankDbContext).
 * - services  : бизнес-логика управления пользователями, картами и переводами.
 * - api       : endpoint-модули (каждый модуль отвечает за свою группу ручек).
 * - dto       : объекты передачи данных (Request/Response) + Mapper.
 *
 * Реализованы сценарии:
 * - просмотр и создание пользователей (клиентов);
 * - выпуск, блокировка и разблокировка банковских карт;
 * - выполнение переводов между картами в рамках одной транзакции (бизнес-сценарий);
 * - выгрузка текстового кассового чека по транзакции.
 */
var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// SWAGGER / OPENAPI
// -----------------------------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "RUT Bank API",
        Version = "v1",
        Description = "Демонстрационный API банка: клиенты, карты, транзакции, блокировки, чеки."
    });
});

// -----------------------------------------------------------------------------
// DATABASE (EF Core + PostgreSQL)
// -----------------------------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException("Не найдена строка подключения ConnectionStrings:Postgres.");

builder.Services.AddDbContext<BankDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// -----------------------------------------------------------------------------
// DEPENDENCY INJECTION (Регистрация сервисов)
// -----------------------------------------------------------------------------
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<ITransferService, TransferService>();
builder.Services.AddSingleton<IMapper, Mapper>();

var app = builder.Build();

// -----------------------------------------------------------------------------
// ИНИЦИАЛИЗАЦИЯ И МИДЛВАР ТРЕДЫ
// -----------------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<BankDbContext>();
        // Автоматически создает базу данных и таблицы, если их нет (для учебного проекта)
        await db.Database.EnsureCreatedAsync();
    }

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "RUT Bank API v1");
        options.RoutePrefix = "swagger";
    });
}

// -----------------------------------------------------------------------------
// МАРШРУТЫ API (Эндпоинты)
// -----------------------------------------------------------------------------
var api = app.MapGroup("/api");

// Подключаем наши модули эндпоинтов
api.MapUsersEndpoints();
api.MapCardEndpoints();
api.MapTransferEndpoints();

// Корневой эндпоинт приветствия
app.MapGet("/", () => Results.Ok(new
{
    message = "RUT Bank API работает успешно. Документация доступна на /swagger. Основные ресурсы: /api/users, /api/cards, /api/transfers."
}));

await app.RunAsync();