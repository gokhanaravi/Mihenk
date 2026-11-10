namespace Mihenk.Vehicle;

/// <summary>
/// TR: Ayrıştırılmış plaka bilgisini temsil eder.
/// <br/>
/// EN: Represents parsed Turkish license plate information.
/// </summary>
/// <param name="ProvinceCode">TR: Plaka il kodu (1-81, Örn: 34). / EN: Province plate code (1-81, e.g., 34).</param>
/// <param name="ProvinceName">TR: İl adı (Örn: "İstanbul"). / EN: Province name (e.g., "İstanbul").</param>
/// <param name="Letters">TR: Plakanın ortasındaki harf grubu (Örn: "ABC"). / EN: Middle letter group (e.g., "ABC").</param>
/// <param name="Digits">TR: Plakanın sonundaki rakam grubu (Örn: "123"). / EN: Trailing digit group (e.g., "123").</param>
public readonly record struct PlateInfo(
    int ProvinceCode,
    string ProvinceName,
    string Letters,
    string Digits)
{
    /// <summary>
    /// TR: Plakanın standart normalize edilmiş halini döner (Örn: "34 ABC 123").
    /// <br/>
    /// EN: Returns the standard normalized license plate string (e.g., "34 ABC 123").
    /// </summary>
    public override string ToString() => $"{ProvinceCode:D2} {Letters} {Digits}";
}

/// <summary>
/// TR: Türkiye araç plakası doğrulama, ayrıştırma ve normalizasyon araçları.
/// <br/>
/// EN: Turkish vehicle license plate validation, parsing, and normalization utilities.
/// </summary>
public static class Plaka
{
    private static readonly string[] Provinces =
    [
        "", // 00
        "Adana", "Adıyaman", "Afyonkarahisar", "Ağrı", "Amasya", "Ankara", "Antalya", "Artvin", "Aydın", "Balıkesir",
        "Bilecik", "Bingöl", "Bitlis", "Bolu", "Burdur", "Bursa", "Çanakkale", "Çankırı", "Çorum", "Denizli",
        "Diyarbakır", "Edirne", "Elazığ", "Erzincan", "Erzurum", "Eskişehir", "Gaziantep", "Giresun", "Gümüşhane", "Hakkari",
        "Hatay", "Isparta", "Mersin", "İstanbul", "İzmir", "Kars", "Kastamonu", "Kayseri", "Kırklareli", "Kırşehir",
        "Kocaeli", "Konya", "Kütahya", "Malatya", "Manisa", "Kahramanmaraş", "Mardin", "Muğla", "Muş", "Nevşehir",
        "Niğde", "Ordu", "Rize", "Sakarya", "Samsun", "Siirt", "Sinop", "Sivas", "Tekirdağ", "Tokat",
        "Trabzon", "Tunceli", "Şanlıurfa", "Uşak", "Van", "Yozgat", "Zonguldak", "Aksaray", "Bayburt", "Karaman",
        "Kırıkkale", "Batman", "Şırnak", "Bartın", "Ardahan", "Iğdır", "Yalova", "Karabük", "Kilis", "Osmaniye",
        "Düzce"
    ];

    /// <summary>
    /// TR: Verilen plakanın Türkiye standartlarına uygun (01-81 il kodu ve geçerli harf/rakam kombinasyonu) olup olmadığını kontrol eder.
    /// <br/>
    /// EN: Checks whether the given license plate conforms to Turkish standards (01-81 province code and valid letter/digit combination).
    /// </summary>
    /// <param name="plate">TR: Kontrol edilecek plaka. / EN: The license plate string to validate.</param>
    public static bool IsValid(string? plate)
    {
        if (string.IsNullOrWhiteSpace(plate))
            return false;

        return IsValid(plate.AsSpan());
    }

    /// <summary>
    /// TR: Verilen karakter dizisinin geçerli bir Türkiye plakası olup olmadığını kontrol eder (Zero-allocation).
    /// <br/>
    /// EN: Checks whether the given character span is a valid Turkish license plate (Zero-allocation).
    /// </summary>
    /// <param name="plate">TR: Kontrol edilecek karakter dizisi. / EN: The character span to validate.</param>
    public static bool IsValid(ReadOnlySpan<char> plate) => Validate(plate).IsValid;

