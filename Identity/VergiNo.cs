namespace Mihenk.Identity;

/// <summary>
/// TR: Vergi Kimlik Numarası (VKN) doğrulama araçları.
/// <br/>
/// EN: Turkish Tax Identification Number (VKN) validation utilities.
/// </summary>
public static class VergiNo
{
    /// <summary>
    /// TR: Verilen değerin 10 haneli geçerli bir Vergi Kimlik Numarası (VKN) olup olmadığını kontrol eder.
    /// <br/>
    /// EN: Checks whether the given value is a valid 10-digit Turkish Tax Identification Number (VKN).
    /// </summary>
    /// <param name="value">TR: Kontrol edilecek VKN. / EN: The VKN string to validate.</param>
    public static bool IsValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return IsValid(value.AsSpan().Trim());
    }

    /// <summary>
    /// TR: Verilen karakter dizisinin 10 haneli geçerli bir Vergi Kimlik Numarası (VKN) olup olmadığını kontrol eder (Zero-allocation).
    /// <br/>
    /// EN: Checks whether the given character span is a valid 10-digit VKN (Zero-allocation).
    /// </summary>
    /// <param name="value">TR: Kontrol edilecek karakter dizisi. / EN: The character span to validate.</param>
    public static bool IsValid(ReadOnlySpan<char> value) => Validate(value).IsValid;

    /// <summary>
    /// TR: Vergi Kimlik Numarasını (VKN) doğrular ve geçersizse hata nedenini açıkça döner.
    /// <br/>
    /// EN: Validates the VKN and returns a detailed error reason if invalid.
    /// </summary>
    /// <param name="value">TR: Doğrulanacak VKN. / EN: The VKN string to validate.</param>
    public static ValidationResult Validate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return ValidationResult.Failure("Vergi Kimlik Numarası boş olamaz.");

        return Validate(value.AsSpan().Trim());
    }

    /// <summary>
    /// TR: Vergi Kimlik Numarasını (VKN) doğrular ve geçersizse hata nedenini açıkça döner (Zero-allocation).
    /// <br/>
    /// EN: Validates the VKN span and returns a detailed error reason if invalid (Zero-allocation).
    /// </summary>
    /// <param name="value">TR: Doğrulanacak karakter dizisi. / EN: The character span to validate.</param>
    public static ValidationResult Validate(ReadOnlySpan<char> value)
    {
        if (value.IsEmpty)
            return ValidationResult.Failure("Vergi Kimlik Numarası boş olamaz.");

        if (value.Length != 10)
            return ValidationResult.Failure("Vergi Kimlik Numarası 10 haneli olmalıdır.");

        int sum = 0;

        for (int i = 0; i < 9; i++)
        {
            int digit = value[i] - '0';
            if ((uint)digit > 9)
                return ValidationResult.Failure("Vergi Kimlik Numarası yalnızca rakamlardan oluşmalıdır.");

            int weight = 9 - i;
            int tmp = (digit + weight) % 10;

            if (tmp == 9)
            {
                sum += 9;
            }
            else if (tmp != 0)
            {
                int mod9 = (tmp * (1 << weight)) % 9;
                sum += mod9;
            }
        }

        int lastDigit = value[9] - '0';
        if ((uint)lastDigit > 9)
            return ValidationResult.Failure("Vergi Kimlik Numarası yalnızca rakamlardan oluşmalıdır.");

        int expectedLastDigit = (10 - (sum % 10)) % 10;
        if (lastDigit != expectedLastDigit)
            return ValidationResult.Failure("Vergi Kimlik Numarası kontrol basamağı algoritmasına uymuyor.");

        return ValidationResult.Success;
    }

    /// <summary>
    /// TR: Verilen değer 10 haneliyse VKN, 11 haneliyse T.C. Kimlik Numarası algoritmasına göre doğrular.
    /// <br/>
    /// EN: Validates the value as a 10-digit VKN or an 11-digit TCKN depending on its length.
    /// </summary>
    /// <param name="value">TR: Kontrol edilecek VKN veya TCKN. / EN: The VKN or TCKN string to validate.</param>
    public static bool IsValidTcOrVkn(string? value) => ValidateTcOrVkn(value).IsValid;

    /// <summary>
    /// TR: Verilen değeri 10 haneliyse VKN, 11 haneliyse TCKN olarak doğrular ve geçersizse hata nedenini döner.
    /// <br/>
    /// EN: Validates the value as a 10-digit VKN or 11-digit TCKN and returns an error reason if invalid.
    /// </summary>
    /// <param name="value">TR: Doğrulanacak VKN veya TCKN. / EN: The VKN or TCKN string to validate.</param>
    public static ValidationResult ValidateTcOrVkn(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return ValidationResult.Failure("T.C. Kimlik veya Vergi Kimlik Numarası boş olamaz.");

        ReadOnlySpan<char> trimmed = value.AsSpan().Trim();
        return trimmed.Length switch
        {
            10 => Validate(trimmed),
            11 => TcKimlik.Validate(trimmed),
            _ => ValidationResult.Failure("Değer 10 haneli Vergi Kimlik Numarası veya 11 haneli T.C. Kimlik Numarası olmalıdır.")
        };
    }

    /// <summary>
    /// TR: VKN değerini doğrular ve geçerliyse temizlenmiş (10 haneli) halini <paramref name="normalized"/> parametresiyle döner.
    /// <br/>
    /// EN: Attempts to validate the VKN and outputs the trimmed 10-digit string if valid.
    /// </summary>
    /// <param name="value">TR: Doğrulanacak VKN. / EN: The VKN string to validate.</param>
    /// <param name="normalized">TR: Geçerliyse temizlenmiş 10 haneli VKN. / EN: The normalized 10-digit VKN if valid.</param>
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

    /// <summary>
    /// TR: TCKN veya VKN değerini doğrular ve geçerliyse temizlenmiş halini <paramref name="normalized"/> parametresiyle döner.
    /// <br/>
    /// EN: Attempts to validate the value as TCKN or VKN and outputs the trimmed string if valid.
    /// </summary>
    /// <param name="value">TR: Doğrulanacak TCKN veya VKN. / EN: The TCKN or VKN string to validate.</param>
    /// <param name="normalized">TR: Geçerliyse temizlenmiş değer. / EN: The normalized string if valid.</param>
    public static bool TryValidateTcOrVkn(string? value, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? normalized)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            ReadOnlySpan<char> trimmed = value.AsSpan().Trim();
            if ((trimmed.Length == 10 && IsValid(trimmed)) || (trimmed.Length == 11 && TcKimlik.IsValid(trimmed)))
            {
                normalized = trimmed.ToString();
                return true;
            }
        }

        normalized = null;
        return false;
    }
}
