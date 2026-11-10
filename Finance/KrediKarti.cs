namespace Mihenk.Finance;

/// <summary>
/// TR: Kredi ve banka kartı şemaları (markaları).
/// <br/>
/// EN: Credit and debit card schemes (brands).
/// </summary>
public enum CardType
{
    /// <summary>TR: Bilinmeyen veya desteklenmeyen kart şeması. / EN: Unknown or unsupported card scheme.</summary>
    Unknown = 0,
    /// <summary>TR: Visa kart şeması. / EN: Visa card scheme.</summary>
    Visa = 1,
    /// <summary>TR: Mastercard kart şeması. / EN: Mastercard card scheme.</summary>
    Mastercard = 2,
    /// <summary>TR: TROY (Türkiye'nin Ödeme Yöntemi) kart şeması. / EN: TROY (Turkey's Payment Method) card scheme.</summary>
    Troy = 3,
    /// <summary>TR: American Express (Amex) kart şeması. / EN: American Express (Amex) card scheme.</summary>
    Amex = 4
}

/// <summary>
/// TR: Kredi / banka kartı numarası doğrulama (Luhn), kart şeması tespit etme ve maskeleme araçları.
/// <br/>
/// EN: Credit/debit card number validation (Luhn), card scheme detection, and PCI-DSS/KVKK masking utilities.
/// </summary>
public static class KrediKarti
{
    /// <summary>
    /// TR: Verilen kart numarasının Luhn algoritmasına göre geçerli olup olmadığını kontrol eder.
    /// <br/>
    /// EN: Checks whether the given card number is valid according to the Luhn algorithm.
    /// </summary>
    /// <param name="cardNumber">TR: Kontrol edilecek kart numarası. / EN: The card number string to validate.</param>
    public static bool IsValid(string? cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return false;

        return IsValid(cardNumber.AsSpan());
    }

    /// <summary>
    /// TR: Verilen karakter dizisinin Luhn algoritmasına göre geçerli bir kart numarası olup olmadığını kontrol eder (Zero-allocation).
    /// <br/>
    /// EN: Checks whether the given character span is a valid card number according to the Luhn algorithm (Zero-allocation).
    /// </summary>
    /// <param name="cardNumber">TR: Kontrol edilecek karakter dizisi. / EN: The character span to validate.</param>
    public static bool IsValid(ReadOnlySpan<char> cardNumber) => Validate(cardNumber).IsValid;

    /// <summary>
    /// TR: Kart numarasını Luhn algoritmasına göre doğrular ve geçersizse hata nedenini açıkça döner.
    /// <br/>
    /// EN: Validates the card number using the Luhn algorithm and returns a detailed error reason if invalid.
    /// </summary>
    /// <param name="cardNumber">TR: Doğrulanacak kart numarası. / EN: The card number string to validate.</param>
    public static ValidationResult Validate(string? cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return ValidationResult.Failure("Kredi kartı numarası boş olamaz.");

        return Validate(cardNumber.AsSpan());
    }

    /// <summary>
    /// TR: Kart numarasını Luhn algoritmasına göre doğrular ve geçersizse hata nedenini açıkça döner (Zero-allocation).
    /// <br/>
    /// EN: Validates the card number span using the Luhn algorithm and returns a detailed error reason if invalid (Zero-allocation).
    /// </summary>
    /// <param name="cardNumber">TR: Doğrulanacak karakter dizisi. / EN: The character span to validate.</param>
    public static ValidationResult Validate(ReadOnlySpan<char> cardNumber)
    {
        if (cardNumber.IsWhiteSpace())
            return ValidationResult.Failure("Kredi kartı numarası boş olamaz.");

        Span<char> digits = stackalloc char[19];
        if (!TryExtractDigitsWithError(cardNumber, digits, out int len, out string? error))
            return ValidationResult.Failure(error!);

        if (len < 13 || len > 19)
            return ValidationResult.Failure("Kredi kartı numarası 13 ile 19 hane arasında olmalıdır.");

        int sum = 0;
        bool alternate = false;
        bool anyNonZero = false;

        for (int i = len - 1; i >= 0; i--)
        {
            int n = digits[i] - '0';
            if (n != 0)
                anyNonZero = true;

            if (alternate)
            {
                n *= 2;
                if (n > 9)
                    n -= 9;
            }

            sum += n;
            alternate = !alternate;
        }

        if (!anyNonZero)
            return ValidationResult.Failure("Kredi kartı numarası tamamen sıfırlardan oluşamaz.");

        return (sum % 10 == 0)
            ? ValidationResult.Success
            : ValidationResult.Failure("Kredi kartı numarası Luhn kontrol algoritmasına uymuyor.");
    }

    /// <summary>
    /// TR: Kart numarasının BIN/IIN ön ekine göre kart tipini (Visa, Mastercard, Troy, Amex) döner.
    /// <br/>
    /// EN: Detects the card scheme (Visa, Mastercard, Troy, Amex) based on its BIN/IIN prefix.
    /// </summary>
    /// <param name="cardNumber">TR: Kart numarası. / EN: The card number string.</param>
    public static CardType GetCardType(string? cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return CardType.Unknown;

        return GetCardType(cardNumber.AsSpan());
    }

