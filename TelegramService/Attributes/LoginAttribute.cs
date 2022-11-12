namespace TelegramService.Attributes;

/// <summary>
/// Атрибут валидации по логину пользователя
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
internal class LoginAttribute : Attribute {

    public LoginAttribute(string login) {
        Login = login;
    }

    /// <summary>
    /// Логин Telegram пользователя
    /// </summary>
    public string Login { get; }
}