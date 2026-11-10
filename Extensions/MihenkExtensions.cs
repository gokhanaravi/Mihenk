using Mihenk.Commerce;
using Mihenk.Communication;
using Mihenk.Finance;
using Mihenk.Identity;
using Mihenk.Security;
using Mihenk.Text;
using Mihenk.Vehicle;

namespace Mihenk.Extensions;

/// <summary>
/// TR: Tüm Mihenk doğrulama, dönüştürme, formatlama ve metin işleme araçları için akıcı (fluent) Extension metotları.
/// <para/>
/// EN: Fluent extension methods for all Mihenk validation, conversion, formatting, and text processing utilities.
/// </summary>
public static class MihenkExtensions
{
    // =========================================================================
    // 1. Kimlik ve Vergi / Identity & Tax
    // =========================================================================

    /// <inheritdoc cref="TcKimlik.IsValid(string?)"/>
    public static bool IsValidTcKimlik(this string? tcNo) => TcKimlik.IsValid(tcNo);

    /// <inheritdoc cref="TcKimlik.Validate(string?)"/>
    public static ValidationResult ValidateTcKimlik(this string? tcNo) => TcKimlik.Validate(tcNo);

    /// <inheritdoc cref="TcKimlik.Mask(string?, char)"/>
    public static string MaskTcKimlik(this string? tcNo, char maskChar = '*') => TcKimlik.Mask(tcNo, maskChar);

    /// <inheritdoc cref="TcKimlik.TryValidate(string?, out string?)"/>
    public static bool TryValidateTcKimlik(this string? tcNo, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? normalized)
        => TcKimlik.TryValidate(tcNo, out normalized);

    /// <inheritdoc cref="VergiNo.IsValid(string?)"/>
    public static bool IsValidVergiNo(this string? taxNo) => VergiNo.IsValid(taxNo);

    /// <inheritdoc cref="VergiNo.Validate(string?)"/>
    public static ValidationResult ValidateVergiNo(this string? taxNo) => VergiNo.Validate(taxNo);

    /// <inheritdoc cref="VergiNo.IsValidTcOrVkn(string?)"/>
    public static bool IsValidTcOrVkn(this string? value) => VergiNo.IsValidTcOrVkn(value);

    /// <inheritdoc cref="VergiNo.ValidateTcOrVkn(string?)"/>
    public static ValidationResult ValidateTcOrVkn(this string? value) => VergiNo.ValidateTcOrVkn(value);

    /// <inheritdoc cref="VergiNo.TryValidate(string?, out string?)"/>
    public static bool TryValidateVergiNo(this string? taxNo, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? normalized)
        => VergiNo.TryValidate(taxNo, out normalized);

    /// <inheritdoc cref="VergiNo.TryValidateTcOrVkn(string?, out string?)"/>
    public static bool TryValidateTcOrVkn(this string? value, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? normalized)
        => VergiNo.TryValidateTcOrVkn(value, out normalized);

    /// <inheritdoc cref="YabanciKimlik.IsValid(string?)"/>
    public static bool IsValidYabanciKimlik(this string? foreignId) => YabanciKimlik.IsValid(foreignId);

    /// <inheritdoc cref="YabanciKimlik.Validate(string?)"/>
    public static ValidationResult ValidateYabanciKimlik(this string? foreignId) => YabanciKimlik.Validate(foreignId);

    /// <inheritdoc cref="YabanciKimlik.TryValidate(string?, out string?)"/>
    public static bool TryValidateYabanciKimlik(this string? foreignId, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? normalized)
        => YabanciKimlik.TryValidate(foreignId, out normalized);

    /// <inheritdoc cref="MersisNo.IsValid(string?)"/>
    public static bool IsValidMersisNo(this string? mersisNo) => MersisNo.IsValid(mersisNo);

    /// <inheritdoc cref="MersisNo.Validate(string?)"/>
    public static ValidationResult ValidateMersisNo(this string? mersisNo) => MersisNo.Validate(mersisNo);

    /// <inheritdoc cref="MersisNo.TryValidate(string?, out string?)"/>
    public static bool TryValidateMersisNo(this string? mersisNo, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? normalized)
        => MersisNo.TryValidate(mersisNo, out normalized);

