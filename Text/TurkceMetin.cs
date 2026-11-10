using System.Globalization;
using System.Text;

namespace Mihenk.Text;

/// <summary>
/// TR: Türkçe dil kurallarına duyarlı metin işleme, dönüştürme, arama ve sayı okuma araçları.
/// <br/>
/// EN: Turkish-aware text manipulation, case conversion, slugification, search, and number-to-words utilities.
/// </summary>
public static class TurkceMetin
{
    private static readonly CultureInfo TrCulture = new("tr-TR");
    private static readonly CompareInfo TrCompare = TrCulture.CompareInfo;

    private static readonly string[] Ones = ["", "bir", "iki", "üç", "dört", "beş", "altı", "yedi", "sekiz", "dokuz"];
    private static readonly string[] Tens = ["", "on", "yirmi", "otuz", "kırk", "elli", "altmış", "yetmiş", "seksen", "doksan"];
    private static readonly string[] Scales = ["", "bin", "milyon", "milyar", "trilyon", "katrilyon"];

    private static readonly (string Corrupted, string Fixed)[] MojibakeTable =
    [
        ("ÃÂ±", "ı"), ("ÃÂ°", "İ"), ("ÃÂŸ", "ş"), ("ÃÂž", "Ş"), ("ÃÂŸ", "ğ"), ("ÃÂž", "Ğ"),
        ("ÃƒÂ¼", "ü"), ("ÃƒÂœ", "Ü"), ("ÃƒÂ¶", "ö"), ("ÃƒÂ–", "Ö"), ("ÃƒÂ§", "ç"), ("ÃƒÂ‡", "Ç"),
        ("Ä±", "ı"), ("Ä°", "İ"), ("ÅŸ", "ş"), ("Åž", "Ş"), ("ÄŸ", "ğ"), ("Äž", "Ğ"),
        ("Ã¼", "ü"), ("Ãœ", "Ü"), ("Ã¶", "ö"), ("Ã–", "Ö"), ("Ã§", "ç"), ("Ã‡", "Ç"),
        ("Ã¢", "â"), ("Ã‚", "Â"), ("Ã®", "î"), ("ÃŽ", "Î"), ("Ã»", "û"), ("Ã›", "Û")
    ];

    /// <summary>
    /// TR: Metni URL dostu (SEO uyumlu) slug formatına çevirir (Örn: <c>"Şehir Güncellemesi"</c> → <c>"sehir-guncellemesi"</c>).
    /// <br/>
    /// EN: Converts the text into a URL-friendly (SEO-compliant) slug (e.g., <c>"Şehir Güncellemesi"</c> → <c>"sehir-guncellemesi"</c>).
    /// </summary>
    /// <param name="text">TR: Dönüştürülecek metin. / EN: The text to slugify.</param>
    public static string ToSlug(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var sb = new StringBuilder(text.Length);
        bool prevDash = false;

        foreach (char raw in text)
        {
            char c = NormalizeCharToAsciiLower(raw);

            if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
            {
                sb.Append(c);
                prevDash = false;
            }
            else if (sb.Length > 0 && !prevDash)
            {
                sb.Append('-');
                prevDash = true;
            }
        }

        if (sb.Length > 0 && sb[^1] == '-')
            sb.Length--;

        return sb.ToString();
    }

    /// <summary>
    /// TR: Metni Türkçe kurallarına göre (<c>i → İ</c>, <c>ı → I</c>) büyük harfe çevirir.
    /// <br/>
    /// EN: Converts the text to uppercase using Turkish culture rules (<c>i → İ</c>, <c>ı → I</c>).
    /// </summary>
    /// <param name="text">TR: Dönüştürülecek metin. / EN: The text to convert.</param>
    public static string ToUpperTr(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        return text.ToUpper(TrCulture);
    }

    /// <summary>
    /// TR: Metni Türkçe kurallarına göre (<c>İ → i</c>, <c>I → ı</c>) küçük harfe çevirir.
    /// <br/>
    /// EN: Converts the text to lowercase using Turkish culture rules (<c>İ → i</c>, <c>I → ı</c>).
    /// </summary>
    /// <param name="text">TR: Dönüştürülecek metin. / EN: The text to convert.</param>
    public static string ToLowerTr(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        return text.ToLower(TrCulture);
    }

