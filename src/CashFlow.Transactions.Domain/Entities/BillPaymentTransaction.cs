namespace CashFlow.Transactions.Domain.Entities;

public class BillPaymentTransaction : Transaction
{
    protected BillPaymentTransaction() { }

    public BillPaymentTransaction(Guid accountId, decimal value) 
        : base(accountId, Direction.Debit, value)
    {
        TransactionType = TransactionType.BillPayment;
    }
}

