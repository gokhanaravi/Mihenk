namespace Mihenk.Text;

/// <summary>
/// TR: Türkçe ünlü uyumları (büyük/küçük ünlü uyumu), yabancı kökenli ince okunan kelimeler (<c>saat'e, harf'e, kalp'i, gol'ü</c>),
/// sayılar (<c>5'e, 10'dan, 100'ü</c>), kısaltmalar (<c>TBMM'ye, TDK'ye, TÜBİTAK'tan</c>), ünsüz benzeşmesi (sertleşme),
/// kaynaştırma harfleri ve ünsüz yumuşaması kurallarına göre çekim eklerini otomatik hesaplayan araç.
/// <br/>
/// EN: Turkish morphological case suffix engine supporting vowel harmony, palatalized loanword exceptions (<c>saat'e, harf'e, kalp'i, gol'ü</c>),
/// numbers (<c>5'e, 10'dan, 100'ü</c>), acronyms (<c>TBMM'ye, TDK'ye, TÜBİTAK'tan</c>), consonant assimilation, buffer letters, and lenition.
/// </summary>
public static class TurkceEk
{
    private static readonly HashSet<string> PalatalizedLoanwordExceptions =
    [
        "saat", "harf", "kalp", "alkol", "gol", "rol", "kontrol", "petrol", "sembol", "protokol",
        "bandrol", "parol", "mentol", "kabul", "meşgul", "mamul", "makbul", "mahsul",
        "hal", "hâl", "misal", "emsal", "istiklal", "istikbal", "ihtimal", "ihlal", "işgal",
        "ithal", "ihmal", "icmal", "celal", "cemal", "kemal", "hilal", "bilal", "meral",
        "meal", "zeval", "hayal", "sual", "faal", "terminal", "festival", "kristal", "metal",
        "normal", "moral", "kral", "orijinal", "liberal", "general", "mareşal", "materyal",
        "lokal", "vokal", "radikal", "müzikal", "sosyal", "spesiyal", "idealist", "ideal",
        "sadakat", "hakikat", "dikkat", "şefkat", "liyakat", "refakat", "mülakat", "meşakkat",
        "sıhhat", "kanaat", "ziraat", "inşaat", "cemaat", "itaat", "kabahat", "seyahat",
        "istirahat", "feragat", "belagat", "nezaket", "adalet", "kefalet", "vekalet", "inhisar"
    ];

    /// <summary>
    /// TR: İsme yönelme hal eki (<c>-e, -a, -ye, -ya</c>) ekler
    /// (Örn: <c>"Gökhan"</c> → <c>"Gökhan'a"</c>, <c>"saat"</c> → <c>"saat'e"</c>, <c>"TBMM"</c> → <c>"TBMM'ye"</c>, <c>"5"</c> → <c>"5'e"</c>).
    /// <br/>
    /// EN: Appends the Turkish dative case suffix (<c>-e, -a, -ye, -ya</c>) to the word.
    /// </summary>
    /// <param name="word">TR: Ek getirilecek kelime/isim. / EN: The target word or proper noun.</param>
    /// <param name="useApostrophe">TR: Kesme işareti (<c>'</c>) kullanılıp kullanılmayacağı. / EN: Whether to separate the suffix with an apostrophe (<c>'</c>).</param>
    public static string ToDative(string? word, bool useApostrophe = true)
    {
        if (string.IsNullOrWhiteSpace(word))
            return string.Empty;

        string trimmed = word.Trim();
        if (!TryAnalyzeWord(trimmed, out char lastVowel, out bool endsWithVowel, out _))
            return trimmed;

        bool front = IsFrontVowel(lastVowel);
        string suffix = endsWithVowel
            ? (front ? "ye" : "ya")
            : (front ? "e" : "a");

        return FormatSuffix(trimmed, suffix, useApostrophe, applySoftening: !useApostrophe && !endsWithVowel);
    }

