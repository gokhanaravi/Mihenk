namespace Mihenk.Finance;

/// <summary>
/// TR: Türkiye IBAN (Uluslararası Banka Hesap Numarası) doğrulama, formatlama, maskeleme ve banka çözümleme araçları.
/// <br/>
/// EN: Turkish IBAN (26-character ISO 7064 Mod-97) validation, formatting, masking, and bank lookup utilities.
/// </summary>
public static class Iban
{
    private static readonly Dictionary<string, string> Banks = new(StringComparer.Ordinal)
    {
        ["00001"] = "Türkiye Cumhuriyet Merkez Bankası",
        ["00004"] = "İller Bankası",
        ["00010"] = "Ziraat Bankası",
        ["00012"] = "Halkbank",
        ["00015"] = "VakıfBank",
        ["00016"] = "Türk Eximbank",
        ["00017"] = "Türkiye Kalkınma ve Yatırım Bankası",
        ["00029"] = "Birleşik Fon Bankası",
        ["00032"] = "Türk Ekonomi Bankası (TEB)",
        ["00046"] = "Akbank",
        ["00059"] = "Şekerbank",
        ["00062"] = "Garanti BBVA",
        ["00064"] = "Türkiye İş Bankası",
        ["00067"] = "Yapı ve Kredi Bankası",
        ["00091"] = "Arap Türk Bankası",
        ["00092"] = "Citibank",
        ["00096"] = "Turkish Bank",
        ["00099"] = "ING Bank",
        ["00100"] = "Adabank",
        ["00103"] = "Fibabanka",
        ["00108"] = "Turkland Bank",
        ["00109"] = "ICBC Turkey Bank",
        ["00111"] = "QNB",
        ["00115"] = "Deutsche Bank",
        ["00123"] = "HSBC",
        ["00124"] = "Alternatifbank",
        ["00125"] = "Burgan Bank",
        ["00134"] = "DenizBank",
        ["00135"] = "Anadolubank",
        ["00143"] = "Aktif Yatırım Bankası",
        ["00146"] = "Odea Bank",
        ["00147"] = "Bank of Tokyo-Mitsubishi UFJ Turkey",
        ["00149"] = "Rabobank",
        ["00150"] = "Golden Global Yatırım Bankası",
        ["00203"] = "Albaraka Türk Katılım Bankası",
        ["00205"] = "Kuveyt Türk Katılım Bankası",
        ["00206"] = "Türkiye Finans Katılım Bankası",
        ["00209"] = "Ziraat Katılım Bankası",
        ["00210"] = "Vakıf Katılım Bankası",
        ["00211"] = "Türkiye Emlak Katılım Bankası",
        ["00212"] = "Hayat Finans Katılım Bankası",
        ["00213"] = "TOM Katılım Bankası",
        ["00214"] = "Dünya Katılım Bankası",
        ["00806"] = "Merkezi Kayıt Kuruluşu",
        ["00807"] = "Posta ve Telgraf Teşkilatı (PTT)"
    };

    /// <summary>
    /// TR: Verilen değerin geçerli bir Türkiye IBAN'ı (TR ile başlayan, 26 karakter, ISO 7064 Mod-97-10 algoritmasına uygun) olup olmadığını kontrol eder.
    /// <br/>
    /// EN: Checks whether the given value is a valid 26-character Turkish IBAN starting with 'TR' and passing Mod-97 verification.
    /// </summary>
    /// <param name="iban">TR: Kontrol edilecek IBAN. / EN: The IBAN string to validate.</param>
    public static bool IsValid(string? iban)
    {
        if (string.IsNullOrWhiteSpace(iban))
            return false;

        return IsValid(iban.AsSpan());
    }

    /// <summary>
    /// TR: Verilen karakter dizisinin geçerli bir Türkiye IBAN'ı olup olmadığını kontrol eder (Zero-allocation).
    /// <br/>
    /// EN: Checks whether the given character span is a valid Turkish IBAN (Zero-allocation).
    /// </summary>
    /// <param name="iban">TR: Kontrol edilecek karakter dizisi. / EN: The character span to validate.</param>
    public static bool IsValid(ReadOnlySpan<char> iban) => Validate(iban).IsValid;

    /// <summary>
    /// TR: TR IBAN numarasını doğrular ve geçersizse hata nedenini açıkça döner.
    /// <br/>
    /// EN: Validates the Turkish IBAN and returns a detailed error reason if invalid.
    /// </summary>
    /// <param name="iban">TR: Doğrulanacak IBAN. / EN: The IBAN string to validate.</param>
    public static ValidationResult Validate(string? iban)
    {
        if (string.IsNullOrWhiteSpace(iban))
            return ValidationResult.Failure("IBAN numarası boş olamaz.");

        return Validate(iban.AsSpan());
    }

