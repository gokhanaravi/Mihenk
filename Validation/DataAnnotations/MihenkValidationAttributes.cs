using System.ComponentModel.DataAnnotations;
using Mihenk.Commerce;
using Mihenk.Communication;
using Mihenk.Finance;
using Mihenk.Identity;
using Mihenk.Vehicle;
using DataAnnotationsValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace Mihenk.Validation.DataAnnotations;

/// <summary>
/// TR: Mihenk doğrulama öznitelikleri (DataAnnotations) için temel sınıf.
/// (<see langword="null"/> veya boş değerler <see cref="RequiredAttribute"/> sorumluluğunda olduğu için geçerli kabul edilir;
/// geçersiz değerlerde <see cref="ValidationResult.ErrorMessage"/> açıklamasını otomatik döner).
/// <br/>
/// EN: Base class for Mihenk DataAnnotations validation attributes.
/// (<see langword="null"/> or whitespace values are considered valid as they fall under <see cref="RequiredAttribute"/>;
/// returns specific <see cref="ValidationResult.ErrorMessage"/> reasons when invalid).
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public abstract class MihenkValidationAttribute : ValidationAttribute
{
    private readonly string _defaultErrorMessage;

    protected MihenkValidationAttribute(string defaultErrorMessage)
        : base(defaultErrorMessage)
    {
        _defaultErrorMessage = defaultErrorMessage;
    }

    public override bool IsValid(object? value) => ValidateValue(value).IsValid;

    protected override DataAnnotationsValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        ValidationResult result = ValidateValue(value);
        if (result.IsValid)
            return DataAnnotationsValidationResult.Success;

        string? memberName = validationContext.MemberName;
        string[]? memberNames = string.IsNullOrEmpty(memberName) ? null : [memberName];

        string message = ErrorMessageString == _defaultErrorMessage && !string.IsNullOrEmpty(result.ErrorMessage)
            ? result.ErrorMessage
            : FormatErrorMessage(validationContext.DisplayName);

        return new DataAnnotationsValidationResult(message, memberNames);
    }

    private ValidationResult ValidateValue(object? value)
    {
        if (value is null)
            return ValidationResult.Success;

        if (value is string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return ValidationResult.Success;

            return ValidateString(str);
        }

        return ValidationResult.Failure(_defaultErrorMessage);
    }

    protected abstract ValidationResult ValidateString(string value);
}

// =========================================================================
// 1. Kimlik ve Vergi (Identity)
// =========================================================================

/// <summary>
/// TR: Alanın geçerli bir T.C. Kimlik Numarası (11 hane, algoritma uyumlu) olmasını zorunlu kılar.
/// <br/>
/// EN: Requires the property to be a valid 11-digit Turkish National ID (TCKN).
/// </summary>
public sealed class TcKimlikAttribute()
    : MihenkValidationAttribute("Geçerli bir T.C. Kimlik Numarası giriniz.")
{
    protected override ValidationResult ValidateString(string value) => TcKimlik.Validate(value);
}

/// <summary>
/// TR: Alanın geçerli bir 10 haneli Vergi Kimlik Numarası (VKN) olmasını zorunlu kılar.
/// <br/>
/// EN: Requires the property to be a valid 10-digit Turkish Tax Identification Number (VKN).
/// </summary>
public sealed class VergiNoAttribute()
    : MihenkValidationAttribute("Geçerli bir Vergi Kimlik Numarası (VKN) giriniz.")
{
    protected override ValidationResult ValidateString(string value) => VergiNo.Validate(value);
}

/// <summary>
/// TR: Alanın 10 haneli VKN veya 11 haneli T.C. Kimlik Numarası olmasını zorunlu kılar.
/// <br/>
/// EN: Requires the property to be a valid 10-digit VKN or 11-digit TCKN.
/// </summary>
public sealed class TcOrVknAttribute()
    : MihenkValidationAttribute("Geçerli bir T.C. Kimlik veya Vergi Kimlik Numarası giriniz.")
{
    protected override ValidationResult ValidateString(string value) => VergiNo.ValidateTcOrVkn(value);
}

