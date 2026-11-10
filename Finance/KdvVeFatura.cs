namespace Mihenk.Finance;

/// <summary>
/// TR: KDV (Katma Değer Vergisi) ve Tevkifat hesaplama sonucunu temsil eder.
/// <br/>
/// EN: Represents the result of a VAT (KDV) and withholding tax (Tevkifat) calculation.
/// </summary>
/// <param name="NetAmount">TR: KDV hariç net tutar (Matrah). / EN: Net amount excluding VAT (Tax base / Matrah).</param>
/// <param name="VatAmount">TR: Hesaplanan toplam KDV tutarı. / EN: Calculated total VAT amount.</param>
/// <param name="TotalAmount">TR: Müşterinin/alıcının ödeyeceği KDV dahil net toplam tutar (<c>NetAmount + VatAmount - WithholdingAmount</c>). / EN: Total payable amount including VAT (<c>NetAmount + VatAmount - WithholdingAmount</c>).</param>
/// <param name="VatRate">TR: Uygulanan KDV yüzdesi (Örn: 1, 10, 20). / EN: Applied VAT percentage rate (e.g., 1, 10, 20).</param>
/// <param name="WithholdingAmount">TR: Varsa kesilen KDV tevkifat tutarı (yoksa 0). / EN: Deducted VAT withholding amount if applicable (otherwise 0).</param>
public readonly record struct VatCalculationResult(
    decimal NetAmount,
    decimal VatAmount,
    decimal TotalAmount,
    int VatRate = 20,
    decimal WithholdingAmount = 0m);

/// <summary>
/// TR: KDV (Dahil/Hariç) ve KDV Tevkifatı hesaplama araçları.
/// Tüm kuruş yuvarlamaları muhasebe/fatura standartlarına uygun olarak <see cref="MidpointRounding.AwayFromZero"/> ile 2 ondalık basamağa yapılır.
/// <br/>
/// EN: VAT (Inclusive/Exclusive) and VAT Withholding (Tevkifat) calculation utilities.
/// All fractional roundings use <see cref="MidpointRounding.AwayFromZero"/> to 2 decimal places.
/// </summary>
public static class Kdv
{
    /// <summary>
    /// TR: Verilen tutar üzerinden KDV hariç (<see cref="VatCalculationResult.NetAmount"/>), KDV tutarı (<see cref="VatCalculationResult.VatAmount"/>)
    /// ve KDV dahil toplamı (<see cref="VatCalculationResult.TotalAmount"/>) hesaplar.
    /// Varsayılan KDV oranı <c>20</c> (%20) olarak ayarlanmıştır; yuvarlama yöntemi <see cref="MidpointRounding.AwayFromZero"/>'dur.
    /// <br/>
    /// EN: Calculates the net amount (excluding VAT), VAT amount, and total amount (including VAT).
    /// Default VAT rate is <c>20</c> (20%); rounding mode is <see cref="MidpointRounding.AwayFromZero"/>.
    /// </summary>
    /// <param name="amount">TR: Hesaplanacak tutar. / EN: The monetary amount to calculate.</param>
    /// <param name="vatRate">TR: KDV yüzdesi (Varsayılan: <c>20</c>). / EN: VAT percentage rate (Default: <c>20</c>).</param>
    /// <param name="isVatIncluded">TR: <paramref name="amount"/> değerinin KDV dahil olup olmadığı. / EN: Whether <paramref name="amount"/> already includes VAT.</param>
    public static VatCalculationResult Calculate(decimal amount, int vatRate = 20, bool isVatIncluded = false)
    {
        if (vatRate < 0)
            throw new ArgumentOutOfRangeException(nameof(vatRate), "KDV oranı negatif olamaz.");

        decimal factor = vatRate / 100m;

        if (isVatIncluded)
        {
            decimal total = Math.Round(amount, 2, MidpointRounding.AwayFromZero);
            decimal net = Math.Round(total / (1m + factor), 2, MidpointRounding.AwayFromZero);
            decimal vat = Math.Round(total - net, 2, MidpointRounding.AwayFromZero);
            return new VatCalculationResult(net, vat, total, vatRate, 0m);
        }
        else
        {
            decimal net = Math.Round(amount, 2, MidpointRounding.AwayFromZero);
            decimal vat = Math.Round(net * factor, 2, MidpointRounding.AwayFromZero);
            return new VatCalculationResult(net, vat, net + vat, vatRate, 0m);
        }
    }

