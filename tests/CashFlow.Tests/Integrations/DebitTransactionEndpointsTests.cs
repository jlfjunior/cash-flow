using CashFlow.Transaction.Api.Sharable.Requests;

namespace CashFlow.Tests.Integrations;

public class DebitTransactionEndpointsTests
{
    public DebitTransactionEndpointsTests()
    {
        
    }

    [Fact]
    public async Task TransactionEndpoints_ShouldCreateDebit_SuccessfullyAsync()
    {
        var request = new DebitTransactionRequest(CustomerId: Guid.CreateVersion7(), Value: 100m);
    }
}