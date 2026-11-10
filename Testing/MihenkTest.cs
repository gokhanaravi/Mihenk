namespace Mihenk.Testing;

/// <summary>
/// TR: Birim testleri (unit tests) ve geliştirme ortamları için algoritmalara %100 uygun rastgele test verileri
/// (T.C. Kimlik, Yabancı Kimlik, Vergi No, TR IBAN, IMEI) üreten sınıf.
/// <br/>
/// EN: Generates algorithmically valid random test data (TCKN, Foreigner ID, Tax Number/VKN, Turkish IBAN, IMEI) for unit testing and development.
/// </summary>
public static class MihenkTest
{
    /// <summary>
    /// TR: Algoritmaya uygun, rastgele geçerli bir 11 haneli T.C. Kimlik Numarası üretir.
    /// <br/>
    /// EN: Generates a random valid 11-digit Turkish National ID (TCKN).
    /// </summary>
    public static string TcKimlik()
    {
        return string.Create(11, Random.Shared, static (span, random) =>
        {
            int firstDigit = random.Next(1, 10);
            span[0] = (char)('0' + firstDigit);

            int oddSum = firstDigit;
            int evenSum = 0;

            for (int i = 1; i < 9; i++)
            {
                int digit = random.Next(0, 10);
                span[i] = (char)('0' + digit);

                if ((i & 1) == 0)
                    oddSum += digit;
                else
                    evenSum += digit;
            }

            int d10 = ((oddSum * 7) - evenSum) % 10;
            if (d10 < 0)
                d10 += 10;

            int d11 = (oddSum + evenSum + d10) % 10;

            span[9] = (char)('0' + d10);
            span[10] = (char)('0' + d11);
        });
    }

    /// <summary>
    /// TR: Algoritmaya uygun, 99 ile başlayan rastgele geçerli bir 11 haneli Yabancı Kimlik Numarası üretir.
    /// <br/>
    /// EN: Generates a random valid 11-digit Turkish Foreigner ID (YKN) starting with '99'.
    /// </summary>
    public static string YabanciKimlik()
    {
        return string.Create(11, Random.Shared, static (span, random) =>
        {
            span[0] = '9';
            span[1] = '9';

            int oddSum = 9;
            int evenSum = 9;

            for (int i = 2; i < 9; i++)
            {
                int digit = random.Next(0, 10);
                span[i] = (char)('0' + digit);

                if ((i & 1) == 0)
                    oddSum += digit;
                else
                    evenSum += digit;
            }

            int d10 = ((oddSum * 7) - evenSum) % 10;
            if (d10 < 0)
                d10 += 10;

            int d11 = (oddSum + evenSum + d10) % 10;

            span[9] = (char)('0' + d10);
            span[10] = (char)('0' + d11);
        });
    }

    /// <summary>
    /// TR: Algoritmaya uygun, rastgele geçerli bir 10 haneli Vergi Kimlik Numarası (VKN) üretir.
    /// <br/>
    /// EN: Generates a random valid 10-digit Turkish Tax Identification Number (VKN).
    /// </summary>
    public static string VergiNo()
    {
        return string.Create(10, Random.Shared, static (span, random) =>
        {
            int sum = 0;

            for (int i = 0; i < 9; i++)
            {
                int digit = random.Next(0, 10);
                span[i] = (char)('0' + digit);

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

            int lastDigit = (10 - (sum % 10)) % 10;
            span[9] = (char)('0' + lastDigit);
        });
    }

    /// <summary>
    /// TR: Luhn algoritmasına uygun, rastgele geçerli bir 15 haneli IMEI numarası üretir.
    /// <br/>
    /// EN: Generates a random valid 15-digit IMEI number passing Luhn verification.
    /// </summary>
    public static string Imei()
    {
        return string.Create(15, Random.Shared, static (span, random) =>
        {
            span[0] = '3';
            span[1] = '5';

            int sum = 3 + 1;

            for (int i = 2; i < 14; i++)
            {
                int d = random.Next(0, 10);
                span[i] = (char)('0' + d);

                if ((i & 1) == 1)
                {
                    d *= 2;
                    if (d > 9)
                        d -= 9;
                }

                sum += d;
            }

            int checkDigit = (10 - (sum % 10)) % 10;
            span[14] = (char)('0' + checkDigit);
        });
    }

    /// <summary>
    /// TR: ISO 7064 Mod-97-10 algoritmasına uygun, rastgele geçerli bir 26 karakterli TR IBAN numarası üretir.
    /// <br/>
    /// EN: Generates a random valid 26-character Turkish IBAN passing Mod-97 verification.
    /// </summary>
    /// <param name="bankCode">TR: 5 haneli TCMB banka kodu (varsayılan: "00064" - İş Bankası). / EN: 5-digit TCMB bank code (default: "00064").</param>
    public static string Iban(string bankCode = "00064")
    {
        if (string.IsNullOrWhiteSpace(bankCode) || bankCode.Length != 5)
            bankCode = "00064";

        return string.Create(26, (Random.Shared, bankCode), static (span, state) =>
        {
            span[0] = 'T';
            span[1] = 'R';
            span[2] = '0';
            span[3] = '0';

            state.bankCode.AsSpan().CopyTo(span.Slice(4, 5));
            span[9] = '0';

            for (int i = 10; i < 26; i++)
            {
                span[i] = (char)('0' + state.Shared.Next(0, 10));
            }

            int remainder = 0;
            for (int i = 4; i < 26; i++)
                remainder = (remainder * 10 + (span[i] - '0')) % 97;

            remainder = (remainder * 100 + 29) % 97; // T
            remainder = (remainder * 100 + 27) % 97; // R
            remainder = (remainder * 100) % 97;      // 00

            int checkDigits = 98 - remainder;
            span[2] = (char)('0' + (checkDigits / 10));
            span[3] = (char)('0' + (checkDigits % 10));
        });
    }
}
