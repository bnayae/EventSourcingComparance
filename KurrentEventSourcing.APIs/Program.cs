using Funds.Abstractions;
using Funds.Events;
using Microsoft.AspNetCore.Mvc;
using KurrentDB.Client;
using Microsoft.Extensions;
using KurrentEventSourcing.APIs;

var builder = WebApplication.CreateBuilder(args);

#region Defaults

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endregion //  Defaults

string connectionString = builder.Configuration.GetConnectionString("KurrentDbConnection")!;
var settings = KurrentDBClientSettings.Create(connectionString);
using var client = new KurrentDBClient(settings);
builder.Services.AddSingleton(client);
builder.Services.AddSingleton<EventStoreRepository>();

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
    async (EventStoreRepository repository, AccountId account) =>
    {
        BalanceView? balance = await repository.GetBalanceViewAsync(account);
        return balance is not null ? Results.Ok(balance) : Results.NotFound();
    });

withdraw.MapPost("/create-account/{account}",
    async (EventStoreRepository repository, AccountId account) =>
    {
        var e = new FundsAccountCreated(account);
        IWriteResult result = await repository.AppendEventAsync(account, e);
        return Results.Ok(new { account, result });
    });

withdraw.MapPost("/deposit/{account}",
    async (EventStoreRepository repository, AccountId account, [FromBody] FundsTransactionData data) =>
    {
        var e = new FundsDeposited(account, data);
        IWriteResult result = await repository.AppendEventAsync(account, e);
        return Results.Ok(new { account, result });
    });

withdraw.MapPost("/withdraw/{account}",
    async (EventStoreRepository repository, AccountId account, [FromBody] FundsTransactionData data) =>
    {
        var e = new FundsWithdrawn(account, data);
        IWriteResult result = await repository.AppendEventAsync(account, e);
        return Results.Ok(new { account, result });
    });

withdraw.MapPost("/commission/{account}",
    async (EventStoreRepository repository, AccountId account, [FromBody] FundsTransactionData data, [FromQuery] Commission commission) =>
    {
        var e = new FundsCommissionTaken(account, data, commission);
        IWriteResult result = await repository.AppendEventAsync(account, e);
        return Results.Ok(new { account, result });
    });

withdraw.MapPost("/account-block/{account}",
    async (EventStoreRepository repository, AccountId account) =>
    {
        var e = new FundsAccountBlocked(account);
        IWriteResult result = await repository.AppendEventAsync(account, e);
        return Results.Ok(new { account, result });
    });

withdraw.MapPost("/account-unblock/{account}",
    async (EventStoreRepository repository, AccountId account) =>
    {
        var e = new FundsAccountUnblocked(account);
        IWriteResult result = await repository.AppendEventAsync(account, e);
        return Results.Ok(new { account, result });
    });

withdraw.MapDelete("/account-close/{account}",
    async (EventStoreRepository repository, AccountId account) =>
    {
        var e = new FundsAccountClosed(account);
        IWriteResult result = await repository.AppendEventAsync(account, e);
        return Results.Ok(new { account, result });
    });

await app.RunAsync();
