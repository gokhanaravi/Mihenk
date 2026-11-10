namespace Mihenk.Communication;

/// <summary>
/// TR: Türkiye cep ve sabit telefon numarası doğrulama, normalizasyon, formatlama ve maskeleme araçları.
/// <br/>
/// EN: Turkish mobile and landline phone number validation, normalization, formatting, and masking utilities.
/// </summary>
public static partial class Telefon
{
    /// <summary>
    /// TR: Verilen telefon numarasının geçerli bir Türkiye cep, sabit veya kurumsal (850) numarası olup olmadığını kontrol eder.
    /// <br/>
    /// EN: Checks whether the given phone number is a valid Turkish mobile, landline, or corporate (850) number.
    /// </summary>
    /// <param name="phone">TR: Kontrol edilecek telefon numarası. / EN: The phone number string to validate.</param>
    public static bool IsValid(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        return IsValid(phone.AsSpan());
    }

    /// <summary>
    /// TR: Verilen karakter dizisinin geçerli bir Türkiye telefon numarası olup olmadığını kontrol eder (Zero-allocation).
    /// <br/>
    /// EN: Checks whether the given character span is a valid Turkish phone number (Zero-allocation).
    /// </summary>
    /// <param name="phone">TR: Kontrol edilecek karakter dizisi. / EN: The character span to validate.</param>
    public static bool IsValid(ReadOnlySpan<char> phone) => Validate(phone).IsValid;

    /// <summary>
    /// TR: Telefon numarasını doğrular ve geçersizse hata nedenini açıkça döner.
    /// <br/>
    /// EN: Validates the phone number and returns a detailed error reason if invalid.
    /// </summary>
    /// <param name="phone">TR: Doğrulanacak telefon numarası. / EN: The phone number string to validate.</param>
    public static ValidationResult Validate(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return ValidationResult.Failure("Telefon numarası boş olamaz.");

        return Validate(phone.AsSpan());
    }

    /// <summary>
    /// TR: Telefon numarasını doğrular ve geçersizse hata nedenini açıkça döner (Zero-allocation).
    /// <br/>
    /// EN: Validates the phone number span and returns a detailed error reason if invalid (Zero-allocation).
    /// </summary>
    /// <param name="phone">TR: Doğrulanacak karakter dizisi. / EN: The character span to validate.</param>
    public static ValidationResult Validate(ReadOnlySpan<char> phone)
    {
        if (phone.IsWhiteSpace())
            return ValidationResult.Failure("Telefon numarası boş olamaz.");

        Span<char> tenDigits = stackalloc char[10];
        if (!TryExtractTenDigitsWithError(phone, tenDigits, out string? error))
            return ValidationResult.Failure(error!);

        if (!IsValidSubscriberNumber(tenDigits))
            return ValidationResult.Failure("Telefon numarası geçerli bir Türkiye alan kodu (5XX, 2XX, 3XX, 4XX veya 850) ile başlamıyor.");

        return ValidationResult.Success;
    }

    /// <summary>
    /// TR: "0532...", "+90 532...", "90532..." ve "532..." gibi yazımların hepsini E.164 standart "+905321234567" formatına çevirir.
    /// <br/>
    /// EN: Normalizes various Turkish phone formats ("0532...", "+90 532...", "532...") into E.164 "+905321234567" format.
    /// </summary>
    /// <param name="phone">TR: Normalize edilecek telefon numarası. / EN: The phone number string to normalize.</param>
    public static string Normalize(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return string.Empty;

        Span<char> tenDigits = stackalloc char[10];
        if (!TryExtractTenDigitsWithError(phone.AsSpan(), tenDigits, out _) || !IsValidSubscriberNumber(tenDigits))
            return phone.Trim();

        Span<char> buffer = stackalloc char[13];
        buffer[0] = '+';
        buffer[1] = '9';
        buffer[2] = '0';
        tenDigits.CopyTo(buffer[3..]);

        return new string(buffer);
    }