    /// <summary>
    /// TR: İsme bulunma hal eki (<c>-de, -da, -te, -ta</c>) ekler
    /// (Örn: <c>"İzmir"</c> → <c>"İzmir'de"</c>, <c>"Sinop"</c> → <c>"Sinop'ta"</c>, <c>"saat"</c> → <c>"saat'te"</c>, <c>"3"</c> → <c>"3'te"</c>).
    /// <br/>
    /// EN: Appends the Turkish locative case suffix (<c>-de, -da, -te, -ta</c>) to the word.
    /// </summary>
    /// <param name="word">TR: Ek getirilecek kelime/isim. / EN: The target word or proper noun.</param>
    /// <param name="useApostrophe">TR: Kesme işareti (<c>'</c>) kullanılıp kullanılmayacağı. / EN: Whether to separate the suffix with an apostrophe (<c>'</c>).</param>
    public static string ToLocative(string? word, bool useApostrophe = true)
    {
        if (string.IsNullOrWhiteSpace(word))
            return string.Empty;

        string trimmed = word.Trim();
        if (!TryAnalyzeWord(trimmed, out char lastVowel, out _, out bool endsWithHardConsonant))
            return trimmed;

        bool front = IsFrontVowel(lastVowel);
        string suffix = endsWithHardConsonant
            ? (front ? "te" : "ta")
            : (front ? "de" : "da");

        return FormatSuffix(trimmed, suffix, useApostrophe, applySoftening: false);
    }

    /// <summary>
    /// TR: İsme ayrılma/çıkma hal eki (<c>-den, -dan, -ten, -tan</c>) ekler
    /// (Örn: <c>"Ankara"</c> → <c>"Ankara'dan"</c>, <c>"Uşak"</c> → <c>"Uşak'tan"</c>, <c>"harf"</c> → <c>"harf'ten"</c>, <c>"5"</c> → <c>"5'ten"</c>).
    /// <br/>
    /// EN: Appends the Turkish ablative case suffix (<c>-den, -dan, -ten, -tan</c>) to the word.
    /// </summary>
    /// <param name="word">TR: Ek getirilecek kelime/isim. / EN: The target word or proper noun.</param>
    /// <param name="useApostrophe">TR: Kesme işareti (<c>'</c>) kullanılıp kullanılmayacağı. / EN: Whether to separate the suffix with an apostrophe (<c>'</c>).</param>
    public static string ToAblative(string? word, bool useApostrophe = true)
    {
        if (string.IsNullOrWhiteSpace(word))
            return string.Empty;

        string trimmed = word.Trim();
        if (!TryAnalyzeWord(trimmed, out char lastVowel, out _, out bool endsWithHardConsonant))
            return trimmed;

        bool front = IsFrontVowel(lastVowel);
        string suffix = endsWithHardConsonant
            ? (front ? "ten" : "tan")
            : (front ? "den" : "dan");

        return FormatSuffix(trimmed, suffix, useApostrophe, applySoftening: false);
    }

    /// <summary>
    /// TR: İsme ilgi / iyelik (tamlayan) eki (<c>-in, -ın, -ün, -un, -nin, -nın, -nün, -nun</c>) ekler
    /// (Örn: <c>"Mehmet"</c> → <c>"Mehmet'in"</c>, <c>"Ayşe"</c> → <c>"Ayşe'nin"</c>, <c>"TBMM"</c> → <c>"TBMM'nin"</c>).
    /// <br/>
    /// EN: Appends the Turkish genitive/possessive suffix (<c>-in, -ın, -ün, -un, -nin, -nın, -nün, -nun</c>) to the word.
    /// </summary>
    /// <param name="word">TR: Ek getirilecek kelime/isim. / EN: The target word or proper noun.</param>
    /// <param name="useApostrophe">TR: Kesme işareti (<c>'</c>) kullanılıp kullanılmayacağı. / EN: Whether to separate the suffix with an apostrophe (<c>'</c>).</param>
    public static string ToGenitive(string? word, bool useApostrophe = true)
    {
        if (string.IsNullOrWhiteSpace(word))
            return string.Empty;

        string trimmed = word.Trim();
        if (!TryAnalyzeWord(trimmed, out char lastVowel, out bool endsWithVowel, out _))
            return trimmed;

        char narrowVowel = GetNarrowVowel(lastVowel);
        string suffix = endsWithVowel ? $"n{narrowVowel}n" : $"{narrowVowel}n";

        return FormatSuffix(trimmed, suffix, useApostrophe, applySoftening: !useApostrophe && !endsWithVowel);
    }

