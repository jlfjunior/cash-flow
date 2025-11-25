using CashFlow.Transactions.Application.Responses;

namespace CashFlow.Transactions.Api.Endpoints;

public static class AccountEndpoints
{
    public static void MapAccountEndpoints(this WebApplication app)
    {
        app.MapGet("accounts", () =>
        {
            return Array.Empty<AccountResponse>();
        });
    }
}