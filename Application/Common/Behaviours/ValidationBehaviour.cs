using FluentValidation;
using MediatR;

namespace Application.Common.Behaviours;

public class ValidationBehaviour<TRequest, TResponce> : IPipelineBehavior<TRequest, TResponce>
    where TRequest : IRequest<TResponce> {
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators) {
        _validators = validators;
    }

    public async Task<TResponce> Handle(TRequest request, RequestHandlerDelegate<TResponce> next, CancellationToken cancellationToken) {
        if (_validators.Any()) {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(e => e.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .Where(e => e.Errors.Any())
                .SelectMany(e => e.Errors);

            if (failures.Any()) {
                throw new ValidationException(failures.ToList());
            }
        }
        return await next();
    }
}