    /// <summary>
    /// TR: Plakayı doğrular ve geçersizse hata nedenini açıkça döner.
    /// <br/>
    /// EN: Validates the license plate and returns a detailed error reason if invalid.
    /// </summary>
    /// <param name="plate">TR: Doğrulanacak plaka. / EN: The license plate to validate.</param>
    public static ValidationResult Validate(string? plate)
    {
        if (string.IsNullOrWhiteSpace(plate))
            return ValidationResult.Failure("Plaka boş olamaz.");

        return Validate(plate.AsSpan());
    }

    /// <summary>
    /// TR: Plakayı doğrular ve geçersizse hata nedenini açıkça döner (Zero-allocation).
    /// <br/>
    /// EN: Validates the license plate span and returns a detailed error reason if invalid (Zero-allocation).
    /// </summary>
    /// <param name="plate">TR: Doğrulanacak karakter dizisi. / EN: The character span to validate.</param>
    public static ValidationResult Validate(ReadOnlySpan<char> plate)
    {
        if (!TryExtractPartsWithError(plate, out int provinceCode, out ReadOnlySpan<char> letters, out ReadOnlySpan<char> digits, out string? error))
            return ValidationResult.Failure(error!);

        if (!IsValidCombination(provinceCode, letters.Length, digits.Length))
            return ValidationResult.Failure($"Plakadaki {letters.Length} harf ve {digits.Length} rakam kombinasyonu Türkiye plaka standartlarına uygun değil.");

        return ValidationResult.Success;
    }

    /// <summary>
    /// TR: Plakayı standart "34 ABC 123" formatına dönüştürür. Geçersizse temizlenmiş/büyük harfe çevrilmiş halini döner.
    /// <br/>
    /// EN: Normalizes the license plate into standard "34 ABC 123" format.
    /// </summary>
    /// <param name="plate">TR: Normalize edilecek plaka. / EN: The license plate string to normalize.</param>
    public static string Normalize(string? plate)
    {
        if (string.IsNullOrWhiteSpace(plate))
            return string.Empty;

        if (!TryExtractPartsWithError(plate.AsSpan(), out int provinceCode, out ReadOnlySpan<char> letters, out ReadOnlySpan<char> digits, out _))
            return plate.Trim().ToUpperInvariant();

        Span<char> buffer = stackalloc char[12];
        buffer[0] = (char)('0' + (provinceCode / 10));
        buffer[1] = (char)('0' + (provinceCode % 10));
        buffer[2] = ' ';

        int pos = 3;
        for (int i = 0; i < letters.Length; i++)
            buffer[pos++] = ToUpperAsciiOrTurkish(letters[i]);

        buffer[pos++] = ' ';

        for (int i = 0; i < digits.Length; i++)
            buffer[pos++] = digits[i];

        return new string(buffer[..pos]);
    }

    /// <summary>
    /// TR: Plakadan il kodunu (1-81 arası <see cref="int"/>, Örn: 34) döner. Geçersizse <see langword="null"/> döner.
    /// <br/>
    /// EN: Returns the province code (1-81 as <see cref="int"/>, e.g., 34) from the license plate, or <see langword="null"/> if invalid.
    /// </summary>
    /// <param name="plate">TR: Plaka metni. / EN: The license plate string.</param>
    public static int? GetProvinceCode(string? plate)
    {
        if (string.IsNullOrWhiteSpace(plate))
            return null;

        if (!TryExtractPartsWithError(plate.AsSpan(), out int provinceCode, out ReadOnlySpan<char> letters, out ReadOnlySpan<char> digits, out _)
            || !IsValidCombination(provinceCode, letters.Length, digits.Length))
            return null;

        return provinceCode;
    }

    /// <summary>
    /// TR: Plakadan il adını döner (Örn: "34abc123" -> "İstanbul"). Geçersizse <see langword="null"/> döner.
    /// <br/>
    /// EN: Returns the province name (e.g., "34abc123" -> "İstanbul") from the license plate, or <see langword="null"/> if invalid.
    /// </summary>
    /// <param name="plate">TR: Plaka metni. / EN: The license plate string.</param>
    public static string? GetProvinceName(string? plate)
    {
        int? code = GetProvinceCode(plate);
        return code.HasValue ? Provinces[code.Value] : null;
    }