/// <summary>
/// TR: Alanın 99 ile başlayan geçerli bir Yabancı Kimlik Numarası (YKN) olmasını zorunlu kılar.
/// <br/>
/// EN: Requires the property to be a valid 11-digit Turkish Foreigner ID (YKN) starting with '99'.
/// </summary>
public sealed class YabanciKimlikAttribute()
    : MihenkValidationAttribute("Geçerli bir Yabancı Kimlik Numarası giriniz.")
{
    protected override ValidationResult ValidateString(string value) => YabanciKimlik.Validate(value);
}

/// <summary>
/// TR: Alanın 16 haneli geçerli bir MERSİS numarası olmasını zorunlu kılar.
/// <br/>
/// EN: Requires the property to be a valid 16-digit MERSIS number.
/// </summary>
public sealed class MersisNoAttribute()
    : MihenkValidationAttribute("Geçerli bir MERSİS numarası giriniz.")
{
    protected override ValidationResult ValidateString(string value) => MersisNo.Validate(value);
}

// =========================================================================
// 2. Araç (Vehicle)
// =========================================================================

/// <summary>
/// TR: Alanın geçerli bir Türkiye araç plakası olmasını zorunlu kılar.
/// <br/>
/// EN: Requires the property to be a valid Turkish vehicle license plate.
/// </summary>
public sealed class PlakaAttribute()
    : MihenkValidationAttribute("Geçerli bir araç plakası giriniz.")
{
    /// <summary>
    /// TR: Yalnızca resmi plakaların kabul edilip edilmeyeceği.
    /// <br/>
    /// EN: Whether only official/government license plates are accepted.
    /// </summary>
    public bool OfficialOnly { get; set; }

    protected override ValidationResult ValidateString(string value)
    {
        ValidationResult result = Plaka.Validate(value);
        if (!result.IsValid)
            return result;

        if (OfficialOnly && !Plaka.IsOfficial(value))
            return ValidationResult.Failure("Plaka resmi bir araç plakası olmalıdır.");

        return ValidationResult.Success;
    }
}

/// <summary>
/// TR: Alanın ISO 3779 uyumlu 17 haneli geçerli bir araç şasi numarası (VIN) olmasını zorunlu kılar.
/// <br/>
/// EN: Requires the property to be a valid 17-character Vehicle Identification Number (VIN).
/// </summary>
public sealed class SasiNoAttribute()
    : MihenkValidationAttribute("Geçerli bir 17 haneli araç şasi numarası (VIN) giriniz.")
{
    protected override ValidationResult ValidateString(string value) => SasiNo.Validate(value);
}

// =========================================================================
// 3. Banka ve Finans (Finance)
// =========================================================================

/// <summary>
/// TR: Alanın geçerli bir TR IBAN numarası (26 karakter, Mod-97 uyumlu) olmasını zorunlu kılar.
/// <br/>
/// EN: Requires the property to be a valid 26-character Turkish IBAN.
/// </summary>
public sealed class IbanAttribute()
    : MihenkValidationAttribute("Geçerli bir TR IBAN numarası giriniz.")
{
    protected override ValidationResult ValidateString(string value) => Iban.Validate(value);
}

/// <summary>
/// TR: Alanın Luhn algoritmasına uygun geçerli bir kredi/banka kartı numarası olmasını zorunlu kılar.
/// <br/>
/// EN: Requires the property to be a valid credit/debit card number passing Luhn verification.
/// </summary>
public sealed class KrediKartiAttribute()
    : MihenkValidationAttribute("Geçerli bir kredi/banka kartı numarası giriniz.")
{
    protected override ValidationResult ValidateString(string value) => KrediKarti.Validate(value);
}

// =========================================================================
// 4. İletişim (Communication)
// =========================================================================