    /// <summary>
    /// TR: Metindeki her kelimenin ilk harfini Türkçe kurallarına uygun olarak (<c>i → İ</c>, <c>I → ı</c>) büyütür, kalanını küçültür.
    /// <br/>
    /// EN: Converts the text to title case using Turkish culture rules (<c>i → İ</c>, <c>I → ı</c>).
    /// </summary>
    /// <param name="text">TR: Dönüştürülecek metin. / EN: The text to convert.</param>
    public static string ToTitleCaseTr(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        return TrCulture.TextInfo.ToTitleCase(text.ToLower(TrCulture));
    }

    /// <summary>
    /// TR: Türkçe karakterleri ASCII karşılıklarına dönüştürür (<c>ğüşıöçĞÜŞİÖÇ → gusiocGUSIOC</c>).
    /// <br/>
    /// EN: Replaces Turkish-specific characters with their ASCII equivalents (<c>ğüşıöçĞÜŞİÖÇ → gusiocGUSIOC</c>).
    /// </summary>
    /// <param name="text">TR: Dönüştürülecek metin. / EN: The text to convert.</param>
    public static string RemoveTurkishChars(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        return string.Create(text.Length, text, static (span, src) =>
        {
            for (int i = 0; i < src.Length; i++)
            {
                span[i] = src[i] switch
                {
                    'ğ' => 'g',
                    'Ğ' => 'G',
                    'ü' => 'u',
                    'Ü' => 'U',
                    'ş' => 's',
                    'Ş' => 'S',
                    'ı' => 'i',
                    'İ' => 'I',
                    'ö' => 'o',
                    'Ö' => 'O',
                    'ç' => 'c',
                    'Ç' => 'C',
                    'â' => 'a',
                    'Â' => 'A',
                    'î' => 'i',
                    'Î' => 'I',
                    'û' => 'u',
                    'Û' => 'U',
                    char c => c
                };
            }
        });
    }

    /// <summary>
    /// TR: Karakter kodlaması (encoding) hatası nedeniyle bozuk görünen (<c>Ã¼ → ü</c>, <c>ÅŸ → ş</c>, <c>Ä± → ı</c> vb.) Türkçe karakterleri düzeltir.
    /// <br/>
    /// EN: Fixes corrupted UTF-8 / Windows-1252 mojibake Turkish characters (<c>Ã¼ → ü</c>, <c>ÅŸ → ş</c>, <c>Ä± → ı</c>, etc.).
    /// </summary>
    /// <param name="text">TR: Düzeltilecek metin. / EN: The corrupted text to fix.</param>
    public static string FixEncoding(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        var sb = new StringBuilder(text);
        foreach (var (corrupted, fixedChar) in MojibakeTable)
        {
            sb.Replace(corrupted, fixedChar);
        }

        return sb.ToString();
    }

    /// <summary>
    /// TR: İki metnin Türkçe büyük/küçük harf kurallarına göre (<c>i == İ</c>, <c>ı == I</c>) eşit olup olmadığını kontrol eder.
    /// <br/>
    /// EN: Checks whether two strings are equal ignoring case according to Turkish culture rules (<c>i == İ</c>, <c>ı == I</c>).
    /// </summary>
    /// <param name="a">TR: Birinci metin. / EN: First string.</param>
    /// <param name="b">TR: İkinci metin. / EN: Second string.</param>
    public static bool EqualsIgnoreCaseTr(string? a, string? b)
    {
        if (ReferenceEquals(a, b))
            return true;
        if (a is null || b is null)
            return false;

        return TrCompare.Compare(a, b, CompareOptions.IgnoreCase) == 0;
    }

    /// <summary>
    /// TR: Metin içinde Türkçe büyük/küçük harf kurallarına duyarlı (<c>i == İ</c>, <c>ı == I</c>) arama yapar.
    /// <br/>
    /// EN: Performs a case-insensitive substring search using Turkish culture rules (<c>i == İ</c>, <c>ı == I</c>).
    /// </summary>
    /// <param name="text">TR: Ana metin. / EN: The source text.</param>
    /// <param name="value">TR: Aranan alt metin. / EN: The substring to seek.</param>
    public static bool ContainsTr(string? text, string? value)
    {
        if (text is null || value is null)
            return false;
        if (value.Length == 0)
            return true;

        return TrCompare.IndexOf(text, value, CompareOptions.IgnoreCase) >= 0;
    }

