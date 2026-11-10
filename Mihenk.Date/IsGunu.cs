namespace Mihenk.Date;

/// <summary>
/// TR: Türkiye resmi tatilleri ve hafta sonlarını dikkate alarak iş günü (çalışma günü) hesaplayan araç.
/// Cumartesi ve Pazar günlerinin iş gününe dahil edilip edilmeyeceği opsiyonel parametrelerle yönetilebilir.
/// <para/>
/// EN: Utility that calculates working days (business days) taking into account Turkish public holidays and weekends.
/// Whether Saturday and Sunday are included as working days can be configured via optional parameters.
/// </summary>
public static class IsGunu
{
    /// <summary>
    /// TR: Verilen tarihin hafta sonu tatili olup olmadığını kontrol eder.
    /// <paramref name="includeSaturday"/> veya <paramref name="includeSunday"/> <see langword="true"/> verilirse ilgili gün çalışma günü kabul edilir (hafta sonu tatili sayılmaz).
    /// <para/>
    /// EN: Checks whether the given date is a weekend non-working day.
    /// If <paramref name="includeSaturday"/> or <paramref name="includeSunday"/> is <see langword="true"/>, that day is treated as a working day (not a weekend off-day).
    /// </summary>
    /// <param name="date">TR: Kontrol edilecek tarih. / EN: The date to check.</param>
    /// <param name="includeSaturday">TR: Cumartesi gününün çalışma günü sayılıp sayılmayacağı (Varsayılan: <see langword="false"/>). / EN: Whether Saturday is counted as a working day (Default: <see langword="false"/>).</param>
    /// <param name="includeSunday">TR: Pazar gününün çalışma günü sayılıp sayılmayacağı (Varsayılan: <see langword="false"/>). / EN: Whether Sunday is counted as a working day (Default: <see langword="false"/>).</param>
    public static bool IsWeekend(DateTime date, bool includeSaturday = false, bool includeSunday = false)
    {
        return date.DayOfWeek switch
        {
            DayOfWeek.Saturday => !includeSaturday,
            DayOfWeek.Sunday => !includeSunday,
            _ => false
        };
    }

    /// <summary>
    /// TR: Verilen tarihin Türkiye'de çalışma günü (iş günü: hafta sonu tatili olmayan ve tam gün resmi tatil olmayan gün) olup olmadığını kontrol eder.
    /// <para/>
    /// EN: Checks whether the given date is a working day (business day: not a weekend off-day and not a full-day public holiday) in Turkey.
    /// </summary>
    /// <param name="date">TR: Kontrol edilecek tarih. / EN: The date to check.</param>
    /// <param name="includeSaturday">TR: Cumartesi gününün iş günü sayılıp sayılmayacağı (Varsayılan: <see langword="false"/>). / EN: Whether Saturday is counted as a working day (Default: <see langword="false"/>).</param>
    /// <param name="includeSunday">TR: Pazar gününün iş günü sayılıp sayılmayacağı (Varsayılan: <see langword="false"/>). / EN: Whether Sunday is counted as a working day (Default: <see langword="false"/>).</param>
    public static bool IsWorkingDay(DateTime date, bool includeSaturday = false, bool includeSunday = false)
        => !IsWeekend(date, includeSaturday, includeSunday) && !ResmiTatil.IsPublicHoliday(date);

    /// <summary>
    /// TR: Başlangıç tarihine belirtilen sayıda iş günü (hafta sonu tatillerini ve resmi tatilleri atlayarak) ekler veya çıkarır.
    /// <para/>
    /// EN: Adds or subtracts the specified number of working days to/from the start date (skipping weekend off-days and public holidays).
    /// </summary>
    /// <param name="startDate">TR: Başlangıç tarihi. / EN: The start date.</param>
    /// <param name="workingDays">TR: Eklenecek (pozitif) veya çıkarılacak (negatif) iş günü sayısı. / EN: Number of working days to add (positive) or subtract (negative).</param>
    /// <param name="includeSaturday">TR: Cumartesi gününün iş günü sayılıp sayılmayacağı (Varsayılan: <see langword="false"/>). / EN: Whether Saturday is counted as a working day (Default: <see langword="false"/>).</param>
    /// <param name="includeSunday">TR: Pazar gününün iş günü sayılıp sayılmayacağı (Varsayılan: <see langword="false"/>). / EN: Whether Sunday is counted as a working day (Default: <see langword="false"/>).</param>
    public static DateTime AddWorkingDays(DateTime startDate, int workingDays, bool includeSaturday = false, bool includeSunday = false)
    {
        if (workingDays == 0)
            return startDate;

        int step = workingDays > 0 ? 1 : -1;
        int remaining = Math.Abs(workingDays);
        DateTime current = startDate;

        while (remaining > 0)
        {
            current = current.AddDays(step);
            if (IsWorkingDay(current, includeSaturday, includeSunday))
                remaining--;
        }

        return current;
    }

