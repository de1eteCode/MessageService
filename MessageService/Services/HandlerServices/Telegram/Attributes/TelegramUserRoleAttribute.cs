namespace MessageService.Services.HandlerServices.Telegram.Attributes {

    /// <summary>
    /// Атрибут валидации по роли пользователя
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class TelegramUserRoleAttribute : Attribute {

        public TelegramUserRoleAttribute(string roleName) {
            RoleName = roleName;
        }

        public string RoleName { get; }
    }
}