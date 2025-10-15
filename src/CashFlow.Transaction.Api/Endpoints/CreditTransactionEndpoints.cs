using CashFlow.Transaction.Api.Domain.Services;
using CashFlow.Transaction.Api.Sharable.Requests;

namespace CashFlow.Transaction.Api.Endpoints;

public static class CreditTransactionEndpoints
{
    public static void MapTransactionEndpoints(this WebApplication app)
    {
        app.MapGet("/transactions", async (ITransactionService transactionService) =>
        {
            var transactions = await transactionService.SearchAsync();
            
            return Results.Ok(transactions);
        })
        .WithSummary("Get all transactions")
        .WithDescription("Retrieves a complete list of all financial transactions in the system, including both credit and debit entries.")
        .WithTags("Transactions")
        .WithName("GetTransactions");

        app.MapPost("/transactions/credit", async (CreditTransactionRequest request, ITransactionService transactionService) =>
            {
                var transaction = await transactionService.CreateCreditAsync(request.AccountId, request.Value);
            
                return Results.Ok(transaction);
            })
            .WithSummary("Add credit transaction")
            .WithDescription("Creates a new credit transaction that increases the account balance.")
            .WithTags("Transactions")
            .WithName("AddCreditTransaction");

        app.MapPost("/transactions/debit", async (DebitTransactionRequest request, ITransactionService transactionService) =>
            {
                var transaction = await transactionService.CreateDebitAsync(request.AccountId, request.Value);
            
                return Results.Ok(transaction);
            })
            .WithSummary("Add debit transaction")
            .WithDescription("Creates a new debit transaction that decreases the account balance.")
            .WithTags("Transactions")
            .WithName("AddDebitTransaction");
    }
}
