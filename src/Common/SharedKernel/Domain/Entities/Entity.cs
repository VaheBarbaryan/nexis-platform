using SharedKernel.Domain.Exceptions;
using SharedKernel.Domain.Rules;

namespace SharedKernel.Domain.Entities;

public abstract class Entity<TId> where TId : EntityId
{
    /// <summary>
    /// Null suppression is intentional — ID is always set via factory methods
    /// or hydrated by EF Core before any domain interaction.
    /// </summary>
    public TId Id { get; protected set; } = null!;

    protected static void CheckRule(IBusinessRule rule)
    {
        if (rule.IsBroken())
        {
            throw new BusinessRuleValidationException(rule);
        }
    }
}
