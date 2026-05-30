using Microsoft.EntityFrameworkCore;
using bank.net.api;
using bank.net.database;
using bank.net.dto;
using bank.net.interfaces;
using bank.net.services;

var builder = WebApplication.CreateBuilder(args);

// SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "RUT Bank API",
        Version = "v1",
        Description = "API банка: клиенты, карты, транзакции, блокировка/разблокировка карт"
    });
});

// БД
var connectionString = builder.Configuration.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException("Не найдена строка подключения ConnectionStrings:Postgres.");

builder.Services.AddDbContext<BankDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// Реализация интерфейсов
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<ITransferService, TransferService>();
builder.Services.AddSingleton<IMapper, Mapper>();

var app = builder.Build();

// В режиме разработки
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<BankDbContext>();
        // Автоматически создаём базу данных и таблицы, если их нет
        await db.Database.EnsureCreatedAsync();
    }
    // Документация
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "RUT Bank API v1");
        options.RoutePrefix = "swagger";
    });
}

// Контроллеры
var api = app.MapGroup("/api");
api.MapUsersEndpoints();
api.MapCardEndpoints();
api.MapTransferEndpoints();

// Корневой эндпоинт приветствия
app.MapGet("/", () => Results.Ok(new
{
    message = "RUT Bank API работает. Документация доступна на /swagger."
}));

await app.RunAsync();