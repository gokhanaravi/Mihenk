namespace Mihenk.Identity;

/// <summary>
/// TR: T.C. Kimlik Numarası doğrulama ve maskeleme araçları.
/// <br/>
/// EN: Turkish National Identification Number (TCKN) validation and masking utilities.
/// </summary>
public static class TcKimlik
{
    /// <summary>
    /// TR: Verilen değerin geçerli bir T.C. Kimlik Numarası olup olmadığını kontrol eder.
    /// <br/>
    /// EN: Checks whether the given value is a valid 11-digit Turkish National ID (TCKN).
    /// </summary>
    /// <param name="value">TR: Kontrol edilecek T.C. Kimlik Numarası. / EN: The TCKN string to validate.</param>
    public static bool IsValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return IsValid(value.AsSpan().Trim());
    }

    /// <summary>
    /// TR: Verilen karakter dizisinin geçerli bir T.C. Kimlik Numarası olup olmadığını kontrol eder (Zero-allocation).
    /// <br/>
    /// EN: Checks whether the given character span is a valid 11-digit Turkish National ID (Zero-allocation).
    /// </summary>
    /// <param name="value">TR: Kontrol edilecek karakter dizisi. / EN: The character span to validate.</param>
    public static bool IsValid(ReadOnlySpan<char> value) => Validate(value).IsValid;

    /// <summary>
    /// TR: T.C. Kimlik Numarasını doğrular ve geçersizse hata nedenini açıkça döner.
    /// <br/>
    /// EN: Validates the Turkish National ID and returns a detailed error reason if invalid.
    /// </summary>
    /// <param name="value">TR: Doğrulanacak T.C. Kimlik Numarası. / EN: The TCKN value to validate.</param>
    public static ValidationResult Validate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return ValidationResult.Failure("T.C. Kimlik Numarası boş olamaz.");

        return Validate(value.AsSpan().Trim());
    }

    /// <summary>
    /// TR: T.C. Kimlik Numarasını doğrular ve geçersizse hata nedenini açıkça döner (Zero-allocation).
    /// <br/>
    /// EN: Validates the Turkish National ID span and returns a detailed error reason if invalid (Zero-allocation).
    /// </summary>
    /// <param name="value">TR: Doğrulanacak karakter dizisi. / EN: The TCKN character span to validate.</param>
    public static ValidationResult Validate(ReadOnlySpan<char> value)
    {
        if (value.IsEmpty)
            return ValidationResult.Failure("T.C. Kimlik Numarası boş olamaz.");

        if (value.Length != 11)
            return ValidationResult.Failure("T.C. Kimlik Numarası 11 haneli olmalıdır.");

        for (int i = 0; i < 11; i++)
        {
            if ((uint)(value[i] - '0') > 9)
                return ValidationResult.Failure("T.C. Kimlik Numarası yalnızca rakamlardan oluşmalıdır.");
        }

        if (value[0] == '0')
            return ValidationResult.Failure("T.C. Kimlik Numarasının ilk hanesi 0 olamaz.");

        int oddSum = 0;
        int evenSum = 0;

        for (int i = 0; i < 9; i++)
        {
            int digit = value[i] - '0';
            if ((i & 1) == 0)
                oddSum += digit;
            else
                evenSum += digit;
        }

        int d10 = value[9] - '0';
        int d11 = value[10] - '0';

        int expectedD10 = ((oddSum * 7) - evenSum) % 10;
        if (expectedD10 < 0)
            expectedD10 += 10;

        if (d10 != expectedD10)
            return ValidationResult.Failure("T.C. Kimlik Numarası 10. hane kontrol algoritmasına uymuyor.");

        int expectedD11 = (oddSum + evenSum + d10) % 10;
        if (d11 != expectedD11)
            return ValidationResult.Failure("T.C. Kimlik Numarası 11. hane kontrol algoritmasına uymuyor.");

        return ValidationResult.Success;
    }

    /// <summary>
    /// TR: KVKK kapsamında T.C. Kimlik Numarasını <c>123******01</c> formatında maskeler.
    /// <br/>
    /// EN: Masks the Turkish National ID in <c>123******01</c> format for KVKK/GDPR privacy compliance.
    /// </summary>
    /// <param name="value">TR: Maskelenecek T.C. Kimlik Numarası. / EN: The TCKN value to mask.</param>
    /// <param name="maskChar">TR: Maskeleme karakteri (varsayılan '*'). / EN: Masking character (default '*').</param>
    public static string Mask(string? value, char maskChar = '*')
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        ReadOnlySpan<char> trimmed = value.AsSpan().Trim();
        if (trimmed.Length != 11)
            return trimmed.ToString();

        return string.Create(11, (tc: trimmed.ToString(), maskChar), static (span, state) =>
        {
            ReadOnlySpan<char> src = state.tc.AsSpan();
            span[0] = src[0];
            span[1] = src[1];
            span[2] = src[2];
            span.Slice(3, 6).Fill(state.maskChar);
            span[9] = src[9];
            span[10] = src[10];
        });
    }

    /// <summary>
    /// TR: T.C. Kimlik Numarasını doğrular ve geçerliyse temizlenmiş (11 haneli) halini <paramref name="normalized"/> parametresiyle döner.
    /// <br/>
    /// EN: Attempts to validate the Turkish National ID and outputs the trimmed 11-digit string if valid.
    /// </summary>
    /// <param name="value">TR: Doğrulanacak değer. / EN: The value to validate.</param>
    /// <param name="normalized">TR: Geçerliyse temizlenmiş 11 haneli TCKN. / EN: The normalized 11-digit TCKN if valid.</param>
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
    /// TR: T.C. Kimlik Numarasını KVKK formatında maskelemeyi dener.
    /// <br/>
    /// EN: Attempts to validate and mask the Turkish National ID.
    /// </summary>
    /// <param name="value">TR: Maskelenecek TCKN. / EN: The TCKN value to mask.</param>
    /// <param name="masked">TR: Maskelenmiş sonuç. / EN: The masked output string.</param>
    /// <param name="maskChar">TR: Maskeleme karakteri. / EN: Masking character.</param>
    public static bool TryMask(string? value, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? masked, char maskChar = '*')
    {
        if (TryValidate(value, out string? validTc))
        {
            masked = Mask(validTc, maskChar);
            return true;
        }

        masked = null;
        return false;
    }
}