    /// <summary>
    /// TR: İsme belirtme hal eki (<c>-i, -ı, -ü, -u, -yi, -yı, -yü, -yu</c>) ekler
    /// (Örn: <c>"İstanbul"</c> → <c>"İstanbul'u"</c>, <c>"Kitap"</c> (<c>useApostrophe: false</c>) → <c>"Kitabı"</c>, <c>"kalp"</c> (<c>false</c>) → <c>"kalbi"</c>).
    /// <br/>
    /// EN: Appends the Turkish accusative case suffix (<c>-i, -ı, -ü, -u, -yi, -yı, -yü, -yu</c>) to the word.
    /// </summary>
    /// <param name="word">TR: Ek getirilecek kelime/isim. / EN: The target word or proper noun.</param>
    /// <param name="useApostrophe">TR: Kesme işareti (<c>'</c>) kullanılıp kullanılmayacağı. / EN: Whether to separate the suffix with an apostrophe (<c>'</c>).</param>
    public static string ToAccusative(string? word, bool useApostrophe = true)
    {
        if (string.IsNullOrWhiteSpace(word))
            return string.Empty;

        string trimmed = word.Trim();
        if (!TryAnalyzeWord(trimmed, out char lastVowel, out bool endsWithVowel, out _))
            return trimmed;

        char narrowVowel = GetNarrowVowel(lastVowel);
        string suffix = endsWithVowel ? $"y{narrowVowel}" : $"{narrowVowel}";

        return FormatSuffix(trimmed, suffix, useApostrophe, applySoftening: !useApostrophe && !endsWithVowel);
    }

    /// <summary>
    /// TR: İsme vasıta / birliktelik eki (<c>-le, -la, -yle, -yla</c>) ekler
    /// (Örn: <c>"Ahmet"</c> → <c>"Ahmet'le"</c>, <c>"Araba"</c> (<c>useApostrophe: false</c>) → <c>"Arabayla"</c>).
    /// <br/>
    /// EN: Appends the Turkish instrumental/comitative suffix (<c>-le, -la, -yle, -yla</c>) to the word.
    /// </summary>
    /// <param name="word">TR: Ek getirilecek kelime/isim. / EN: The target word or proper noun.</param>
    /// <param name="useApostrophe">TR: Kesme işareti (<c>'</c>) kullanılıp kullanılmayacağı. / EN: Whether to separate the suffix with an apostrophe (<c>'</c>).</param>
    public static string ToInstrumental(string? word, bool useApostrophe = true)
    {
        if (string.IsNullOrWhiteSpace(word))
            return string.Empty;

        string trimmed = word.Trim();
        if (!TryAnalyzeWord(trimmed, out char lastVowel, out bool endsWithVowel, out _))
            return trimmed;

        bool front = IsFrontVowel(lastVowel);
        string suffix = endsWithVowel
            ? (front ? "yle" : "yla")
            : (front ? "le" : "la");

        return FormatSuffix(trimmed, suffix, useApostrophe, applySoftening: false);
    }

    /// <summary>
    /// TR: Kelimeye çoğul eki (<c>-ler, -lar</c>) ekler (Örn: <c>"Kitap"</c> → <c>"Kitaplar"</c>, <c>"saat"</c> → <c>"saatler"</c>, <c>"harf"</c> → <c>"harfler"</c>).
    /// <br/>
    /// EN: Appends the Turkish plural suffix (<c>-ler, -lar</c>) to the word.
    /// </summary>
    /// <param name="word">TR: Çoğul yapılacak kelime. / EN: The word to pluralize.</param>
    public static string ToPlural(string? word)
    {
        if (string.IsNullOrWhiteSpace(word))
            return string.Empty;

        string trimmed = word.Trim();
        if (!TryAnalyzeWord(trimmed, out char lastVowel, out _, out _))
            return trimmed;

        string suffix = IsFrontVowel(lastVowel) ? "ler" : "lar";
        return FormatSuffix(trimmed, suffix, useApostrophe: false, applySoftening: false);
    }

