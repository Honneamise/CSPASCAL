namespace Pascal;


public abstract class Ast
{
    public override string ToString() => "[" + GetType().Name + "]";
}

public class AstEmpty : Ast { }

public class AstNum : Ast
{
    public string StrValue { get; }
    public int IntValue { get => int.Parse(StrValue); }
    public float FloatValue { get => float.Parse(StrValue); }

    public AstNum(string str)
    {
        StrValue = str;
    }
}

public class AstVar : Ast
{
    public string Name { get; }

    public AstVar(string name) 
    { 
        Name = name;
    }
}

public class AstUnaryOp : Ast
{
    public Ast Expr { get; }
    public TokenType Op { get; }

    public AstUnaryOp(TokenType op, Ast expr)
    {
        Op = op;
        Expr = expr;
    }
}

public class AstBinOp : Ast
{
    public Ast Left { get; }
    public TokenType Op { get; }
    public Ast Right { get; }

    public AstBinOp(Ast left, TokenType op, Ast right)
    {
        Left = left;
        Op = op;
        Right = right;
    }
}

public class AstAssign : Ast
{
    public AstVar Left { get; }
    public Ast Right { get; }

    public AstAssign(AstVar left, Ast right)
    {
        Left = left;
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
    public string Name { get; }

    public AstType(string name)
    {
        Name=name;
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