    /// <summary>
    /// TR: "+90 (555) 316 76 76", "0555 316 7676", "905553167676" gibi yazımların hepsini
    /// başında 0 ya da 90 olmadan 10 haneli "5553167676" formatına çevirir.
    /// <br/>
    /// EN: Converts any Turkish phone format into a 10-digit subscriber number ("5553167676") without leading '0' or '90'.
    /// </summary>
    /// <param name="phone">TR: Dönüştürülecek telefon numarası. / EN: The phone number string to convert.</param>
    public static string ToShort(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return string.Empty;

        Span<char> tenDigits = stackalloc char[10];
        if (!TryExtractTenDigitsWithError(phone.AsSpan(), tenDigits, out _) || !IsValidSubscriberNumber(tenDigits))
            return phone.Trim();

        return new string(tenDigits);
    }

    /// <summary>
    /// TR: Telefon numarasını "0 (532) 123 45 67" okunabilir formatına çevirir.
    /// <br/>
    /// EN: Formats the phone number into readable "0 (532) 123 45 67" format.
    /// </summary>
    /// <param name="phone">TR: Formatlanacak telefon numarası. / EN: The phone number string to format.</param>
    public static string Format(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return string.Empty;

        Span<char> d = stackalloc char[10];
        if (!TryExtractTenDigitsWithError(phone.AsSpan(), d, out _) || !IsValidSubscriberNumber(d))
            return phone.Trim();

        Span<char> formatted = stackalloc char[17];
        formatted[0] = '0';
        formatted[1] = ' ';
        formatted[2] = '(';
        formatted[3] = d[0];
        formatted[4] = d[1];
        formatted[5] = d[2];
        formatted[6] = ')';
        formatted[7] = ' ';
        formatted[8] = d[3];
        formatted[9] = d[4];
        formatted[10] = d[5];
        formatted[11] = ' ';
        formatted[12] = d[6];
        formatted[13] = d[7];
        formatted[14] = ' ';
        formatted[15] = d[8];
        formatted[16] = d[9];

        return new string(formatted);
    }

    /// <summary>
    /// TR: Telefon numarasını KVKK uyumlu olarak maskeler (Örn: <c>"05321234567"</c> → <c>"0 (532) *** ** 67"</c>).
    /// <br/>
    /// EN: Masks the phone number for KVKK/GDPR privacy compliance (e.g., <c>"0 (532) *** ** 67"</c>).
    /// </summary>
    /// <param name="phone">TR: Maskelenecek telefon numarası. / EN: The phone number string to mask.</param>
    /// <param name="maskChar">TR: Maskeleme karakteri. / EN: Masking character.</param>
    public static string Mask(string? phone, char maskChar = '*')
    {
        if (string.IsNullOrWhiteSpace(phone))
            return string.Empty;

        if (!TryToShort(phone, out string? shortPhone))
            return phone.Trim();

        Span<char> buf = stackalloc char[17];
        buf[0] = '0';
        buf[1] = ' ';
        buf[2] = '(';
        buf[3] = shortPhone[0];
        buf[4] = shortPhone[1];
        buf[5] = shortPhone[2];
        buf[6] = ')';
        buf[7] = ' ';
        buf[8] = maskChar;
        buf[9] = maskChar;
        buf[10] = maskChar;
        buf[11] = ' ';
        buf[12] = maskChar;
        buf[13] = maskChar;
        buf[14] = ' ';
        buf[15] = shortPhone[8];
        buf[16] = shortPhone[9];

        return new string(buf);
    }

    /// <summary>
    /// TR: Verilen numaranın geçerli bir Türkiye cep telefonu numarası (5XX XXX XX XX) olup olmadığını kontrol eder.
    /// <br/>
    /// EN: Checks whether the given phone number is a valid Turkish mobile number (5XX XXX XX XX).
    /// </summary>
    /// <param name="phone">TR: Kontrol edilecek telefon numarası. / EN: The phone number string to check.</param>
    public static bool IsMobile(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        Span<char> tenDigits = stackalloc char[10];
        return TryExtractTenDigitsWithError(phone.AsSpan(), tenDigits, out _) && IsValidMobileSubscriber(tenDigits);
    }

