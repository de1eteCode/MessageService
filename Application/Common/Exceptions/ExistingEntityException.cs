namespace Application.Common.Exceptions;

public class ExistingEntityException : Exception {

    public ExistingEntityException() {
    }

    public ExistingEntityException(string message) : base(message) {
    }

    public ExistingEntityException(string message, Exception inner) : base(message, inner) {
    }

    public ExistingEntityException(string name, object key)
        : base($"The entity \"{name}\" ({key}) was previously created.") {
    }
}