    private static string FormatSuffix(string word, string suffix, bool useApostrophe, bool applySoftening)
    {
        bool isAllUpperWord = !useApostrophe && IsAllUpperCaseWord(word.AsSpan());
        string stem = word;

        if (applySoftening && stem.Length > 1)
        {
            char last = stem[^1];
            char? softened = last switch
            {
                'p' => 'b',
                'P' => 'B',
                'ç' => 'c',
                'Ç' => 'C',
                't' => 'd',
                'T' => 'D',
                'k' => (stem.Length >= 2 && ToLowerTrChar(stem[^2]) == 'n') ? 'g' : 'ğ',
                'K' => (stem.Length >= 2 && ToLowerTrChar(stem[^2]) == 'n') ? 'G' : 'Ğ',
                _ => null
            };

            if (softened.HasValue)
            {
                char softChar = softened.Value;
                stem = $"{stem[..^1]}{softChar}";
            }
        }

        string formattedSuffix = isAllUpperWord ? TurkceMetin.ToUpperTr(suffix) : suffix;
        return useApostrophe ? $"{stem}'{formattedSuffix}" : $"{stem}{formattedSuffix}";
    }

    private static bool TryAnalyzeWord(
        string rawInput,
        out char lastVowel,
        out bool endsWithVowel,
        out bool endsWithHardConsonant)
    {
        lastVowel = 'a';
        endsWithVowel = false;
        endsWithHardConsonant = false;

        ReadOnlySpan<char> span = rawInput.AsSpan();

        int lastIdx = -1;
        for (int i = span.Length - 1; i >= 0; i--)
        {
            if (char.IsLetterOrDigit(span[i]))
            {
                lastIdx = i;
                break;
            }
        }

        if (lastIdx < 0)
            return false;

        if (char.IsDigit(span[lastIdx]))
        {
            return TryAnalyzeTrailingNumber(span[..(lastIdx + 1)], out lastVowel, out endsWithVowel, out endsWithHardConsonant);
        }

        int wordStart = lastIdx;
        while (wordStart > 0 && char.IsLetter(span[wordStart - 1]))
            wordStart--;

        ReadOnlySpan<char> lastWord = span.Slice(wordStart, lastIdx - wordStart + 1);

        if (IsAbbreviationReadByLetter(lastWord))
        {
            char lastLetter = ToUpperTrChar(lastWord[^1]);
            GetTurkishLetterPronunciation(lastLetter, out lastVowel, out endsWithVowel, out endsWithHardConsonant);
            return true;
        }

        char lastCharLower = ToLowerTrChar(lastWord[^1]);
        endsWithVowel = IsVowel(lastCharLower);
        endsWithHardConsonant = IsHardConsonant(lastCharLower);

        for (int i = lastWord.Length - 1; i >= 0; i--)
        {
            char c = ToLowerTrChar(lastWord[i]);
            if (IsVowel(c))
            {
                lastVowel = c;
                break;
            }
        }

        string lowerLastWord = TurkceMetin.ToLowerTr(lastWord.ToString());
        if (PalatalizedLoanwordExceptions.Contains(lowerLastWord))
        {
            lastVowel = lastVowel switch
            {
                'a' or 'â' => 'e',
                'o' => 'ö',
                'u' or 'û' => 'ü',
                'ı' => 'i',
                _ => lastVowel
            };
        }

        return true;
    }

    private static bool TryAnalyzeTrailingNumber(
        ReadOnlySpan<char> textEndingWithDigit,
        out char lastVowel,
        out bool endsWithVowel,
        out bool endsWithHardConsonant)
    {
        int numStart = textEndingWithDigit.Length - 1;
        while (numStart > 0 && char.IsDigit(textEndingWithDigit[numStart - 1]))
            numStart--;

        ReadOnlySpan<char> digits = textEndingWithDigit[numStart..];

        if (long.TryParse(digits, out long numberValue))
        {
            string spoken = TurkceMetin.NumberToWords(numberValue);
            return TryAnalyzeSpokenWord(spoken.AsSpan(), out lastVowel, out endsWithVowel, out endsWithHardConsonant);
        }

        int lastDigit = digits[^1] - '0';
        string lastDigitSpoken = TurkceMetin.NumberToWords(lastDigit);
        return TryAnalyzeSpokenWord(lastDigitSpoken.AsSpan(), out lastVowel, out endsWithVowel, out endsWithHardConsonant);
    }

