using FluentValidation;
using Mihenk.Commerce;
using Mihenk.Communication;
using Mihenk.Finance;
using Mihenk.Identity;
using Mihenk.Vehicle;

namespace Mihenk.Validation.FluentValidation;

/// <summary>
/// TR: FluentValidation <see cref="IRuleBuilder{T, TProperty}"/> için Mihenk doğrulama kuralları.
/// Geçersiz değerlerde <see cref="ValidationResult.ErrorMessage"/> mesajını otomatik olarak hata çıktısına yansıtır.
/// <para/>
/// EN: Mihenk validation rules for FluentValidation <see cref="IRuleBuilder{T, TProperty}"/>.
/// Automatically propagates the specific <see cref="ValidationResult.ErrorMessage"/> on validation failure.
/// </summary>
public static class MihenkFluentValidationExtensions
{
    private static IRuleBuilderOptionsConditions<T, string?> ValidateWithMihenk<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        Func<string, ValidationResult> validator)
    {
        return ruleBuilder.Custom((value, context) =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            ValidationResult result = validator(value);
            if (!result.IsValid)
            {
                context.AddFailure(result.ErrorMessage ?? $"'{context.DisplayName}' geçerli değil.");
            }
        });
    }

    // =========================================================================
    // 1. Kimlik ve Vergi / Identity & Tax
    // =========================================================================

    /// <summary>
    /// TR: Alanın geçerli bir T.C. Kimlik Numarası olmasını doğrular ve geçersizse spesifik hata nedenini döner.
    /// <para/>
    /// EN: Validates that the property is a valid Turkish Republic Identity Number (TCKN) and returns the specific error reason if invalid.
    /// </summary>
    public static IRuleBuilderOptionsConditions<T, string?> TcKimlik<T>(this IRuleBuilder<T, string?> ruleBuilder)
        => ruleBuilder.ValidateWithMihenk(Identity.TcKimlik.Validate);

    /// <summary>
    /// TR: Alanın geçerli bir 10 haneli Vergi Kimlik Numarası (VKN) olmasını doğrular ve geçersizse spesifik hata nedenini döner.
    /// <para/>
    /// EN: Validates that the property is a valid 10-digit Turkish Tax Identification Number (VKN) and returns the specific error reason if invalid.
    /// </summary>
    public static IRuleBuilderOptionsConditions<T, string?> VergiNo<T>(this IRuleBuilder<T, string?> ruleBuilder)
        => ruleBuilder.ValidateWithMihenk(Identity.VergiNo.Validate);

    /// <summary>
    /// TR: Alanın geçerli bir T.C. Kimlik Numarası (11 hane) veya Vergi Kimlik Numarası (10 hane) olmasını doğrular.
    /// <para/>
    /// EN: Validates that the property is either a valid Turkish Identity Number (11 digits) or Tax Identification Number (10 digits).
    /// </summary>
    public static IRuleBuilderOptionsConditions<T, string?> TcOrVkn<T>(this IRuleBuilder<T, string?> ruleBuilder)
        => ruleBuilder.ValidateWithMihenk(Identity.VergiNo.ValidateTcOrVkn);

    /// <summary>
    /// TR: Alanın 99 ile başlayan geçerli bir Yabancı Kimlik Numarası (YKN) olmasını doğrular.
    /// <para/>
    /// EN: Validates that the property is a valid Foreigner Identity Number (YKN) starting with 99.
    /// </summary>
    public static IRuleBuilderOptionsConditions<T, string?> YabanciKimlik<T>(this IRuleBuilder<T, string?> ruleBuilder)
        => ruleBuilder.ValidateWithMihenk(Identity.YabanciKimlik.Validate);

    /// <summary>
    /// TR: Alanın 16 haneli geçerli bir MERSİS numarası olmasını doğrular.
    /// <para/>
    /// EN: Validates that the property is a valid 16-digit MERSIS number.
    /// </summary>
    public static IRuleBuilderOptionsConditions<T, string?> MersisNo<T>(this IRuleBuilder<T, string?> ruleBuilder)
        => ruleBuilder.ValidateWithMihenk(Identity.MersisNo.Validate);

    // =========================================================================
    // 2. Araç / Vehicle
    // =========================================================================

    /// <summary>
    /// TR: Alanın geçerli bir Türkiye araç plakası olmasını doğrular.
    /// <para/>
    /// EN: Validates that the property is a valid Turkish vehicle license plate.
    /// </summary>
    public static IRuleBuilderOptionsConditions<T, string?> Plaka<T>(this IRuleBuilder<T, string?> ruleBuilder)
        => ruleBuilder.ValidateWithMihenk(Vehicle.Plaka.Validate);

    /// <summary>
    /// TR: Alanın geçerli bir resmi araç plakası olmasını doğrular.
    /// <para/>
    /// EN: Validates that the property is a valid official Turkish vehicle license plate.
    /// </summary>
    public static IRuleBuilderOptionsConditions<T, string?> OfficialPlate<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.ValidateWithMihenk(static value =>
        {
            ValidationResult result = Vehicle.Plaka.Validate(value);
            if (!result.IsValid)
                return result;

            return Vehicle.Plaka.IsOfficial(value)
                ? ValidationResult.Success
                : ValidationResult.Failure("Plaka resmi bir araç plakası olmalıdır.");
        });
    }

    /// <summary>
    /// TR: Alanın 17 haneli geçerli bir araç şasi numarası (VIN) olmasını doğrular.
    /// <para/>
    /// EN: Validates that the property is a valid 17-character Vehicle Identification Number (VIN).
    /// </summary>
    public static IRuleBuilderOptionsConditions<T, string?> SasiNo<T>(this IRuleBuilder<T, string?> ruleBuilder)
        => ruleBuilder.ValidateWithMihenk(Vehicle.SasiNo.Validate);

    // =========================================================================
    // 3. Banka ve Finans / Banking & Finance
    // =========================================================================

    /// <summary>
    /// TR: Alanın geçerli bir TR IBAN numarası (26 karakter, Mod-97) olmasını doğrular.
    /// <para/>
    /// EN: Validates that the property is a valid TR IBAN number (26 characters, ISO 7064 Mod-97).
    /// </summary>
    public static IRuleBuilderOptionsConditions<T, string?> Iban<T>(this IRuleBuilder<T, string?> ruleBuilder)
        => ruleBuilder.ValidateWithMihenk(Finance.Iban.Validate);

    /// <summary>
    /// TR: Alanın Luhn algoritmasına uygun geçerli bir kredi/banka kartı numarası olmasını doğrular.
    /// <para/>
    /// EN: Validates that the property is a valid credit/debit card number according to the Luhn algorithm.
    /// </summary>
    public static IRuleBuilderOptionsConditions<T, string?> KrediKarti<T>(this IRuleBuilder<T, string?> ruleBuilder)
        => ruleBuilder.ValidateWithMihenk(Finance.KrediKarti.Validate);

    // =========================================================================
    // 4. İletişim / Communication
    // =========================================================================

    /// <summary>
    /// TR: Alanın geçerli bir Türkiye telefon numarası (cep veya sabit) olmasını doğrular.
    /// <para/>
    /// EN: Validates that the property is a valid Turkish phone number (mobile or landline).
    /// </summary>
    public static IRuleBuilderOptionsConditions<T, string?> Telefon<T>(this IRuleBuilder<T, string?> ruleBuilder)
        => ruleBuilder.ValidateWithMihenk(Communication.Telefon.Validate);

    /// <summary>
    /// TR: Alanın geçerli bir Türkiye cep telefonu numarası (5XX) olmasını doğrular.
    /// <para/>
    /// EN: Validates that the property is a valid Turkish mobile phone number (5XX).
    /// </summary>
    public static IRuleBuilderOptionsConditions<T, string?> MobilePhone<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.ValidateWithMihenk(static value =>
        {
            ValidationResult result = Communication.Telefon.Validate(value);
            if (!result.IsValid)
                return result;

            return Communication.Telefon.IsMobile(value)
                ? ValidationResult.Success
                : ValidationResult.Failure("Telefon numarası geçerli bir cep telefonu numarası (5XX) olmalıdır.");
        });
    }

    /// <summary>
    /// TR: Alanın geçerli bir Türkiye sabit hat telefon numarası olmasını doğrular.
    /// <para/>
    /// EN: Validates that the property is a valid Turkish landline phone number.
    /// </summary>
    public static IRuleBuilderOptionsConditions<T, string?> LandlinePhone<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.ValidateWithMihenk(static value =>
        {
            ValidationResult result = Communication.Telefon.Validate(value);
            if (!result.IsValid)
                return result;

            return Communication.Telefon.IsLandline(value)
                ? ValidationResult.Success
                : ValidationResult.Failure("Telefon numarası geçerli bir sabit hat numarası olmalıdır.");
        });
    }

    /// <summary>
    /// TR: Alanın geçerli bir e-posta adresi olmasını doğrular.
    /// <para/>
    /// EN: Validates that the property is a valid email address.
    /// </summary>
    public static IRuleBuilderOptionsConditions<T, string?> MihenkEmail<T>(this IRuleBuilder<T, string?> ruleBuilder)
        => ruleBuilder.ValidateWithMihenk(Email.Validate);

    /// <summary>
    /// TR: Alanın geçerli bir 5 haneli Türkiye posta kodu olmasını doğrular.
    /// <para/>
    /// EN: Validates that the property is a valid 5-digit Turkish postal code.
    /// </summary>
    public static IRuleBuilderOptionsConditions<T, string?> PostaKodu<T>(this IRuleBuilder<T, string?> ruleBuilder)
        => ruleBuilder.ValidateWithMihenk(Communication.PostaKodu.Validate);

    // =========================================================================
    // 5. Fatura, Barkod ve IMEI / Invoice, Barcode & IMEI
    // =========================================================================

    /// <summary>
    /// TR: Alanın GİB standartlarına uygun 16 haneli geçerli bir E-Fatura / E-Arşiv numarası olmasını doğrular.
    /// <para/>
    /// EN: Validates that the property is a valid 16-character Turkish E-Invoice / E-Archive number compliant with GIB standards.
    /// </summary>
    public static IRuleBuilderOptionsConditions<T, string?> FaturaNo<T>(this IRuleBuilder<T, string?> ruleBuilder)
        => ruleBuilder.ValidateWithMihenk(Finance.FaturaNo.Validate);

    /// <summary>
    /// TR: Alanın geçerli bir ürün barkodu (EAN-8, UPC-A, EAN-13, GTIN-14) olmasını doğrular.
    /// <para/>
    /// EN: Validates that the property is a valid product barcode (EAN-8, UPC-A, EAN-13, GTIN-14).
    /// </summary>
    public static IRuleBuilderOptionsConditions<T, string?> Barkod<T>(this IRuleBuilder<T, string?> ruleBuilder)
        => ruleBuilder.ValidateWithMihenk(Commerce.Barkod.Validate);

    /// <summary>
    /// TR: Alanın GS1 Türkiye (868 veya 869 ön ekiyle başlayan) geçerli bir EAN-13 barkodu olmasını doğrular.
    /// <para/>
    /// EN: Validates that the property is a valid EAN-13 barcode with a GS1 Turkey prefix (868 or 869).
    /// </summary>
    public static IRuleBuilderOptionsConditions<T, string?> TurkeyGs1Barcode<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.ValidateWithMihenk(static value =>
        {
            ValidationResult result = Commerce.Barkod.Validate(value);
            if (!result.IsValid)
                return result;

            return Commerce.Barkod.IsTurkeyGs1(value)
                ? ValidationResult.Success
                : ValidationResult.Failure("Barkod GS1 Türkiye (868 veya 869) ön ekiyle başlayan 13 haneli bir barkod olmalıdır.");
        });
    }

    /// <summary>
    /// TR: Alanın 15 haneli Luhn algoritmasına uygun geçerli bir IMEI numarası olmasını doğrular.
    /// <para/>
    /// EN: Validates that the property is a valid 15-digit IMEI number according to the Luhn algorithm.
    /// </summary>
    public static IRuleBuilderOptionsConditions<T, string?> Imei<T>(this IRuleBuilder<T, string?> ruleBuilder)
        => ruleBuilder.ValidateWithMihenk(Commerce.Imei.Validate);
}
