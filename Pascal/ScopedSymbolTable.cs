namespace Pascal;


public class ScopedSymbolTable
{
    public string Name { get; }
    public uint Level { get; }
    public Dictionary<string, Symbol> Symbols { get; }

    public ScopedSymbolTable(string name, uint level)
    {
        Name = name;
        Level = level;
        Symbols = [];

        //inititialize builtin types
        Define(new Symbol("INTEGER"));
        Define(new Symbol("REAL"));
        Define(new Symbol("PROCEDURE"));//???
    }

    public void Define(Symbol symbol)
    {
        //Console.WriteLine($"Define : {symbol.Name}");

        Symbols[symbol.Name] = symbol;
    }

    public Symbol? Lookup(string name)
    {
        //Console.WriteLine($"Lookup : {name}");

        if (Symbols.TryGetValue(name, out var symbol)) { return symbol; }

        return null;
    }

    public override string ToString()
    {

        string str = $"TABLE:{Name} LEVEL:{Level}\n";

        foreach (var item in Symbols)
        {
            str += item.Value.ToString() + "\n";
        }

        return str;
    }
}

