using Funds.Abstractions;
using Funds.Events;
using KurrentDB.Client;
using KurrentEventSourcing.APIs;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

#region Defaults

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endregion //  Defaults

string connectionString = builder.Configuration.GetConnectionString("KurrentDbConnection")!;
var settings = KurrentDBClientSettings.Create(connectionString);
using var client = new KurrentDBClient(settings);
builder.Services.AddSingleton(client);
builder.Services.AddSingleton<IRepository, EventStoreRepository>();

var app = builder.Build();

#region Defaults

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


#endregion //  Defaults

var withdraw = app.MapGroup("funds")
 .WithTags("funds");

withdraw.MapGet("/{account}",
    async (IRepository repository, AccountId account) =>
    {
        Balance balance = await repository.GetBalanceAsync(account);
        return balance is not null ? Results.Ok(balance) : Results.NotFound();
    });

withdraw.MapPost("/create-account/{account}",
    async (IRepository repository, AccountId account) =>
    {
        var e = new FundsAccountCreated(account);
        ulong result = await repository.AppendEventAsync(account, e);
        return Results.Ok(new { account, result });
    });

withdraw.MapPost("/deposit/{account}",
    async (IRepository repository, AccountId account, [FromBody] FundsTransactionData data) =>
    {
        var e = new FundsDeposited(account, data);
        ulong result = await repository.AppendEventAsync(account, e);
        return Results.Ok(new { account, result });
    });

withdraw.MapPost("/withdraw/{account}",
    async (IRepository repository, AccountId account, [FromBody] FundsTransactionData data) =>
    {
        var e = new FundsWithdrawn(account, data);
        ulong result = await repository.AppendEventAsync(account, e);
        return Results.Ok(new { account, result });
    });

withdraw.MapPost("/commission/{account}",
    async (IRepository repository, AccountId account, [FromBody] FundsTransactionData data, [FromQuery] Commission commission) =>
    {
        var e = new FundsCommissionTaken(account, data, commission);
        ulong result = await repository.AppendEventAsync(account, e);
        return Results.Ok(new { account, result });
    });

withdraw.MapPost("/account-block/{account}",
    async (IRepository repository, AccountId account) =>
    {
        var e = new FundsAccountBlocked(account);
        ulong result = await repository.AppendEventAsync(account, e);
        return Results.Ok(new { account, result });
    });

withdraw.MapPost("/account-unblock/{account}",
    async (IRepository repository, AccountId account) =>
    {
        var e = new FundsAccountUnblocked(account);
        ulong result = await repository.AppendEventAsync(account, e);
        return Results.Ok(new { account, result });
    });

withdraw.MapDelete("/account-close/{account}",
    async (IRepository repository, AccountId account) =>
    {
        var e = new FundsAccountClosed(account);
        ulong result = await repository.AppendEventAsync(account, e);
        return Results.Ok(new { account, result });
    });

await app.RunAsync();
