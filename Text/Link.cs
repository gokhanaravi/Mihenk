using System.Text.RegularExpressions;

namespace Mihenk.Text;

/// <summary>
/// TR: Metin içerisinden URL / bağlantı (link) ayıklama araçları.
/// <br/>
/// EN: URL / hyperlink extraction utilities from text.
/// </summary>
public static partial class Link
{
    /// <summary>
    /// TR: Verilen metin içindeki tüm bağlantıları (<c>http://</c>, <c>https://</c>, <c>ftp://</c> veya <c>www.</c> ile başlayan) ayıklar ve salt okunur liste olarak döner.
    /// <br/>
    /// EN: Extracts all URLs (starting with <c>http://</c>, <c>https://</c>, <c>ftp://</c>, or <c>www.</c>) from the given text as a read-only list.
    /// </summary>
    /// <param name="text">TR: Taranacak metin. / EN: The text to search.</param>
    public static IReadOnlyList<string> Extract(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return [];

        var results = new List<string>();
        foreach (Match match in UrlRegex().Matches(text))
        {
            ReadOnlySpan<char> val = match.Value.AsSpan();

            while (val.Length > 0 && IsTrailingPunctuation(val[^1], val))
                val = val[..^1];

            if (val.Length > 0)
                results.Add(val.ToString());
        }

        return results;
    }

    private static bool IsTrailingPunctuation(char last, ReadOnlySpan<char> current)
    {
        if (last is '.' or ',' or '!' or '?' or ';' or ':' or '\'' or '"' or ']' or '>')
            return true;

        if (last == ')')
        {
            int openCount = 0;
            int closeCount = 0;
            for (int i = 0; i < current.Length; i++)
            {
                if (current[i] == '(') openCount++;
                else if (current[i] == ')') closeCount++;
            }
            return closeCount > openCount;
        }

        return false;
    }

    [GeneratedRegex(@"(?:(?:https?|ftp)://|www\.)[^\s<>""]+", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex UrlRegex();
}