    /// <summary>
    /// TR: Kart numarasının BIN/IIN ön ekine göre kart tipini (Visa, Mastercard, Troy, Amex) döner (Zero-allocation).
    /// <br/>
    /// EN: Detects the card scheme (Visa, Mastercard, Troy, Amex) from a character span (Zero-allocation).
    /// </summary>
    /// <param name="cardNumber">TR: Kart numarası karakter dizisi. / EN: The card number character span.</param>
    public static CardType GetCardType(ReadOnlySpan<char> cardNumber)
    {
        Span<char> digits = stackalloc char[19];
        if (!TryExtractDigitsWithError(cardNumber, digits, out int len, out _) || len < 13)
            return CardType.Unknown;

        ReadOnlySpan<char> d = digits[..len];

        if (len == 16 && d.StartsWith("9792".AsSpan()))
            return CardType.Troy;

        if (len == 15 && d[0] == '3' && (d[1] == '4' || d[1] == '7'))
            return CardType.Amex;

        if (d[0] == '4' && (len == 13 || len == 16 || len == 19))
            return CardType.Visa;

        if (len == 16)
        {
            int prefix2 = (d[0] - '0') * 10 + (d[1] - '0');
            if (prefix2 >= 51 && prefix2 <= 55)
                return CardType.Mastercard;

            int prefix4 = prefix2 * 100 + (d[2] - '0') * 10 + (d[3] - '0');
            if (prefix4 >= 2221 && prefix4 <= 2720)
                return CardType.Mastercard;
        }

        return CardType.Unknown;
    }

    /// <summary>
    /// TR: PCI-DSS / KVKK standartlarına uygun olarak kart numarasının ilk 4 ve son 4 hanesini bırakıp ortasını maskeler
    /// (Örn: "4543 **** **** 1234").
    /// <br/>
    /// EN: Masks the card number leaving only the first 4 and last 4 digits visible ("4543 **** **** 1234").
    /// </summary>
    /// <param name="cardNumber">TR: Maskelenecek kart numarası. / EN: The card number string to mask.</param>
    /// <param name="maskChar">TR: Maskeleme karakteri. / EN: Masking character.</param>
    public static string Mask(string? cardNumber, char maskChar = '*')
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return string.Empty;

        Span<char> digits = stackalloc char[19];
        if (!TryExtractDigitsWithError(cardNumber.AsSpan(), digits, out int len, out _) || len < 12)
            return cardNumber.Trim();

        if (len == 16)
        {
            Span<char> formatted = stackalloc char[19];
            digits[..4].CopyTo(formatted);
            formatted[4] = ' ';
            formatted.Slice(5, 4).Fill(maskChar);
            formatted[9] = ' ';
            formatted.Slice(10, 4).Fill(maskChar);
            formatted[14] = ' ';
            digits.Slice(12, 4).CopyTo(formatted[15..]);
            return new string(formatted);
        }

        Span<char> result = stackalloc char[len];
        digits[..4].CopyTo(result);
        result.Slice(4, len - 8).Fill(maskChar);
        digits.Slice(len - 4, 4).CopyTo(result[(len - 4)..]);
        return new string(result);
    }

    /// <summary>
    /// TR: Kart numarasını Luhn algoritmasına göre doğrular ve geçerliyse boşluksuz rakam halini <paramref name="normalized"/> ile döner.
    /// <br/>
    /// EN: Attempts to validate the card number and outputs the unspaced digit string if valid.
    /// </summary>
    /// <param name="cardNumber">TR: Doğrulanacak kart numarası. / EN: The card number string to validate.</param>
    /// <param name="normalized">TR: Boşluksuz kart numarası. / EN: Normalized digit-only card number.</param>
    public static bool TryValidate(string? cardNumber, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? normalized)
    {
        if (!string.IsNullOrWhiteSpace(cardNumber))
        {
            Span<char> digits = stackalloc char[19];
            if (TryExtractDigitsWithError(cardNumber.AsSpan(), digits, out int len, out _) && IsValid(digits[..len]))
            {
                normalized = new string(digits[..len]);
                return true;
            }
        }

        normalized = null;
        return false;
    }

    /// <summary>
    /// TR: Kart numarasının şemasını (Visa, Mastercard, Troy, Amex) tespit etmeyi dener.
    /// <br/>
    /// EN: Attempts to detect the card scheme (Visa, Mastercard, Troy, Amex).
    /// </summary>
    /// <param name="cardNumber">TR: Kart numarası. / EN: The card number string.</param>
    /// <param name="cardType">TR: Tespit edilen kart şeması. / EN: Detected card scheme.</param>
    public static bool TryGetCardType(string? cardNumber, out CardType cardType)
    {
        cardType = GetCardType(cardNumber);
        return cardType != CardType.Unknown;
    }

    /// <summary>
    /// TR: Geçerli kart numarasını maskelemeyi dener.
    /// <br/>
    /// EN: Attempts to validate and mask the card number.
    /// </summary>
    /// <param name="cardNumber">TR: Kart numarası. / EN: The card number string.</param>
    /// <param name="masked">TR: Maskelenmiş kart numarası. / EN: Masked card number string.</param>
    /// <param name="maskChar">TR: Maskeleme karakteri. / EN: Masking character.</param>
    public static bool TryMask(string? cardNumber, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? masked, char maskChar = '*')
    {
        if (TryValidate(cardNumber, out string? validNo))
        {
            masked = Mask(validNo, maskChar);
            return true;
        }

        masked = null;
        return false;
    }

    private static bool TryExtractDigitsWithError(ReadOnlySpan<char> input, Span<char> buffer, out int length, out string? error)
    {
        length = 0;
        error = null;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (c == ' ' || c == '-')
                continue;

            if ((uint)(c - '0') > 9)
            {
                error = "Kredi kartı numarası yalnızca rakamlardan oluşmalıdır.";
                return false;
            }

            if (length >= buffer.Length)
            {
                error = "Kredi kartı numarası en fazla 19 haneli olabilir.";
                return false;
            }

            buffer[length++] = c;
        }

        if (length == 0)
        {
            error = "Kredi kartı numarası boş olamaz.";
            return false;
        }

        return true;
    }
}
