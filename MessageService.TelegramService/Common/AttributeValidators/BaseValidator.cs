using System.Reflection;

namespace MessageService.TelegramService.Common.AttributeValidators;

internal abstract class BaseValidator<TAttribute> where TAttribute : Attribute {

    protected static IEnumerable<TAttribute> GetAttributes<TObject>(TObject obj)
        where TObject : class {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));

        var type = obj.GetType();

        var inherited = typeof(TAttribute).GetCustomAttribute<AttributeUsageAttribute>(true)?.Inherited ?? throw new Exception($"Не найден {nameof(AttributeUsageAttribute)}");

        return type.GetCustomAttributes<TAttribute>(inherited).Where(attr => attr != null);
    }
}