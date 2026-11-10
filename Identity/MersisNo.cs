namespace Mihenk.Identity;

/// <summary>
/// TR: Merkezi Sicil Kayıt Sistemi (MERSİS) Numarası doğrulama araçları.
/// <br/>
/// EN: Turkish Central Registry System (MERSIS) 16-digit company number validation utilities.
/// </summary>
public static class MersisNo
{
    /// <summary>
    /// TR: Verilen değerin 16 haneli geçerli bir MERSİS numarası olup olmadığını kontrol eder.
    /// <br/>
    /// EN: Checks whether the given value is a valid 16-digit MERSIS number.
    /// </summary>
    /// <param name="value">TR: Kontrol edilecek MERSİS numarası. / EN: The MERSIS number to validate.</param>
    public static bool IsValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return IsValid(value.AsSpan().Trim());
    }

    /// <summary>
    /// TR: Verilen karakter dizisinin 16 haneli geçerli bir MERSİS numarası olup olmadığını kontrol eder (Zero-allocation).
    /// <br/>
    /// EN: Checks whether the given character span is a valid 16-digit MERSIS number (Zero-allocation).
    /// </summary>
    /// <param name="value">TR: Kontrol edilecek karakter dizisi. / EN: The character span to validate.</param>
    public static bool IsValid(ReadOnlySpan<char> value) => Validate(value).IsValid;

    /// <summary>
    /// TR: MERSİS numarasını doğrular ve geçersizse hata nedenini açıkça döner.
    /// <br/>
    /// EN: Validates the MERSIS number and returns a detailed error reason if invalid.
    /// </summary>
    /// <param name="value">TR: Doğrulanacak MERSİS numarası. / EN: The MERSIS number to validate.</param>
    public static ValidationResult Validate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return ValidationResult.Failure("MERSİS numarası boş olamaz.");

        return Validate(value.AsSpan().Trim());
    }

    /// <summary>
    /// TR: MERSİS numarasını doğrular ve geçersizse hata nedenini açıkça döner (Zero-allocation).
    /// <br/>
    /// EN: Validates the MERSIS number span and returns a detailed error reason if invalid (Zero-allocation).
    /// </summary>
    /// <param name="value">TR: Doğrulanacak karakter dizisi. / EN: The character span to validate.</param>
    public static ValidationResult Validate(ReadOnlySpan<char> value)
    {
        if (value.IsEmpty)
            return ValidationResult.Failure("MERSİS numarası boş olamaz.");

        if (value.Length != 16)
            return ValidationResult.Failure("MERSİS numarası 16 haneli olmalıdır.");

        for (int i = 0; i < 16; i++)
        {
            if ((uint)(value[i] - '0') > 9)
                return ValidationResult.Failure("MERSİS numarası yalnızca rakamlardan oluşmalıdır.");
        }

        if (value[0] == '0')
        {
            ValidationResult vknResult = VergiNo.Validate(value.Slice(1, 10));
            return vknResult.IsValid
                ? ValidationResult.Success
                : ValidationResult.Failure("MERSİS numarası içerisindeki 10 haneli Vergi Kimlik Numarası (VKN) geçersiz.");
        }

        ValidationResult tcResult = TcKimlik.Validate(value.Slice(0, 11));
        return tcResult.IsValid
            ? ValidationResult.Success
            : ValidationResult.Failure("MERSİS numarası içerisindeki 11 haneli T.C. Kimlik Numarası geçersiz.");
    }

    /// <summary>
    /// TR: MERSİS Numarasını doğrular ve geçerliyse temizlenmiş (16 haneli) halini <paramref name="normalized"/> parametresiyle döner.
    /// <br/>
    /// EN: Attempts to validate the MERSIS number and outputs the trimmed 16-digit string if valid.
    /// </summary>
    /// <param name="value">TR: Doğrulanacak MERSİS numarası. / EN: The MERSIS number to validate.</param>
    /// <param name="normalized">TR: Geçerliyse temizlenmiş 16 haneli MERSİS numarası. / EN: The normalized 16-digit MERSIS number if valid.</param>
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
