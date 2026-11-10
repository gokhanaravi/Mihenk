using System.Text;

namespace Mihenk.Communication;

/// <summary>
/// TR: E-posta adresi doğrulama, ayıklama ve maskeleme araçları.
/// <br/>
/// EN: Email address validation, extraction, and masking utilities.
/// </summary>
public static class Email
{
    /// <summary>
    /// TR: Verilen değerin geçerli bir e-posta adresi formatında olup olmadığını kontrol eder.
    /// <br/>
    /// EN: Checks whether the given string is in a valid email address format.
    /// </summary>
    /// <param name="email">TR: Kontrol edilecek e-posta adresi. / EN: The email address string to validate.</param>
    public static bool IsValid(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return IsValid(email.AsSpan().Trim());
    }

    /// <summary>
    /// TR: Verilen karakter dizisinin geçerli bir e-posta adresi olup olmadığını kontrol eder (Zero-allocation).
    /// <br/>
    /// EN: Checks whether the given character span is a valid email address (Zero-allocation).
    /// </summary>
    /// <param name="email">TR: Kontrol edilecek karakter dizisi. / EN: The character span to validate.</param>
    public static bool IsValid(ReadOnlySpan<char> email) => Validate(email).IsValid;

    /// <summary>
    /// TR: E-posta adresini doğrular ve geçersizse hata nedenini açıkça döner.
    /// <br/>
    /// EN: Validates the email address and returns a detailed error reason if invalid.
    /// </summary>
    /// <param name="email">TR: Doğrulanacak e-posta adresi. / EN: The email address string to validate.</param>
    public static ValidationResult Validate(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return ValidationResult.Failure("E-posta adresi boş olamaz.");

        return Validate(email.AsSpan().Trim());
    }

    /// <summary>
    /// TR: E-posta adresini doğrular ve geçersizse hata nedenini açıkça döner (Zero-allocation).
    /// <br/>
    /// EN: Validates the email address span and returns a detailed error reason if invalid (Zero-allocation).
    /// </summary>
    /// <param name="email">TR: Doğrulanacak karakter dizisi. / EN: The character span to validate.</param>
    public static ValidationResult Validate(ReadOnlySpan<char> email)
    {
        if (email.IsEmpty)
            return ValidationResult.Failure("E-posta adresi boş olamaz.");

        if (email.Length < 6)
            return ValidationResult.Failure("E-posta adresi çok kısa.");

        if (email.Length > 254)
            return ValidationResult.Failure("E-posta adresi en fazla 254 karakter olabilir.");

        int atIndex = email.IndexOf('@');
        if (atIndex < 0)
            return ValidationResult.Failure("E-posta adresi '@' işareti içermelidir.");

        if (atIndex == 0)
            return ValidationResult.Failure("E-posta adresinde '@' işaretinden önce kullanıcı adı bulunmalıdır.");

        if (atIndex != email.LastIndexOf('@'))
            return ValidationResult.Failure("E-posta adresi birden fazla '@' işareti içeremez.");

        if (atIndex == email.Length - 1)
            return ValidationResult.Failure("E-posta adresinde '@' işaretinden sonra alan adı (domain) bulunmalıdır.");

        ReadOnlySpan<char> local = email[..atIndex];
        ReadOnlySpan<char> domain = email[(atIndex + 1)..];

        if (local.Length > 64)
            return ValidationResult.Failure("E-posta kullanıcı adı kısmı en fazla 64 karakter olabilir.");

        if (domain.Length < 4 || domain.Length > 253)
            return ValidationResult.Failure("E-posta alan adı (domain) uzunluğu geçersiz.");

        if (local[0] == '.' || local[^1] == '.')
            return ValidationResult.Failure("E-posta kullanıcı adı nokta ile başlayamaz veya bitemez.");

        for (int i = 0; i < local.Length; i++)
        {
            char c = local[i];
            if (c == '.')
            {
                if (i + 1 < local.Length && local[i + 1] == '.')
                    return ValidationResult.Failure("E-posta adresi ardışık iki nokta ('..') içeremez.");
                continue;
            }

            if (!IsAllowedLocalChar(c))
                return ValidationResult.Failure($"E-posta kullanıcı adında geçersiz karakter ('{c}') bulunuyor.");
        }

        if (domain[0] is '.' or '-' || domain[^1] is '.' or '-')
            return ValidationResult.Failure("E-posta alan adı nokta veya tire ile başlayamaz veya bitemez.");

        int lastDotIndex = domain.LastIndexOf('.');
        if (lastDotIndex <= 0)
            return ValidationResult.Failure("E-posta alan adı geçerli bir uzantı (örn. .com, .com.tr) içermelidir.");

        ReadOnlySpan<char> tld = domain[(lastDotIndex + 1)..];
        if (tld.Length < 2)
            return ValidationResult.Failure("E-posta alan adı uzantısı (TLD) en az 2 harf olmalıdır.");

        for (int i = 0; i < tld.Length; i++)
        {
            if (!IsAsciiLetter(tld[i]))
                return ValidationResult.Failure("E-posta alan adı uzantısı yalnızca harflerden oluşmalıdır.");
        }

        for (int i = 0; i < domain.Length; i++)
        {
            char c = domain[i];
            if (c == '.')
            {
                if (i + 1 < domain.Length && (domain[i + 1] is '.' or '-'))
                    return ValidationResult.Failure("E-posta alan adı geçersiz nokta/tire dizilimi içeriyor.");
                if (i > 0 && domain[i - 1] == '-')
                    return ValidationResult.Failure("E-posta alan adı geçersiz tire/nokta dizilimi içeriyor.");
                continue;
            }

            if (c == '-' || IsAsciiLetterOrDigit(c))
                continue;

            return ValidationResult.Failure($"E-posta alan adında geçersiz karakter ('{c}') bulunuyor.");
        }

        return ValidationResult.Success;
    }