    /// <summary>
    /// TR: TR IBAN numarasını doğrular ve geçersizse hata nedenini açıkça döner (Zero-allocation).
    /// <br/>
    /// EN: Validates the Turkish IBAN span and returns a detailed error reason if invalid (Zero-allocation).
    /// </summary>
    /// <param name="iban">TR: Doğrulanacak karakter dizisi. / EN: The character span to validate.</param>
    public static ValidationResult Validate(ReadOnlySpan<char> iban)
    {
        Span<char> clean = stackalloc char[26];
        if (!TryNormalizeToBufferWithError(iban, clean, out string? error))
            return ValidationResult.Failure(error!);

        int remainder = 0;

        for (int i = 4; i < 26; i++)
        {
            char c = clean[i];
            if ((uint)(c - '0') <= 9)
            {
                remainder = (remainder * 10 + (c - '0')) % 97;
            }
            else if (c >= 'A' && c <= 'Z')
            {
                int val = c - 'A' + 10;
                remainder = (remainder * 100 + val) % 97;
            }
            else
            {
                return ValidationResult.Failure("IBAN geçersiz karakterler içeriyor.");
            }
        }

        remainder = (remainder * 100 + 29) % 97;
        remainder = (remainder * 100 + 27) % 97;
        remainder = (remainder * 10 + (clean[2] - '0')) % 97;
        remainder = (remainder * 10 + (clean[3] - '0')) % 97;

        return remainder == 1
            ? ValidationResult.Success
            : ValidationResult.Failure("IBAN kontrol basamağı (Mod-97) algoritmasına uymuyor.");
    }

    /// <summary>
    /// TR: IBAN'ı standart "TR12 3456 7890 1234 5678 9012 34" (4'erli gruplar) formatına dönüştürür.
    /// <br/>
    /// EN: Formats the IBAN into standard 4-character blocks ("TR12 3456 7890 1234 5678 9012 34").
    /// </summary>
    /// <param name="iban">TR: Formatlanacak IBAN. / EN: The IBAN string to format.</param>
    public static string Format(string? iban)
    {
        if (string.IsNullOrWhiteSpace(iban))
            return string.Empty;

        Span<char> clean = stackalloc char[26];
        if (!TryNormalizeToBufferWithError(iban.AsSpan(), clean, out _))
            return iban.Trim().ToUpperInvariant();

        Span<char> formatted = stackalloc char[32];
        int dst = 0;

        for (int i = 0; i < 26; i++)
        {
            if (i > 0 && (i & 3) == 0)
                formatted[dst++] = ' ';

            formatted[dst++] = clean[i];
        }

        return new string(formatted);
    }

    /// <summary>
    /// TR: TR IBAN numarasını ilk 4 (<c>TRxx</c>) ve son 2 karakteri açık kalacak şekilde KVKK uyumlu olarak maskeler
    /// (Örn: <c>"TR12 **** **** **** **** **** 34"</c>).
    /// <br/>
    /// EN: Masks the Turkish IBAN leaving only the first 4 (<c>TRxx</c>) and last 2 characters visible.
    /// </summary>
    /// <param name="iban">TR: Maskelenecek IBAN. / EN: The IBAN string to mask.</param>
    /// <param name="maskChar">TR: Maskeleme karakteri. / EN: Masking character.</param>
    public static string Mask(string? iban, char maskChar = '*')
    {
        if (string.IsNullOrWhiteSpace(iban))
            return string.Empty;

        if (!TryValidate(iban, out string? validIban))
            return iban.Trim();

        Span<char> formatted = stackalloc char[32];
        int dst = 0;

        for (int i = 0; i < 26; i++)
        {
            if (i > 0 && (i & 3) == 0)
                formatted[dst++] = ' ';

            formatted[dst++] = (i < 4 || i >= 24) ? validIban[i] : maskChar;
        }

        return new string(formatted);
    }

    /// <summary>
    /// TR: Geçerli bir TR IBAN içinden 5 haneli TCMB banka kodunu döner (Örn: "00064"). Geçersizse <see langword="null"/> döner.
    /// <br/>
    /// EN: Extracts the 5-digit TCMB bank code (e.g., "00064") from a valid Turkish IBAN, or returns <see langword="null"/> if invalid.
    /// </summary>
    /// <param name="iban">TR: IBAN metni. / EN: The IBAN string.</param>
    public static string? GetBankCode(string? iban)
    {
        if (string.IsNullOrWhiteSpace(iban))
            return null;

        Span<char> clean = stackalloc char[26];
        if (!TryNormalizeToBufferWithError(iban.AsSpan(), clean, out _) || !IsValid(clean))
            return null;

        return new string(clean.Slice(4, 5));
    }

    /// <summary>
    /// TR: Geçerli bir TR IBAN içinden banka adını döner (Örn: "Türkiye İş Bankası"). Bilinmeyen kod veya geçersiz IBAN ise <see langword="null"/> döner.
    /// <br/>
    /// EN: Resolves the bank name (e.g., "Türkiye İş Bankası") from a valid Turkish IBAN, or returns <see langword="null"/> if unknown/invalid.
    /// </summary>
    /// <param name="iban">TR: IBAN metni. / EN: The IBAN string.</param>
    public static string? GetBankName(string? iban)
    {
        string? code = GetBankCode(iban);
        if (code is null)
            return null;

        return Banks.TryGetValue(code, out string? bankName) ? bankName : null;
    }

