namespace SharedKernel.Infrastructure;

public sealed class BiMap<TFirst, TSecond>
    where TFirst : notnull
    where TSecond : notnull
{
    private readonly Dictionary<TFirst, TSecond> _firstToSecond = new();

    private readonly Dictionary<TSecond, TFirst> _secondToFirst = new();

    public void Add(TFirst first, TSecond second)
    {
        if (_firstToSecond.ContainsKey(first) ||
            _secondToFirst.ContainsKey(second))
        {
            throw new ArgumentException("Duplicate first or second");
        }

        _firstToSecond.Add(first, second);
        _secondToFirst.Add(second, first);
    }

    public bool TryGetByFirst(TFirst first, out TSecond second)
    {
        return _firstToSecond.TryGetValue(first, out second!);
    }

    public bool TryGetBySecond(TSecond second, out TFirst first)
    {
        return _secondToFirst.TryGetValue(second, out first!);
    }
}