    /// <summary>
    /// TR: E-posta adresini KVKK uyumlu olarak maskeler (Örn: <c>"gokhan@example.com"</c> → <c>"go****@ex*****.com"</c>).
    /// <br/>
    /// EN: Masks the email address for KVKK/GDPR privacy compliance (e.g., <c>"gokhan@example.com"</c> → <c>"go****@ex*****.com"</c>).
    /// </summary>
    /// <param name="email">TR: Maskelenecek e-posta adresi. / EN: The email address string to mask.</param>
    /// <param name="maskChar">TR: Maskeleme karakteri. / EN: Masking character.</param>
    public static string Mask(string? email, char maskChar = '*')
    {
        if (string.IsNullOrWhiteSpace(email))
            return string.Empty;

        string trimmed = email.Trim();
        int atIndex = trimmed.IndexOf('@');
        if (atIndex <= 0 || atIndex == trimmed.Length - 1)
            return trimmed;

        ReadOnlySpan<char> local = trimmed.AsSpan(0, atIndex);
        ReadOnlySpan<char> domain = trimmed.AsSpan(atIndex + 1);

        int lastDot = domain.LastIndexOf('.');
        var sb = new StringBuilder(trimmed.Length);

        AppendMaskedSegment(sb, local, maskChar);
        sb.Append('@');

        if (lastDot > 0)
        {
            AppendMaskedSegment(sb, domain[..lastDot], maskChar);
            sb.Append(domain[lastDot..]);
        }
        else
        {
            AppendMaskedSegment(sb, domain, maskChar);
        }

        return sb.ToString();
    }

    /// <summary>
    /// TR: Metin içindeki tüm geçerli e-posta adreslerini ayıklar ve salt okunur liste olarak döner.
    /// <br/>
    /// EN: Extracts all valid email addresses from the given text as a read-only list.
    /// </summary>
    /// <param name="text">TR: Taranacak metin. / EN: The text to search.</param>
    public static IReadOnlyList<string> Extract(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return [];

        var results = new List<string>();
        ReadOnlySpan<char> span = text.AsSpan();
        int i = 0;

        while (i < span.Length)
        {
            int relAt = span[i..].IndexOf('@');
            if (relAt < 0)
                break;

            int atPos = i + relAt;

            int start = atPos;
            while (start > 0 && (IsAllowedLocalChar(span[start - 1]) || span[start - 1] == '.'))
                start--;

            while (start < atPos && span[start] == '.')
                start++;

            int end = atPos + 1;
            while (end < span.Length && (IsAsciiLetterOrDigit(span[end]) || span[end] is '.' or '-'))
                end++;

            while (end > atPos + 1 && span[end - 1] is '.' or '-')
                end--;

            if (start < atPos && end > atPos + 1)
            {
                ReadOnlySpan<char> candidate = span[start..end];
                if (IsValid(candidate))
                    results.Add(candidate.ToString());
            }

            i = end > atPos ? end : atPos + 1;
        }

        return results;
    }

    /// <summary>
    /// TR: E-posta adresini doğrular ve geçerliyse küçük harfe çevrilmiş/temizlenmiş halini <paramref name="normalized"/> ile döner.
    /// <br/>
    /// EN: Attempts to validate the email address and outputs the lowercase trimmed string if valid.
    /// </summary>
    /// <param name="email">TR: Doğrulanacak e-posta adresi. / EN: The email address string to validate.</param>
    /// <param name="normalized">TR: Küçük harfe çevrilmiş e-posta adresi. / EN: Normalized lowercase email address.</param>
    public static bool TryValidate(string? email, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? normalized)
    {
        if (!string.IsNullOrWhiteSpace(email))
        {
            ReadOnlySpan<char> trimmed = email.AsSpan().Trim();
            if (IsValid(trimmed))
            {
                normalized = trimmed.ToString().ToLowerInvariant();
                return true;
            }
        }

        normalized = null;
        return false;
    }

    /// <summary>
    /// TR: E-posta adresini KVKK formatında maskelemeyi dener.
    /// <br/>
    /// EN: Attempts to validate and mask the email address.
    /// </summary>
    /// <param name="email">TR: E-posta adresi. / EN: The email address string.</param>
    /// <param name="masked">TR: Maskelenmiş e-posta adresi. / EN: Masked email address string.</param>
    /// <param name="maskChar">TR: Maskeleme karakteri. / EN: Masking character.</param>
    public static bool TryMask(string? email, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? masked, char maskChar = '*')
    {
        if (TryValidate(email, out string? validEmail))
        {
            masked = Mask(validEmail, maskChar);
            return true;
        }

        masked = null;
        return false;
    }

    private static void AppendMaskedSegment(StringBuilder sb, ReadOnlySpan<char> segment, char maskChar)
    {
        if (segment.Length <= 1)
        {
            sb.Append(maskChar);
        }
        else if (segment.Length <= 3)
        {
            sb.Append(segment[0]);
            sb.Append(maskChar, segment.Length - 1);
        }
        else
        {
            sb.Append(segment[0]).Append(segment[1]);
            sb.Append(maskChar, segment.Length - 2);
        }
    }

    private static bool IsAllowedLocalChar(char c)
    {
        if (IsAsciiLetterOrDigit(c))
            return true;

        return c is '_' or '-' or '+' or '!' or '#' or '$' or '%' or '&' or '\'' or '*' or '/' or '=' or '?' or '^' or '`' or '{' or '|' or '}' or '~';
    }

    private static bool IsAsciiLetter(char c) => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');

    private static bool IsAsciiLetterOrDigit(char c) => IsAsciiLetter(c) || ((uint)(c - '0') <= 9);
}
