using CashFlow.Lib.Sharable;
using CashFlow.Transactions.Application.Requests;
using CashFlow.Transactions.Application.Responses;

namespace CashFlow.Api.Endpoints;

public static class TransactionEndpoints
{
    public static void MapTransactionEndpoints(this WebApplication app)
    {
        app.MapPost("/transactions/credit",
                async (CreateTransactionRequest request, ICommand<CreateTransactionRequest, AccountResponse> command) =>
                {
                    var transaction = await command.ExecuteAsync(request,  CancellationToken.None);
        
                    return Results.Ok(transaction);
                })
            .WithSummary("Add credit transaction")
            .WithDescription("Creates a new credit or debit transaction into an account.")
            .WithTags("Transactions")
            .WithName("AddTransaction");
        
        app.MapPost("/transactions/pay-bill",
                async (PayBillRequest request, ICommand<PayBillRequest, AccountResponse> command) =>
                {
                    var result = await command.ExecuteAsync(request, CancellationToken.None);
        
                    return Results.Ok(result);
                })
            .WithSummary("Pay bill")
            .WithDescription("Processes a bill payment (boleto) as a debit transaction. Validates sufficient balance before processing.")
            .WithTags("Transactions")
            .WithName("PayBill");
    }
}