    /// <summary>
    /// TR: Verilen numaranın geçerli bir Türkiye sabit hat numarası (2XX, 3XX, 4XX veya 850) olup olmadığını kontrol eder.
    /// <br/>
    /// EN: Checks whether the given phone number is a valid Turkish landline or corporate (850) number.
    /// </summary>
    /// <param name="phone">TR: Kontrol edilecek telefon numarası. / EN: The phone number string to check.</param>
    public static bool IsLandline(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        Span<char> tenDigits = stackalloc char[10];
        return TryExtractTenDigitsWithError(phone.AsSpan(), tenDigits, out _) && IsValidLandlineSubscriber(tenDigits);
    }

    /// <summary>
    /// TR: Metin içindeki Türkiye telefon numaralarını (cep, sabit ve 850) ayıklar ve salt okunur liste olarak döner.
    /// <br/>
    /// EN: Extracts all valid Turkish phone numbers from the given text as a read-only list.
    /// </summary>
    /// <param name="text">TR: Taranacak metin. / EN: The text to search.</param>
    public static IReadOnlyList<string> Extract(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return [];

        var results = new List<string>();
        foreach (System.Text.RegularExpressions.Match match in PhoneExtractRegex().Matches(text))
        {
            string raw = match.Value.Trim();
            if (IsValid(raw))
                results.Add(raw);
        }

        return results;
    }

    /// <summary>
    /// TR: Telefon numarasını doğrular ve geçerliyse "+905321234567" formatında <paramref name="normalized"/> ile döner.
    /// <br/>
    /// EN: Attempts to validate and normalize the phone number into "+905321234567" format.
    /// </summary>
    /// <param name="phone">TR: Telefon numarası. / EN: The phone number string.</param>
    /// <param name="normalized">TR: Normalize edilmiş telefon numarası. / EN: Normalized E.164 phone number.</param>
    public static bool TryNormalize(string? phone, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? normalized)
    {
        if (!string.IsNullOrWhiteSpace(phone))
        {
            Span<char> tenDigits = stackalloc char[10];
            if (TryExtractTenDigitsWithError(phone.AsSpan(), tenDigits, out _) && IsValidSubscriberNumber(tenDigits))
            {
                Span<char> buffer = stackalloc char[13];
                buffer[0] = '+';
                buffer[1] = '9';
                buffer[2] = '0';
                tenDigits.CopyTo(buffer[3..]);
                normalized = new string(buffer);
                return true;
            }
        }

        normalized = null;
        return false;
    }

    /// <summary>
    /// TR: Telefon numarasını doğrular ve geçerliyse başında 0/90 olmadan 10 haneli "5321234567" formatında <paramref name="shortPhone"/> ile döner.
    /// <br/>
    /// EN: Attempts to validate and convert the phone number into a 10-digit subscriber number ("5321234567").
    /// </summary>
    /// <param name="phone">TR: Telefon numarası. / EN: The phone number string.</param>
    /// <param name="shortPhone">TR: 10 haneli kısa telefon numarası. / EN: 10-digit short phone number.</param>
    public static bool TryToShort(string? phone, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? shortPhone)
    {
        if (!string.IsNullOrWhiteSpace(phone))
        {
            Span<char> tenDigits = stackalloc char[10];
            if (TryExtractTenDigitsWithError(phone.AsSpan(), tenDigits, out _) && IsValidSubscriberNumber(tenDigits))
            {
                shortPhone = new string(tenDigits);
                return true;
            }
        }

        shortPhone = null;
        return false;
    }

