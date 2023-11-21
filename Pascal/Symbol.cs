namespace Pascal;


public class Symbol(string name)
{
    public string Name { get; } = name;

    public override string ToString() => $"<{Name}>";
}

public class SymbolVar(string name, Symbol symbol) : Symbol(name)
{
    public Symbol Symbol { get; } = symbol;

    public override string ToString() => $"<{Name}:{Symbol.Name}>";
}

public class SymbolProcedure(string name, List<SymbolVar> parameters) : Symbol(name)
{
    public List<SymbolVar> Parameters { get; } = parameters;

    public override string ToString()
    {
        string str = $"<{Name}>";

        foreach(SymbolVar var in Parameters)
        {
            str += var.ToString();
        }

        return str;
    }
}

