namespace CashFlow.Domain.Entities;

public class WithdrawTransaction : Transaction
{
    protected WithdrawTransaction() { }

    public WithdrawTransaction(Guid accountId, decimal value) 
        : base(accountId, Direction.Debit, value)
    {
        TransactionType = TransactionType.Withdraw;
    }
}
