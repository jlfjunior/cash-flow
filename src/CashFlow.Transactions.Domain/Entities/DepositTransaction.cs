namespace CashFlow.Transactions.Domain.Entities;

public class DepositTransaction : Transaction
{
    protected DepositTransaction() { }

    public DepositTransaction(Guid accountId, decimal value) 
        : base(accountId, Direction.Credit, value)
    {
        TransactionType = TransactionType.Deposit;
    }
}