    private static bool TryAnalyzeSpokenWord(
        ReadOnlySpan<char> spoken,
        out char lastVowel,
        out bool endsWithVowel,
        out bool endsWithHardConsonant)
    {
        lastVowel = 'e';
        char lastChar = ToLowerTrChar(spoken[^1]);
        endsWithVowel = IsVowel(lastChar);
        endsWithHardConsonant = IsHardConsonant(lastChar);

        for (int i = spoken.Length - 1; i >= 0; i--)
        {
            char c = ToLowerTrChar(spoken[i]);
            if (IsVowel(c))
            {
                lastVowel = c;
                break;
            }
        }

        return true;
    }

    private static bool IsAbbreviationReadByLetter(ReadOnlySpan<char> word)
    {
        if (word.Length < 2)
            return false;

        int upperCount = 0;
        int vowelCount = 0;

        for (int i = 0; i < word.Length; i++)
        {
            if (char.IsUpper(word[i]))
                upperCount++;
            if (IsVowel(ToLowerTrChar(word[i])))
                vowelCount++;
        }

        if (vowelCount == 0)
            return true;

        if (upperCount == word.Length && word.Length <= 5)
        {
            bool lastIsVowel = IsVowel(ToLowerTrChar(word[^1]));
            bool prevIsVowel = IsVowel(ToLowerTrChar(word[^2]));
            if (!lastIsVowel && !prevIsVowel)
                return true;
        }

        return false;
    }

    private static void GetTurkishLetterPronunciation(
        char upperLetter,
        out char lastVowel,
        out bool endsWithVowel,
        out bool endsWithHardConsonant)
    {
        endsWithVowel = true;
        endsWithHardConsonant = false;

        lastVowel = upperLetter switch
        {
            'A' => 'a',
            'I' => 'ı',
            'O' => 'o',
            'U' => 'u',
            'İ' => 'i',
            'Ö' => 'ö',
            'Ü' => 'ü',
            _ => 'e'
        };
    }

    private static bool IsAllUpperCaseWord(ReadOnlySpan<char> word)
    {
        int letterCount = 0;
        for (int i = 0; i < word.Length; i++)
        {
            if (char.IsLetter(word[i]))
            {
                letterCount++;
                if (char.IsLower(word[i]))
                    return false;
            }
        }

        return letterCount > 1;
    }

    private static bool IsVowel(char c) => c is 'a' or 'e' or 'ı' or 'i' or 'o' or 'ö' or 'u' or 'ü' or 'â' or 'î' or 'û';

    private static bool IsFrontVowel(char c) => c is 'e' or 'i' or 'ö' or 'ü' or 'î';

    private static bool IsHardConsonant(char c) => c is 'f' or 's' or 't' or 'k' or 'ç' or 'ş' or 'h' or 'p';

    private static char GetNarrowVowel(char vowel) => vowel switch
    {
        'a' or 'ı' or 'â' => 'ı',
        'e' or 'i' or 'î' => 'i',
        'o' or 'u' or 'û' => 'u',
        'ö' or 'ü' => 'ü',
        _ => 'ı'
    };

    private static char ToLowerTrChar(char c) => c switch
    {
        'İ' => 'i',
        'I' => 'ı',
        'Ç' => 'ç',
        'Ğ' => 'ğ',
        'Ö' => 'ö',
        'Ş' => 'ş',
        'Ü' => 'ü',
        'Â' => 'â',
        'Î' => 'î',
        'Û' => 'û',
        >= 'A' and <= 'Z' => (char)(c + 32),
        _ => c
    };

    private static char ToUpperTrChar(char c) => c switch
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
