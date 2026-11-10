namespace Mihenk.Vehicle;

/// <summary>
/// TR: Araç Şasi Numarası (VIN - Vehicle Identification Number) doğrulama araçları.
/// <br/>
/// EN: Vehicle Identification Number (VIN - ISO 3779 17-character) validation utilities.
/// </summary>
public static class SasiNo
{
    /// <summary>
    /// TR: Verilen değerin ISO 3779 standardına uygun 17 haneli geçerli bir Şasi Numarası (VIN) olup olmadığını kontrol eder.
    /// <br/>
    /// EN: Checks whether the given value is a valid 17-character VIN conforming to ISO 3779.
    /// </summary>
    /// <param name="vin">TR: Kontrol edilecek VIN. / EN: The VIN string to validate.</param>
    public static bool IsValid(string? vin)
    {
        if (string.IsNullOrWhiteSpace(vin))
            return false;

        return IsValid(vin.AsSpan().Trim());
    }

    /// <summary>
    /// TR: Verilen karakter dizisinin 17 haneli geçerli bir Şasi Numarası (VIN) olup olmadığını kontrol eder (Zero-allocation).
    /// <br/>
    /// EN: Checks whether the given character span is a valid 17-character VIN (Zero-allocation).
    /// </summary>
    /// <param name="vin">TR: Kontrol edilecek karakter dizisi. / EN: The character span to validate.</param>
    public static bool IsValid(ReadOnlySpan<char> vin) => Validate(vin).IsValid;

    /// <summary>
    /// TR: Şasi numarasını (VIN) doğrular ve geçersizse hata nedenini açıkça döner.
    /// <br/>
    /// EN: Validates the VIN and returns a detailed error reason if invalid.
    /// </summary>
    /// <param name="vin">TR: Doğrulanacak VIN. / EN: The VIN string to validate.</param>
    public static ValidationResult Validate(string? vin)
    {
        if (string.IsNullOrWhiteSpace(vin))
            return ValidationResult.Failure("Şasi numarası (VIN) boş olamaz.");

        return Validate(vin.AsSpan().Trim());
    }

    /// <summary>
    /// TR: Şasi numarasını (VIN) doğrular ve geçersizse hata nedenini açıkça döner (Zero-allocation).
    /// <br/>
    /// EN: Validates the VIN span and returns a detailed error reason if invalid (Zero-allocation).
    /// </summary>
    /// <param name="vin">TR: Doğrulanacak karakter dizisi. / EN: The character span to validate.</param>
    public static ValidationResult Validate(ReadOnlySpan<char> vin)
    {
        if (vin.IsEmpty)
            return ValidationResult.Failure("Şasi numarası (VIN) boş olamaz.");

        if (vin.Length != 17)
            return ValidationResult.Failure("Şasi numarası (VIN) 17 karakter olmalıdır.");

        for (int i = 0; i < 17; i++)
        {
            char c = vin[i];

            if ((uint)(c - '0') <= 9)
                continue;

            if (c >= 'a' && c <= 'z')
                c = (char)(c - 32);

            if (c < 'A' || c > 'Z')
                return ValidationResult.Failure("Şasi numarası (VIN) yalnızca İngilizce harf ve rakamlardan oluşmalıdır.");

            if (c is 'I' or 'O' or 'Q')
                return ValidationResult.Failure($"Şasi numarası (VIN) ISO 3779 standardına göre '{c}' harfini içeremez.");
        }

        return ValidationResult.Success;
    }

    /// <summary>
    /// TR: Şasi numarasını (VIN) doğrular ve geçerliyse büyük harfe çevrilmiş 17 haneli halini <paramref name="normalized"/> ile döner.
    /// <br/>
    /// EN: Attempts to validate the VIN and outputs the uppercase 17-character string if valid.
    /// </summary>
    /// <param name="vin">TR: Doğrulanacak VIN. / EN: The VIN string to validate.</param>
    /// <param name="normalized">TR: Büyük harfe çevrilmiş 17 karakterlik VIN. / EN: Normalized 17-character uppercase VIN.</param>
    public static bool TryValidate(string? vin, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? normalized)
    {
        if (!string.IsNullOrWhiteSpace(vin))
        {
            ReadOnlySpan<char> trimmed = vin.AsSpan().Trim();
            if (IsValid(trimmed))
            {
                normalized = trimmed.ToString().ToUpperInvariant();
                return true;
            }
        }

        normalized = null;
        return false;
    }
}
