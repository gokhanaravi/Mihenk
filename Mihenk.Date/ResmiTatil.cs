namespace Mihenk.Date;

/// <summary>
/// TR: Resmi tatil günü bilgisini temsil eder.
/// <para/>
/// EN: Represents information about an official public holiday.
/// </summary>
/// <param name="Date">TR: Tatilin tarihi. / EN: The date of the holiday.</param>
/// <param name="Name">TR: Tatilin resmi adı (Örn: "Cumhuriyet Bayramı", "Ramazan Bayramı 1. Gün"). / EN: Official name of the holiday.</param>
/// <param name="IsHalfDay">TR: Tatilin öğleden sonra başlayan yarım gün (Arife / 28 Ekim) olup olmadığı. / EN: Indicates whether the holiday is a half-day (Eve / October 28).</param>
public readonly record struct HolidayInfo(
    DateTime Date,
    string Name,
    bool IsHalfDay = false);

/// <summary>
/// TR: Türkiye ulusal ve dini resmi tatil günlerini Diyanet İşleri Başkanlığı resmi takvim tablosuna dayanarak
/// (tahmin yürütmeden, %100 offline/in-memory) hesaplayan araç.
/// Desteklenen yıl aralığı: <see cref="MinSupportedYear"/> (2020) – <see cref="MaxSupportedYear"/> (2035).
/// <para/>
/// EN: Utility that calculates Turkish national and religious public holidays based on the official calendar table
/// of the Presidency of Religious Affairs (Diyanet) without astronomical estimation (%100 offline/in-memory).
/// Supported year range: <see cref="MinSupportedYear"/> (2020) – <see cref="MaxSupportedYear"/> (2035).
/// </summary>
public static class ResmiTatil
{
    /// <summary>
    /// TR: Dini bayramlar tablosunda desteklenen en küçük yıl (2020).
    /// <para/>
    /// EN: Minimum supported year in the religious holidays table (2020).
    /// </summary>
    public const int MinSupportedYear = 2020;

    /// <summary>
    /// TR: Dini bayramlar tablosunda desteklenen en büyük yıl (2035).
    /// <para/>
    /// EN: Maximum supported year in the religious holidays table (2035).
    /// </summary>
    public const int MaxSupportedYear = 2035;

    // Diyanet İşleri Başkanlığı Vakit Hesaplama Dairesi resmi dini günler listesi (2020 - 2035).
    // (Miladi yılda Ramazan veya Kurban Bayramı 1. Gün başlangıç tarihleri; 2033 yılında iki Ramazan Bayramı vardır).
    private static readonly Dictionary<int, (DateTime[] RamadanFirstDays, DateTime[] SacrificeFirstDays)> DiyanetHolidayTable = new()
    {
        [2020] = ([new DateTime(2020, 5, 24)], [new DateTime(2020, 7, 31)]),
        [2021] = ([new DateTime(2021, 5, 13)], [new DateTime(2021, 7, 20)]),
        [2022] = ([new DateTime(2022, 5, 2)], [new DateTime(2022, 7, 9)]),
        [2023] = ([new DateTime(2023, 4, 21)], [new DateTime(2023, 6, 28)]),
        [2024] = ([new DateTime(2024, 4, 10)], [new DateTime(2024, 6, 16)]),
        [2025] = ([new DateTime(2025, 3, 30)], [new DateTime(2025, 6, 6)]),
        [2026] = ([new DateTime(2026, 3, 20)], [new DateTime(2026, 5, 27)]),
        [2027] = ([new DateTime(2027, 3, 9)], [new DateTime(2027, 5, 16)]),
        [2028] = ([new DateTime(2028, 2, 26)], [new DateTime(2028, 5, 5)]),
        [2029] = ([new DateTime(2029, 2, 14)], [new DateTime(2029, 4, 24)]),
        [2030] = ([new DateTime(2030, 2, 4)], [new DateTime(2030, 4, 13)]),
        [2031] = ([new DateTime(2031, 1, 24)], [new DateTime(2031, 4, 3)]),
        [2032] = ([new DateTime(2032, 1, 14)], [new DateTime(2032, 3, 22)]),
        [2033] = ([new DateTime(2033, 1, 2), new DateTime(2033, 12, 23)], [new DateTime(2033, 3, 11)]),
        [2034] = ([new DateTime(2034, 12, 12)], [new DateTime(2034, 3, 1)]),
        [2035] = ([new DateTime(2035, 12, 2)], [new DateTime(2035, 2, 18)])
    };

    /// <summary>
    /// TR: Verilen tarihin Türkiye'de tam gün resmi tatil olup olmadığını kontrol eder.
    /// Desteklenen yıl aralığı (<c>2020-2035</c>) dışındaki bir tarih verilirse <see cref="ArgumentOutOfRangeException"/> fırlatır.
    /// <para/>
    /// EN: Checks whether the given date is a full-day official public holiday in Turkey.
    /// Throws <see cref="ArgumentOutOfRangeException"/> if the year is outside the supported range (<c>2020-2035</c>).
    /// </summary>
    /// <param name="date">TR: Kontrol edilecek tarih. / EN: The date to check.</param>
    /// <param name="includeHalfDays">TR: Yarım gün tatillerin (Arife günleri) dahil edilip edilmeyeceği. / EN: Whether to include half-day holidays (Eve days).</param>
    public static bool IsPublicHoliday(DateTime date, bool includeHalfDays = false)
    {
        return TryGetHoliday(date, out HolidayInfo info) && (includeHalfDays || !info.IsHalfDay);
    }