    // =========================================================================
    // 2. Araç / Vehicle
    // =========================================================================

    /// <inheritdoc cref="Plaka.IsValid(string?)"/>
    public static bool IsValidPlate(this string? plate) => Plaka.IsValid(plate);

    /// <inheritdoc cref="Plaka.Validate(string?)"/>
    public static ValidationResult ValidatePlate(this string? plate) => Plaka.Validate(plate);

    /// <inheritdoc cref="Plaka.Normalize(string?)"/>
    public static string NormalizePlate(this string? plate) => Plaka.Normalize(plate);

    /// <inheritdoc cref="Plaka.GetProvinceCode(string?)"/>
    public static int? GetPlateProvinceCode(this string? plate) => Plaka.GetProvinceCode(plate);

    /// <inheritdoc cref="Plaka.GetProvinceName(string?)"/>
    public static string? GetPlateProvinceName(this string? plate) => Plaka.GetProvinceName(plate);

    /// <inheritdoc cref="Plaka.Parse(string?)"/>
    public static PlateInfo? ParsePlate(this string? plate) => Plaka.Parse(plate);

    /// <inheritdoc cref="Plaka.TryParse(string?, out PlateInfo)"/>
    public static bool TryParsePlate(this string? plate, out PlateInfo plateInfo) => Plaka.TryParse(plate, out plateInfo);

    /// <inheritdoc cref="Plaka.TryNormalize(string?, out string?)"/>
    public static bool TryNormalizePlate(this string? plate, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? normalized)
        => Plaka.TryNormalize(plate, out normalized);

    /// <inheritdoc cref="Plaka.IsOfficial(string?)"/>
    public static bool IsOfficialPlate(this string? plate) => Plaka.IsOfficial(plate);

    /// <inheritdoc cref="SasiNo.IsValid(string?)"/>
    public static bool IsValidSasiNo(this string? vin) => SasiNo.IsValid(vin);

    /// <inheritdoc cref="SasiNo.Validate(string?)"/>
    public static ValidationResult ValidateSasiNo(this string? vin) => SasiNo.Validate(vin);

    /// <inheritdoc cref="SasiNo.TryValidate(string?, out string?)"/>
    public static bool TryValidateSasiNo(this string? vin, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? normalized)
        => SasiNo.TryValidate(vin, out normalized);

    // =========================================================================
    // 3. Banka ve Finans / Banking & Finance
    // =========================================================================

    /// <inheritdoc cref="Iban.IsValid(string?)"/>
    public static bool IsValidIban(this string? iban) => Iban.IsValid(iban);

    /// <inheritdoc cref="Iban.Validate(string?)"/>
    public static ValidationResult ValidateIban(this string? iban) => Iban.Validate(iban);

    /// <inheritdoc cref="Iban.Format(string?)"/>
    public static string FormatIban(this string? iban) => Iban.Format(iban);

    /// <inheritdoc cref="Iban.Mask(string?, char)"/>
    public static string MaskIban(this string? iban, char maskChar = '*') => Iban.Mask(iban, maskChar);

    /// <inheritdoc cref="Iban.GetBankCode(string?)"/>
    public static string? GetIbanBankCode(this string? iban) => Iban.GetBankCode(iban);

    /// <inheritdoc cref="Iban.GetBankName(string?)"/>
    public static string? GetIbanBankName(this string? iban) => Iban.GetBankName(iban);

    /// <inheritdoc cref="Iban.TryValidate(string?, out string?)"/>
    public static bool TryValidateIban(this string? iban, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? normalized)
        => Iban.TryValidate(iban, out normalized);

    /// <inheritdoc cref="Iban.TryFormat(string?, out string?)"/>
    public static bool TryFormatIban(this string? iban, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? formatted)
        => Iban.TryFormat(iban, out formatted);

    /// <inheritdoc cref="KrediKarti.IsValid(string?)"/>
    public static bool IsValidCreditCard(this string? cardNumber) => KrediKarti.IsValid(cardNumber);

    /// <inheritdoc cref="KrediKarti.Validate(string?)"/>
    public static ValidationResult ValidateCreditCard(this string? cardNumber) => KrediKarti.Validate(cardNumber);

