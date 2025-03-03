using System.Globalization;

namespace Invoices.Extensions;

public static class DecimalExtensions
{
    public static string ToCurrencyString(this decimal currency)
    {
        return currency.ToString("C2", new CultureInfo("pl-PL"));
    }
}