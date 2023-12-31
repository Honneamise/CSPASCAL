﻿namespace Pascal;


public class Symbol
{
    public string Name { get; }
    public uint Level { get; set; }

    public Symbol(string name)
    {  
        Name = name; 
        Level = 0;
    }

    public override string ToString()
    {
        return $"<{Name}>";
    }
}

public class SymbolVar : Symbol
{
    public Symbol Symbol { get; }

    public SymbolVar(string name, Symbol symbol) : base(name) 
    {
        Symbol = symbol;
    }

    public override string ToString()
    {
        return $"<{Name}:{Symbol.Name}>";
    }
}

public class SymbolProcedure : Symbol
{
    public List<SymbolVar> Parameters { get; }
    public AstBlock? Block { get; set; }

    public SymbolProcedure(string name, List<SymbolVar> parameters) : base(name)
    {
        Parameters = parameters;
    }
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