/// <summary>
/// TR: Alanın geçerli bir Türkiye telefon numarası (cep veya sabit) olmasını zorunlu kılar.
/// <br/>
/// EN: Requires the property to be a valid Turkish phone number (mobile or landline).
/// </summary>
public sealed class TelefonAttribute()
    : MihenkValidationAttribute("Geçerli bir telefon numarası giriniz.")
{
    /// <summary>
    /// TR: Yalnızca cep telefonu (5XX) numaralarının kabul edilip edilmeyeceği.
    /// <br/>
    /// EN: Whether only mobile phone numbers (5XX) are accepted.
    /// </summary>
    public bool MobileOnly { get; set; }

    /// <summary>
    /// TR: Yalnızca sabit hat numaralarının kabul edilip edilmeyeceği.
    /// <br/>
    /// EN: Whether only landline phone numbers are accepted.
    /// </summary>
    public bool LandlineOnly { get; set; }

    protected override ValidationResult ValidateString(string value)
    {
        ValidationResult result = Telefon.Validate(value);
        if (!result.IsValid)
            return result;

        if (MobileOnly && !Telefon.IsMobile(value))
            return ValidationResult.Failure("Telefon numarası geçerli bir cep telefonu (5XX) olmalıdır.");

        if (LandlineOnly && !Telefon.IsLandline(value))
            return ValidationResult.Failure("Telefon numarası geçerli bir sabit hat numarası olmalıdır.");

        return ValidationResult.Success;
    }
}

/// <summary>
/// TR: Alanın geçerli bir e-posta adresi olmasını zorunlu kılar.
/// <br/>
/// EN: Requires the property to be a valid email address.
/// </summary>
public sealed class MihenkEmailAttribute()
    : MihenkValidationAttribute("Geçerli bir e-posta adresi giriniz.")
{
    protected override ValidationResult ValidateString(string value) => Email.Validate(value);
}

/// <summary>
/// TR: Alanın geçerli bir 5 haneli Türkiye posta kodu olmasını zorunlu kılar.
/// <br/>
/// EN: Requires the property to be a valid 5-digit Turkish postal code.
/// </summary>
public sealed class PostaKoduAttribute()
    : MihenkValidationAttribute("Geçerli bir posta kodu giriniz.")
{
    protected override ValidationResult ValidateString(string value) => PostaKodu.Validate(value);
}

// =========================================================================
// 5. Ticari, Fatura, Barkod ve IMEI (Finance & Commerce)
// =========================================================================

/// <summary>
/// TR: Alanın GİB standartlarına uygun 16 haneli geçerli bir E-Fatura / E-Arşiv numarası olmasını zorunlu kılar.
/// <br/>
/// EN: Requires the property to be a valid 16-character GIB E-Invoice number.
/// </summary>
public sealed class FaturaNoAttribute()
    : MihenkValidationAttribute("Geçerli bir 16 haneli GİB fatura numarası giriniz.")
{
    protected override ValidationResult ValidateString(string value) => FaturaNo.Validate(value);
}

/// <summary>
/// TR: Alanın geçerli bir ürün barkodu (EAN-8, UPC-A, EAN-13, GTIN-14) olmasını zorunlu kılar.
/// <br/>
/// EN: Requires the property to be a valid product barcode (EAN-8, UPC-A, EAN-13, GTIN-14).
/// </summary>
public sealed class BarkodAttribute()
    : MihenkValidationAttribute("Geçerli bir barkod numarası giriniz.")
{
    /// <summary>
    /// TR: Yalnızca GS1 Türkiye (868 / 869 ile başlayan) barkodların kabul edilip edilmeyeceği.
    /// <br/>
    /// EN: Whether only GS1 Turkey (starting with 868 or 869) EAN-13 barcodes are accepted.
    /// </summary>
    public bool TurkeyGs1Only { get; set; }

    protected override ValidationResult ValidateString(string value)
    {
        ValidationResult result = Barkod.Validate(value);
        if (!result.IsValid)
            return result;

        if (TurkeyGs1Only && !Barkod.IsTurkeyGs1(value))
            return ValidationResult.Failure("Barkod GS1 Türkiye (868 veya 869) ön ekiyle başlayan 13 haneli bir barkod olmalıdır.");

        return ValidationResult.Success;
    }
}

/// <summary>
/// TR: Alanın 15 haneli Luhn algoritmasına uygun geçerli bir IMEI numarası olmasını zorunlu kılar.
/// <br/>
/// EN: Requires the property to be a valid 15-digit IMEI number.
/// </summary>
public sealed class ImeiAttribute()
    : MihenkValidationAttribute("Geçerli bir 15 haneli IMEI numarası giriniz.")
{
    protected override ValidationResult ValidateString(string value) => Imei.Validate(value);
}
