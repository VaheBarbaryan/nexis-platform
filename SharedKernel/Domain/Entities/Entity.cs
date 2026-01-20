using SharedKernel.Domain.Exceptions;
using SharedKernel.Domain.Rules;

namespace SharedKernel.Domain.Entities;

public abstract class Entity<TId>
{
    public TId Id { get; protected set; } = default!;

    protected void CheckRule(IBusinessRule rule)
    {
        if (rule.IsBroken())
            throw new BusinessRuleValidationException(rule);
    }
}