    /// <inheritdoc cref="KrediKarti.GetCardType(string?)"/>
    public static CardType GetCreditCardType(this string? cardNumber) => KrediKarti.GetCardType(cardNumber);

    /// <inheritdoc cref="KrediKarti.Mask(string?, char)"/>
    public static string MaskCreditCard(this string? cardNumber, char maskChar = '*') => KrediKarti.Mask(cardNumber, maskChar);

    /// <inheritdoc cref="KrediKarti.TryValidate(string?, out string?)"/>
    public static bool TryValidateCreditCard(this string? cardNumber, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? normalized)
        => KrediKarti.TryValidate(cardNumber, out normalized);

    /// <inheritdoc cref="Para.ToWords(decimal, string, string)"/>
    public static string ToWords(this decimal amount, string currency = "TL", string subUnit = "kuruş")
        => Para.ToWords(amount, currency, subUnit);

    /// <inheritdoc cref="Para.FormatTl(decimal)"/>
    public static string FormatTl(this decimal amount) => Para.FormatTl(amount);

    // =========================================================================
    // 4. İletişim / Communication
    // =========================================================================

    /// <inheritdoc cref="Telefon.IsValid(string?)"/>
    public static bool IsValidPhone(this string? phone) => Telefon.IsValid(phone);

    /// <inheritdoc cref="Telefon.Validate(string?)"/>
    public static ValidationResult ValidatePhone(this string? phone) => Telefon.Validate(phone);

    /// <inheritdoc cref="Telefon.Normalize(string?)"/>
    public static string NormalizePhone(this string? phone) => Telefon.Normalize(phone);

    /// <inheritdoc cref="Telefon.ToShort(string?)"/>
    public static string ToShortPhone(this string? phone) => Telefon.ToShort(phone);

    /// <inheritdoc cref="Telefon.Format(string?)"/>
    public static string FormatPhone(this string? phone) => Telefon.Format(phone);

    /// <inheritdoc cref="Telefon.Mask(string?, char)"/>
    public static string MaskPhone(this string? phone, char maskChar = '*') => Telefon.Mask(phone, maskChar);

    /// <inheritdoc cref="Telefon.IsMobile(string?)"/>
    public static bool IsMobilePhone(this string? phone) => Telefon.IsMobile(phone);

    /// <inheritdoc cref="Telefon.IsLandline(string?)"/>
    public static bool IsLandlinePhone(this string? phone) => Telefon.IsLandline(phone);

    /// <inheritdoc cref="Telefon.TryNormalize(string?, out string?)"/>
    public static bool TryNormalizePhone(this string? phone, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? normalized)
        => Telefon.TryNormalize(phone, out normalized);

    /// <inheritdoc cref="Telefon.TryToShort(string?, out string?)"/>
    public static bool TryToShortPhone(this string? phone, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? shortPhone)
        => Telefon.TryToShort(phone, out shortPhone);

    /// <inheritdoc cref="Telefon.TryFormat(string?, out string?)"/>
    public static bool TryFormatPhone(this string? phone, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? formatted)
        => Telefon.TryFormat(phone, out formatted);

    /// <inheritdoc cref="Telefon.Extract(string?)"/>
    public static IReadOnlyList<string> ExtractPhones(this string? text) => Telefon.Extract(text);

    /// <inheritdoc cref="Email.IsValid(string?)"/>
    public static bool IsValidEmail(this string? email) => Email.IsValid(email);

    /// <inheritdoc cref="Email.Validate(string?)"/>
    public static ValidationResult ValidateEmail(this string? email) => Email.Validate(email);

    /// <inheritdoc cref="Email.Mask(string?, char)"/>
    public static string MaskEmail(this string? email, char maskChar = '*') => Email.Mask(email, maskChar);

    /// <inheritdoc cref="Email.TryValidate(string?, out string?)"/>
    public static bool TryValidateEmail(this string? email, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? normalized)
        => Email.TryValidate(email, out normalized);

    /// <inheritdoc cref="Email.Extract(string?)"/>
    public static IReadOnlyList<string> ExtractEmails(this string? text) => Email.Extract(text);

