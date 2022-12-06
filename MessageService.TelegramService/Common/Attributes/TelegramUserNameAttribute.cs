namespace MessageService.TelegramService.Common.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
internal class TelegramUserNameAttribute : Attribute {
    public string UserName { get; set; }

    public TelegramUserNameAttribute(string userName) {
        UserName = userName;
    }
}