namespace CashFlow.Customer.Application.Requests;

public record UpdateCustomerRequest(Guid Id) : CustomerRequest;