    /// <summary>
    /// TR: TR IBAN değerini doğrular ve geçerliyse boşluksuz 26 karakterlik halini <paramref name="normalized"/> ile döner.
    /// <br/>
    /// EN: Attempts to validate the Turkish IBAN and outputs the unspaced 26-character string if valid.
    /// </summary>
    /// <param name="iban">TR: Doğrulanacak IBAN. / EN: The IBAN string to validate.</param>
    /// <param name="normalized">TR: Boşluksuz 26 karakterlik IBAN. / EN: Normalized 26-character IBAN.</param>
    public static bool TryValidate(string? iban, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? normalized)
    {
        if (!string.IsNullOrWhiteSpace(iban))
        {
            Span<char> clean = stackalloc char[26];
            if (TryNormalizeToBufferWithError(iban.AsSpan(), clean, out _) && IsValid(clean))
            {
                normalized = new string(clean);
                return true;
            }
        }

        normalized = null;
        return false;
    }

    /// <summary>
    /// TR: TR IBAN değerini doğrular ve geçerliyse 4'lü bloklar halinde formatlanmış halini <paramref name="formatted"/> ile döner.
    /// <br/>
    /// EN: Attempts to validate and format the Turkish IBAN into 4-character blocks.
    /// </summary>
    /// <param name="iban">TR: IBAN metni. / EN: The IBAN string.</param>
    /// <param name="formatted">TR: Formatlanmış IBAN. / EN: Formatted IBAN string.</param>
    public static bool TryFormat(string? iban, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? formatted)
    {
        if (TryValidate(iban, out string? validIban))
        {
            formatted = Format(validIban);
            return true;
        }

        formatted = null;
        return false;
    }

    /// <summary>
    /// TR: TR IBAN değerini KVKK formatında maskelemeyi dener.
    /// <br/>
    /// EN: Attempts to validate and mask the Turkish IBAN.
    /// </summary>
    /// <param name="iban">TR: Maskelenecek IBAN. / EN: The IBAN string to mask.</param>
    /// <param name="masked">TR: Maskelenmiş IBAN. / EN: Masked IBAN string.</param>
    /// <param name="maskChar">TR: Maskeleme karakteri. / EN: Masking character.</param>
    public static bool TryMask(string? iban, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? masked, char maskChar = '*')
    {
        if (TryValidate(iban, out string? validIban))
        {
            masked = Mask(validIban, maskChar);
            return true;
        }

        masked = null;
        return false;
    }

    /// <summary>
    /// TR: Geçerli TR IBAN içinden 5 haneli banka kodunu almayı dener.
    /// <br/>
    /// EN: Attempts to extract the 5-digit bank code from a Turkish IBAN.
    /// </summary>
    /// <param name="iban">TR: IBAN metni. / EN: The IBAN string.</param>
    /// <param name="bankCode">TR: 5 haneli banka kodu. / EN: 5-digit bank code.</param>
    public static bool TryGetBankCode(string? iban, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? bankCode)
    {
        bankCode = GetBankCode(iban);
        return bankCode is not null;
    }

    /// <summary>
    /// TR: Geçerli TR IBAN içinden banka adını almayı dener.
    /// <br/>
    /// EN: Attempts to resolve the bank name from a Turkish IBAN.
    /// </summary>
    /// <param name="iban">TR: IBAN metni. / EN: The IBAN string.</param>
    /// <param name="bankName">TR: Banka adı. / EN: Bank name.</param>
    public static bool TryGetBankName(string? iban, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? bankName)
    {
        bankName = GetBankName(iban);
        return bankName is not null;
    }

    private static bool TryNormalizeToBufferWithError(ReadOnlySpan<char> input, Span<char> output26, out string? error)
    {
        error = null;
        if (input.IsWhiteSpace())
        {
            error = "IBAN numarası boş olamaz.";
            return false;
        }

        int idx = 0;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (c == ' ' || c == '-')
                continue;

            if (idx >= 26)
            {
                error = "TR IBAN numarası 26 karakter olmalıdır.";
                return false;
            }

            if (c >= 'a' && c <= 'z')
                c = (char)(c - 32);

            output26[idx++] = c;
        }

        if (idx == 0)
        {
            error = "IBAN numarası boş olamaz.";
            return false;
        }

        if (idx >= 2 && (output26[0] != 'T' || output26[1] != 'R'))
        {
            error = "Türkiye IBAN numarası 'TR' ülke kodu ile başlamalıdır.";
            return false;
        }

        if (idx != 26)
        {
            error = "TR IBAN numarası 26 karakter olmalıdır.";
            return false;
        }

        for (int i = 2; i < 9; i++)
        {
            if ((uint)(output26[i] - '0') > 9)
            {
                error = "IBAN kontrol basamakları ve banka kodu rakamlardan oluşmalıdır.";
                return false;
            }
        }

        return true;
    }
}