    /// <summary>
    /// TR: Verilen tarihin resmi tatil bilgisini döner. Tatil değilse <see langword="null"/> döner.
    /// Desteklenen yıl aralığı (<c>2020-2035</c>) dışındaki bir tarih verilirse <see cref="ArgumentOutOfRangeException"/> fırlatır.
    /// <para/>
    /// EN: Returns the public holiday information for the given date, or <see langword="null"/> if it is not a holiday.
    /// Throws <see cref="ArgumentOutOfRangeException"/> if the year is outside the supported range (<c>2020-2035</c>).
    /// </summary>
    /// <param name="date">TR: Kontrol edilecek tarih. / EN: The date to check.</param>
    public static HolidayInfo? GetHolidayInfo(DateTime date)
    {
        return TryGetHoliday(date, out HolidayInfo info) ? info : null;
    }

    /// <summary>
    /// TR: Verilen tarihin resmi tatil olup olmadığını kontrol eder ve tatilse detayını <paramref name="holiday"/> parametresiyle döner.
    /// Desteklenen yıl aralığı (<c>2020-2035</c>) dışındaki bir tarih verilirse <see cref="ArgumentOutOfRangeException"/> fırlatır.
    /// <para/>
    /// EN: Checks whether the given date is a public holiday and outputs the details via <paramref name="holiday"/> if so.
    /// Throws <see cref="ArgumentOutOfRangeException"/> if the year is outside the supported range (<c>2020-2035</c>).
    /// </summary>
    /// <param name="date">TR: Kontrol edilecek tarih. / EN: The date to check.</param>
    /// <param name="holiday">TR: Resmi tatil bilgisi çıktısı. / EN: The output holiday information.</param>
    public static bool TryGetHoliday(DateTime date, out HolidayInfo holiday)
    {
        DateTime d = date.Date;
        foreach (HolidayInfo item in GetHolidays(d.Year))
        {
            if (item.Date == d)
            {
                holiday = item;
                return true;
            }
        }

        holiday = default;
        return false;
    }

    /// <summary>
    /// TR: Belirtilen yıl içindeki tüm ulusal ve dini resmi tatilleri tarihe göre sıralı olarak döner.
    /// Desteklenen yıl aralığı (<c>2020-2035</c>) dışındaki bir yıl verilirse <see cref="ArgumentOutOfRangeException"/> fırlatır.
    /// <para/>
    /// EN: Returns all national and religious public holidays in the specified year, ordered by date.
    /// Throws <see cref="ArgumentOutOfRangeException"/> if the year is outside the supported range (<c>2020-2035</c>).
    /// </summary>
    /// <param name="year">TR: Sorgulanacak yıl (2020-2035). / EN: The year to query (2020-2035).</param>
    public static IReadOnlyList<HolidayInfo> GetHolidays(int year)
    {
        if (year < MinSupportedYear || year > MaxSupportedYear)
        {
            throw new ArgumentOutOfRangeException(
                nameof(year),
                year,
                $"Diyanet resmi tatil tablosu yalnızca {MinSupportedYear} ile {MaxSupportedYear} yılları arasını desteklemektedir.");
        }

        var list = new List<HolidayInfo>(16)
        {
            new(new DateTime(year, 1, 1), "Yılbaşı"),
            new(new DateTime(year, 4, 23), "Ulusal Egemenlik ve Çocuk Bayramı"),
            new(new DateTime(year, 5, 1), "Emek ve Dayanışma Günü"),
            new(new DateTime(year, 5, 19), "Atatürk'ü Anma, Gençlik ve Spor Bayramı"),
            new(new DateTime(year, 7, 15), "Demokrasi ve Milli Birlik Günü"),
            new(new DateTime(year, 8, 30), "Zafer Bayramı"),
            new(new DateTime(year, 10, 28), "Cumhuriyet Bayramı Arifesi", IsHalfDay: true),
            new(new DateTime(year, 10, 29), "Cumhuriyet Bayramı")
        };

        var (ramadans, sacrifices) = DiyanetHolidayTable[year];

        foreach (DateTime r1 in ramadans)
        {
            TryAddIfSameYear(list, year, r1.AddDays(-1), "Ramazan Bayramı Arifesi", isHalfDay: true);
            TryAddIfSameYear(list, year, r1, "Ramazan Bayramı 1. Gün");
            TryAddIfSameYear(list, year, r1.AddDays(1), "Ramazan Bayramı 2. Gün");
            TryAddIfSameYear(list, year, r1.AddDays(2), "Ramazan Bayramı 3. Gün");
        }

        foreach (DateTime k1 in sacrifices)
        {
            TryAddIfSameYear(list, year, k1.AddDays(-1), "Kurban Bayramı Arifesi", isHalfDay: true);
            TryAddIfSameYear(list, year, k1, "Kurban Bayramı 1. Gün");
            TryAddIfSameYear(list, year, k1.AddDays(1), "Kurban Bayramı 2. Gün");
            TryAddIfSameYear(list, year, k1.AddDays(2), "Kurban Bayramı 3. Gün");
            TryAddIfSameYear(list, year, k1.AddDays(3), "Kurban Bayramı 4. Gün");
        }

        list.Sort(static (a, b) => a.Date.CompareTo(b.Date));
        return list;
    }

    private static void TryAddIfSameYear(List<HolidayInfo> list, int year, DateTime date, string name, bool isHalfDay = false)
    {
        if (date.Year == year)
        {
            list.Add(new HolidayInfo(date.Date, name, isHalfDay));
        }
    }
}
