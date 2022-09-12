namespace MessageService.Services.HandlerServices.Telegram.Attributes;

/// <summary>
/// Атрибут валидации по логину пользователя
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class TelegramLoginAttribute : Attribute {

    public TelegramLoginAttribute(string login) {
        Login = login;
    }

    public string Login { get; }
}