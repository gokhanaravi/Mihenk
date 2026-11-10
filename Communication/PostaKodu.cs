namespace Mihenk.Communication;

/// <summary>
/// TR: Türkiye Posta Kodu doğrulama araçları.
/// <br/>
/// EN: Turkish 5-digit Postal Code validation utilities.
/// </summary>
public static class PostaKodu
{
    /// <summary>
    /// TR: Verilen değerin geçerli bir Türkiye posta kodu (5 hane, ilk iki hane 01-81 arası il kodu) olup olmadığını kontrol eder.
    /// <br/>
    /// EN: Checks whether the given value is a valid 5-digit Turkish postal code (first two digits 01-81 province code).
    /// </summary>
    /// <param name="postalCode">TR: Kontrol edilecek posta kodu. / EN: The postal code string to validate.</param>
    public static bool IsValid(string? postalCode)
    {
        if (string.IsNullOrWhiteSpace(postalCode))
            return false;

        return IsValid(postalCode.AsSpan().Trim());
    }

    /// <summary>
    /// TR: Verilen karakter dizisinin geçerli bir Türkiye posta kodu olup olmadığını kontrol eder (Zero-allocation).
    /// <br/>
    /// EN: Checks whether the given character span is a valid 5-digit Turkish postal code (Zero-allocation).
    /// </summary>
    /// <param name="postalCode">TR: Kontrol edilecek karakter dizisi. / EN: The character span to validate.</param>
    public static bool IsValid(ReadOnlySpan<char> postalCode) => Validate(postalCode).IsValid;

    /// <summary>
    /// TR: Posta kodunu doğrular ve geçersizse hata nedenini açıkça döner.
    /// <br/>
    /// EN: Validates the postal code and returns a detailed error reason if invalid.
    /// </summary>
    /// <param name="postalCode">TR: Doğrulanacak posta kodu. / EN: The postal code string to validate.</param>
    public static ValidationResult Validate(string? postalCode)
    {
        if (string.IsNullOrWhiteSpace(postalCode))
            return ValidationResult.Failure("Posta kodu boş olamaz.");

        return Validate(postalCode.AsSpan().Trim());
    }

    /// <summary>
    /// TR: Posta kodunu doğrular ve geçersizse hata nedenini açıkça döner (Zero-allocation).
    /// <br/>
    /// EN: Validates the postal code span and returns a detailed error reason if invalid (Zero-allocation).
    /// </summary>
    /// <param name="postalCode">TR: Doğrulanacak karakter dizisi. / EN: The character span to validate.</param>
    public static ValidationResult Validate(ReadOnlySpan<char> postalCode)
    {
        if (postalCode.IsEmpty)
            return ValidationResult.Failure("Posta kodu boş olamaz.");

        if (postalCode.Length != 5)
            return ValidationResult.Failure("Posta kodu 5 haneli olmalıdır.");

        for (int i = 0; i < 5; i++)
        {
            if ((uint)(postalCode[i] - '0') > 9)
                return ValidationResult.Failure("Posta kodu yalnızca rakamlardan oluşmalıdır.");
        }

        int provinceCode = (postalCode[0] - '0') * 10 + (postalCode[1] - '0');
        if (provinceCode < 1 || provinceCode > 81)
            return ValidationResult.Failure("Posta kodunun ilk 2 hanesi geçerli bir il plaka kodu (01-81) olmalıdır.");

        if (postalCode[2] == '0' && postalCode[3] == '0' && postalCode[4] == '0')
            return ValidationResult.Failure("Posta kodunun son 3 hanesi '000' olamaz.");

        return ValidationResult.Success;
    }

    /// <summary>
    /// TR: Posta kodunu doğrular ve geçerliyse 5 haneli temizlenmiş halini döner.
    /// <br/>
    /// EN: Attempts to validate the postal code and outputs the trimmed 5-digit string if valid.
    /// </summary>
    /// <param name="postalCode">TR: Doğrulanacak posta kodu. / EN: The postal code string to validate.</param>
    /// <param name="normalized">TR: 5 haneli temizlenmiş posta kodu. / EN: Normalized 5-digit postal code.</param>
    public static bool TryValidate(string? postalCode, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? normalized)
    {
        if (!string.IsNullOrWhiteSpace(postalCode))
        {
            ReadOnlySpan<char> trimmed = postalCode.AsSpan().Trim();
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
