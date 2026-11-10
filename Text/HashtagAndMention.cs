using System.Text.RegularExpressions;

namespace Mihenk.Text;

/// <summary>
/// TR: Metin içerisinden hashtag (<c>#etiket</c>) ayıklama araçları.
/// <br/>
/// EN: Hashtag (<c>#tag</c>) extraction utilities from text.
/// </summary>
public static partial class Hashtag
{
    /// <summary>
    /// TR: Verilen metin içindeki tüm hashtag'leri (<c>#</c> işareti dahil) ayıklar ve salt okunur liste olarak döner.
    /// <br/>
    /// EN: Extracts all hashtags (including the leading <c>#</c>) from the given text as a read-only list.
    /// </summary>
    /// <param name="text">TR: Taranacak metin. / EN: The text to search.</param>
    public static IReadOnlyList<string> Extract(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return [];

        var results = new List<string>();
        foreach (Match match in HashtagRegex().Matches(text))
        {
            results.Add(match.Value);
        }

        return results;
    }

    [GeneratedRegex(@"(?<![\p{L}\p{N}_])#[\p{L}\p{N}_]*[\p{L}_][\p{L}\p{N}_]*", RegexOptions.CultureInvariant)]
    private static partial Regex HashtagRegex();
}

/// <summary>
/// TR: Metin içerisinden kullanıcı etiketlerini / mention (<c>@kullanici</c>) ayıklama araçları.
/// <br/>
/// EN: User mention (<c>@username</c>) extraction utilities from text.
/// </summary>
public static partial class Mention
{
    /// <summary>
    /// TR: Verilen metin içindeki tüm mention'ları (<c>@</c> işareti dahil, e-posta adreslerini hariç tutarak) ayıklar ve salt okunur liste olarak döner.
    /// <br/>
    /// EN: Extracts all user mentions (including the leading <c>@</c>, excluding email addresses) from the given text as a read-only list.
    /// </summary>
    /// <param name="text">TR: Taranacak metin. / EN: The text to search.</param>
    public static IReadOnlyList<string> Extract(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return [];

        var results = new List<string>();
        foreach (Match match in MentionRegex().Matches(text))
        {
            results.Add(match.Value);
        }

        return results;
    }

    [GeneratedRegex(@"(?<![\p{L}\p{N}_.])@[\p{L}\p{N}_]+(?:\.[\p{L}\p{N}_]+)*(?![\p{L}\p{N}_@])", RegexOptions.CultureInvariant)]
    private static partial Regex MentionRegex();
}
