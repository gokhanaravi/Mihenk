<div align="center">

<img src="https://raw.githubusercontent.com/gokhanaravi/Mihenk/master/icon.png" width="96" height="96" alt="Mihenk Logo" />

# Mihenk

**Türkiye'ye özgü verileri doğrulamak, dönüştürmek ve maskelemek için sıfır bağımlılıklı .NET kütüphanesi.**

*Zero-dependency .NET toolkit for validating, formatting and masking Turkey-specific data.*

[![NuGet](https://img.shields.io/nuget/v/Mihenk?logo=nuget&label=Mihenk&color=004880)](https://www.nuget.org/packages/Mihenk)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Mihenk?logo=nuget&label=indirme&color=004880)](https://www.nuget.org/packages/Mihenk)
[![Version](https://img.shields.io/badge/sürüm-1.0.2-2ea44f)](https://github.com/gokhanaravi/Mihenk/releases)
[![.NET](https://img.shields.io/badge/.NET-7%20%7C%208%20%7C%209%20%7C%2010-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-latest-239120?logo=csharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)
[![Dependencies](https://img.shields.io/badge/bağımlılık-0-brightgreen)](#-paketler)
[![Offline](https://img.shields.io/badge/API%20çağrısı-yok-brightgreen)](#-felsefe)
[![License](https://img.shields.io/badge/lisans-MIT-yellow.svg)](LICENSE)

[Kurulum](#-kurulum) •
[Hızlı Başlangıç](#-hızlı-başlangıç) •
[Kimlik](#-kimlik-ve-vergi--mihenkidentity) •
[Araç](#-araç--mihenkvehicle) •
[Finans](#-finans--mihenkfinance) •
[İletişim](#-i̇letişim--mihenkcommunication) •
[Metin](#-metin--mihenktext) •
[KVKK](#-kvkk-maskeleme--mihenksecurity) •
[Tarih](#-resmi-tatil-ve-iş-günü--mihenkdate) •
[Coğrafya](#-coğrafya--mihenkgeography) •
[Validation](#-dataannotations-öznitelikleri)

</div>

---

## ✨ Öne Çıkanlar

| | |
|---|---|
| 🧩 **Sıfır bağımlılık** | Çekirdek `Mihenk` paketi hiçbir NuGet paketine bağlı değildir. |
| 🔌 **%100 offline** | Hiçbir dış API'ye istek atılmaz; tüm veriler (il/ilçe, banka kodları, Diyanet tatil tablosu) DLL içine gömülüdür. |
| 💬 **Hata nedeni döner** | `Validate()` yalnızca "geçersiz" demez, **"T.C. Kimlik Numarası 11 haneli olmalıdır."** gibi açıklama döner. |
| 🧪 **4 farklı kullanım** | Statik metotlar · Extension metotlar · DataAnnotations · FluentValidation |
| 🔁 **Try deseni** | Her dönüştürme için `TryXxx(input, out result)` karşılığı vardır. |
| ⚡ **Zero-allocation** | Doğrulama metotlarının `ReadOnlySpan<char>` overload'ları vardır. |
| 🌍 **Çift dilli dokümantasyon** | Tüm XML `<summary>` açıklamaları Türkçe + İngilizce'dir; IntelliSense'te ikisini birden görürsünüz. |

---

## 📦 Paketler

| Paket | Sürüm | Açıklama | Bağımlılık |
|---|---|---|---|
| **Mihenk** | [![NuGet](https://img.shields.io/nuget/v/Mihenk?label=)](https://www.nuget.org/packages/Mihenk) | Kimlik, vergi, araç, finans, iletişim, metin, KVKK, ticaret, test verisi, DataAnnotations | — |
| **Mihenk.Date** | [![NuGet](https://img.shields.io/nuget/v/Mihenk.Date?label=)](https://www.nuget.org/packages/Mihenk.Date) | Diyanet tablosuna dayalı resmi tatiller (2020–2035) ve iş günü hesaplama | `Mihenk` |
| **Mihenk.Geography** | [![NuGet](https://img.shields.io/nuget/v/Mihenk.Geography?label=)](https://www.nuget.org/packages/Mihenk.Geography) | 81 il, 973 ilçe, alan kodları, bölgeler, kuş uçuşu mesafe | `Mihenk` |
| **Mihenk.Validation.FluentValidation** | [![NuGet](https://img.shields.io/nuget/v/Mihenk.Validation.FluentValidation?label=)](https://www.nuget.org/packages/Mihenk.Validation.FluentValidation) | FluentValidation kural uzantıları | `Mihenk`, `FluentValidation 11.x` |

### Uyumluluk

| Bileşen | Sürüm |
|---|---|
| Kütüphane sürümü | `1.0.2` |
| Hedef çatılar (TFM) | `net7.0` · `net8.0` · `net9.0` · `net10.0` |
| C# dil sürümü | `latest` |
| Nullable reference types | ✅ Etkin |
| Resmi tatil veri aralığı | `2020` – `2035` |

---

## 🚀 Kurulum

```bash
# Çekirdek paket (bağımlılık yok)
dotnet add package Mihenk

# İsteğe bağlı paketler
dotnet add package Mihenk.Date
dotnet add package Mihenk.Geography
dotnet add package Mihenk.Validation.FluentValidation
```

---

## ⚡ Hızlı Başlangıç

Aynı doğrulamayı dört farklı şekilde yapabilirsiniz:

```csharp
using Mihenk.Identity;
using Mihenk.Extensions;

// 1) Statik metot
bool ok = TcKimlik.IsValid("10000000146");                 // true

// 2) Extension metot
bool ok2 = "10000000146".IsValidTcKimlik();               // true

// 3) Hata nedeni ile
var result = TcKimlik.Validate("1234");
Console.WriteLine(result.ErrorMessage);                    // "T.C. Kimlik Numarası 11 haneli olmalıdır."

// 4) Try deseni (boşlukları temizler, normalize edilmiş değeri döner)
if (TcKimlik.TryValidate(" 10000000146 ", out string? tc))
    Console.WriteLine(tc);                                 // "10000000146"
```

```csharp
// 5) DataAnnotations
public class CustomerRequest
{
    [TcKimlik] public string? TcNo { get; set; }
}

// 6) FluentValidation
RuleFor(x => x.TcNo).NotEmpty().TcKimlik();
```

---

## ✅ ValidationResult

Tüm `Validate()` metotları `Mihenk.ValidationResult` döner.

```csharp
using Mihenk;

ValidationResult result = TcKimlik.Validate("1234");

result.IsValid;        // false
result.ErrorMessage;   // "T.C. Kimlik Numarası 11 haneli olmalıdır."
result.ToString();     // "T.C. Kimlik Numarası 11 haneli olmalıdır."  (geçerliyse "Valid")

// bool'a örtük dönüşüm
if (TcKimlik.Validate("10000000146")) { /* geçerli */ }

// Kendi doğrulayıcılarınızda
ValidationResult ok   = ValidationResult.Success;                     // IsValid = true
ValidationResult fail = ValidationResult.Failure("Hatalı değer.");    // IsValid = false
```

> [!NOTE]
> Her `IsValid` / `Validate` metodunun `ReadOnlySpan<char>` alan ve bellek ayırmayan (zero-allocation) bir overload'ı vardır:
> `TcKimlik.IsValid("10000000146".AsSpan())`

---

## 🪪 Kimlik ve Vergi — `Mihenk.Identity`

### TcKimlik — T.C. Kimlik Numarası

```csharp
using Mihenk.Identity;

TcKimlik.IsValid("10000000146");                    // true
TcKimlik.IsValid("10000000146".AsSpan());           // true  (zero-allocation)

TcKimlik.Validate("10000000146");                   // Valid
TcKimlik.Validate("1234");                          // "T.C. Kimlik Numarası 11 haneli olmalıdır."
TcKimlik.Validate("01234567890");                   // "T.C. Kimlik Numarasının ilk hanesi 0 olamaz."
TcKimlik.Validate("10000000147");                   // "T.C. Kimlik Numarası 11. hane kontrol algoritmasına uymuyor."
TcKimlik.Validate("10000000146".AsSpan());          // Valid

TcKimlik.Mask("10000000146");                       // "100******46"
TcKimlik.Mask("10000000146", '#');                  // "100######46"

TcKimlik.TryValidate(" 10000000146 ", out var tc);  // true  → tc = "10000000146"
TcKimlik.TryMask("10000000146", out var masked);    // true  → masked = "100******46"
```

### VergiNo — Vergi Kimlik Numarası (VKN)

```csharp
VergiNo.IsValid("4840847211");                      // true
VergiNo.IsValid("4840847211".AsSpan());             // true

VergiNo.Validate("4840847211");                     // Valid
VergiNo.Validate("1234567891");                     // "Vergi Kimlik Numarası kontrol basamağı algoritmasına uymuyor."
VergiNo.Validate("4840847211".AsSpan());            // Valid

// TCKN (11 hane) veya VKN (10 hane) – fatura/cari formları için ideal
VergiNo.IsValidTcOrVkn("10000000146");              // true
VergiNo.IsValidTcOrVkn("4840847211");               // true
VergiNo.ValidateTcOrVkn("123");                     // "Değer 10 haneli Vergi Kimlik Numarası veya 11 haneli T.C. Kimlik Numarası olmalıdır."

VergiNo.TryValidate("4840847211", out var vkn);              // true → "4840847211"
VergiNo.TryValidateTcOrVkn("10000000146", out var tcOrVkn);  // true → "10000000146"
```

### YabanciKimlik — Yabancı Kimlik Numarası (YKN)

```csharp
YabanciKimlik.IsValid("99000000042");               // true
YabanciKimlik.IsValid("99000000042".AsSpan());      // true

YabanciKimlik.Validate("99000000042");              // Valid
YabanciKimlik.Validate("10000000146");              // "Yabancı Kimlik Numarası '99' ile başlamalıdır."
YabanciKimlik.Validate("99000000042".AsSpan());     // Valid

YabanciKimlik.TryValidate("99000000042", out var ykn);   // true → "99000000042"
```

### MersisNo — MERSİS Numarası

```csharp
MersisNo.IsValid("0484084721100001");               // true  (0 + VKN + 5 hane)
MersisNo.IsValid("0484084721100001".AsSpan());      // true

MersisNo.Validate("0484084721100001");              // Valid
MersisNo.Validate("123");                           // "MERSİS numarası 16 haneli olmalıdır."
MersisNo.Validate("0484084721100001".AsSpan());     // Valid

MersisNo.TryValidate("0484084721100001", out var mersis);   // true → "0484084721100001"
```

---

## 🚗 Araç — `Mihenk.Vehicle`

### Plaka — Türkiye Araç Plakası

```csharp
using Mihenk.Vehicle;

Plaka.IsValid("34 ABC 123");                        // true
Plaka.IsValid("34ABC123".AsSpan());                 // true

Plaka.Validate("06 A 1234");                        // Valid
Plaka.Validate("99 A 123");                         // "Plaka il kodu 01 ile 81 arasında olmalıdır."
Plaka.Validate("34 ABC 12345");                     // "Plakadaki 3 harf ve 5 rakam kombinasyonu Türkiye plaka standartlarına uygun değil."
Plaka.Validate("34ABC123".AsSpan());                // Valid

Plaka.Normalize("34abc123");                        // "34 ABC 123"
Plaka.GetProvinceCode("34ABC123");                  // 34
Plaka.GetProvinceName("06 A 1234");                 // "Ankara"

PlateInfo? info = Plaka.Parse("35 KS 4567");        // "35 KS 4567"
// info.Value.ProvinceCode → 35
// info.Value.ProvinceName → "İzmir"
// info.Value.Letters      → "KS"
// info.Value.Digits       → "4567"

Plaka.IsOfficial("06 A 12345");                     // true   (resmi plaka)
Plaka.IsOfficial("34 ABC 123");                     // false

Plaka.TryParse("34ABC123", out PlateInfo plate);             // true → "34 ABC 123"
Plaka.TryNormalize("34abc123", out var normalized);          // true → "34 ABC 123"
Plaka.TryGetProvinceCode("16 BB 999", out int code);         // true → 16
Plaka.TryGetProvinceName("16 BB 999", out var provinceName); // true → "Bursa"
```

### SasiNo — Şasi Numarası (VIN, ISO 3779)

```csharp
SasiNo.IsValid("1HGCM82633A004352");                // true
SasiNo.IsValid("1HGCM82633A004352".AsSpan());       // true

SasiNo.Validate("1HGCM82633A004352");               // Valid
SasiNo.Validate("1HGCM82633A00435I");               // "Şasi numarası (VIN) ISO 3779 standardına göre 'I' harfini içeremez."
SasiNo.Validate("1HGCM82633A004352".AsSpan());      // Valid

SasiNo.TryValidate("1hgcm82633a004352", out var vin);   // true → "1HGCM82633A004352"
```

---

## 💳 Finans — `Mihenk.Finance`

### Iban — TR IBAN

```csharp
using Mihenk.Finance;

Iban.IsValid("TR170001007301665990304734");                     // true
Iban.IsValid("TR17 0001 0073 0166 5990 3047 34");               // true (boşluklar yok sayılır)
Iban.IsValid("TR170001007301665990304734".AsSpan());            // true

Iban.Validate("TR170001007301665990304734");                    // Valid
Iban.Validate("TR170001007301665990304735");                    // "IBAN kontrol basamağı (Mod-97) algoritmasına uymuyor."
Iban.Validate("DE89370400440532013000");                        // "Türkiye IBAN numarası 'TR' ülke kodu ile başlamalıdır."
Iban.Validate("TR170001007301665990304734".AsSpan());           // Valid

Iban.Format("TR170001007301665990304734");                      // "TR17 0001 0073 0166 5990 3047 34"
Iban.Mask("TR170001007301665990304734");                        // "TR17 **** **** **** **** **** 34"
Iban.GetBankCode("TR170001007301665990304734");                 // "00010"
Iban.GetBankName("TR170001007301665990304734");                 // "Ziraat Bankası"

Iban.TryValidate("tr17 0001 0073 0166 5990 3047 34", out var iban);   // true → "TR170001007301665990304734"
Iban.TryFormat("TR170001007301665990304734", out var formatted);       // true → "TR17 0001 0073 0166 5990 3047 34"
Iban.TryMask("TR170001007301665990304734", out var maskedIban);        // true → "TR17 **** **** **** **** **** 34"
Iban.TryGetBankCode("TR170001007301665990304734", out var bankCode);   // true → "00010"
Iban.TryGetBankName("TR170001007301665990304734", out var bankName);   // true → "Ziraat Bankası"
```

### KrediKarti — Kredi / Banka Kartı (Luhn)

```csharp
KrediKarti.IsValid("4111111111111111");             // true
KrediKarti.IsValid("4111111111111111".AsSpan());    // true

KrediKarti.Validate("4111 1111 1111 1111");         // Valid
KrediKarti.Validate("4111111111111112");            // "Kredi kartı numarası Luhn kontrol algoritmasına uymuyor."
KrediKarti.Validate("4111111111111111".AsSpan());   // Valid

KrediKarti.GetCardType("4111111111111111");         // CardType.Visa
KrediKarti.GetCardType("5555555555554444");         // CardType.Mastercard
KrediKarti.GetCardType("9792030000000000");         // CardType.Troy
KrediKarti.GetCardType("378282246310005");          // CardType.Amex
KrediKarti.GetCardType("9792030000000000".AsSpan());// CardType.Troy

KrediKarti.Mask("4111111111111111");                // "4111 **** **** 1111"

KrediKarti.TryValidate("4111 1111 1111 1111", out var card);      // true → "4111111111111111"
KrediKarti.TryGetCardType("5555555555554444", out CardType type); // true → CardType.Mastercard
KrediKarti.TryMask("5555555555554444", out var maskedCard);       // true → "5555 **** **** 4444"
```

### Para — Tutarı Yazıya Çevirme ve TL Formatı

```csharp
Para.ToWords(1250.75m);                             // "Bin iki yüz elli TL yetmiş beş kuruş"
Para.ToWords(1000000m);                             // "Bir milyon TL"
Para.ToWords(100.50m, "Dolar", "sent");             // "Yüz Dolar elli sent"

Para.FormatTl(1234567.891m);                        // "1.234.567,89 ₺"
Para.FormatTl(0m);                                  // "0,00 ₺"
```

### Kdv — KDV ve Tevkifat Hesaplama

`Kdv.Calculate` bir `VatCalculationResult { NetAmount, VatAmount, TotalAmount, VatRate, WithholdingAmount }` döner. Yuvarlama `MidpointRounding.AwayFromZero` ile 2 haneye yapılır.

> [!IMPORTANT]
> Varsayılan KDV oranı **%20**'dir. KDV oranları mevzuatla değişebileceği için oranı açıkça vermeniz önerilir.

```csharp
Kdv.Calculate(1000m);
// NetAmount = 1000, VatAmount = 200, TotalAmount = 1200, VatRate = 20

Kdv.Calculate(1200m, vatRate: 20, isVatIncluded: true);   // KDV dahil tutardan geriye hesap
// NetAmount = 1000, VatAmount = 200, TotalAmount = 1200

Kdv.Calculate(100m, 10);
// NetAmount = 100, VatAmount = 10, TotalAmount = 110, VatRate = 10

// Tevkifatlı KDV (ör. 5/10 tevkifat)
Kdv.CalculateWithholding(10000m, vatRate: 20, withholdingNumerator: 5);
// NetAmount = 10000, VatAmount = 2000, WithholdingAmount = 1000, TotalAmount = 11000

Kdv.CalculateWithholding(10000m, 20, 9, 10);              // 9/10 tevkifat
// VatAmount = 2000, WithholdingAmount = 1800, TotalAmount = 10200
```

### FaturaNo — GİB E-Fatura / E-Arşiv Numarası

```csharp
FaturaNo.IsValid("ABC2026000000001");               // true
FaturaNo.IsValid("ABC2026000000001".AsSpan());      // true

FaturaNo.Validate("ABC2026000000001");              // Valid
FaturaNo.Validate("ABC2026");                       // "GİB fatura numarası 16 karakter olmalıdır."
FaturaNo.Validate("ABC2026000000000");              // "Fatura sıra numarası '000000000' olamaz."
FaturaNo.Validate("ABC2026000000001".AsSpan());     // Valid

FaturaNo.TryValidate("ABC2026000000001", out var invoiceNo);   // true → "ABC2026000000001"

// Seri (3 harf) zorunludur, varsayılan değer yoktur
FaturaNo.Generate("ABC", 2026);                     // "ABC2026000000001"
FaturaNo.Generate("XYZ", 2026, 42);                 // "XYZ2026000000042"
FaturaNo.Generate("AB", 2026, 1);                   // ArgumentException: "Fatura seri/birim kodu tam olarak 3 harften oluşmalıdır."
```

---

## 📞 İletişim — `Mihenk.Communication`

### Telefon

```csharp
using Mihenk.Communication;

Telefon.IsValid("0532 123 45 67");                  // true
Telefon.IsValid("+905321234567".AsSpan());          // true

Telefon.Validate("+90 532 123 45 67");              // Valid
Telefon.Validate("0532123");                        // "Telefon numarası (ülke kodu hariç) 10 haneli olmalıdır."
Telefon.Validate("05321234567".AsSpan());           // Valid

Telefon.Normalize("0532 123 45 67");                // "+905321234567"   (E.164)
Telefon.ToShort("+90 532 123 45 67");               // "5321234567"
Telefon.Format("5321234567");                       // "0 (532) 123 45 67"
Telefon.Mask("05321234567");                        // "0 (532) *** ** 67"

Telefon.IsMobile("05321234567");                    // true
Telefon.IsMobile("0 (212) 345 67 89");              // false
Telefon.IsLandline("0 (212) 345 67 89");            // true
Telefon.IsLandline("08501234567");                  // true

Telefon.Extract("Bize 0532 123 45 67 veya +90 (212) 345 67 89 numarasından ulaşın.");
// ["0532 123 45 67", "+90 (212) 345 67 89"]

Telefon.TryNormalize("0532 123 45 67", out var e164);   // true → "+905321234567"
Telefon.TryToShort("+905321234567", out var shortNo);   // true → "5321234567"
Telefon.TryFormat("5321234567", out var pretty);        // true → "0 (532) 123 45 67"
Telefon.TryMask("05321234567", out var maskedPhone);    // true → "0 (532) *** ** 67"
```

### Email

```csharp
Email.IsValid("ali.veli@example.com");              // true
Email.IsValid("ali.veli@example.com".AsSpan());     // true

Email.Validate("ali.veli@example.com");             // Valid
Email.Validate("ali..veli@example.com");            // "E-posta adresi ardışık iki nokta ('..') içeremez."
Email.Validate("ali@");                             // "E-posta adresi çok kısa."
Email.Validate("ali.veli@example.com".AsSpan());    // Valid

Email.Mask("ali.veli@example.com");                 // "al******@ex*****.com"

Email.Extract("İletişim: info@mihenk.dev, destek@mihenk.dev");
// ["info@mihenk.dev", "destek@mihenk.dev"]

Email.TryValidate(" AHMET@Firma.COM.TR ", out var email);       // true → "ahmet@firma.com.tr"
Email.TryMask("ahmet.yilmaz@gmail.com", out var maskedEmail);   // true → "ah**********@gm***.com"
```

### PostaKodu

```csharp
PostaKodu.IsValid("06100");                         // true
PostaKodu.IsValid("06100".AsSpan());                // true

PostaKodu.Validate("06100");                        // Valid
PostaKodu.Validate("3400");                         // "Posta kodu 5 haneli olmalıdır."
PostaKodu.Validate("82000");                        // "Posta kodunun ilk 2 hanesi geçerli bir il plaka kodu (01-81) olmalıdır."
PostaKodu.Validate("34000");                        // "Posta kodunun son 3 hanesi '000' olamaz."
PostaKodu.Validate("06100".AsSpan());               // Valid

PostaKodu.TryValidate("06100", out var postalCode); // true → "06100"
```

---

## 📝 Metin — `Mihenk.Text`

### Link, Hashtag, Mention

```csharp
using Mihenk.Text;

Link.Extract("Detaylar https://mihenk.dev ve www.github.com/mihenk adresinde.");
// ["https://mihenk.dev", "www.github.com/mihenk"]

Hashtag.Extract("Bugün #dotnet ve #Türkçe ile çalıştık #csharp");
// ["#dotnet", "#Türkçe", "#csharp"]

Mention.Extract("@ahmet ve @ayşe_yılmaz toplantıya katıldı. mail@test.com");
// ["@ahmet", "@ayşe_yılmaz"]
```

> [!NOTE]
> `Extract` metotları hiçbir zaman hata fırlatmaz; eşleşme yoksa boş bir `IReadOnlyList<string>` döner.

### TurkceMetin — Türkçe Metin İşlemleri

```csharp
TurkceMetin.ToSlug("Çağrı Şüheda'nın İlk Ödevi: Güzel Örnek!");   // "cagri-suheda-nin-ilk-odevi-guzel-ornek"

TurkceMetin.ToUpperTr("istanbul ılık");             // "İSTANBUL ILIK"
TurkceMetin.ToLowerTr("İSTANBUL IŞIK");             // "istanbul ışık"
TurkceMetin.ToTitleCaseTr("iSTANBUL ve izmir ılıcası");   // "İstanbul Ve İzmir Ilıcası"

TurkceMetin.RemoveTurkishChars("Çağrı Şüheda Ilık Öğle"); // "Cagri Suheda Ilik Ogle"
TurkceMetin.FixEncoding("Ä°stanbul'da gÃ¼zel bir gÃ¼n");   // "İstanbul'da güzel bir gün"

TurkceMetin.EqualsIgnoreCaseTr("İSTANBUL", "istanbul");   // true
TurkceMetin.ContainsTr("Merhaba İSTANBUL", "istanbul");   // true

TurkceMetin.NumberToWords(1984);                    // "bin dokuz yüz seksen dört"
TurkceMetin.NumberToWords(-25);                     // "eksi yirmi beş"
TurkceMetin.NumberToWords(0);                       // "sıfır"
TurkceMetin.NumberToWords(1_250_000_000L);          // "bir milyar iki yüz elli milyon"

var now = new DateTime(2026, 11, 10, 12, 0, 0);
TurkceMetin.ToRelativeTimeTr(now.AddSeconds(-20), now);   // "az önce"
TurkceMetin.ToRelativeTimeTr(now.AddMinutes(-5), now);    // "5 dakika önce"
TurkceMetin.ToRelativeTimeTr(now.AddHours(-3), now);      // "3 saat önce"
TurkceMetin.ToRelativeTimeTr(now.AddDays(-1), now);       // "dün"
TurkceMetin.ToRelativeTimeTr(now.AddDays(-14), now);      // "2 hafta önce"
TurkceMetin.ToRelativeTimeTr(now.AddMonths(-3), now);     // "3 ay önce"
TurkceMetin.ToRelativeTimeTr(now.AddYears(-2), now);      // "2 yıl önce"
TurkceMetin.ToRelativeTimeTr(now.AddDays(3), now);        // "3 gün sonra"
TurkceMetin.ToRelativeTimeTr(someDate);                   // referans verilmezse DateTime.Now kullanılır
```

### TurkceEk — Türkçe Hal Ekleri

Ünlü uyumu, ünsüz benzeşmesi, kaynaştırma harfleri, kısaltmalar ve sayılar desteklenir. Varsayılan olarak özel isimler için kesme işareti (`'`) eklenir.

```csharp
TurkceEk.ToDative("Ankara");                        // "Ankara'ya"     (yönelme  -e)
TurkceEk.ToLocative("İstanbul");                    // "İstanbul'da"   (bulunma  -de)
TurkceEk.ToAblative("Ahmet");                       // "Ahmet'ten"     (ayrılma  -den)
TurkceEk.ToGenitive("Türkiye");                     // "Türkiye'nin"   (ilgi     -in)
TurkceEk.ToAccusative("İstanbul");                  // "İstanbul'u"    (belirtme -i)
TurkceEk.ToInstrumental("Ankara");                  // "Ankara'yla"    (vasıta   -le)
TurkceEk.ToPlural("kitap");                         // "kitaplar"      (çoğul    -ler)

// İstisnalar ve özel durumlar
TurkceEk.ToDative("saat");                          // "saat'e"   (saat'a değil)
TurkceEk.ToDative("harf");                          // "harf'e"
TurkceEk.ToDative("TBMM");                          // "TBMM'ye"  (kısaltma okunuşu: te-be-me-me)
TurkceEk.ToLocative("PTT");                         // "PTT'de"
TurkceEk.ToPlural("TBMM");                          // "TBMM'ler"
TurkceEk.ToDative("5");                             // "5'e"      (beş)
TurkceEk.ToPlural("1990");                          // "1990'lar"
TurkceEk.ToGenitive("2026");                        // "2026'nın" (iki bin yirmi altı)

// Cins isimler için kesme işaretsiz (ünsüz yumuşaması uygulanır)
TurkceEk.ToDative("kitap", useApostrophe: false);   // "kitaba"
TurkceEk.ToLocative("ev", useApostrophe: false);    // "evde"
```

---

## 🔒 KVKK Maskeleme — `Mihenk.Security`

`Kvkk` sınıfı tüm maskeleme işlemleri için **tek giriş noktası**dır.

```csharp
using Mihenk.Security;

Kvkk.MaskTcKimlik("10000000146");                   // "100******46"
Kvkk.MaskCreditCard("4111111111111111");            // "4111 **** **** 1111"
Kvkk.MaskIban("TR170001007301665990304734");        // "TR17 **** **** **** **** **** 34"
Kvkk.MaskPhone("05321234567");                      // "0 (532) *** ** 67"
Kvkk.MaskEmail("ahmet.yilmaz@gmail.com");           // "ah**********@gm***.com"
Kvkk.MaskFullName("Ahmet Yılmaz");                  // "Ah*** Yı****"
Kvkk.MaskFullName("Mehmet Ali Öztürk");             // "Me**** A** Öz****"
Kvkk.MaskIp("192.168.1.42");                        // "192.168.1.***"
Kvkk.MaskIp("2001:0db8:85a3:0000:0000:8a2e:0370:7334");   // "2001:0db8:85a3:0000:0000:8a2e:0370:****"

// Serbest metin içindeki tüm kişisel verileri otomatik bulup maskeler (log'lar için ideal)
Kvkk.MaskText("TC: 10000000146, Tel: 0532 123 45 67, Mail: ahmet@gmail.com, " +
              "IBAN: TR330006100519786457841326, Kart: 4111 1111 1111 1111");
// "TC: 100******46, Tel: 0 (532) *** ** 67, Mail: ah***@gm***.com,
//  IBAN: TR33 **** **** **** **** **** 26, Kart: 4111 **** **** 1111"

// Tüm metotlar özel maske karakteri alır
Kvkk.MaskTcKimlik("10000000146", 'X');              // "100XXXXXX46"
```

---

## 🛒 Ticaret — `Mihenk.Commerce`

### Barkod — EAN-8, UPC-A, EAN-13, GTIN-14

```csharp
using Mihenk.Commerce;

Barkod.IsValid("8690000000012");                    // true
Barkod.IsValid("8690000000012".AsSpan());           // true

Barkod.Validate("96385074");                        // Valid  (EAN-8)
Barkod.Validate("036000291452");                    // Valid  (UPC-A)
Barkod.Validate("4006381333931");                   // Valid  (EAN-13)
Barkod.Validate("10614141000415");                  // Valid  (GTIN-14)
Barkod.Validate("8690000000019");                   // "Barkod numarası GS1 kontrol basamağı algoritmasına uymuyor."
Barkod.Validate("12345");                           // "Barkod numarası 8 (EAN-8), 12 (UPC-A), 13 (EAN-13) veya 14 (GTIN-14) haneli olmalıdır."
Barkod.Validate("8690000000012".AsSpan());          // Valid

// 868/869 GS1 Türkiye ön ekidir. Barkodun Türkiye'de kayıtlı bir firmaya ait olduğunu gösterir,
// ürünün Türkiye'de üretildiğini GÖSTERMEZ.
Barkod.IsTurkeyGs1("8690000000012");                // true
Barkod.IsTurkeyGs1("4006381333931");                // false

Barkod.TryValidate(" 8690000000012 ", out var barcode);   // true → "8690000000012"
```

### Imei

```csharp
Imei.IsValid("490154203237518");                    // true
Imei.IsValid("490154203237518".AsSpan());           // true

Imei.Validate("490154203237518");                   // Valid
Imei.Validate("490154203237519");                   // "IMEI numarası Luhn kontrol algoritmasına uymuyor."
Imei.Validate("490154203237518".AsSpan());          // Valid

Imei.TryValidate("49-015420-323751-8", out var imei);   // true → "490154203237518"
```

---

## 🧪 Test Verisi — `Mihenk.Testing`

Algoritmik olarak **geçerli**, rastgele test verisi üretir. Üretim kodunun IntelliSense listesini kirletmemesi için ayrı bir namespace'tedir.

> [!CAUTION]
> Üretilen değerler yalnızca algoritmik olarak geçerlidir; gerçek kişi veya kurumlara ait olabilir. Yalnızca test ortamında kullanın.

```csharp
using Mihenk.Testing;

MihenkTest.TcKimlik();                              // "50754794168"   (her çağrıda farklı)
MihenkTest.YabanciKimlik();                         // "99268473862"
MihenkTest.VergiNo();                               // "1191219969"
MihenkTest.Imei();                                  // "355211455509616"
MihenkTest.Iban();                                  // "TR900006404685228009132073"  (varsayılan banka: 00064)
MihenkTest.Iban("00010");                           // "TR170001007301665990304734"  (Ziraat Bankası)
```

---

## 📅 Resmi Tatil ve İş Günü — `Mihenk.Date`

```bash
dotnet add package Mihenk.Date
```

### ResmiTatil

Dini bayram tarihleri **hesaplanmaz**; Diyanet İşleri Başkanlığı'nın açıkladığı resmi tarihler gömülü tablodan okunur. Desteklenen aralık dışındaki bir yıl için tahmin yürütülmez, hata fırlatılır.

```csharp
using Mihenk.Date;

ResmiTatil.MinSupportedYear;                        // 2020
ResmiTatil.MaxSupportedYear;                        // 2035

ResmiTatil.IsPublicHoliday(new DateTime(2026, 10, 29));                 // true   (Cumhuriyet Bayramı)
ResmiTatil.IsPublicHoliday(new DateTime(2026, 10, 28));                 // false  (arife yarım gün)
ResmiTatil.IsPublicHoliday(new DateTime(2026, 10, 28), includeHalfDays: true);   // true

HolidayInfo? info = ResmiTatil.GetHolidayInfo(new DateTime(2026, 3, 20));
// info.Value.Date      → 20.03.2026
// info.Value.Name      → "Ramazan Bayramı 1. Gün"
// info.Value.IsHalfDay → false

ResmiTatil.GetHolidayInfo(new DateTime(2026, 11, 10));                  // null

ResmiTatil.TryGetHoliday(new DateTime(2026, 5, 27), out HolidayInfo h); // true → h.Name = "Kurban Bayramı 1. Gün"

IReadOnlyList<HolidayInfo> holidays = ResmiTatil.GetHolidays(2026);     // 17 kayıt, tarihe göre sıralı
ResmiTatil.GetHolidays(2040);   // ArgumentOutOfRangeException: "Diyanet resmi tatil tablosu yalnızca 2020 ile 2035 yılları arasını desteklemektedir."
```

<details>
<summary><b>2026 yılı çıktısı</b></summary>

| Tarih | Tatil | Yarım Gün |
|---|---|:-:|
| 2026-01-01 | Yılbaşı | |
| 2026-03-19 | Ramazan Bayramı Arifesi | ✅ |
| 2026-03-20 | Ramazan Bayramı 1. Gün | |
| 2026-03-21 | Ramazan Bayramı 2. Gün | |
| 2026-03-22 | Ramazan Bayramı 3. Gün | |
| 2026-04-23 | Ulusal Egemenlik ve Çocuk Bayramı | |
| 2026-05-01 | Emek ve Dayanışma Günü | |
| 2026-05-19 | Atatürk'ü Anma, Gençlik ve Spor Bayramı | |
| 2026-05-26 | Kurban Bayramı Arifesi | ✅ |
| 2026-05-27 | Kurban Bayramı 1. Gün | |
| 2026-05-28 | Kurban Bayramı 2. Gün | |
| 2026-05-29 | Kurban Bayramı 3. Gün | |
| 2026-05-30 | Kurban Bayramı 4. Gün | |
| 2026-07-15 | Demokrasi ve Milli Birlik Günü | |
| 2026-08-30 | Zafer Bayramı | |
| 2026-10-28 | Cumhuriyet Bayramı Arifesi | ✅ |
| 2026-10-29 | Cumhuriyet Bayramı | |

</details>

### IsGunu — İş Günü Hesaplama

Varsayılan olarak Cumartesi ve Pazar tatildir. `includeSaturday` / `includeSunday` parametreleriyle 6 veya 7 gün çalışan işyerlerine göre hesap yapabilirsiniz.

```csharp
var friday   = new DateTime(2026, 11, 13);
var saturday = new DateTime(2026, 11, 14);
var sunday   = new DateTime(2026, 11, 15);

// Hafta sonu kontrolü
IsGunu.IsWeekend(saturday);                                   // true
IsGunu.IsWeekend(saturday, includeSaturday: true);            // false (Cumartesi çalışılıyor)
IsGunu.IsWeekend(sunday, includeSunday: true);                // false

// İş günü kontrolü (hafta sonu + tam gün resmi tatil hariç)
IsGunu.IsWorkingDay(new DateTime(2026, 11, 10));              // true
IsGunu.IsWorkingDay(saturday);                                // false
IsGunu.IsWorkingDay(saturday, includeSaturday: true);         // true
IsGunu.IsWorkingDay(new DateTime(2026, 10, 29));              // false (Cumhuriyet Bayramı)

// İş günü ekleme / çıkarma
IsGunu.AddWorkingDays(friday, 1);                             // 2026-11-16 Pazartesi
IsGunu.AddWorkingDays(friday, 1, includeSaturday: true);      // 2026-11-14 Cumartesi
IsGunu.AddWorkingDays(new DateTime(2026, 10, 27), 3);         // 2026-11-02 Pazartesi (29 Ekim atlanır)
IsGunu.AddWorkingDays(new DateTime(2026, 11, 10), -5);        // 2026-11-03 Salı

// İki tarih arasındaki iş günü sayısı
var start = new DateTime(2026, 11, 1);
var end   = new DateTime(2026, 11, 30);
IsGunu.GetWorkingDaysCount(start, end);                                         // 21
IsGunu.GetWorkingDaysCount(start, end, includeSaturday: true);                  // 25
IsGunu.GetWorkingDaysCount(start, end, includeSaturday: true, includeSunday: true); // 29
IsGunu.GetWorkingDaysCount(end, start);                                         // -21

// Sonraki ilk iş günü
IsGunu.GetNextWorkingDay(friday);                             // 2026-11-16 Pazartesi
IsGunu.GetNextWorkingDay(friday, includeToday: true);         // 2026-11-13 Cuma
IsGunu.GetNextWorkingDay(friday, includeSaturday: true);      // 2026-11-14 Cumartesi
IsGunu.GetNextWorkingDay(new DateTime(2026, 5, 26));          // 2026-06-01 Pazartesi (Kurban Bayramı atlanır)
```

### DateTime Extension Metotları

```csharp
var date = new DateTime(2026, 10, 29);

date.IsPublicHoliday();                             // true
date.IsPublicHoliday(includeHalfDays: true);        // true
date.TryGetPublicHoliday(out HolidayInfo holiday);  // true → holiday.Name = "Cumhuriyet Bayramı"
date.IsWeekend();                                   // false
date.IsWeekend(includeSaturday: true);              // false
date.IsWorkingDay();                                // false
date.AddWorkingDays(5);                             // 2026-11-05
date.AddWorkingDays(5, includeSaturday: true);      // 2026-11-04
date.GetWorkingDaysCount(new DateTime(2026, 11, 30));   // iki tarih arası iş günü
date.GetNextWorkingDay();                           // 2026-10-30 Cuma
date.GetNextWorkingDay(includeToday: true);         // 2026-10-30 Cuma
```

---

## 🗺️ Coğrafya — `Mihenk.Geography`

```bash
dotnet add package Mihenk.Geography
```

```csharp
using Mihenk.Geography;

// Tüm iller
IReadOnlyList<ProvinceInfo> provinces = Turkiye.GetProvinces();      // 81 il (toplam 973 ilçe)

// Plaka koduna göre
ProvinceInfo? ist = Turkiye.GetProvince(34);
// ist.PlateCode      → 34
// ist.Name           → "İstanbul"
// ist.AreaCode       → "212/216"
// ist.Region         → GeographicalRegion.Marmara
// ist.IsMetropolitan → true
// ist.Latitude       → 41.0082
// ist.Longitude      → 28.9784
// ist.Districts      → ["Adalar", "Arnavutköy", ... ] (39 ilçe)

Turkiye.GetProvince(99);                            // null

// İl adına göre (Türkçe büyük/küçük harf duyarsız; plaka kodu metni de kabul edilir)
Turkiye.GetProvince("izmir");                       // İzmir
Turkiye.GetProvince("06");                          // Ankara

Turkiye.TryGetProvince(6, out var ankara);          // true → ankara.Name = "Ankara"
Turkiye.TryGetProvince("ESKİŞEHİR", out var esk);   // true → esk.PlateCode = 26

// İlçeler
Turkiye.GetDistricts(77);                           // ["Merkez", "Altınova", "Armutlu", "Çınarcık", "Çiftlikköy", "Termal"]
Turkiye.GetDistricts("bayburt");                    // ["Merkez", "Aydıntepe", "Demirözü"]

Turkiye.IsValidDistrict(34, "kadıköy");             // true
Turkiye.IsValidDistrict("İzmir", "BORNOVA");        // true
Turkiye.IsValidDistrict("Ankara", "Kadıköy");       // false

// Kuş uçuşu mesafe (Haversine, km) — karayolu mesafesi DEĞİLDİR
Turkiye.GetAirDistanceKm("İstanbul", "Ankara");     // 349.36
Turkiye.GetAirDistanceKm(34, 35);                   // 327.45
Turkiye.CalculateHaversineKm(41.0082, 28.9784, 39.9334, 32.8597);   // 349.36
```

`GeographicalRegion` değerleri: `Marmara`, `Ege`, `Akdeniz`, `IcAnadolu`, `Karadeniz`, `DoguAnadolu`, `GuneydoguAnadolu`.

---

## 🧷 DataAnnotations Öznitelikleri

Çekirdek `Mihenk` paketine dahildir — ek paket gerekmez. `null` ve boş değerler geçerli sayılır (zorunluluk için `[Required]` kullanın). Hata mesajı olarak `Validate()`'in döndüğü **spesifik neden** kullanılır; `ErrorMessage` verirseniz sizinki önceliklidir.

```csharp
using System.ComponentModel.DataAnnotations;
using Mihenk.Validation.DataAnnotations;

public class CustomerRequest
{
    [Required, TcKimlik]                  public string? TcNo { get; set; }
    [VergiNo]                             public string? TaxNo { get; set; }
    [TcOrVkn]                             public string? TcOrTaxNo { get; set; }
    [YabanciKimlik]                       public string? ForeignId { get; set; }
    [MersisNo]                            public string? MersisNo { get; set; }

    [Plaka]                               public string? Plate { get; set; }
    [Plaka(OfficialOnly = true)]          public string? OfficialPlate { get; set; }
    [SasiNo]                              public string? Vin { get; set; }

    [Iban]                                public string? Iban { get; set; }
    [KrediKarti]                          public string? CardNumber { get; set; }

    [Telefon]                             public string? Phone { get; set; }
    [Telefon(MobileOnly = true)]          public string? MobilePhone { get; set; }
    [Telefon(LandlineOnly = true)]        public string? LandlinePhone { get; set; }
    [MihenkEmail]                         public string? Email { get; set; }
    [PostaKodu]                           public string? PostalCode { get; set; }

    [FaturaNo]                            public string? InvoiceNo { get; set; }
    [Barkod]                              public string? Barcode { get; set; }
    [Barkod(TurkeyGs1Only = true)]        public string? TurkishBarcode { get; set; }
    [Imei]                                public string? Imei { get; set; }

    [TcKimlik(ErrorMessage = "Kimlik numaranızı kontrol edin.")]
    public string? CustomMessageTcNo { get; set; }
}
```

Örnek hata çıktıları:

```text
TcNo           → T.C. Kimlik Numarası 10. hane kontrol algoritmasına uymuyor.
MobilePhone    → Telefon numarası geçerli bir cep telefonu (5XX) olmalıdır.
OfficialPlate  → Plaka resmi bir araç plakası olmalıdır.
TurkishBarcode → Barkod GS1 Türkiye (868 veya 869) ön ekiyle başlayan 13 haneli bir barkod olmalıdır.
```

---

## 🧱 FluentValidation Kuralları

```bash
dotnet add package Mihenk.Validation.FluentValidation
```

```csharp
using FluentValidation;
using Mihenk.Validation.FluentValidation;

public class CustomerValidator : AbstractValidator<CustomerRequest>
{
    public CustomerValidator()
    {
        // Kimlik ve Vergi
        RuleFor(x => x.TcNo).NotEmpty().TcKimlik();
        RuleFor(x => x.TaxNo).VergiNo();
        RuleFor(x => x.TcOrTaxNo).TcOrVkn();
        RuleFor(x => x.ForeignId).YabanciKimlik();
        RuleFor(x => x.MersisNo).MersisNo();

        // Araç
        RuleFor(x => x.Plate).Plaka();
        RuleFor(x => x.OfficialPlate).OfficialPlate();
        RuleFor(x => x.Vin).SasiNo();

        // Finans
        RuleFor(x => x.Iban).Iban();
        RuleFor(x => x.CardNumber).KrediKarti();

        // İletişim
        RuleFor(x => x.Phone).Telefon();
        RuleFor(x => x.MobilePhone).MobilePhone();
        RuleFor(x => x.LandlinePhone).LandlinePhone();
        RuleFor(x => x.Email).MihenkEmail();
        RuleFor(x => x.PostalCode).PostaKodu();

        // Fatura, Barkod, IMEI
        RuleFor(x => x.InvoiceNo).FaturaNo();
        RuleFor(x => x.Barcode).Barkod();
        RuleFor(x => x.TurkishBarcode).TurkeyGs1Barcode();
        RuleFor(x => x.Imei).Imei();
    }
}
```

> [!TIP]
> Kurallar boş değerleri atlar; zorunluluk için `NotEmpty()` ile zincirleyin. Hata mesajı otomatik olarak `ValidationResult.ErrorMessage`'dan gelir ve `.WithMessage("...")` ile ezilebilir.

---

## 🔗 Extension Metotları — `Mihenk.Extensions`

Tüm statik metotların akıcı (fluent) karşılıkları. Tek bir `using` yeterlidir:

```csharp
using Mihenk.Extensions;
```

### Kimlik ve Vergi

```csharp
"10000000146".IsValidTcKimlik();                    // true
"1234".ValidateTcKimlik();                          // "T.C. Kimlik Numarası 11 haneli olmalıdır."
"10000000146".MaskTcKimlik();                       // "100******46"
"10000000146".TryValidateTcKimlik(out var tc);      // true

"4840847211".IsValidVergiNo();                      // true
"4840847211".ValidateVergiNo();                     // Valid
"4840847211".TryValidateVergiNo(out var vkn);       // true

"10000000146".IsValidTcOrVkn();                     // true
"123".ValidateTcOrVkn();                            // "Değer 10 haneli Vergi Kimlik Numarası veya 11 haneli ..."
"4840847211".TryValidateTcOrVkn(out var tcOrVkn);   // true

"99000000042".IsValidYabanciKimlik();               // true
"99000000042".ValidateYabanciKimlik();              // Valid
"99000000042".TryValidateYabanciKimlik(out var ykn);// true

"0484084721100001".IsValidMersisNo();               // true
"0484084721100001".ValidateMersisNo();              // Valid
"0484084721100001".TryValidateMersisNo(out var m);  // true
```

### Araç

```csharp
"34 ABC 123".IsValidPlate();                        // true
"99 A 123".ValidatePlate();                         // "Plaka il kodu 01 ile 81 arasında olmalıdır."
"34abc123".NormalizePlate();                        // "34 ABC 123"
"34ABC123".GetPlateProvinceCode();                  // 34
"06 A 1234".GetPlateProvinceName();                 // "Ankara"
"35 KS 4567".ParsePlate();                          // "35 KS 4567" → ProvinceName = "İzmir", Letters = "KS", Digits = "4567"
"34ABC123".TryParsePlate(out PlateInfo plate);      // true
"34abc123".TryNormalizePlate(out var normalized);   // true → "34 ABC 123"
"06 A 12345".IsOfficialPlate();                     // true

"1HGCM82633A004352".IsValidSasiNo();                // true
"1HGCM82633A004352".ValidateSasiNo();               // Valid
"1hgcm82633a004352".TryValidateSasiNo(out var vin); // true → "1HGCM82633A004352"
```

### Finans

```csharp
const string iban = "TR170001007301665990304734";
iban.IsValidIban();                                 // true
iban.ValidateIban();                                // Valid
iban.FormatIban();                                  // "TR17 0001 0073 0166 5990 3047 34"
iban.MaskIban();                                    // "TR17 **** **** **** **** **** 34"
iban.GetIbanBankCode();                             // "00010"
iban.GetIbanBankName();                             // "Ziraat Bankası"
iban.TryValidateIban(out var normalizedIban);       // true
iban.TryFormatIban(out var formattedIban);          // true

"4111111111111111".IsValidCreditCard();             // true
"4111111111111112".ValidateCreditCard();            // "Kredi kartı numarası Luhn kontrol algoritmasına uymuyor."
"9792030000000000".GetCreditCardType();             // CardType.Troy
"4111111111111111".MaskCreditCard();                // "4111 **** **** 1111"
"4111 1111 1111 1111".TryValidateCreditCard(out var card);   // true → "4111111111111111"

1250.75m.ToWords();                                 // "Bin iki yüz elli TL yetmiş beş kuruş"
100.50m.ToWords("Dolar", "sent");                   // "Yüz Dolar elli sent"
1234567.891m.FormatTl();                            // "1.234.567,89 ₺"

1000m.CalculateVat();                               // { NetAmount = 1000, VatAmount = 200, TotalAmount = 1200 }
1200m.CalculateVat(20, isVatIncluded: true);        // { NetAmount = 1000, VatAmount = 200, TotalAmount = 1200 }

"ABC2026000000001".IsValidInvoiceNo();              // true
"ABC2026".ValidateInvoiceNo();                      // "GİB fatura numarası 16 karakter olmalıdır."
"ABC2026000000001".TryValidateInvoiceNo(out var inv);   // true
```

### İletişim

```csharp
"0532 123 45 67".IsValidPhone();                    // true
"0532123".ValidatePhone();                          // "Telefon numarası (ülke kodu hariç) 10 haneli olmalıdır."
"0532 123 45 67".NormalizePhone();                  // "+905321234567"
"+90 532 123 45 67".ToShortPhone();                 // "5321234567"
"5321234567".FormatPhone();                         // "0 (532) 123 45 67"
"05321234567".MaskPhone();                          // "0 (532) *** ** 67"
"05321234567".IsMobilePhone();                      // true
"02123456789".IsLandlinePhone();                    // true
"0532 123 45 67".TryNormalizePhone(out var e164);   // true → "+905321234567"
"+905321234567".TryToShortPhone(out var shortNo);   // true → "5321234567"
"5321234567".TryFormatPhone(out var pretty);        // true → "0 (532) 123 45 67"
"Ara: 0532 123 45 67".ExtractPhones();              // ["0532 123 45 67"]

"ali.veli@example.com".IsValidEmail();              // true
"ali@".ValidateEmail();                             // "E-posta adresi çok kısa."
"ali.veli@example.com".MaskEmail();                 // "al******@ex*****.com"
" AHMET@Firma.COM.TR ".TryValidateEmail(out var email);   // true → "ahmet@firma.com.tr"
"Yaz: info@mihenk.dev".ExtractEmails();             // ["info@mihenk.dev"]

"06100".IsValidPostalCode();                        // true
"3400".ValidatePostalCode();                        // "Posta kodu 5 haneli olmalıdır."
"06100".TryValidatePostalCode(out var postalCode);  // true
```

### Metin

```csharp
"Bak: https://mihenk.dev".ExtractLinks();           // ["https://mihenk.dev"]
"#dotnet ve #csharp".ExtractHashtags();             // ["#dotnet", "#csharp"]
"@ahmet ve @ayşe_yılmaz".ExtractMentions();         // ["@ahmet", "@ayşe_yılmaz"]

"Çağrı Şüheda'nın İlk Ödevi".ToSlug();              // "cagri-suheda-nin-ilk-odevi"
"istanbul ılık".ToUpperTr();                        // "İSTANBUL ILIK"
"İSTANBUL IŞIK".ToLowerTr();                        // "istanbul ışık"
"iSTANBUL ve izmir".ToTitleCaseTr();                // "İstanbul Ve İzmir"
"Çağrı Şüheda".RemoveTurkishChars();                // "Cagri Suheda"
"Ä°stanbul'da gÃ¼zel".FixEncoding();                // "İstanbul'da güzel"
"İSTANBUL".EqualsIgnoreCaseTr("istanbul");          // true
"Merhaba İSTANBUL".ContainsTr("istanbul");          // true

1984.NumberToWords();                               // "bin dokuz yüz seksen dört"
1_250_000_000L.NumberToWords();                     // "bir milyar iki yüz elli milyon"
DateTime.Now.AddMinutes(-5).ToRelativeTimeTr();     // "5 dakika önce"
pastDate.ToRelativeTimeTr(referenceDate);           // referans tarihe göre
```

### Türkçe Ekler

```csharp
"Ankara".ToDativeTr();                              // "Ankara'ya"
"İstanbul".ToLocativeTr();                          // "İstanbul'da"
"Ahmet".ToAblativeTr();                             // "Ahmet'ten"
"Türkiye".ToGenitiveTr();                           // "Türkiye'nin"
"İstanbul".ToAccusativeTr();                        // "İstanbul'u"
"Ankara".ToInstrumentalTr();                        // "Ankara'yla"
"kitap".ToPluralTr();                               // "kitaplar"
"kitap".ToDativeTr(useApostrophe: false);           // "kitaba"
```

### KVKK, Barkod ve IMEI

```csharp
"Ahmet Yılmaz".MaskFullName();                      // "Ah*** Yı****"
"192.168.1.42".MaskIp();                            // "192.168.1.***"
"TC: 10000000146".MaskKvkkText();                   // "TC: 100******46"

"8690000000012".IsValidBarcode();                   // true
"12345".ValidateBarcode();                          // "Barkod numarası 8 (EAN-8), 12 (UPC-A), 13 (EAN-13) veya 14 (GTIN-14) haneli olmalıdır."
"8690000000012".IsTurkeyGs1Barcode();               // true
" 8690000000012 ".TryValidateBarcode(out var bc);   // true → "8690000000012"

"490154203237518".IsValidImei();                    // true
"490154203237519".ValidateImei();                   // "IMEI numarası Luhn kontrol algoritmasına uymuyor."
"49-015420-323751-8".TryValidateImei(out var imei); // true → "490154203237518"
```

---

## 🧭 Felsefe

- **Tahmin yürütme yok.** Veri yoksa (ör. 2036 resmi tatilleri) hata fırlatılır, yanlış sonuç üretilmez.
- **Ağ yok.** Hiçbir metot internete çıkmaz; kapalı ağlarda ve sunucusuz ortamlarda güvenle çalışır.
- **Açıklayıcı hatalar.** Kullanıcıya "geçersiz" yerine *neden* geçersiz olduğu söylenir.
- **Tutarlı API.** Her tipte `IsValid` → `Validate` → `TryValidate` → (varsa) `Format` / `Mask` / `Normalize` sırası korunur.

---

## 🗺️ Yol Haritası

- [x] Kimlik, vergi, araç, finans, iletişim doğrulamaları
- [x] DataAnnotations ve FluentValidation entegrasyonu
- [x] Türkçe metin ve hal ekleri
- [x] KVKK maskeleme
- [x] Diyanet tabanlı resmi tatil ve iş günü hesaplama
- [x] İl / ilçe / alan kodu veritabanı
- [ ] Kapsamlı birim test projesi
- [ ] Benchmark sonuçları (BenchmarkDotNet)
- [ ] 2036+ resmi tatil verisi (Diyanet açıkladıkça)

---

## 🤝 Katkı

1. Repoyu fork'layın
2. Yeni bir dal açın: `git checkout -b feature/yeni-ozellik`
3. Değişikliklerinizi commit'leyin: `git commit -m "feat: yeni özellik"`
4. Dalınızı push'layın: `git push origin feature/yeni-ozellik`
5. Pull Request açın

Hata bildirimi ve öneriler için [Issues](https://github.com/gokhanaravi/Mihenk/issues) sayfasını kullanabilirsiniz.

```bash
git clone https://github.com/gokhanaravi/Mihenk.git
cd Mihenk
dotnet build
```

---

## 📄 Lisans

Bu proje [MIT](LICENSE) lisansı ile lisanslanmıştır.

<div align="center">

**Mihenk** — *mihenk taşı: bir şeyin değerini ölçmeye yarayan ölçüt.*

⭐ Projeyi faydalı bulduysanız yıldız vermeyi unutmayın!

</div>
