namespace Pascal;


public abstract class Ast
{
    public override string ToString() => "[" + GetType().Name + "]";
}

public class AstEmpty() : Ast { }

public class AstNum(string str) : Ast
{
    public string StrValue { get; } = str;
    public int IntValue { get => int.Parse(StrValue); }
    public float FloatValue { get => float.Parse(StrValue); }
}

public class AstVar(string str) : Ast
{
    public string Name { get; } = str;
}

public class AstUnaryOp(TokenType op, Ast expr) : Ast
{
    public Ast Expr { get; } = expr;
    public TokenType Op { get; } = op;
}

public class AstBinOp(Ast left, TokenType op, Ast right) : Ast
{
    public Ast Left { get; } = left;
    public TokenType Op { get; } = op;
    public Ast Right { get; } = right;
}

public class AstAssign(AstVar left, Ast right) : Ast
{
    public AstVar Left { get; } = left;
    public Ast Right { get; } = right;
}

public class AstCompound() : Ast
{
    public List<Ast> Nodes { get; } = [];
}

public class AstType(string type) : Ast
{
    public string Type { get; } = type;
}

public class AstParam(AstVar var, AstType type) : Ast
{
    public AstVar Var { get; } = var;
    public AstType Type { get; } = type;
}

public class AstProcedureDecl(string name, List<AstParam> parameters, AstBlock block) : Ast
{
    public string Name { get; } = name;
    public List<AstParam> Params = parameters;
    public AstBlock Block { get; } = block;
}

public class AstVarDecl(AstVar varNode, AstType typeNode) : Ast
{
    public AstVar VarNode { get; } = varNode;
    public AstType TypeNode { get; } = typeNode;
}

public class AstBlock(List<Ast> declarations, AstCompound compound) : Ast
{
    public List<Ast> Declarations { get; } = declarations;
    public AstCompound Compound { get; } = compound;
}

public class AstProgram(string name, AstBlock block) : Ast
{
    public string Name { get; } = name;
    public AstBlock Block { get; } = block;
}
