namespace Mihenk.Commerce;

/// <summary>
/// TR: Ürün barkodu (EAN-8, UPC-A, EAN-13, GTIN-14) doğrulama ve GS1 Türkiye ön eki (868/869) kontrol araçları.
/// <br/>
/// EN: Product barcode (EAN-8, UPC-A, EAN-13, GTIN-14) validation and GS1 Turkey prefix (868/869) verification utilities.
/// </summary>
public static class Barkod
{
    /// <summary>
    /// TR: Verilen barkod numarasının GS1 standartlarına (8, 12, 13 veya 14 haneli GTIN/EAN kontrol basamağı algoritmasına) göre geçerli olup olmadığını kontrol eder.
    /// <br/>
    /// EN: Checks whether the given barcode number is valid according to GS1 check-digit standards (8, 12, 13, or 14 digits).
    /// </summary>
    /// <param name="barcode">TR: Kontrol edilecek barkod numarası. / EN: The barcode string to validate.</param>
    public static bool IsValid(string? barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            return false;

        return IsValid(barcode.AsSpan().Trim());
    }

    /// <summary>
    /// TR: Verilen karakter dizisinin geçerli bir EAN/GTIN barkodu olup olmadığını kontrol eder (Zero-allocation).
    /// <br/>
    /// EN: Checks whether the given character span is a valid EAN/GTIN barcode (Zero-allocation).
    /// </summary>
    /// <param name="barcode">TR: Kontrol edilecek karakter dizisi. / EN: The character span to validate.</param>
    public static bool IsValid(ReadOnlySpan<char> barcode) => Validate(barcode).IsValid;

    /// <summary>
    /// TR: Barkod numarasını doğrular ve geçersizse hata nedenini açıkça döner.
    /// <br/>
    /// EN: Validates the barcode number and returns a detailed error reason if invalid.
    /// </summary>
    /// <param name="barcode">TR: Doğrulanacak barkod numarası. / EN: The barcode string to validate.</param>
    public static ValidationResult Validate(string? barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            return ValidationResult.Failure("Barkod numarası boş olamaz.");

        return Validate(barcode.AsSpan().Trim());
    }

    /// <summary>
    /// TR: Barkod numarasını doğrular ve geçersizse hata nedenini açıkça döner (Zero-allocation).
    /// <br/>
    /// EN: Validates the barcode span and returns a detailed error reason if invalid (Zero-allocation).
    /// </summary>
    /// <param name="barcode">TR: Doğrulanacak karakter dizisi. / EN: The character span to validate.</param>
    public static ValidationResult Validate(ReadOnlySpan<char> barcode)
    {
        if (barcode.IsEmpty)
            return ValidationResult.Failure("Barkod numarası boş olamaz.");

        int len = barcode.Length;
        if (len is not (8 or 12 or 13 or 14))
            return ValidationResult.Failure("Barkod numarası 8 (EAN-8), 12 (UPC-A), 13 (EAN-13) veya 14 (GTIN-14) haneli olmalıdır.");

        int sum = 0;
        bool anyNonZero = false;

        bool weight3 = true;
        for (int i = len - 2; i >= 0; i--)
        {
            int d = barcode[i] - '0';
            if ((uint)d > 9)
                return ValidationResult.Failure("Barkod numarası yalnızca rakamlardan oluşmalıdır.");

            if (d != 0)
                anyNonZero = true;

            sum += weight3 ? d * 3 : d;
            weight3 = !weight3;
        }

        int checkDigit = barcode[len - 1] - '0';
        if ((uint)checkDigit > 9)
            return ValidationResult.Failure("Barkod numarası yalnızca rakamlardan oluşmalıdır.");

        if (!anyNonZero)
            return ValidationResult.Failure("Barkod numarası tamamen sıfırlardan oluşamaz.");

        int expectedCheck = (10 - (sum % 10)) % 10;
        return checkDigit == expectedCheck
            ? ValidationResult.Success
            : ValidationResult.Failure("Barkod numarası GS1 kontrol basamağı algoritmasına uymuyor.");
    }

    /// <summary>
    /// TR: Barkodun geçerli bir EAN-13 barkodu olup olmadığını ve GS1 Türkiye (TOBB) ülke ön ekiyle (<c>868</c> veya <c>869</c>)
    /// Türkiye'de kayıtlı bir firmaya ait olup olmadığını kontrol eder.
    /// <br/>
    /// EN: Checks whether the barcode is a valid 13-digit EAN-13 registered under GS1 Turkey prefixes (<c>868</c> or <c>869</c>).
    /// </summary>
    /// <param name="barcode">TR: Kontrol edilecek barkod numarası. / EN: The barcode string to check.</param>
    public static bool IsTurkeyGs1(string? barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            return false;

        ReadOnlySpan<char> trimmed = barcode.AsSpan().Trim();
        return trimmed.Length == 13
            && (trimmed.StartsWith("868".AsSpan()) || trimmed.StartsWith("869".AsSpan()))
            && IsValid(trimmed);
    }