    /// <summary>
    /// TR: Plakayı il kodu (<see cref="int"/>), il adı, harf grubu ve rakam grubu olarak ayrıştırır. Geçersizse <see langword="null"/> döner.
    /// <br/>
    /// EN: Parses the license plate into province code, province name, letter group, and digit group. Returns <see langword="null"/> if invalid.
    /// </summary>
    /// <param name="plate">TR: Ayrıştırılacak plaka. / EN: The license plate string to parse.</param>
    public static PlateInfo? Parse(string? plate)
    {
        if (string.IsNullOrWhiteSpace(plate))
            return null;

        if (!TryExtractPartsWithError(plate.AsSpan(), out int provinceCode, out ReadOnlySpan<char> letters, out ReadOnlySpan<char> digits, out _)
            || !IsValidCombination(provinceCode, letters.Length, digits.Length))
            return null;

        Span<char> upperLetters = stackalloc char[letters.Length];
        for (int i = 0; i < letters.Length; i++)
            upperLetters[i] = ToUpperAsciiOrTurkish(letters[i]);

        return new PlateInfo(
            ProvinceCode: provinceCode,
            ProvinceName: Provinces[provinceCode],
            Letters: new string(upperLetters),
            Digits: digits.ToString());
    }

    /// <summary>
    /// TR: Plakanın resmi kurum (A, AA), polis (A, AA, AAA), jandarma (JAA-JZZ), sahil güvenlik (SGH) veya diplomatik/konsolosluk (CA, CC, CD, CG, CM) plakası olup olmadığını kontrol eder.
    /// <br/>
    /// EN: Checks whether the license plate belongs to an official government, police, gendarmerie, coast guard, or diplomatic vehicle.
    /// </summary>
    /// <param name="plate">TR: Kontrol edilecek plaka. / EN: The license plate string to check.</param>
    public static bool IsOfficial(string? plate)
    {
        if (string.IsNullOrWhiteSpace(plate))
            return false;

        if (!TryExtractPartsWithError(plate.AsSpan(), out int provinceCode, out ReadOnlySpan<char> letters, out ReadOnlySpan<char> digits, out _)
            || !IsValidCombination(provinceCode, letters.Length, digits.Length))
            return false;

        Span<char> l = stackalloc char[letters.Length];
        for (int i = 0; i < letters.Length; i++)
            l[i] = ToUpperAsciiOrTurkish(letters[i]);

        if (l.Length == 1 && l[0] == 'A')
            return true;
        if (l.Length == 2 && l[0] == 'A' && l[1] == 'A')
            return true;
        if (l.Length == 3 && l[0] == 'A' && l[1] == 'A' && l[2] == 'A')
            return true;

        if (l.Length == 2 && l[0] == 'C' && (l[1] == 'A' || l[1] == 'C' || l[1] == 'D' || l[1] == 'G' || l[1] == 'M'))
            return true;

        if (l.Length == 3 && l[0] == 'J')
            return true;
        if (l.Length == 3 && l[0] == 'S' && l[1] == 'G' && l[2] == 'H')
            return true;

        return false;
    }

    /// <summary>
    /// TR: Plakayı ayrıştırmayı dener; geçerliyse <see langword="true"/> ve <paramref name="info"/> döner.
    /// <br/>
    /// EN: Attempts to parse the license plate and outputs <paramref name="info"/> if valid.
    /// </summary>
    /// <param name="plate">TR: Plaka metni. / EN: The license plate string.</param>
    /// <param name="info">TR: Ayrıştırılmış plaka bilgisi. / EN: Parsed license plate info.</param>
    public static bool TryParse(string? plate, out PlateInfo info)
    {
        PlateInfo? parsed = Parse(plate);
        if (parsed.HasValue)
        {
            info = parsed.Value;
            return true;
        }

        info = default;
        return false;
    }

    /// <summary>
    /// TR: Plakayı doğrular ve geçerliyse standart "34 ABC 123" formatına çevirerek <paramref name="normalized"/> ile döner.
    /// <br/>
    /// EN: Attempts to validate and normalize the license plate into "34 ABC 123" format.
    /// </summary>
    /// <param name="plate">TR: Plaka metni. / EN: The license plate string.</param>
    /// <param name="normalized">TR: Normalize edilmiş plaka. / EN: Normalized license plate string.</param>
    public static bool TryNormalize(string? plate, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? normalized)
    {
        if (TryParse(plate, out PlateInfo info))
        {
            normalized = info.ToString();
            return true;
        }

        normalized = null;
        return false;
    }

    /// <summary>
    /// TR: Plakadan il kodunu (1 - 81 arası <see cref="int"/>) almayı dener.
    /// <br/>
    /// EN: Attempts to extract the province code (1-81) from the license plate.
    /// </summary>
    /// <param name="plate">TR: Plaka metni. / EN: The license plate string.</param>
    /// <param name="provinceCode">TR: İl plaka kodu (1-81). / EN: Province plate code (1-81).</param>
    public static bool TryGetProvinceCode(string? plate, out int provinceCode)
    {
        int? code = GetProvinceCode(plate);
        if (code.HasValue)
        {
            provinceCode = code.Value;
            return true;
        }

        provinceCode = 0;
        return false;
    }

