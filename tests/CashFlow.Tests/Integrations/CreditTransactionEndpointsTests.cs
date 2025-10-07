using System.Net.Http.Json;
using CashFlow.Transaction.Api.Domain.Services;
using CashFlow.Transaction.Api.Sharable.Requests;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.Tests.Integrations;

public class CreditTransactionEndpointsTests : IClassFixture<ServerFactory>
{
    private readonly HttpClient _httpClient;
    private readonly IServiceScope _serviceScope;

    public CreditTransactionEndpointsTests(ServerFactory factory)
    {
        _httpClient = factory.CreateClient();
        _serviceScope = factory.Services.CreateScope();
    }

    [Fact]
    public async Task TransactionEndpoints_ShouldCreateCredit_SuccessfullyAsync()
    {
        var serviceScope = _serviceScope.ServiceProvider.GetRequiredService<ITransactionService>();
        var request = new CreateCreditTransactionRequest(CustomerId: Guid.CreateVersion7(), Value: 100m);
        var response = await _httpClient.PostAsJsonAsync("transactions/credit", request);
        var transactions = await serviceScope.SearchAsync(customerId: request.CustomerId);
        
        Assert.True(response.IsSuccessStatusCode);
        Assert.Single(transactions);
    }
}