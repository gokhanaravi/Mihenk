namespace Mihenk;

/// <summary>
/// TR: Bir doğrulama işleminin sonucunu ve geçersizse hata mesajını temsil eder.
/// <br/>
/// EN: Represents the result of a validation operation and its error message if invalid.
/// </summary>
/// <param name="IsValid">TR: Değerin geçerli olup olmadığı. / EN: Whether the value is valid.</param>
/// <param name="ErrorMessage">TR: Değer geçersizse hata mesajı; geçerliyse <see langword="null"/>. / EN: Error message if invalid; otherwise <see langword="null"/>.</param>
public readonly record struct ValidationResult(bool IsValid, string? ErrorMessage = null)
{
    /// <summary>
    /// TR: Başarılı doğrulama sonucu (<c>IsValid = true, ErrorMessage = null</c>).
    /// <br/>
    /// EN: Successful validation result (<c>IsValid = true, ErrorMessage = null</c>).
    /// </summary>
    public static ValidationResult Success => new(true, null);

    /// <summary>
    /// TR: Belirtilen hata mesajıyla başarısız doğrulama sonucu oluşturur.
    /// <br/>
    /// EN: Creates a failed validation result with the specified error message.
    /// </summary>
    public static ValidationResult Failure(string errorMessage) => new(false, errorMessage);

    /// <summary>
    /// TR: Doğrudan <c>if (TcKimlik.Validate(tc))</c> şeklinde boolean olarak kullanılabilmesini sağlar.
    /// <br/>
    /// EN: Allows implicit conversion to boolean for direct usage like <c>if (TcKimlik.Validate(tc))</c>.
    /// </summary>
    public static implicit operator bool(ValidationResult result) => result.IsValid;

    /// <inheritdoc/>
    public override string ToString() => IsValid ? "Valid" : (ErrorMessage ?? "Invalid");
}
