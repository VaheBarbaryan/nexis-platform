using SharedKernel.Domain.Exceptions;
using SharedKernel.Domain.Rules;

namespace SharedKernel.Domain.Entities;

public abstract class Entity<TId>
{
    public TId Id { get; protected set; } = default!;

    protected static void CheckRule(IBusinessRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);

        if (rule.IsBroken())
            throw new BusinessRuleValidationException(rule);
    }
}
