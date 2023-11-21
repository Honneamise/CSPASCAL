namespace Pascal;


public class SymbolTable
{
    public Dictionary<string, Symbol> Symbols { get; }

    public SymbolTable()
    {
        Symbols = [];

        //inititialize builtin types
        Define(new Symbol("INTEGER"));
        Define(new Symbol("REAL"));
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
        string str = "";

        foreach (var item in Symbols)
        {
            str += item.Value.ToString() + "\n";
        }

        return str;
    }
}

