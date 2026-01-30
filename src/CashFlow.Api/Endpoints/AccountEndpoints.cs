using System.Net;
using CashFlow.Features.CreateAccount;

namespace CashFlow.Api.Endpoints;

public static class AccountEndpoints
{
    public static void MapAccountEndpoints(this WebApplication app)
    {
        app.MapGet("accounts", () =>
        {
            return Array.Empty<AccountResponse>();
        })
        .WithName("GetAccounts")
        .WithTags("Transactions")
        .WithSummary("Get Accounts")
        .WithDescription("Retrieves all accounts in the system.")
        .Produces<AccountResponse[]>((int)HttpStatusCode.OK);
    }
}