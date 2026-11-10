using System.Globalization;
using System.Text;

namespace Mihenk.Finance;

/// <summary>
/// TR: Para birimi (Türk Lirası) formatlama ve tutarı yazıya çevirme araçları.
/// <br/>
/// EN: Currency (Turkish Lira) formatting and amount-to-words conversion utilities.
/// </summary>
public static class Para
{
    private static readonly CultureInfo TrCulture = new("tr-TR");

    private static readonly string[] Ones = ["", "bir", "iki", "üç", "dört", "beş", "altı", "yedi", "sekiz", "dokuz"];
    private static readonly string[] Tens = ["", "on", "yirmi", "otuz", "kırk", "elli", "altmış", "yetmiş", "seksen", "doksan"];
    private static readonly string[] Scales = ["", "bin", "milyon", "milyar", "trilyon", "katrilyon"];

    /// <summary>
    /// TR: Verilen tutarı Türkçe fatura/çek standartlarına uygun olarak yazıya çevirir
    /// (Örn: <c>1250.50m</c> → <c>"Bin iki yüz elli TL elli kuruş"</c>).
    /// <br/>
    /// EN: Converts the given monetary amount into Turkish words suitable for invoices and checks
    /// (e.g., <c>1250.50m</c> → <c>"Bin iki yüz elli TL elli kuruş"</c>).
    /// </summary>
    /// <param name="amount">TR: Yazıya çevrilecek parasal tutar. / EN: Monetary amount to convert.</param>
    /// <param name="currencyUnit">TR: Ana para birimi adı (varsayılan: "TL"). / EN: Main currency unit name (default: "TL").</param>
    /// <param name="subUnit">TR: Kuruş/alt para birimi adı (varsayılan: "kuruş"). / EN: Sub-currency unit name (default: "kuruş").</param>
    public static string ToWords(decimal amount, string currencyUnit = "TL", string subUnit = "kuruş")
    {
        bool negative = amount < 0;
        decimal abs = Math.Round(Math.Abs(amount), 2, MidpointRounding.AwayFromZero);

        long integerPart = (long)Math.Truncate(abs);
        int fractionalPart = (int)Math.Round((abs - integerPart) * 100m);

        var sb = new StringBuilder(64);

        if (negative && (integerPart > 0 || fractionalPart > 0))
            sb.Append("eksi ");

        if (integerPart == 0)
        {
            sb.Append("sıfır ").Append(currencyUnit);
        }
        else
        {
            AppendInteger(sb, integerPart);
            sb.Append(' ').Append(currencyUnit);
        }

        if (fractionalPart > 0)
        {
            sb.Append(' ');
            AppendThreeDigits(sb, fractionalPart);
            sb.Append(' ').Append(subUnit);
        }

        if (sb.Length > 0)
        {
            sb[0] = sb[0] switch
            {
                'i' => 'İ',
                'ı' => 'I',
                'ç' => 'Ç',
                'ğ' => 'Ğ',
                'ö' => 'Ö',
                'ş' => 'Ş',
                'ü' => 'Ü',
                char c => char.ToUpperInvariant(c)
            };
        }

        return sb.ToString();
    }

    /// <summary>
    /// TR: Verilen tutarı standart Türk Lirası görünümünde formatlar (Örn: <c>1250.5m</c> → <c>"1.250,50 ₺"</c>).
    /// <br/>
    /// EN: Formats the given amount as standard Turkish Lira currency string (e.g., <c>1250.5m</c> → <c>"1.250,50 ₺"</c>).
    /// </summary>
    /// <param name="amount">TR: Formatlanacak tutar. / EN: The amount to format.</param>
    public static string FormatTl(decimal amount)
    {
        return $"{amount.ToString("N2", TrCulture)} \u20BA";
    }

    private static void AppendInteger(StringBuilder sb, long number)
    {
        Span<int> groups = stackalloc int[6];
        int groupCount = 0;

        while (number > 0 && groupCount < groups.Length)
        {
            groups[groupCount++] = (int)(number % 1000);
            number /= 1000;
        }

        bool firstWordWritten = false;

        for (int i = groupCount - 1; i >= 0; i--)
        {
            int group = groups[i];
            if (group == 0)
                continue;

            if (firstWordWritten)
                sb.Append(' ');

            if (i == 1 && group == 1)
            {
                sb.Append("bin");
            }
            else
            {
                AppendThreeDigits(sb, group);
                if (i > 0)
                {
                    sb.Append(' ').Append(Scales[i]);
                }
            }

            firstWordWritten = true;
        }
    }

    private static void AppendThreeDigits(StringBuilder sb, int group)
    {
        int hundreds = group / 100;
        int tens = (group % 100) / 10;
        int ones = group % 10;

        bool written = false;

        if (hundreds > 0)
        {
            if (hundreds > 1)
            {
                sb.Append(Ones[hundreds]).Append(' ');
            }
            sb.Append("yüz");
            written = true;
        }

        if (tens > 0)
        {
            if (written)
                sb.Append(' ');
            sb.Append(Tens[tens]);
            written = true;
        }

        if (ones > 0)
        {
            if (written)
                sb.Append(' ');
            sb.Append(Ones[ones]);
        }
    }
}