    /// <summary>
    /// TR: KDV tevkifatlı fatura hesaplaması yapar (Örn: 9/10, 7/10, 5/10, 4/10, 2/10 tevkifat).
    /// Yuvarlama yöntemi olarak <see cref="MidpointRounding.AwayFromZero"/> (2 basamak) kullanılır.
    /// <br/>
    /// EN: Calculates invoice VAT with withholding tax (Tevkifat, e.g., 9/10, 7/10, 5/10) using <see cref="MidpointRounding.AwayFromZero"/>.
    /// </summary>
    /// <param name="netAmount">TR: KDV hariç matrah tutarı. / EN: Net amount excluding VAT (Tax base).</param>
    /// <param name="vatRate">TR: KDV yüzdesi (Örn: 20). / EN: VAT percentage rate (e.g., 20).</param>
    /// <param name="withholdingNumerator">TR: Tevkifat oranı payı (Örn: 9/10 için 9). / EN: Withholding ratio numerator (e.g., 9 for 9/10).</param>
    /// <param name="withholdingDenominator">TR: Tevkifat oranı paydası (varsayılan: 10). / EN: Withholding ratio denominator (default: 10).</param>
    public static VatCalculationResult CalculateWithholding(decimal netAmount, int vatRate, int withholdingNumerator, int withholdingDenominator = 10)
    {
        if (withholdingDenominator <= 0 || withholdingNumerator < 0 || withholdingNumerator > withholdingDenominator)
            throw new ArgumentOutOfRangeException(nameof(withholdingNumerator), "Geçersiz tevkifat oranı.");

        decimal net = Math.Round(netAmount, 2, MidpointRounding.AwayFromZero);
        decimal vatAmount = Math.Round(net * (vatRate / 100m), 2, MidpointRounding.AwayFromZero);
        decimal withholdingAmount = Math.Round(vatAmount * withholdingNumerator / withholdingDenominator, 2, MidpointRounding.AwayFromZero);
        decimal total = net + (vatAmount - withholdingAmount);

        return new VatCalculationResult(net, vatAmount, total, vatRate, withholdingAmount);
    }
}

/// <summary>
/// TR: GİB (Gelir İdaresi Başkanlığı) standart 16 haneli E-Fatura / E-Arşiv Fatura Numarası doğrulama ve üretme araçları.
/// Format: 3 harf/karakter birim kodu + 4 hane fatura yılı + 9 hane müteselsil sıra numarası (Örn: <c>ABC2026000000001</c>).
/// <br/>
/// EN: Turkish Revenue Administration (GIB) 16-character E-Invoice / E-Archive number validation and generation utilities.
/// </summary>
public static class FaturaNo
{
    /// <summary>
    /// TR: Verilen değerin GİB standartlarına uygun 16 karakterli geçerli bir E-Fatura / E-Arşiv numarası olup olmadığını kontrol eder.
    /// <br/>
    /// EN: Checks whether the given value is a valid 16-character GIB E-Invoice / E-Archive number.
    /// </summary>
    /// <param name="invoiceNumber">TR: Kontrol edilecek fatura numarası. / EN: The invoice number string to validate.</param>
    public static bool IsValid(string? invoiceNumber)
    {
        if (string.IsNullOrWhiteSpace(invoiceNumber))
            return false;

        return IsValid(invoiceNumber.AsSpan().Trim());
    }

    /// <summary>
    /// TR: Verilen karakter dizisinin 16 haneli geçerli bir GİB E-Fatura numarası olup olmadığını kontrol eder (Zero-allocation).
    /// <br/>
    /// EN: Checks whether the given character span is a valid 16-character GIB E-Invoice number (Zero-allocation).
    /// </summary>
    /// <param name="invoiceNumber">TR: Kontrol edilecek karakter dizisi. / EN: The character span to validate.</param>
    public static bool IsValid(ReadOnlySpan<char> invoiceNumber) => Validate(invoiceNumber).IsValid;

    /// <summary>
    /// TR: GİB E-Fatura numarasını doğrular ve geçersizse hata nedenini açıkça döner.
    /// <br/>
    /// EN: Validates the GIB E-Invoice number and returns a detailed error reason if invalid.
    /// </summary>
    /// <param name="invoiceNumber">TR: Doğrulanacak fatura numarası. / EN: The invoice number string to validate.</param>
    public static ValidationResult Validate(string? invoiceNumber)
    {
        if (string.IsNullOrWhiteSpace(invoiceNumber))
            return ValidationResult.Failure("Fatura numarası boş olamaz.");

        return Validate(invoiceNumber.AsSpan().Trim());
    }