    /// <summary>
    /// TR: Barkodu doğrular ve geçerliyse temizlenmiş halini <paramref name="normalized"/> ile döner.
    /// <br/>
    /// EN: Attempts to validate the barcode and outputs the trimmed string if valid.
    /// </summary>
    /// <param name="barcode">TR: Doğrulanacak barkod numarası. / EN: The barcode string to validate.</param>
    /// <param name="normalized">TR: Temizlenmiş barkod numarası. / EN: Normalized barcode string.</param>
    public static bool TryValidate(string? barcode, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? normalized)
    {
        if (!string.IsNullOrWhiteSpace(barcode))
        {
            ReadOnlySpan<char> trimmed = barcode.AsSpan().Trim();
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

/// <summary>
/// TR: Mobil cihaz 15 haneli IMEI (International Mobile Equipment Identity) numarası doğrulama araçları.
/// <br/>
/// EN: 15-digit mobile device IMEI (International Mobile Equipment Identity) Luhn validation utilities.
/// </summary>
public static class Imei
{
    /// <summary>
    /// TR: Verilen değerin 15 haneli Luhn algoritmasına uygun geçerli bir IMEI numarası olup olmadığını kontrol eder.
    /// <br/>
    /// EN: Checks whether the given value is a valid 15-digit IMEI number according to the Luhn algorithm.
    /// </summary>
    /// <param name="imei">TR: Kontrol edilecek IMEI numarası. / EN: The IMEI string to validate.</param>
    public static bool IsValid(string? imei)
    {
        if (string.IsNullOrWhiteSpace(imei))
            return false;

        return IsValid(imei.AsSpan());
    }

    /// <summary>
    /// TR: Verilen karakter dizisinin 15 haneli geçerli bir IMEI numarası olup olmadığını kontrol eder (Zero-allocation).
    /// <br/>
    /// EN: Checks whether the given character span is a valid 15-digit IMEI number (Zero-allocation).
    /// </summary>
    /// <param name="imei">TR: Kontrol edilecek karakter dizisi. / EN: The character span to validate.</param>
    public static bool IsValid(ReadOnlySpan<char> imei) => Validate(imei).IsValid;

    /// <summary>
    /// TR: IMEI numarasını doğrular ve geçersizse hata nedenini açıkça döner.
    /// <br/>
    /// EN: Validates the IMEI number and returns a detailed error reason if invalid.
    /// </summary>
    /// <param name="imei">TR: Doğrulanacak IMEI numarası. / EN: The IMEI string to validate.</param>
    public static ValidationResult Validate(string? imei)
    {
        if (string.IsNullOrWhiteSpace(imei))
            return ValidationResult.Failure("IMEI numarası boş olamaz.");

        return Validate(imei.AsSpan());
    }

    /// <summary>
    /// TR: IMEI numarasını doğrular ve geçersizse hata nedenini açıkça döner (Zero-allocation).
    /// <br/>
    /// EN: Validates the IMEI span and returns a detailed error reason if invalid (Zero-allocation).
    /// </summary>
    /// <param name="imei">TR: Doğrulanacak karakter dizisi. / EN: The character span to validate.</param>
    public static ValidationResult Validate(ReadOnlySpan<char> imei)
    {
        if (imei.IsWhiteSpace())
            return ValidationResult.Failure("IMEI numarası boş olamaz.");

        Span<char> digits = stackalloc char[15];
        int idx = 0;

        for (int i = 0; i < imei.Length; i++)
        {
            char c = imei[i];
            if (c is ' ' or '-' or '/')
                continue;

            if ((uint)(c - '0') > 9)
                return ValidationResult.Failure("IMEI numarası yalnızca rakamlardan oluşmalıdır.");

            if (idx >= 15)
                return ValidationResult.Failure("IMEI numarası 15 haneli olmalıdır.");

            digits[idx++] = c;
        }

        if (idx != 15)
            return ValidationResult.Failure("IMEI numarası 15 haneli olmalıdır.");

        int sum = 0;
        bool anyNonZero = false;

        for (int i = 0; i < 15; i++)
        {
            int d = digits[i] - '0';
            if (d != 0)
                anyNonZero = true;

            if ((i & 1) == 1)
            {
                d *= 2;
                if (d > 9)
                    d -= 9;
            }

            sum += d;
        }

        if (!anyNonZero)
            return ValidationResult.Failure("IMEI numarası tamamen sıfırlardan oluşamaz.");

        return (sum % 10 == 0)
            ? ValidationResult.Success
            : ValidationResult.Failure("IMEI numarası Luhn kontrol algoritmasına uymuyor.");
    }

    /// <summary>
    /// TR: IMEI numarasını doğrular ve geçerliyse boşluksuz/tiresiz 15 haneli halini <paramref name="normalized"/> ile döner.
    /// <br/>
    /// EN: Attempts to validate the IMEI number and outputs the unspaced 15-digit string if valid.
    /// </summary>
    /// <param name="imei">TR: Doğrulanacak IMEI numarası. / EN: The IMEI string to validate.</param>
    /// <param name="normalized">TR: Boşluksuz 15 haneli IMEI numarası. / EN: Normalized 15-digit IMEI number.</param>
    public static bool TryValidate(string? imei, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? normalized)
    {
        if (!string.IsNullOrWhiteSpace(imei))
        {
            Span<char> digits = stackalloc char[15];
            int idx = 0;
            ReadOnlySpan<char> span = imei.AsSpan();

            for (int i = 0; i < span.Length; i++)
            {
                char c = span[i];
                if (c is ' ' or '-' or '/')
                    continue;

                if ((uint)(c - '0') > 9 || idx >= 15)
                {
                    normalized = null;
                    return false;
                }

                digits[idx++] = c;
            }

            if (idx == 15 && IsValid(digits))
            {
                normalized = new string(digits);
                return true;
            }
        }

        normalized = null;
        return false;
    }
}
