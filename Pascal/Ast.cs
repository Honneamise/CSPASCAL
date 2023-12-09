namespace Pascal;


public abstract class Ast
{
    public override string ToString() => "[" + GetType().Name + "]";
}

public class AstEmpty : Ast { }

public class AstNum : Ast
{
    public Token Token { get; }
    public string StrValue { get; }
    public int IntValue { get => int.Parse(StrValue); }
    public float FloatValue { get => float.Parse(StrValue); }

    public AstNum(Token token)
    {
        Token = token;
        StrValue = token.Value;
    }
}

public class AstVar : Ast
{
    public Token Token { get; }
    public string Name { get; }

    public AstVar(Token token) 
    { 
        Token = token;
        Name = token.Value;
    }
}

public class AstVarDecl : Ast
{
    public AstVar VarNode { get; }
    public AstType TypeNode { get; }

    public AstVarDecl(AstVar varNode, AstType typeNode)
    {
        VarNode = varNode;
        TypeNode = typeNode;
    }
}

public class AstUnaryOp : Ast
{
    public Ast Expr { get; }
    public Token Token { get; }
    public Token Op { get; }

    public AstUnaryOp(Token token, Ast expr)
    {
        Token = token;
        Op = token;
        Expr = expr;
    }
}

public class AstBinOp : Ast
{
    public Ast Left { get; }
    public Token Token { get; }
    public Token Op { get; }
    public Ast Right { get; }

    public AstBinOp(Ast left, Token token, Ast right)
    {
        Left = left;
        Token = token;
        Op = token;
        Right = right;
    }
}

public class AstAssign : Ast
{
    public AstVar Left { get; }
    public Token Token { get; }
    public Token Op { get; }
    public Ast Right { get; }

    public AstAssign(AstVar left, Token token, Ast right)
    {
        Left = left;
        Token = token;
        Op = token;
        Right = right;
    }
}

public class AstCompound : Ast
{
    public List<Ast> Nodes { get; }

    public AstCompound()
    {
        Nodes = new List<Ast>();
    }
}

public class AstType : Ast
{
    public Token Token { get; }
    public string Name { get; }

    public AstType(Token token)
    {
        Token = token;
        Name = token.Value;
    }
}

public class AstParam : Ast
{
    public AstVar Var { get; }
    public AstType Type { get; }

    public AstParam(AstVar var, AstType type)
    {
        Var = var;
        Type = type;
    }
}

public class AstProcedureCall: Ast
{
    public string Name { get; } 
    public List<Ast> Nodes { get; }
    public Token Token { get; }

    public AstProcedureCall(string name, List<Ast> nodes, Token token)
    {
        Name = name;
        Nodes = nodes;
        Token = token;
    }
}

public class AstProcedureDecl: Ast
{
    public string Name { get; }
    public List<AstParam> Parameters { get; }
    public AstBlock Block { get; }

    public AstProcedureDecl(string name, List<AstParam> parameters, AstBlock block)
    {
        Name = name;
        Parameters = parameters;
        Block = block;
    }
}


public class AstBlock : Ast
{
    public List<Ast> Declarations { get; }
    public AstCompound Compound { get; }

    public AstBlock(List<Ast> declarations, AstCompound compound)
    {
        Declarations = declarations;
        Compound = compound;
    }
}

public class AstProgram : Ast
{
    public string Name { get; }
    public AstBlock Block { get; }

    public AstProgram(string name, AstBlock block)
    {
        Name = name;
        Block = block;
    }
}