    /// <summary>
    /// TR: GİB E-Fatura numarasını doğrular ve geçersizse hata nedenini açıkça döner (Zero-allocation).
    /// <br/>
    /// EN: Validates the GIB E-Invoice number span and returns a detailed error reason if invalid (Zero-allocation).
    /// </summary>
    /// <param name="invoiceNumber">TR: Doğrulanacak karakter dizisi. / EN: The character span to validate.</param>
    public static ValidationResult Validate(ReadOnlySpan<char> invoiceNumber)
    {
        if (invoiceNumber.IsEmpty)
            return ValidationResult.Failure("Fatura numarası boş olamaz.");

        if (invoiceNumber.Length != 16)
            return ValidationResult.Failure("GİB fatura numarası 16 karakter olmalıdır.");

        for (int i = 0; i < 3; i++)
        {
            char c = invoiceNumber[i];
            bool isDigit = (uint)(c - '0') <= 9;
            bool isUpper = c >= 'A' && c <= 'Z';
            bool isLower = c >= 'a' && c <= 'z';
            if (!isDigit && !isUpper && !isLower)
                return ValidationResult.Failure("Fatura numarasının ilk 3 karakteri (birim/seri kodu) yalnızca İngilizce harf veya rakamlardan oluşmalıdır.");
        }

        int year = 0;
        for (int i = 3; i < 7; i++)
        {
            int d = invoiceNumber[i] - '0';
            if ((uint)d > 9)
                return ValidationResult.Failure("Fatura numarasının 4-7. karakterleri 4 haneli fatura yılını belirtmelidir.");
            year = year * 10 + d;
        }

        if (year < 2000 || year > 2099)
            return ValidationResult.Failure("Fatura yılı 2000 ile 2099 arasında olmalıdır.");

        bool anyNonZero = false;
        for (int i = 7; i < 16; i++)
        {
            int d = invoiceNumber[i] - '0';
            if ((uint)d > 9)
                return ValidationResult.Failure("Fatura numarasının son 9 hanesi (müteselsil sıra numarası) yalnızca rakamlardan oluşmalıdır.");
            if (d != 0)
                anyNonZero = true;
        }

        if (!anyNonZero)
            return ValidationResult.Failure("Fatura sıra numarası '000000000' olamaz.");

        return ValidationResult.Success;
    }

    /// <summary>
    /// TR: GİB E-Fatura numarasını doğrular ve geçerliyse büyük harfe çevrilmiş 16 karakterlik halini <paramref name="normalized"/> ile döner.
    /// <br/>
    /// EN: Attempts to validate the GIB E-Invoice number and outputs the uppercase 16-character string if valid.
    /// </summary>
    /// <param name="invoiceNumber">TR: Doğrulanacak fatura numarası. / EN: The invoice number string to validate.</param>
    /// <param name="normalized">TR: Büyük harfli 16 karakterlik fatura numarası. / EN: Normalized 16-character invoice number.</param>
    public static bool TryValidate(string? invoiceNumber, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? normalized)
    {
        if (!string.IsNullOrWhiteSpace(invoiceNumber))
        {
            ReadOnlySpan<char> trimmed = invoiceNumber.AsSpan().Trim();
            if (IsValid(trimmed))
            {
                normalized = trimmed.ToString().ToUpperInvariant();
                return true;
            }
        }

        normalized = null;
        return false;
    }

    /// <summary>
    /// TR: Belirtilen 3 harfli birim/seri kodu, yıl ve sıra numarası ile 16 haneli standart GİB E-Fatura numarası üretir
    /// (Örn: <c>Generate("MHK", 2026, 42)</c> → <c>"MHK2026000000042"</c>).
    /// <br/>
    /// EN: Generates a 16-character standard GIB E-Invoice number from the 3-letter prefix, 4-digit year, and sequence number.
    /// </summary>
    /// <param name="prefix">TR: Zorunlu 3 harfli birim/seri kodu (Örn: "ABC"). / EN: Mandatory 3-letter unit/series prefix (e.g., "ABC").</param>
    /// <param name="year">TR: 4 haneli fatura yılı (2000-2099). / EN: 4-digit invoice year (2000-2099).</param>
    /// <param name="sequenceNumber">TR: 1 ile 999.999.999 arasında müteselsil sıra numarası. / EN: Sequential number between 1 and 999,999,999.</param>
    public static string Generate(string prefix, int year, long sequenceNumber = 1)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            throw new ArgumentException("Fatura seri/birim kodu boş olamaz.", nameof(prefix));

        ReadOnlySpan<char> trimmedPrefix = prefix.AsSpan().Trim();
        if (trimmedPrefix.Length != 3)
            throw new ArgumentException("Fatura seri/birim kodu tam olarak 3 harften oluşmalıdır.", nameof(prefix));

        for (int i = 0; i < 3; i++)
        {
            char c = trimmedPrefix[i];
            bool isAsciiLetter = (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
            if (!isAsciiLetter)
                throw new ArgumentException("Fatura seri/birim kodu 3 adet İngilizce ASCII harften (A-Z) oluşmalıdır.", nameof(prefix));
        }

        if (year < 2000 || year > 2099)
            throw new ArgumentOutOfRangeException(nameof(year), "Fatura yılı 2000 ile 2099 arasında olmalıdır.");

        if (sequenceNumber < 1 || sequenceNumber > 999_999_999)
            throw new ArgumentOutOfRangeException(nameof(sequenceNumber), "Sıra numarası 1 ile 999.999.999 arasında olmalıdır.");

        return $"{trimmedPrefix.ToString().ToUpperInvariant()}{year:D4}{sequenceNumber:D9}";
    }
}
