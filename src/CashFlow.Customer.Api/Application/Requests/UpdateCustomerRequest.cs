namespace CashFlow.Customer.Api.Application.Requests;

public record UpdateCustomerRequest(Guid Id) : CustomerRequest;