    /// <inheritdoc cref="PostaKodu.IsValid(string?)"/>
    public static bool IsValidPostalCode(this string? postalCode) => PostaKodu.IsValid(postalCode);

    /// <inheritdoc cref="PostaKodu.Validate(string?)"/>
    public static ValidationResult ValidatePostalCode(this string? postalCode) => PostaKodu.Validate(postalCode);

    /// <inheritdoc cref="PostaKodu.TryValidate(string?, out string?)"/>
    public static bool TryValidatePostalCode(this string? postalCode, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? normalized)
        => PostaKodu.TryValidate(postalCode, out normalized);

    // =========================================================================
    // 5. Metin İşlemleri / Text Processing
    // =========================================================================

    /// <inheritdoc cref="Link.Extract(string?)"/>
    public static IReadOnlyList<string> ExtractLinks(this string? text) => Link.Extract(text);

    /// <inheritdoc cref="Hashtag.Extract(string?)"/>
    public static IReadOnlyList<string> ExtractHashtags(this string? text) => Hashtag.Extract(text);

    /// <inheritdoc cref="Mention.Extract(string?)"/>
    public static IReadOnlyList<string> ExtractMentions(this string? text) => Mention.Extract(text);

    /// <inheritdoc cref="TurkceMetin.ToSlug(string?)"/>
    public static string ToSlug(this string? text) => TurkceMetin.ToSlug(text);

    /// <inheritdoc cref="TurkceMetin.ToUpperTr(string?)"/>
    public static string ToUpperTr(this string? text) => TurkceMetin.ToUpperTr(text);

    /// <inheritdoc cref="TurkceMetin.ToLowerTr(string?)"/>
    public static string ToLowerTr(this string? text) => TurkceMetin.ToLowerTr(text);

    /// <inheritdoc cref="TurkceMetin.ToTitleCaseTr(string?)"/>
    public static string ToTitleCaseTr(this string? text) => TurkceMetin.ToTitleCaseTr(text);

    /// <inheritdoc cref="TurkceMetin.RemoveTurkishChars(string?)"/>
    public static string RemoveTurkishChars(this string? text) => TurkceMetin.RemoveTurkishChars(text);

    /// <inheritdoc cref="TurkceMetin.FixEncoding(string?)"/>
    public static string FixEncoding(this string? text) => TurkceMetin.FixEncoding(text);

    /// <inheritdoc cref="TurkceMetin.EqualsIgnoreCaseTr(string?, string?)"/>
    public static bool EqualsIgnoreCaseTr(this string? first, string? second) => TurkceMetin.EqualsIgnoreCaseTr(first, second);

    /// <inheritdoc cref="TurkceMetin.ContainsTr(string?, string?)"/>
    public static bool ContainsTr(this string? text, string? searchValue) => TurkceMetin.ContainsTr(text, searchValue);

    /// <inheritdoc cref="TurkceMetin.NumberToWords(int)"/>
    public static string NumberToWords(this int number) => TurkceMetin.NumberToWords(number);

    /// <inheritdoc cref="TurkceMetin.NumberToWords(long)"/>
    public static string NumberToWords(this long number) => TurkceMetin.NumberToWords(number);

    /// <inheritdoc cref="TurkceMetin.ToRelativeTimeTr(DateTime, DateTime?)"/>
    public static string ToRelativeTimeTr(this DateTime date, DateTime? referenceDate = null)
        => TurkceMetin.ToRelativeTimeTr(date, referenceDate);

    // =========================================================================
    // 6. Türkçe Çekim Ekleri / Turkish Suffixes (TurkceEk)
    // =========================================================================

    /// <inheritdoc cref="TurkceEk.ToDative(string?, bool)"/>
    public static string ToDativeTr(this string? word, bool useApostrophe = true) => TurkceEk.ToDative(word, useApostrophe);

    /// <inheritdoc cref="TurkceEk.ToLocative(string?, bool)"/>
    public static string ToLocativeTr(this string? word, bool useApostrophe = true) => TurkceEk.ToLocative(word, useApostrophe);

    /// <inheritdoc cref="TurkceEk.ToAblative(string?, bool)"/>
    public static string ToAblativeTr(this string? word, bool useApostrophe = true) => TurkceEk.ToAblative(word, useApostrophe);

