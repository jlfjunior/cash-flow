namespace CashFlow.Lib.Sharable;

public static class DecimalExtensions
{
    public static bool IsLessThanOrEqualTo(this decimal reference, decimal value) => reference <= value;
    public static bool IsLessThan(this decimal reference, decimal value) => reference < value;
    public static bool IsGreaterThanOrEqualTo(this decimal reference, decimal value) => reference >= value;
    public static bool IsGreaterThan(this decimal reference, decimal value) => reference > value;
}