    /// <summary>
    /// TR: Plakadan il adını ("İstanbul" vb.) almayı dener.
    /// <br/>
    /// EN: Attempts to extract the province name ("İstanbul", etc.) from the license plate.
    /// </summary>
    /// <param name="plate">TR: Plaka metni. / EN: The license plate string.</param>
    /// <param name="provinceName">TR: İl adı. / EN: Province name.</param>
    public static bool TryGetProvinceName(string? plate, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? provinceName)
    {
        provinceName = GetProvinceName(plate);
        return provinceName is not null;
    }

    private static bool TryExtractPartsWithError(
        ReadOnlySpan<char> input,
        out int provinceCode,
        out ReadOnlySpan<char> letters,
        out ReadOnlySpan<char> digits,
        out string? error)
    {
        provinceCode = 0;
        letters = default;
        digits = default;
        error = null;

        ReadOnlySpan<char> s = input.Trim();
        if (s.IsEmpty)
        {
            error = "Plaka boş olamaz.";
            return false;
        }

        if (s.Length < 5)
        {
            error = "Plaka çok kısa.";
            return false;
        }

        if ((uint)(s[0] - '0') > 9 || (uint)(s[1] - '0') > 9)
        {
            error = "Plaka 2 haneli il kodu (01-81) ile başlamalıdır.";
            return false;
        }

        provinceCode = (s[0] - '0') * 10 + (s[1] - '0');
        if (provinceCode < 1 || provinceCode > 81)
        {
            error = "Plaka il kodu 01 ile 81 arasında olmalıdır.";
            return false;
        }

        int idx = 2;
        while (idx < s.Length && (s[idx] == ' ' || s[idx] == '-'))
            idx++;

        int letterStart = idx;
        while (idx < s.Length && char.IsLetter(s[idx]))
        {
            if (!IsValidPlateLetter(s[idx]))
            {
                error = $"Plakada '{s[idx]}' harfi kullanılamaz (Q, W, X harfleri geçersizdir).";
                return false;
            }
            idx++;
        }

        int letterLen = idx - letterStart;
        if (letterLen < 1 || letterLen > 3)
        {
            error = "Plaka harf grubu 1 ile 3 harf arasında olmalıdır.";
            return false;
        }

        letters = s.Slice(letterStart, letterLen);

        while (idx < s.Length && (s[idx] == ' ' || s[idx] == '-'))
            idx++;

        int digitStart = idx;
        while (idx < s.Length && (uint)(s[idx] - '0') <= 9)
            idx++;

        if (idx != s.Length)
        {
            error = "Plaka geçersiz karakterler içeriyor.";
            return false;
        }

        int digitLen = idx - digitStart;
        if (digitLen < 2 || digitLen > 5)
        {
            error = "Plaka rakam grubu 2 ile 5 hane arasında olmalıdır.";
            return false;
        }

        digits = s.Slice(digitStart, digitLen);

        bool anyNonZero = false;
        for (int i = 0; i < digits.Length; i++)
        {
            if (digits[i] != '0')
            {
                anyNonZero = true;
                break;
            }
        }

        if (!anyNonZero)
        {
            error = "Plaka rakam grubu tamamen sıfırlardan oluşamaz.";
            return false;
        }

        return true;
    }

    private static bool IsValidCombination(int provinceCode, int letterCount, int digitCount)
    {
        if (provinceCode < 1 || provinceCode > 81)
            return false;

        return letterCount switch
        {
            1 => digitCount is 4 or 5,
            2 => digitCount is 3 or 4,
            3 => digitCount is 2 or 3,
            _ => false
        };
    }

    private static bool IsValidPlateLetter(char c)
    {
        char u = ToUpperAsciiOrTurkish(c);
        if (u is 'Q' or 'W' or 'X')
            return false;

        if (u >= 'A' && u <= 'Z')
            return true;

        return u is 'Ç' or 'Ğ' or 'İ' or 'Ö' or 'Ş' or 'Ü';
    }

    private static char ToUpperAsciiOrTurkish(char c) => c switch
    {
        'i' => 'İ',
        'ı' => 'I',
        'ç' => 'Ç',
        'ğ' => 'Ğ',
        'ö' => 'Ö',
        'ş' => 'Ş',
        'ü' => 'Ü',
        >= 'a' and <= 'z' => (char)(c - 32),
        _ => c
    };
}