    /// <inheritdoc cref="TurkceEk.ToGenitive(string?, bool)"/>
    public static string ToGenitiveTr(this string? word, bool useApostrophe = true) => TurkceEk.ToGenitive(word, useApostrophe);

    /// <inheritdoc cref="TurkceEk.ToAccusative(string?, bool)"/>
    public static string ToAccusativeTr(this string? word, bool useApostrophe = true) => TurkceEk.ToAccusative(word, useApostrophe);

    /// <inheritdoc cref="TurkceEk.ToInstrumental(string?, bool)"/>
    public static string ToInstrumentalTr(this string? word, bool useApostrophe = true) => TurkceEk.ToInstrumental(word, useApostrophe);

    /// <inheritdoc cref="TurkceEk.ToPlural(string?)"/>
    public static string ToPluralTr(this string? word) => TurkceEk.ToPlural(word);

    // =========================================================================
    // 7. KVKK & Gizlilik Maskeleme / KVKK & Privacy Masking (Kvkk)
    // =========================================================================

    /// <inheritdoc cref="Kvkk.MaskFullName(string?, char)"/>
    public static string MaskFullName(this string? fullName, char maskChar = '*') => Kvkk.MaskFullName(fullName, maskChar);

    /// <inheritdoc cref="Kvkk.MaskIp(string?, char)"/>
    public static string MaskIp(this string? ipAddress, char maskChar = '*') => Kvkk.MaskIp(ipAddress, maskChar);

    /// <inheritdoc cref="Kvkk.MaskText(string?, char)"/>
    public static string MaskKvkkText(this string? text, char maskChar = '*') => Kvkk.MaskText(text, maskChar);

    // =========================================================================
    // 8. KDV, Fatura, Barkod ve IMEI / VAT, Invoice, Barcode & IMEI
    // =========================================================================

    /// <inheritdoc cref="Kdv.Calculate(decimal, int, bool)"/>
    public static VatCalculationResult CalculateVat(this decimal amount, int vatRate = 20, bool isVatIncluded = false)
        => Kdv.Calculate(amount, vatRate, isVatIncluded);

    /// <inheritdoc cref="FaturaNo.IsValid(string?)"/>
    public static bool IsValidInvoiceNo(this string? invoiceNo) => FaturaNo.IsValid(invoiceNo);

    /// <inheritdoc cref="FaturaNo.Validate(string?)"/>
    public static ValidationResult ValidateInvoiceNo(this string? invoiceNo) => FaturaNo.Validate(invoiceNo);

    /// <inheritdoc cref="FaturaNo.TryValidate(string?, out string?)"/>
    public static bool TryValidateInvoiceNo(this string? invoiceNo, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? normalized)
        => FaturaNo.TryValidate(invoiceNo, out normalized);

    /// <inheritdoc cref="Barkod.IsValid(string?)"/>
    public static bool IsValidBarcode(this string? barcode) => Barkod.IsValid(barcode);

    /// <inheritdoc cref="Barkod.Validate(string?)"/>
    public static ValidationResult ValidateBarcode(this string? barcode) => Barkod.Validate(barcode);

    /// <inheritdoc cref="Barkod.IsTurkeyGs1(string?)"/>
    public static bool IsTurkeyGs1Barcode(this string? barcode) => Barkod.IsTurkeyGs1(barcode);

    /// <inheritdoc cref="Barkod.TryValidate(string?, out string?)"/>
    public static bool TryValidateBarcode(this string? barcode, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? normalized)
        => Barkod.TryValidate(barcode, out normalized);

    /// <inheritdoc cref="Imei.IsValid(string?)"/>
    public static bool IsValidImei(this string? imei) => Imei.IsValid(imei);

    /// <inheritdoc cref="Imei.Validate(string?)"/>
    public static ValidationResult ValidateImei(this string? imei) => Imei.Validate(imei);

    /// <inheritdoc cref="Imei.TryValidate(string?, out string?)"/>
    public static bool TryValidateImei(this string? imei, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? normalized)
        => Imei.TryValidate(imei, out normalized);
}
