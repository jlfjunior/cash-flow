namespace CashFlow.Customers.Application.Requests;

public record UpdateCustomerRequest(Guid Id) : CustomerRequest;