    /// <summary>
    /// TR: Telefon numarasını doğrular ve geçerliyse "0 (532) 123 45 67" formatında <paramref name="formatted"/> ile döner.
    /// <br/>
    /// EN: Attempts to validate and format the phone number into "0 (532) 123 45 67" format.
    /// </summary>
    /// <param name="phone">TR: Telefon numarası. / EN: The phone number string.</param>
    /// <param name="formatted">TR: Formatlanmış telefon numarası. / EN: Formatted phone number string.</param>
    public static bool TryFormat(string? phone, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? formatted)
    {
        if (TryToShort(phone, out string? shortPhone))
        {
            formatted = Format(shortPhone);
            return true;
        }

        formatted = null;
        return false;
    }

    /// <summary>
    /// TR: Telefon numarasını KVKK formatında maskelemeyi dener.
    /// <br/>
    /// EN: Attempts to validate and mask the phone number.
    /// </summary>
    /// <param name="phone">TR: Telefon numarası. / EN: The phone number string.</param>
    /// <param name="masked">TR: Maskelenmiş telefon numarası. / EN: Masked phone number string.</param>
    /// <param name="maskChar">TR: Maskeleme karakteri. / EN: Masking character.</param>
    public static bool TryMask(string? phone, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? masked, char maskChar = '*')
    {
        if (TryToShort(phone, out string? shortPhone))
        {
            masked = Mask(shortPhone, maskChar);
            return true;
        }

        masked = null;
        return false;
    }

    [System.Text.RegularExpressions.GeneratedRegex(@"(?<!\d)(?:\+90|0090|090|90|0)?[\s\.\-]*\(?(?:5\d{2}|[234]\d{2}|850)\)?[\s\.\-]*\d{3}[\s\.\-]*\d{2}[\s\.\-]*\d{2}(?!\d)", System.Text.RegularExpressions.RegexOptions.CultureInvariant)]
    private static partial System.Text.RegularExpressions.Regex PhoneExtractRegex();

    private static bool TryExtractTenDigitsWithError(ReadOnlySpan<char> input, Span<char> output10, out string? error)
    {
        error = null;
        Span<char> rawDigits = stackalloc char[16];
        int count = 0;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (c is ' ' or '-' or '(' or ')' or '.' or '+')
                continue;

            if ((uint)(c - '0') > 9)
            {
                error = "Telefon numarası geçersiz karakterler içeriyor.";
                return false;
            }

            if (count >= rawDigits.Length)
            {
                error = "Telefon numarası çok uzun.";
                return false;
            }

            rawDigits[count++] = c;
        }

        ReadOnlySpan<char> slice = rawDigits[..count];

        if (slice.Length == 14 && slice.StartsWith("0090".AsSpan()))
        {
            slice.Slice(4, 10).CopyTo(output10);
            return true;
        }

        if (slice.Length == 13 && slice.StartsWith("090".AsSpan()))
        {
            slice.Slice(3, 10).CopyTo(output10);
            return true;
        }

        if (slice.Length == 12 && slice.StartsWith("90".AsSpan()))
        {
            slice.Slice(2, 10).CopyTo(output10);
            return true;
        }

        if (slice.Length == 11 && slice[0] == '0')
        {
            slice.Slice(1, 10).CopyTo(output10);
            return true;
        }

        if (slice.Length == 10)
        {
            slice.CopyTo(output10);
            return true;
        }

        error = "Telefon numarası (ülke kodu hariç) 10 haneli olmalıdır.";
        return false;
    }

    private static bool IsValidSubscriberNumber(ReadOnlySpan<char> d)
        => IsValidMobileSubscriber(d) || IsValidLandlineSubscriber(d);

    private static bool IsValidMobileSubscriber(ReadOnlySpan<char> d)
    {
        if (d.Length != 10 || d[0] != '5')
            return false;

        char second = d[1];
        return second is '0' or '1' or '2' or '3' or '4' or '5' or '6' or '9';
    }

    private static bool IsValidLandlineSubscriber(ReadOnlySpan<char> d)
    {
        if (d.Length != 10)
            return false;

        if (d[0] is '2' or '3' or '4')
            return true;

        if (d[0] == '8' && d[1] == '5' && d[2] == '0')
            return true;

        return false;
    }
}
