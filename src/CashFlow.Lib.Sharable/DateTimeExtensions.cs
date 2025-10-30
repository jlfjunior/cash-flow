using System;
using System.Collections.Generic;
using System.Linq;

namespace CashFlow.Lib.Sharable;

public static class DateTimeExtensions
{
    public static bool IsBusinessDay(this DateTime dateTime, IEnumerable<DateTime> holidays)
    {
        if (dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday)
            return false;

        var dateOnly = dateTime.Date;

        if (holidays != null && holidays.Any(h => h.Date == dateOnly))
            return false;

        return true;
    }

    public static DateTime NextBusinessDay(this DateTime dateTime, IEnumerable<DateTime> holidays)
    {
        var next = dateTime.AddDays(1);

        while (!next.IsBusinessDay(holidays))
        {
            next = next.AddDays(1);
        }

        return next;
    }

    public static DateTime LastBusinessDay(this DateTime dateTime, IEnumerable<DateTime> holidays)
    {
        var last = dateTime.AddDays(-1);

        while (!last.IsBusinessDay(holidays))
        {
            last = last.AddDays(-1);
        }

        return last;
    }
}