using System.Diagnostics.CodeAnalysis;
using SharedKernel.Domain.Rules;

namespace SharedKernel.Domain.Exceptions;

[SuppressMessage("Design", "CA1032")]
public sealed class BusinessRuleValidationException : DomainException
{
    public IBusinessRule BrokenRule { get; }

    public BusinessRuleValidationException(IBusinessRule brokenRule)
        : base(brokenRule?.Message
               ?? throw new ArgumentNullException(nameof(brokenRule)))
    {
        BrokenRule = brokenRule;
    }

    public override string ToString()
        => $"{BrokenRule.GetType().Name}: {Message}";
}
