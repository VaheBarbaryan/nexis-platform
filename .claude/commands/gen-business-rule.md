Generate a domain business rule implementing IBusinessRule. Arguments: `$ARGUMENTS`

Expected format: `<RuleName> <Module> <Entity>`

Example: `ContentMaxLength Posts Post`
→ generates `ContentMaxLengthRule` in `Modules.Posts.Domain/Posts/Rules/`

The rule name should describe the invariant being enforced, without "Rule" suffix (it's added automatically).

## File to generate

`src/Modules/<Module>/Modules.<Module>.Domain/<Entity>/Rules/<RuleName>Rule.cs`

```csharp
using SharedKernel.Domain.Rules;

namespace Modules.<Module>.Domain.<Entity>.Rules;

public sealed class <RuleName>Rule : IBusinessRule
{
    private readonly <FieldType> _value;

    public <RuleName>Rule(<FieldType> value) => _value = value;

    public bool IsBroken() => /* condition */;

    public string Message => "<Human-readable violation message.>";
}
```

## IBusinessRule interface (reference)

```csharp
public interface IBusinessRule
{
    bool IsBroken();
    string Message { get; }
}
```

Rules are checked via `Entity<T>.CheckRule(rule)` which throws `BusinessRuleValidationException` when `IsBroken()` returns true.

## Common patterns

**Not empty:**
```csharp
public bool IsBroken() => string.IsNullOrWhiteSpace(_value);
public string Message => "<Field> cannot be empty.";
```

**Max length:**
```csharp
public bool IsBroken() => _value.Length > _maxLength;
public string Message => $"<Field> cannot exceed {_maxLength} characters.";
```

**Ownership check:**
```csharp
private readonly AuthorId _ownerId;
private readonly AuthorId _requestingId;
public bool IsBroken() => _ownerId != _requestingId;
public string Message => "Operation not permitted: resource belongs to another user.";
```

**State guard (e.g., already deleted):**
```csharp
private readonly bool _isDeleted;
public bool IsBroken() => _isDeleted;
public string Message => "<Entity> has been deleted and cannot be modified.";
```

## Rules
- Single responsibility: one invariant per rule class
- Constructor injects ONLY what's needed to evaluate `IsBroken()`
- Message is past-tense declarative: describes the violation, not the fix
- After generating, show me where in the aggregate to call `CheckRule(new <RuleName>Rule(...))`
