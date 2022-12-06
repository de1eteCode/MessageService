namespace MessageService.TelegramService.Common.Extends;

internal static class DateTimeHelper {

    public static DateTime ConvertUnixToDateTime(ulong unixTimeStamp) {
        DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        dt = dt.AddSeconds(unixTimeStamp).ToLocalTime();
        return dt;
    }
}