    /// <summary>
    /// TR: Tam sayıyı Türkçe okunuşuna (yazıya) çevirir (Örn: <c>125</c> → <c>"yüz yirmi beş"</c>).
    /// <br/>
    /// EN: Converts an integer into its Turkish word representation (e.g., <c>125</c> → <c>"yüz yirmi beş"</c>).
    /// </summary>
    /// <param name="number">TR: Yazıya çevrilecek tam sayı. / EN: The integer to convert.</param>
    public static string NumberToWords(int number) => NumberToWords((long)number);

    /// <summary>
    /// TR: 64-bit tam sayıyı Türkçe okunuşuna (yazıya) çevirir (Örn: <c>125</c> → <c>"yüz yirmi beş"</c>).
    /// <br/>
    /// EN: Converts a 64-bit integer into its Turkish word representation (e.g., <c>125</c> → <c>"yüz yirmi beş"</c>).
    /// </summary>
    /// <param name="number">TR: Yazıya çevrilecek 64-bit tam sayı. / EN: The 64-bit integer to convert.</param>
    public static string NumberToWords(long number)
    {
        if (number == 0)
            return "sıfır";

        var sb = new StringBuilder(32);
        if (number < 0)
        {
            sb.Append("eksi ");
            number = number == long.MinValue ? long.MaxValue : -number;
        }

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

        return sb.ToString();
    }

    /// <summary>
    /// TR: Tarihi referans zamana göre Türkçe göreli metne çevirir
    /// (Örn: <c>"az önce"</c>, <c>"5 dakika önce"</c>, <c>"dün"</c>, <c>"3 gün sonra"</c>).
    /// <br/>
    /// EN: Converts a <see cref="DateTime"/> into a Turkish relative time string compared to <paramref name="referenceTime"/>
    /// (e.g., <c>"az önce"</c>, <c>"5 dakika önce"</c>, <c>"dün"</c>, <c>"3 gün sonra"</c>).
    /// </summary>
    /// <param name="dateTime">TR: Dönüştürülecek tarih/zaman. / EN: The target date/time.</param>
    /// <param name="referenceTime">TR: Karşılaştırılacak referans zaman (varsayılan <see cref="DateTime.Now"/>). / EN: Reference time (defaults to <see cref="DateTime.Now"/>).</param>
    public static string ToRelativeTimeTr(DateTime dateTime, DateTime? referenceTime = null)
    {
        DateTime now = referenceTime ?? DateTime.Now;
        TimeSpan diff = now - dateTime;
        bool future = diff.TotalSeconds < 0;
        TimeSpan abs = future ? diff.Duration() : diff;

        string suffix = future ? "sonra" : "önce";

        if (abs.TotalSeconds < 45)
            return future ? "birazdan" : "az önce";

        if (abs.TotalMinutes < 60)
        {
            int mins = Math.Max(1, (int)Math.Round(abs.TotalMinutes));
            return $"{mins} dakika {suffix}";
        }

        if (abs.TotalHours < 24)
        {
            int hours = Math.Max(1, (int)Math.Round(abs.TotalHours));
            return $"{hours} saat {suffix}";
        }

        int days = (int)Math.Round(abs.TotalDays);
        if (days == 1)
            return future ? "yarın" : "dün";

        if (days < 7)
            return $"{days} gün {suffix}";

        if (days < 30)
        {
            int weeks = Math.Max(1, days / 7);
            return $"{weeks} hafta {suffix}";
        }

        if (days < 365)
        {
            int months = Math.Max(1, days / 30);
            return $"{months} ay {suffix}";
        }

        int years = Math.Max(1, days / 365);
        return $"{years} yıl {suffix}";
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
                sb.Append(Ones[hundreds]).Append(' ');
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

    private static char NormalizeCharToAsciiLower(char c) => c switch
    {
        'ğ' or 'Ğ' => 'g',
        'ü' or 'Ü' or 'û' or 'Û' => 'u',
        'ş' or 'Ş' => 's',
        'ı' or 'I' or 'İ' or 'i' or 'î' or 'Î' => 'i',
        'ö' or 'Ö' => 'o',
        'ç' or 'Ç' => 'c',
        'â' or 'Â' => 'a',
        >= 'A' and <= 'Z' => (char)(c + 32),
        _ => c
    };
}
