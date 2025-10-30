namespace CashFlow.Lib.Sharable;

public static class ObjectExtensions
{
    public static bool IsNull(this object value) => value is null;
    public static bool IsNotNull(this object value) => value is not null;
}
