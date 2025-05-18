using Funds.Abstractions;
using Funds.Events;
using Marten;
using Marten.Events;
using MartenEventSourcing.APIs.Views;
using Microsoft.AspNetCore.Mvc;
using Weasel.Core;

var builder = WebApplication.CreateBuilder(args);

#region Defaults

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endregion //  Defaults

// This is the absolute, simplest way to integrate Marten into your
// .NET application with Marten's default configuration
builder.Services.AddMarten(options =>
{
    // Establish the connection string to your Marten database
    options.Connection(builder.Configuration.GetConnectionString("Marten")!);

    // Specify that we want to use STJ as our serializer
    options.UseSystemTextJsonForSerialization();
    options.Projections.Add<BalanceProjection>(Marten.Events.Projections.ProjectionLifecycle.Inline);

    // If we're running in development mode, let Marten just take care
    // of all necessary schema building and patching behind the scenes
    //if (builder.Environment.IsDevelopment())
    //{
    options.AutoCreateSchemaObjects = AutoCreate.All;
    //}

    options.Schema.For<FundsAccountCreated>().Identity(x => x.AccountId);
    options.Schema.For<FundsDeposited>().Identity(x => x.AccountId);
    options.Schema.For<FundsWithdrawn>().Identity(x => x.AccountId);
    options.Schema.For<FundsAccountBlocked>().Identity(x => x.AccountId);
    options.Schema.For<FundsAccountUnblocked>().Identity(x => x.AccountId);
    options.Schema.For<FundsAccountClosed>().Identity(x => x.AccountId);
});

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
    async (IQuerySession session, AccountId account) =>
    {
        var e = new FundsAccountCreated(account);

        //BalanceView? balance = await session.Events.AggregateStreamAsync<BalanceView>(account);
        BalanceView? balance = await session.LoadAsync<BalanceView>(account);

        return balance is not null ? Results.Ok(balance) : Results.NotFound();
    });

withdraw.MapPost("/create-account/{account}",
    async (IDocumentStore store, AccountId account) =>
    {
        var e = new FundsAccountCreated(account);

        using var session = store.LightweightSession();
        session.Events.Append(account, e);
        await session.SaveChangesAsync();
        return Results.Ok(account);
    });

withdraw.MapPost("/deposit/{account}",
    async (IDocumentStore store, AccountId account, [FromBody] FundsTransactionData data) =>
    {
        var e = new FundsDeposited(account, data);

        using var session = store.LightweightSession();
        session.Events.Append(account, e);
        await session.SaveChangesAsync();
        return Results.Ok(account);
    });

withdraw.MapPost("/withdraw/{account}",
    async (IDocumentStore store, AccountId account, [FromBody] FundsTransactionData data) =>
    {
        var e = new FundsWithdrawn(account, data);

        using var session = store.LightweightSession();
        session.Events.Append(account, e);
        await session.SaveChangesAsync();
        return Results.Ok(account);
    });

withdraw.MapPost("/commission/{account}",
    async (IDocumentStore store, AccountId account, [FromBody]FundsTransactionData data, [FromQuery] Commission commission) =>
    {
        var e = new FundsCommissionTaken(account, data, commission);

        using var session = store.LightweightSession();
        session.Events.Append(account, e);
        await session.SaveChangesAsync();
        return Results.Ok(account);
    });

withdraw.MapPost("/account-block/{account}",
    async (IDocumentStore store, AccountId account) =>
    {
        var e = new FundsAccountBlocked(account);

        using var session = store.LightweightSession();
        session.Events.Append(account, e);
        await session.SaveChangesAsync();
        return Results.Ok(account);
    });

withdraw.MapPost("/account-unblock/{account}",
    async (IDocumentStore store, AccountId account) =>
    {
        var e = new FundsAccountUnblocked(account);

        using var session = store.LightweightSession();
        session.Events.Append(account, e);
        await session.SaveChangesAsync();
        return Results.Ok(account);
    });

withdraw.MapDelete("/account-close/{account}",
    async (IDocumentStore store, AccountId account) =>
    {
        var e = new FundsAccountClosed(account);

        using var session = store.LightweightSession();
        session.Events.Append(account, e);
        await session.SaveChangesAsync();
        return Results.Ok(account);
    });

await app.RunAsync();
