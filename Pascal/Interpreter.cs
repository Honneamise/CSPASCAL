
namespace Pascal;

public class Interpreter
{
    public Ast Ast { get; }
    Dictionary<string, float> GlobalMemory { get; }

    public Interpreter(Ast ast)
    {
        Ast = ast;
        GlobalMemory= new();
    }

    public override string ToString()
    {
        string str = "---VARIABLES VALUES---\n";

        foreach (var item in GlobalMemory)
        {
            str += $"{item.Key} : {item.Value}\n";
        }

        return str;
    }

    private float Visit(AstProgram node)
    {
        Visit(node.Block);

        return 0.0f;
    }

    private float Visit(AstBlock node)
    {
        foreach (Ast n in node.Declarations)
        {
            Visit(n);
        }

        Visit(node.Compound);

        return 0.0f;
    }

    private float Visit(AstBinOp node)
    {
        float lval = Visit(node.Left);
        float rval = Visit(node.Right);

        switch (node.Op.Type)
        {
            case TokenType.PLUS: return (lval + rval);

            case TokenType.MINUS: return (lval - rval);

            case TokenType.MUL: return (lval* rval);

            case TokenType.INTEGER_DIV: return ((int)lval / (int)rval);
            
            case TokenType.FLOAT_DIV: return (lval / rval);

            default: return 0.0f;
        }
    }

    private float Visit(AstUnaryOp node)
    {
        switch(node.Op.Type)
        {
            case TokenType.PLUS: return +Visit(node.Expr);

            case TokenType.MINUS: return -Visit(node.Expr);

            default: return 0.0f;
        };
    }

    private float Visit(AstCompound node)
    {
        foreach (Ast n in node.Nodes)
        {
            _ = Visit(n);
        }

        return 0.0f;
    }

    private float Visit(AstAssign node)
    {
        GlobalMemory[node.Left.Name] = Visit(node.Right);

        return 0.0f;
    }

    private float Visit(AstVar node)
    {
        float val = GlobalMemory[node.Name];

        return val;
    }

    private float Visit(Ast node)
    {
        switch(node)
        {
            case AstProgram: return Visit((AstProgram)node);
            case AstBlock: return Visit((AstBlock)node);
            case AstVarDecl: return 0.0f;
            case AstType: return 0.0f;
            case AstBinOp: return Visit((AstBinOp)node);
            case AstNum: return ((AstNum)node).FloatValue;
            case AstUnaryOp: return Visit((AstUnaryOp)node);
            case AstCompound: return Visit((AstCompound)node);
            case AstAssign: return Visit((AstAssign)node);
            case AstVar: return Visit((AstVar)node);
            case AstEmpty: return 0.0f;
            case AstProcedureDecl: return 0.0f;
            case AstProcedureCall: return 0.0f;

            default: return 0.0f;
        };
    }

    public void Execute()
    {
        Visit(Ast);

        Console.WriteLine(ToString());
    }

}