    /// <summary>
    /// TR: İki tarih arasındaki (başlangıçtan bitişe kadar) toplam net iş günü sayısını döner.
    /// <para/>
    /// EN: Returns the total net number of working days between two dates (from start date to end date).
    /// </summary>
    /// <param name="startDate">TR: Başlangıç tarihi. / EN: The start date.</param>
    /// <param name="endDate">TR: Bitiş tarihi. / EN: The end date.</param>
    /// <param name="includeSaturday">TR: Cumartesi gününün iş günü sayılıp sayılmayacağı (Varsayılan: <see langword="false"/>). / EN: Whether Saturday is counted as a working day (Default: <see langword="false"/>).</param>
    /// <param name="includeSunday">TR: Pazar gününün iş günü sayılıp sayılmayacağı (Varsayılan: <see langword="false"/>). / EN: Whether Sunday is counted as a working day (Default: <see langword="false"/>).</param>
    public static int GetWorkingDaysCount(DateTime startDate, DateTime endDate, bool includeSaturday = false, bool includeSunday = false)
    {
        DateTime start = startDate.Date;
        DateTime end = endDate.Date;

        if (start == end)
            return 0;

        bool negative = start > end;
        if (negative)
            (start, end) = (end, start);

        int count = 0;
        DateTime cursor = start;
        while (cursor < end)
        {
            cursor = cursor.AddDays(1);
            if (IsWorkingDay(cursor, includeSaturday, includeSunday))
                count++;
        }

        return negative ? -count : count;
    }

    /// <summary>
    /// TR: Verilen tarihten sonraki (veya <paramref name="includeToday"/> <see langword="true"/> ise kendisi dahil) ilk iş gününü döner.
    /// <para/>
    /// EN: Returns the first working day after the given date (or including the given date itself if <paramref name="includeToday"/> is <see langword="true"/>).
    /// </summary>
    /// <param name="date">TR: Referans tarih. / EN: The reference date.</param>
    /// <param name="includeToday">TR: Verilen tarihin kendisinin de kontrol edilip edilmeyeceği (Varsayılan: <see langword="false"/>). / EN: Whether to check the given date itself (Default: <see langword="false"/>).</param>
    /// <param name="includeSaturday">TR: Cumartesi gününün iş günü sayılıp sayılmayacağı (Varsayılan: <see langword="false"/>). / EN: Whether Saturday is counted as a working day (Default: <see langword="false"/>).</param>
    /// <param name="includeSunday">TR: Pazar gününün iş günü sayılıp sayılmayacağı (Varsayılan: <see langword="false"/>). / EN: Whether Sunday is counted as a working day (Default: <see langword="false"/>).</param>
    public static DateTime GetNextWorkingDay(DateTime date, bool includeToday = false, bool includeSaturday = false, bool includeSunday = false)
    {
        DateTime cursor = includeToday ? date : date.AddDays(1);
        while (!IsWorkingDay(cursor, includeSaturday, includeSunday))
        {
            cursor = cursor.AddDays(1);
        }

        return cursor;
    }
}

/// <summary>
/// TR: <see cref="ResmiTatil"/> ve <see cref="IsGunu"/> için <see cref="DateTime"/> uzantı (extension) metotları.
/// <para/>
/// EN: <see cref="DateTime"/> extension methods for <see cref="ResmiTatil"/> and <see cref="IsGunu"/>.
/// </summary>
public static class MihenkDateExtensions
{
    /// <inheritdoc cref="ResmiTatil.IsPublicHoliday(DateTime, bool)"/>
    public static bool IsPublicHoliday(this DateTime date, bool includeHalfDays = false)
        => ResmiTatil.IsPublicHoliday(date, includeHalfDays);

    /// <inheritdoc cref="ResmiTatil.TryGetHoliday(DateTime, out HolidayInfo)"/>
    public static bool TryGetPublicHoliday(this DateTime date, out HolidayInfo holiday)
        => ResmiTatil.TryGetHoliday(date, out holiday);

    /// <inheritdoc cref="IsGunu.IsWeekend(DateTime, bool, bool)"/>
    public static bool IsWeekend(this DateTime date, bool includeSaturday = false, bool includeSunday = false)
        => IsGunu.IsWeekend(date, includeSaturday, includeSunday);

    /// <inheritdoc cref="IsGunu.IsWorkingDay(DateTime, bool, bool)"/>
    public static bool IsWorkingDay(this DateTime date, bool includeSaturday = false, bool includeSunday = false)
        => IsGunu.IsWorkingDay(date, includeSaturday, includeSunday);

    /// <inheritdoc cref="IsGunu.AddWorkingDays(DateTime, int, bool, bool)"/>
    public static DateTime AddWorkingDays(this DateTime startDate, int workingDays, bool includeSaturday = false, bool includeSunday = false)
        => IsGunu.AddWorkingDays(startDate, workingDays, includeSaturday, includeSunday);

    /// <inheritdoc cref="IsGunu.GetWorkingDaysCount(DateTime, DateTime, bool, bool)"/>
    public static int GetWorkingDaysCount(this DateTime startDate, DateTime endDate, bool includeSaturday = false, bool includeSunday = false)
        => IsGunu.GetWorkingDaysCount(startDate, endDate, includeSaturday, includeSunday);

    /// <inheritdoc cref="IsGunu.GetNextWorkingDay(DateTime, bool, bool, bool)"/>
    public static DateTime GetNextWorkingDay(this DateTime date, bool includeToday = false, bool includeSaturday = false, bool includeSunday = false)
        => IsGunu.GetNextWorkingDay(date, includeToday, includeSaturday, includeSunday);
}
