using CashFlow.Transaction.Api.Application;
using CashFlow.Transaction.Api.Application.Models;

namespace CashFlow.Transaction.Api.Endpoints;

public static class TransactionEndpoints
{
    public static void MapTransactionEndpoints(this WebApplication app)
    {
        app.MapGet("/transactions", (TransactionService transactionService) =>
        {
            var transactions = new List<TransactionResponse>();
            
            return Results.Ok(transactions);
        })
        .WithSummary("Get all transactions")
        .WithDescription("Retrieves a complete list of all financial transactions in the system, including both credit and debit entries.")
        .WithTags("Transactions")
        .WithName("GetTransactions");

        app.MapPost("/transactions/credit", (CreateCreditTransactionRequest request, TransactionService transactionService) =>
            {
                var transaction = transactionService.CreateCredit(
                    request.CustomerId,
                    request.Value,
                    request.ReferenceDate
                );
            
                return Results.Ok(transaction);
            })
            .WithSummary("Add credit transaction")
            .WithDescription("Creates a new credit transaction that increases the account balance.")
            .WithTags("Transactions")
            .WithName("AddCreditTransaction");

        app.MapPost("/transactions/debit", (CreateDebitTransactionRequest request, TransactionService transactionService) =>
            {
                var transaction = transactionService.CreateDebit(
                    request.CustomerId,
                    request.Value,
                    request.ReferenceDate
                );
            
                return Results.Ok(transaction);
            })
            .WithSummary("Add debit transaction")
            .WithDescription("Creates a new debit transaction that decreases the account balance.")
            .WithTags("Transactions")
            .WithName("AddDebitTransaction");
    }
}
