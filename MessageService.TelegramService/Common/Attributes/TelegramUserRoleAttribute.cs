namespace MessageService.TelegramService.Common.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
internal class TelegramUserRoleAttribute : Attribute {
    public string RoleName { get; }

    public TelegramUserRoleAttribute(string roleName) {
        RoleName = roleName;
    }
}