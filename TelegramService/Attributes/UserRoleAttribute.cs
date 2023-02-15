namespace TelegramService.Attributes;

/// <summary>
/// Атрибут валидации по роли пользователя
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
internal class UserRoleAttribute : Attribute {

    public UserRoleAttribute(string roleName) {
        RoleName = roleName;
    }

    /// <summary>
    /// Наименование <see cref="Domain.Entities.Role"/> пользователя
    /// </summary>
    public string RoleName { get; }
}