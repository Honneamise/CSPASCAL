
namespace Pascal;

public class Interpreter
{
    public ScopedSymbolTable Symtab { get; }

    public Dictionary<string, float> GlobalScope { get; }

    public Interpreter(ScopedSymbolTable symtab)
    {
        Symtab = symtab;
        GlobalScope = new();

        foreach (var item in symtab.Symbols)
        {
            if(item.Value is SymbolVar)
            {
                GlobalScope.Add(item.Value.Name, 0.0f);
            }
        }
    }

    public override string ToString()
    {
        string str = "";

        foreach (var item in GlobalScope)
        {
            str += $"{item.Key} : {item.Value}\n";
        }

        return str;
    }

    private float Visit(AstVar node)
    {
        float val = GlobalScope[node.Name];

        return val;
    }

    private float Visit(AstUnaryOp node)
    {
        return node.Op switch
        {
            TokenType.PLUS => +Visit(node.Expr),
            TokenType.MINUS => -Visit(node.Expr),
            _ => throw new Exception("Invalid node type")
        };
    }

    private float Visit(AstBinOp node)
    {
        float lval = Visit(node.Left);
        float rval = Visit(node.Right);

        return node.Op switch
        {
            TokenType.PLUS => lval + rval,
            TokenType.MINUS => lval - rval,
            TokenType.MUL => lval * rval,

            TokenType.INTEGER_DIV => (int)lval / (int)rval,
            TokenType.FLOAT_DIV => lval / rval,

            _ => throw new Exception("Invalid node type")
        };
    }

    private float Visit(AstAssign node)
    {
        GlobalScope[node.Left.Name] = Visit(node.Right);

        return 0;
    }

    private float Visit(AstCompound node)
    {
        foreach (Ast n in node.Nodes)
        {
            _ = Visit(n);
        }

        return 0;
    }

    private float Visit(AstBlock node)
    {
        foreach (Ast n in node.Declarations)
        {
            Visit(n);
        }

        Visit(node.Compound);

        return 0;
    }

    private float Visit(AstProgram node)
    {
        Visit(node.Block);

        return 0;
    }

    private float Visit(Ast node)
    {
        return node switch
        {
            AstEmpty => 0.0f,
            AstNum => ((AstNum)node).FloatValue,
            AstVar => Visit((AstVar)node),
            AstUnaryOp => Visit((AstUnaryOp)node),
            AstBinOp => Visit((AstBinOp)node),
            AstAssign => Visit((AstAssign)node),
            AstCompound => Visit((AstCompound)node),
            AstProcedureDecl => 0.0f,
            AstType => 0.0f,
            AstVarDecl => 0.0f,
            AstBlock => Visit((AstBlock)node),
            AstProgram => Visit((AstProgram)node),

            _ => throw new Exception("Invalid node type found")
        };
    }

    public void Execute(Ast ast)
    {
        Visit(ast);
    }

}
