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
        Symbols = new();
        Enclosing = enclosing;

        //inititialize builtin types
        Define(new Symbol("INTEGER"));
        Define(new Symbol("REAL"));
    }

    public void Define(Symbol symbol)
    {
        Symbols[symbol.Name] = symbol;
    }

    public Symbol? Lookup(string name, bool limit = false)
    {
        if (Symbols.TryGetValue(name, out Symbol? symbol)) { return symbol; }

        if (limit) {  return null; }
        
        if (Enclosing == null) {  return null; }

        return Enclosing.Lookup(name);

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

