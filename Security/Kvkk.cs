using System.Text;
using System.Text.RegularExpressions;
using Mihenk.Communication;
using Mihenk.Finance;
using Mihenk.Identity;

namespace Mihenk.Security;

/// <summary>
/// TR: KVKK (Kişisel Verilerin Korunması Kanunu) kapsamında tüm kişisel verileri (T.C. Kimlik, Kredi Kartı, IBAN, Telefon, E-posta, Ad Soyad, IP ve serbest metin)
/// tek bir giriş noktasından maskeleyen ön yüz (facade) sınıfı.
/// <br/>
/// EN: Unified KVKK/GDPR privacy masking facade for Turkish National ID, Credit Card, IBAN, Phone, Email, Full Name, IP Address, and free-form text.
/// </summary>
public static partial class Kvkk
{
    /// <inheritdoc cref="TcKimlik.Mask(string?, char)"/>
    public static string MaskTcKimlik(string? value, char maskChar = '*') => TcKimlik.Mask(value, maskChar);

    /// <inheritdoc cref="KrediKarti.Mask(string?, char)"/>
    public static string MaskCreditCard(string? cardNumber, char maskChar = '*') => KrediKarti.Mask(cardNumber, maskChar);

    /// <inheritdoc cref="Iban.Mask(string?, char)"/>
    public static string MaskIban(string? iban, char maskChar = '*') => Iban.Mask(iban, maskChar);

    /// <inheritdoc cref="Telefon.Mask(string?, char)"/>
    public static string MaskPhone(string? phone, char maskChar = '*') => Telefon.Mask(phone, maskChar);

    /// <inheritdoc cref="Email.Mask(string?, char)"/>
    public static string MaskEmail(string? email, char maskChar = '*') => Email.Mask(email, maskChar);

    /// <summary>
    /// TR: Ad ve soyadın her kelimesinin ilk 2 harfini (kelime 2-3 harfliyse ilk harfini) bırakıp kalanını maskeler
    /// (Örn: <c>"Gökhan Aravi"</c> → <c>"Gö**** Ar***"</c>).
    /// <br/>
    /// EN: Masks each word of a full name leaving the first 2 characters (or 1st character for 2-3 letter words) visible
    /// (e.g., <c>"Gökhan Aravi"</c> → <c>"Gö**** Ar***"</c>).
    /// </summary>
    /// <param name="fullName">TR: Maskelenecek ad soyad. / EN: The full name string to mask.</param>
    /// <param name="maskChar">TR: Maskeleme karakteri. / EN: Masking character.</param>
    public static string MaskFullName(string? fullName, char maskChar = '*')
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return string.Empty;

        string[] parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return string.Empty;

        var sb = new StringBuilder(fullName.Length);
        for (int i = 0; i < parts.Length; i++)
        {
            if (i > 0)
                sb.Append(' ');

            string word = parts[i];
            if (word.Length <= 1)
            {
                sb.Append(maskChar);
            }
            else if (word.Length <= 3)
            {
                sb.Append(word[0]);
                sb.Append(maskChar, word.Length - 1);
            }
            else
            {
                sb.Append(word[0]).Append(word[1]);
                sb.Append(maskChar, word.Length - 2);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// TR: IPv4 veya IPv6 adresinin son oktetini/bloğunu KVKK kapsamında maskeler (Örn: <c>"192.168.1.45"</c> → <c>"192.168.1.***"</c>).
    /// <br/>
    /// EN: Masks the last octet/block of an IPv4 or IPv6 address (e.g., <c>"192.168.1.45"</c> → <c>"192.168.1.***"</c>).
    /// </summary>
    /// <param name="ipAddress">TR: Maskelenecek IP adresi. / EN: The IP address string to mask.</param>
    /// <param name="maskChar">TR: Maskeleme karakteri. / EN: Masking character.</param>
    public static string MaskIp(string? ipAddress, char maskChar = '*')
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            return string.Empty;

        string trimmed = ipAddress.Trim();
        int lastDot = trimmed.LastIndexOf('.');
        if (lastDot > 0)
            return $"{trimmed[..(lastDot + 1)]}{new string(maskChar, 3)}";

        int lastColon = trimmed.LastIndexOf(':');
        if (lastColon > 0)
            return $"{trimmed[..(lastColon + 1)]}{new string(maskChar, 4)}";

        return trimmed;
    }

    /// <summary>
    /// TR: Serbest bir metin (log, destek mesajı, açıklama vb.) içindeki tüm T.C. Kimlik, TR IBAN, Kredi Kartı, Telefon ve E-posta
    /// verilerini otomatik tespit edip ilgili sınıfın kendi maskeleme metoduyla KVKK uyumlu olarak sansürler.
    /// <br/>
    /// EN: Automatically detects and masks all valid TCKN, Turkish IBAN, Credit Card, Phone Number, and Email instances within a free-form text.
    /// </summary>
    /// <param name="text">TR: Maskelenecek serbest metin. / EN: The free-form text to sanitize.</param>
    /// <param name="maskChar">TR: Maskeleme karakteri. / EN: Masking character.</param>
    public static string MaskText(string? text, char maskChar = '*')
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        string result = text;

        result = IbanInTextRegex().Replace(result, m =>
            Iban.IsValid(m.Value) ? Iban.Mask(m.Value, maskChar) : m.Value);

        result = EmailInTextRegex().Replace(result, m =>
            Email.IsValid(m.Value) ? Email.Mask(m.Value, maskChar) : m.Value);

        result = CardInTextRegex().Replace(result, m =>
            KrediKarti.IsValid(m.Value) ? KrediKarti.Mask(m.Value, maskChar) : m.Value);

        result = TcInTextRegex().Replace(result, m =>
            TcKimlik.IsValid(m.Value) ? TcKimlik.Mask(m.Value, maskChar) : m.Value);

        result = PhoneInTextRegex().Replace(result, m =>
            Telefon.IsValid(m.Value) ? Telefon.Mask(m.Value, maskChar) : m.Value);

        return result;
    }

    [GeneratedRegex(@"\bTR\d{2}(?:\s?\d{4}){5}\s?\d{2}\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex IbanInTextRegex();

    [GeneratedRegex(@"[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}", RegexOptions.CultureInvariant)]
    private static partial Regex EmailInTextRegex();

    [GeneratedRegex(@"(?<!\d)(?:\d{4}[\s\-]?){3}\d{3,4}(?!\d)", RegexOptions.CultureInvariant)]
    private static partial Regex CardInTextRegex();

    [GeneratedRegex(@"(?<!\d)[1-9]\d{10}(?!\d)", RegexOptions.CultureInvariant)]
    private static partial Regex TcInTextRegex();

    [GeneratedRegex(@"(?<!\d)(?:\+90|0090|090|90|0)?[\s\.\-]*\(?(?:5\d{2}|[234]\d{2}|850)\)?[\s\.\-]*\d{3}[\s\.\-]*\d{2}[\s\.\-]*\d{2}(?!\d)", RegexOptions.CultureInvariant)]
    private static partial Regex PhoneInTextRegex();
}
