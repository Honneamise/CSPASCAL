namespace Pascal;


public class ScopedSymbolTable
{
    public string Name { get; }
    public uint Level { get; }
    public Dictionary<string, Symbol> Symbols { get; }
    public ScopedSymbolTable? Enclosing { get; }

    public ScopedSymbolTable(string name, uint level, ScopedSymbolTable? enclosing)
    {
        Name = name;
        Level = level;
        Symbols = [];
        Enclosing = enclosing;

        //inititialize builtin types
        Define(new Symbol("integer"));
        Define(new Symbol("real"));
    }

    public void Define(Symbol symbol)
    {
        //Console.WriteLine($"Define : {symbol.Name}");

        Symbols[symbol.Name] = symbol;
    }

    public Symbol? Lookup(string name)
    {
        //Console.WriteLine($"Lookup : {name}");

        if (Symbols.TryGetValue(name, out Symbol? symbol)) { return symbol; }
        else if (Enclosing != null)
        {
            return Enclosing.Lookup(name);
        }
        else
        {
            return null;
        }

    }

    public override string ToString()
    {
        string str = $"TABLE:{Name}\nLEVEL:{Level}\nENCLOSING:{Enclosing?.Name}\n";

        foreach (var item in Symbols)
        {
            str += item.Value.ToString() + "\n";
        }

        return str;
    }
}

