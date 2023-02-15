namespace Application.Common.Security;

/// <summary>
/// Указывает класс, к которому применяется этот атрибут, требующий авторизации.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class AuthorizeAttribute : Attribute {

    public AuthorizeAttribute() {
    }

    /// <summary>
    /// Получает или задает разделенный пробелами список ролей, которым разрешен доступ к ресурсу.
    /// </summary>
    public string Roles { get; set; } = string.Empty;
}