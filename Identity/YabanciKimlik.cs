namespace Mihenk.Identity;

/// <summary>
/// TR: Yabancı Kimlik Numarası (YKN) doğrulama araçları.
/// <br/>
/// EN: Turkish Foreigner Identification Number (YKN, starting with 99) validation utilities.
/// </summary>
public static class YabanciKimlik
{
    /// <summary>
    /// TR: Verilen değerin 99 ile başlayan 11 haneli geçerli bir Yabancı Kimlik Numarası (YKN) olup olmadığını kontrol eder.
    /// <br/>
    /// EN: Checks whether the given value is a valid 11-digit Foreigner ID (YKN) starting with '99'.
    /// </summary>
    /// <param name="value">TR: Kontrol edilecek YKN. / EN: The YKN string to validate.</param>
    public static bool IsValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return IsValid(value.AsSpan().Trim());
    }

    /// <summary>
    /// TR: Verilen karakter dizisinin 99 ile başlayan 11 haneli geçerli bir Yabancı Kimlik Numarası olup olmadığını kontrol eder (Zero-allocation).
    /// <br/>
    /// EN: Checks whether the given character span is a valid 11-digit Foreigner ID starting with '99' (Zero-allocation).
    /// </summary>
    /// <param name="value">TR: Kontrol edilecek karakter dizisi. / EN: The character span to validate.</param>
    public static bool IsValid(ReadOnlySpan<char> value) => Validate(value).IsValid;

    /// <summary>
    /// TR: Yabancı Kimlik Numarasını doğrular ve geçersizse hata nedenini açıkça döner.
    /// <br/>
    /// EN: Validates the Foreigner ID (YKN) and returns a detailed error reason if invalid.
    /// </summary>
    /// <param name="value">TR: Doğrulanacak YKN. / EN: The YKN string to validate.</param>
    public static ValidationResult Validate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return ValidationResult.Failure("Yabancı Kimlik Numarası boş olamaz.");

        return Validate(value.AsSpan().Trim());
    }

    /// <summary>
    /// TR: Yabancı Kimlik Numarasını doğrular ve geçersizse hata nedenini açıkça döner (Zero-allocation).
    /// <br/>
    /// EN: Validates the Foreigner ID (YKN) span and returns a detailed error reason if invalid (Zero-allocation).
    /// </summary>
    /// <param name="value">TR: Doğrulanacak karakter dizisi. / EN: The character span to validate.</param>
    public static ValidationResult Validate(ReadOnlySpan<char> value)
    {
        if (value.IsEmpty)
            return ValidationResult.Failure("Yabancı Kimlik Numarası boş olamaz.");

        if (value.Length != 11)
            return ValidationResult.Failure("Yabancı Kimlik Numarası 11 haneli olmalıdır.");

        if (value[0] != '9' || value[1] != '9')
            return ValidationResult.Failure("Yabancı Kimlik Numarası '99' ile başlamalıdır.");

        ValidationResult tcResult = TcKimlik.Validate(value);
        return tcResult.IsValid
            ? ValidationResult.Success
            : ValidationResult.Failure("Yabancı Kimlik Numarası kontrol basamağı algoritmasına uymuyor.");
    }

    /// <summary>
    /// TR: Yabancı Kimlik Numarasını doğrular ve geçerliyse temizlenmiş (11 haneli) halini <paramref name="normalized"/> parametresiyle döner.
    /// <br/>
    /// EN: Attempts to validate the Foreigner ID (YKN) and outputs the trimmed 11-digit string if valid.
    /// </summary>
    /// <param name="value">TR: Doğrulanacak YKN. / EN: The YKN string to validate.</param>
    /// <param name="normalized">TR: Geçerliyse temizlenmiş 11 haneli YKN. / EN: The normalized 11-digit YKN if valid.</param>
    public static bool TryValidate(string? value, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? normalized)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            ReadOnlySpan<char> trimmed = value.AsSpan().Trim();
            if (IsValid(trimmed))
            {
                normalized = trimmed.ToString();
                return true;
            }
        }

        normalized = null;
        return false;
    }
}
